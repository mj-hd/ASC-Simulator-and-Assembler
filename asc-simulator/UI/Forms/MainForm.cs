/*
 * MainForm.cs
 *  
 * マシンの状態、メモリの状態、レジスタの状態などの表示、マシンの操作を行うフォーム
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Simulator.UI.Forms
{
    public partial class MainForm : Form
    {

        // MM、レジスタの内容と同期するDataTable
        // 同期は、マシンで書換イベントが起こった時に行われる
        public DataTable MMTable;
        public DataTable RegistersTable;

        // マシンの状態を表示するコントロール
        public MachineDisplay.MachineDisplay Display;

        // プログラムの設定を格納するクラス
        public Simulator.Common.ApplicationSettings Settings;



        public Machine.ASC Machine
        {
            get
            {
                return this._Machine;
            }
            set
            {
                this._Machine = value;

                // マシンの各々のイベントにMainFormのメソッドを割り当てる
                this._Machine.CycleBegin += () => this.DidCycleBegin();
                this._Machine.CycleEnd   += () => this.DidCycleEnd();
                this._Machine.CycleDecode += () => this.DidCycleDecode();
                this._Machine.CycleOpecode += () => this.DidCycleOpecode();
                this._Machine.CycleUpdateIR += () => this.DidCycleUpdateIR();
                this._Machine.CycleUpdatePC += () => this.DidCycleUpdatePC();

                this._Machine.Stepped += () => this.DidStepped();

                this.Machine.Registers.DataChanged += (de) => { this.DidDataChanged(de); };
                this.Machine.Registers.DataAccessed += (de) => { this.DidDataAccessed(de); };

                this.Machine.Memory.MemoryChanged += (me) => { this.DidMemoryChanged(me); };

                this.Machine.DataMoved += (dme) => { this.DidDataMoved(dme); };
            }
        }


        public MainForm()
        {
            // this.Settingsを初期化
            this.Settings = new Simulator.Common.ApplicationSettings();

            // Setting.simcのパス
            string settingFile = Application.UserAppDataPath + "\\Settings.simc";

            // 設定ファイル(Settings.simc)が存在すれば、読み込む
            if (File.Exists(settingFile))
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(settingFile, FileMode.OpenOrCreate))
                {
                    // ファイルの内容をデシリアライズ
                    this.Settings = (Simulator.Common.ApplicationSettings)bf.Deserialize(fs);
                }
            }

            InitializeComponent();

            // this.Displayを初期化
            // ApplicationSettingsクラスを利用するため、参照渡し
            this.Display = new MachineDisplay.MachineDisplay(this.Settings);
            this.Display.Dock = DockStyle.Fill;
            this.DisplayPanel.Controls.Add(this.Display);

            // this._Breakpointsを初期化
            // ブレークポイントのアドレスが格納されたList
            this._Breakpoints = new List<ushort>();
            
            // MMの内容を表示のために格納しておくDataTable
            // 実際のMMの内容と同期している
            this.MMTable = new DataTable("MM");
            // カラムの設定
            MMTable.Columns.Add("アドレス");
            MMTable.Columns["アドレス"].DataType = Type.GetType("System.String");
            MMTable.Columns.Add("バイナリ");
            MMTable.Columns["バイナリ"].DataType = Type.GetType("System.String");
            MMTable.Columns.Add("ニーモニック");
            MMTable.Columns["ニーモニック"].DataType = Type.GetType("System.String");

            // 主キーを設定
            MMTable.PrimaryKey = new DataColumn[] { MMTable.Columns["アドレス"] };

            // ニーモニックとバイナリ列だけ書き換え可能に
            MMTable.Columns["アドレス"].ReadOnly = true;
            MMTable.Columns["ニーモニック"].ReadOnly = false;
            MMTable.Columns["バイナリ"].ReadOnly = false;

            // MMTableをデータソースとして設定
            MMView.DataSource = MMTable;
            // レイアウトを設定
            MMView.Columns["アドレス"].Width = this.Settings.MMViewAddressWidth;
            MMView.Columns["ニーモニック"].Width = this.Settings.MMViewMnemonicWidth; 
            MMView.Columns["バイナリ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // ソートできないように
            MMView.Columns["アドレス"].SortMode = DataGridViewColumnSortMode.NotSortable;
            MMView.Columns["ニーモニック"].SortMode = DataGridViewColumnSortMode.NotSortable;
            MMView.Columns["バイナリ"].SortMode = DataGridViewColumnSortMode.NotSortable;
            // チェックアイコン(ブレークポイントの赤い丸)を有効化
            MMView.IsEnabledCheckIcon = true;

            // MMの表示の内容を0で埋める
            for (int i = 0; i <= Simulator.Memory.ASC_Memory.MaxSize; i++)
            {
                MMTable.Rows.Add("0x"+Convert.ToString(i, 16), "0000000000000000", "LD 0x0");
            }

            // レジスタの内容を表示のために格納しておくDataTable
            // レジスタの内容と同期している
            this.RegistersTable = new DataTable("Registers");

            // データソースとして設定
            this.RegistersView.DataSource = this.RegistersTable;

            this.RegistersView.DefaultCellStyle = new DataGridViewCellStyle();

            // カラムの設定
            RegistersTable.Columns.Add("名前");
            RegistersTable.Columns["名前"].DataType = Type.GetType("System.String");
            RegistersTable.Columns.Add("値");
            RegistersTable.Columns["値"].DataType = Type.GetType("System.String");

            // 値だけを書き換えられるように
            RegistersTable.Columns["名前"].ReadOnly = true;
            RegistersTable.Columns["値"].ReadOnly = false;

            // 主キーを設定
            RegistersTable.PrimaryKey = new DataColumn[] { this.RegistersTable.Columns["名前"] };

            // レイアウトを設定
            this.RegistersView.Columns["名前"].Width = this.Settings.RegistersViewNameWidth;
            this.RegistersView.Columns["値"].Width = this.RegistersView.Width - this.Settings.RegistersViewNameWidth - 2;

            // レジスタの表示を初期化
            RegistersTable.Rows.Add("PC", "初期化されていません。");
            RegistersTable.Rows.Add("IR", "初期化されていません。");
            RegistersTable.Rows.Add("R", "初期化されていません。");
            RegistersTable.Rows.Add("MAR", "初期化されていません。");
            RegistersTable.Rows.Add("N", "初期化されていません。");
            RegistersTable.Rows.Add("Z", "初期化されていません。");

            // 実行速度のトラックバーの設定
            this.SpeedTrackBar.TrackBar.Minimum = 1;
            this.SpeedTrackBar.TrackBar.Maximum = 100;
            this.SpeedTrackBar.TrackBar.TickFrequency = 10;
            // 実行速度が変更されたとき、TrackBar_Scrollを呼び出す
            this.SpeedTrackBar.TrackBar.Scroll += TrackBar_Scroll;

            // 「プログラムが起動したときにマシンを初期化する」設定が友好の場合、マシンを初期化する
            if (this.Settings.MachineShouldInitializeWithStartup)
            {
                this.初期化ToolStripMenuItem_Click(this, new EventArgs());
            }

            // ファイルを開く、閉じるダイアログの初期パスを、設定の内容から設定
            this.OpenDialog.InitialDirectory = this.Settings.SimulatorCurrentPath;
            this.SaveDialog.InitialDirectory = this.Settings.SimulatorCurrentPath;
            // 実行速度を設定の内容から設定
            this.SpeedTrackBar.TrackBar.Value = this.Settings.MachineSpeed;
            // イベントを呼び出して、実行速度を反映させる
            this.TrackBar_Scroll(this, new EventArgs());

            this.StatusLabel.Text = "準備完了";
        }

        public new void Dispose()
        {
            base.Dispose();

            this._Machine.Dispose();
        }

        // ファイルへ保存する
        // fileName: 保存したいパス
        // エラーが発生した場合は、throwされる。
        // 表示の更新なども行われるので、必要ない場合は直接マシンのSaveを呼び出すべき
        public void SaveFile(string fileName)
        {
            this.Display.ResetHighlights();

            FileStream w = new FileStream(this.SaveDialog.FileName, FileMode.Create);

            if (!w.CanWrite)
            {
                MessageBox.Show("ファイルに書き込めませんでした。");
                return;
            }

            try
            {
                this._Machine.Save(w);
            }
            catch (Exception we)
            {
                MessageBox.Show("ファイルの書き込みに失敗しました。"+Environment.NewLine+"エラーメッセージ:"+we.Message);
            }
            finally
            {
                w.Close();
            }

            // ステータスの更新
            this.StatusLabel.Text = this.SaveDialog.FileName + "の書き込み完了";

            // 表示を更新
            this.Display.Invalidate();

            this.Text = fileName + " ASC Simulator";
        }

        // ファイルを開く
        // fileName: 開きたい.ascoファイルのパス
        // エラーが発生した場合はthrowされる。
        // 表示の更新なども行われるので、必要ない場合は直接マシンのLoadを呼び出すべき
        public void OpenFile(string fileName)
        {
            this.Display.ResetHighlights();

            FileStream r = new FileStream(fileName, FileMode.Open);

            if (!r.CanRead)
            {
                MessageBox.Show("ファイルへのアクセスが拒否されました。");
                return;
            }

            Dictionary<String, ushort> address;
            try
            {
                // マシンにStreamを渡す
                address = this._Machine.Load(r);
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show("ファイルを開くことに失敗しました。正しいファイルか確認してください。" + Environment.NewLine + "エラーの内容:" + ee.Message, "ファイル読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            finally
            {
                r.Close();
            }

            // ステータスの更新
            this.StatusLabel.Text = fileName+"の読み込み完了。ORGを0x"+Convert.ToString(address["org"], 16)+"に設定しました。";

            // MMViewのハイライト行を更新
            this.MMView.HighlightedIndex = (int)this.Machine.CurrentAddress;
            // MMViewをハイライト行までスクロール
	        this.MMView.FirstDisplayedScrollingRowIndex = Math.Max(this.Machine.CurrentAddress - this.Settings.MMViewScrollPositionFromTop, 0);

            // 表示を更新
            // 読み込みでメモリなどが更新されたため
            this.Display.Invalidate();

            // MainFormのウィンドウタイトルを変更
            this.Text = fileName + " ASC Simulator";

        }


        #region プライベート

        // マシン。実際にシミュレーションを実行する
        private Machine.ASC _Machine;

        // ブレークポイントのアドレスを格納する
        private List<ushort> _Breakpoints;

        // ステップランをしているか否か
        // 通常実行の場合はfalseが入る


        // コントロールを全て無効にする
        // 実行中などのユーザの操作を防ぐ為
        private void _DisableViews()
        {
            // シミュレートの実行中は別スレッドで動いているので、
            // Invokeを使用する
            Invoke(new MethodInvoker(() =>
            {
                //this.MMView.ReadOnly = true;
                //this.RegistersView.ReadOnly = true;
                this.MMView.Enabled = false;
                this.RegistersView.Enabled = false;
                this.InitStripButton.Enabled = false;
                this.MenuBar.Enabled = false;
                this.AllowDrop = false;
            }));
        }
        // コントロールを全て有効にする
        private void _EnableViews()
        {
            Invoke(new MethodInvoker(() =>
            {
                //this.MMView.ReadOnly = false;
                //this.RegistersView.ReadOnly = false;
                this.MMView.Enabled = true;
                this.RegistersView.Enabled = true;
                this.InitStripButton.Enabled = true;
                this.MenuBar.Enabled = true;
                this.AllowDrop = true;
            }));
        }
        // マシンの動作を停止する
        private void _StopMachine()
        {
            this.Machine.HLT();
            this._EnableViews();
            this.Display.Invalidate();
        }

        #endregion

        #region イベントハンドラ
        // 状態単位ステップラン
        private void 実行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 参照、書換の状態をリセット
            this.Display.ResetHighlights();

            // 1状態進める
            this.Machine.RunStep();

            // ユーザの操作を無効化
            this._DisableViews();
        }

        // 命令単位ステップラン
        private void ステップを進めるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 参照、書換の状態をリセット
            this.Display.ResetHighlights();
            // 実行
            this.Machine.RunCycle();

            // ユーザの操作を無効に
            this._DisableViews();
        }

        // 通常実行
        private void NextStepStripButton_Click(object sender, EventArgs e)
        {
            // 参照、書換の状態をリセット
            this.Display.ResetHighlights();
            // 実行
            this.Machine.Run();

            // ユーザの操作を無効化
            this._DisableViews();
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            // 実行速度を反映させる
            this.Settings.MachineSpeed = this.SpeedTrackBar.TrackBar.Value;
            // ステータスを更新
            this.SpeedLabel.Text = "実行速度: " + this.Settings.MachineSpeed.ToString() + "%";
        }

        private void PauseStripButton_Click(object sender, EventArgs e)
        {
            this._StopMachine();
        }

        private void MMView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // 編集されたのがニーモニック列なら、
            if (e.ColumnIndex ==  this.MMTable.Columns["ニーモニック"].Ordinal) {
                // 編集された内容を格納
                String mnemonic = this.MMTable.Rows[e.RowIndex]["ニーモニック"].ToString();

                #region 書式のバリデート
                // 空白で区切り配列へ
                String[] statements = mnemonic.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                Common.Defines.OPECODE opecode;
                ushort operand = 0;

                // セルが空白
                if (statements.Length == 0)
                {
                    this.MMTable.Rows[e.RowIndex].SetColumnError("ニーモニック", "何も入力されていません。");
                    this.MMView.InvalidateRow(e.RowIndex);
                    return;
                }

                // オペコード文字列をCommon.Defines.OPECODE列挙体へ変換
                opecode = Common.Defines.ToOPECODE(statements[0]);

                // 未知のオペコード
                if (opecode == Common.Defines.OPECODE.UNKNOWN)
                {
                    this.MMTable.Rows[e.RowIndex].SetColumnError("ニーモニック", statements[0]+"は存在しない命令です。");
                    this.MMView.InvalidateRow(e.RowIndex);
                    return;
                }

                if (opecode == Common.Defines.OPECODE.HLT)
                {
                    // HLTなのにオペランドがある
                    if (statements.Length != 1)
                    {
                        this.MMTable.Rows[e.RowIndex].SetColumnError("ニーモニック", "書式が正しくありません。");
                        this.MMView.InvalidateRow(e.RowIndex);
                        return;
                    }
                }
                else
                {
                    // HLT以外で、オペランドがない、または二つある場合
                    if (statements.Length != 2)
                    {
                        this.MMTable.Rows[e.RowIndex].SetColumnError("ニーモニック", "書式が正しくありません。");
                        this.MMView.InvalidateRow(e.RowIndex);
                        return;
                    }

                    try
                    {
                        // オペランドを数値へ変換
                        operand = Convert.ToUInt16(statements[1], 16);
                    }
                    catch
                    {
                        this.MMTable.Rows[e.RowIndex].SetColumnError("ニーモニック", statements[1] + "は数値ではありません。");
                        this.MMView.InvalidateRow(e.RowIndex);
                        return;
                    }

                }

                // オペランドが12bitの範囲を超えた場合
                if (operand > 4095)
                {
                    this.MMTable.Rows[e.RowIndex].SetColumnError("ニーモニック", "値が12ビットの範囲外です。");
                    this.MMView.InvalidateRow(e.RowIndex);
                    return;
                }

                #endregion

                #region 二進数へ変換
                // オペコードとオペランドから16bitの二進数を生成
                ushort line = (ushort)((int)Common.Defines.ToDecimal(opecode) << 12);
                line |= operand;

                #endregion

                // 参照、書換の状態をリセット
                this.Display.ResetHighlights();

                // エラーをクリア
                this.MMTable.Rows[e.RowIndex].ClearErrors();

                // メモリを書き換える
                this.Machine.Memory[(ushort)e.RowIndex] = line;
            }
            // 編集されたのがバイナリ列なら
            else if (e.ColumnIndex == this.MMTable.Columns["バイナリ"].Ordinal)
            {

                #region 書式のバリデート

                // 1文字ごとにバリデート
                foreach (char c in this.MMTable.Rows[e.RowIndex]["バイナリ"].ToString())
                {

                    // 0, 1以外の文字なら
                    if ((c < '0') || (c > '1'))
                    {
                        this.MMTable.Rows[e.RowIndex].SetColumnError("バイナリ", "値が二進数ではありません。");
                        this.MMView.InvalidateRow(e.RowIndex);
                        return;
                    }

                }

                // 16桁以上だった場合
                if (this.MMTable.Rows[e.RowIndex]["バイナリ"].ToString().Length != 16)
                {
                    this.MMTable.Rows[e.RowIndex].SetColumnError("バイナリ", "値が16桁ではありません。");
                    this.MMView.InvalidateRow(e.RowIndex);
                    return;
                }

                #endregion

                // 参照、書換の状態をリセット
                this.Display.ResetHighlights();

                // メモリを書き換える
                this.Machine.Memory[(ushort)e.RowIndex] = Convert.ToUInt16(this.MMTable.Rows[e.RowIndex]["バイナリ"].ToString(), 2);

                // エラーをクリア
                this.MMTable.Rows[e.RowIndex].ClearErrors();
            }

            // 表示を更新
            this.MMView.InvalidateRow(e.RowIndex);
            this.Display.Invalidate();
        }
        private void MMView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
        }


        private void RegistersView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // 編集されたのが値列だった場合
            if (e.ColumnIndex == this.RegistersTable.Columns["値"].Ordinal)
            {

                #region 変換
                // 文字列として格納
                String number = this.RegistersTable.Rows[e.RowIndex]["値"].ToString();
                ushort val = 0; // 変換後の数値としての値を格納する
                bool val_bool = false; // 変換後のboolとしての値を格納する

                try
                {

                    // 編集されたのがN、Zレジスタだった場合、boolとして変換
                    if ((string)this.RegistersTable.Rows[e.RowIndex]["名前"] == "N" ||
                        (string)this.RegistersTable.Rows[e.RowIndex]["名前"] == "Z")
                    {
                        if ((string)this.RegistersTable.Rows[e.RowIndex]["値"] == "0")
                        {
                            val_bool = false;
                        }
                        else if ((string)this.RegistersTable.Rows[e.RowIndex]["値"] == "1")
                        {
                            val_bool = true;
                        }
                        else { throw new Exception(); }
                    }
                    else
                    {
                        // 数値だった場合、変換を試みる
                        val = Convert.ToUInt16(number, 16);
                    }
                  
                }
                catch
                {
                    this.RegistersTable.Rows[e.RowIndex].SetColumnError("値", number + "は正常な値ではありません。");
                    this.RegistersView.InvalidateRow(e.RowIndex);
                    return;
                }

                #endregion

                // 参照、書換の状態をリセット
                this.Display.ResetHighlights();

                // レジスタの内容を書き換える
                switch ((string)this.RegistersTable.Rows[e.RowIndex]["名前"]) {
                    case "PC":
                        this.Machine.Registers.PC = val;
                        break;
                    case "IR":
                        this.Machine.Registers.IR = val;
                        break;
                    case "MAR":
                        this.Machine.Registers.MAR = val;
                        break;
                    case "R":
                        this.Machine.Registers.R = val;
                        break;
                    case "N":
                        this.Machine.Registers.N = val_bool;
                        break;
                    case "Z":
                        this.Machine.Registers.Z = val_bool;
                        break;
                }

                // 表示を更新
                this.RegistersTable.Rows[e.RowIndex].ClearErrors();
                this.RegistersView.InvalidateRow(e.RowIndex);

                this.Display.Invalidate();
            }
        }

        private void 設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Simulator.UI.Forms.SettingForm(this.Settings)).ShowDialog();
        }

        private void 表示を更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Display.Invalidate();
            this.MMView.Invalidate();
            this.RegistersView.Invalidate();

            this.StatusLabel.Text = "表示を更新。";
        }

        private void メモリを初期化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Machine.Memory.Init();

            this.StatusLabel.Text = "メモリの初期化が完了。";

            this.Display.Invalidate();
        }

        private void ブレークポイントを初期化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._Breakpoints.Clear();
            this.MMView.ClearChecks();

            this.StatusLabel.Text = "ブレークポイントの初期化が完了。";
        }

        private void このプログラムについてToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Simulator.UI.Forms.AboutBox()).ShowDialog();
        }

        private void 使い方ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Simulator.UI.Forms.HelpForm()).Show();
        }

        private void MMView_RowChecked(object sender, DataGridViewCellMouseEventArgs e)
        {
            this._Breakpoints.Add((ushort)e.RowIndex);
        }
        private void MMView_RowUnchecked(object sender, DataGridViewCellMouseEventArgs e)
        {
            this._Breakpoints.Remove((ushort)e.RowIndex);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 現在のディレクトリを設定に書き込む
            this.Settings.SimulatorCurrentPath = this.OpenDialog.InitialDirectory;

            // Setting.simcのパス
            string settingFile = Application.UserAppDataPath + "\\Settings.simc";


            // 設定をファイルに書き込む
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(settingFile, FileMode.Create))
                {
                    // 設定クラスをシリアライズ
                    bf.Serialize(fs, this.Settings);
                }
            }
            catch (Exception fe)
            {
            }

            this.Dispose();
        }

        private void 指定アドレスを表示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JumpBox jBox = new JumpBox();
            if (jBox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.MMView.ScrollToIndexRow((int)jBox.AddressUpDown.Value - this.Settings.MMViewScrollPositionFromTop);
            }
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void プログラムを開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenDialog.ShowDialog(this);
        }

        private void OpenDialog_FileOk(object sender, CancelEventArgs e)
        {

            this.OpenFile(this.OpenDialog.FileName);

        }

        private void ファイルへ書き出すToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveDialog.ShowDialog(this);
        }

        private void SaveDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.SaveFile(this.SaveDialog.FileName);
        }

        private void 初期化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 参照、書換の状態をリセットする
            this.Display.ResetHighlights();
            // 表示を更新
            this.Display.Invalidate();

            this.Machine.Registers.Init();

            // ステータスを更新
            this.StatusLabel.Text = "レジスタとフラグの初期化が完了";
        }
        
        private void InitStripButton_Click(object sender, EventArgs e)
        {
            // 参照、書換の状態をリセットする
            this.Display.ResetHighlights();
            // 表示を更新
            this.Display.Invalidate();

            // ステータスを更新
            this.StatusLabel.Text = "初期化が完了";

            // マシンを初期化
            this._Machine.Init();
            // ハイライト行をリセット
            this.MMView.HighlightedIndex = -1;

        }
  
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            }
            else {
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileName = (string[]) e.Data.GetData(DataFormats.FileDrop, false);

            this.OpenFile(fileName[0]);
        }

        #endregion

        #region サイクルイベントハンドラ
        private void DidCycleBegin()
        {
            Invoke(new MethodInvoker(() =>
            {
                // ステータスを更新
	            this.StatusLabel.Text = "命令サイクルを開始";
                // スクロール
                this.MMView.ScrollToIndexRow((int)this.Machine.CurrentAddress - this.Settings.MMViewScrollPositionFromTop);

                // レジスタの参照、書換イベントを無効に
                // 現在のアドレスがブレークポイントのリストに含まれていれば停止
                if (this._Breakpoints.Contains(this.Machine.CurrentAddress))
	            {
                    this._StopMachine();

                    this.StatusLabel.Text = "ブレークしました。";
	            }

            }));

            if (this.Machine.StopMode == Simulator.Machine.StopMode.DoNotStop)
            {
                this.Display.Invalidate();

                // シミュレーションスレッドを実行速度に合わせてだけ一時停止
                System.Threading.Thread.Sleep(this.Settings.MachineBaseSpeed - (int)(this.Settings.MachineBaseSpeed * ((double)this.Settings.MachineSpeed / 100.0)));
                // 参照、書換の状態をリセット
                //this.Display.ResetHighlights();
            }

        }
        private void DidCycleEnd()
        {
            Invoke(new MethodInvoker(() =>
            {
                //this.StatusLabel.Text = "命令サイクルを終了";

            }));

        }
        private void DidCycleDecode()
        {
            Invoke(new MethodInvoker(() =>
            {
                this.StatusLabel.Text = "S2: 操作コード(op)による分岐";

            }));
        }
        private void DidCycleOpecode()
        {
            Invoke(new MethodInvoker(() =>
            {

                // オペコードごとにステータスを変更
                switch (Common.Defines.ToOPECODE(this.Machine.Registers.IR >> 12))
                {
                    case Common.Defines.OPECODE.LD:
                        this.StatusLabel.Text = "S3: LD(op=0000)";
                        break;
                     case Common.Defines.OPECODE.ST:
                        this.StatusLabel.Text = "S4: ST(op=0001)";
                        break;
                     case Common.Defines.OPECODE.ADD:
                        this.StatusLabel.Text = "S5: ADD(op=0010)";
                        break;
                      case Common.Defines.OPECODE.SUB:
                        this.StatusLabel.Text = "S6: SUB(op=0011)";
                        break;
                     case Common.Defines.OPECODE.AND:
                        this.StatusLabel.Text = "S7: AND(op=0100)";
                        break;
                     case Common.Defines.OPECODE.OR:
                        this.StatusLabel.Text = "S8: OR(op=0101)";
                        break;
                     case Common.Defines.OPECODE.B:
                        this.StatusLabel.Text = "S9: B(op=0110)";
                        break;
                     case Common.Defines.OPECODE.BZ:
                        this.StatusLabel.Text = "S10: BZ(op=0111)";
                        break;
                     case Common.Defines.OPECODE.BN:
                        this.StatusLabel.Text = "S11: BN(op=1000)";
                        break;
                     case Common.Defines.OPECODE.HLT:
                        this.StatusLabel.Text = "S12: HLT(op=1111)";
                        break;
                }
            }));

        }
        private void DidCycleUpdatePC()
        {
            Invoke(new MethodInvoker(() =>
            {
                // ステータスを更新
                this.StatusLabel.Text = "S0: PCの設定";
            }));

        }
        private void DidCycleUpdateIR()
        {
            Invoke(new MethodInvoker(() =>
            {
                // ステータスを更新
                this.StatusLabel.Text = "S1: IRへの読み出し";

                // フェッチ後からハイライトする
                this.MMView.HighlightedIndex = (int)this.Machine.CurrentAddress;
            }));

        }

        private void DidStepped()
        {
            this._StopMachine();
        }

#endregion

        #region データ変更イベントハンドラ

        private void DidDataMoved(Common.DataMovedEventArgs dme)
        {
            // 操作単位ステップランの場合。
            // このステップランはボタンが配置されていなため、
            // 今現在使われていない
            if (this.Machine.StopMode == Simulator.Machine.StopMode.PerOperation)
            {
                // 表示を更新
                this.Display.Invalidate();
                // 実行速度に基づいてマシンを一時停止
                System.Threading.Thread.Sleep(this.Settings.MachineBaseSpeed - (int)(this.Settings.MachineBaseSpeed * ((double)this.Settings.MachineSpeed / 100.0)));
            }
        }
        private void DidDataChanged(Common.DataEventArgs de)
        {
            Invoke(new MethodInvoker(() =>
            {
                // PCが更新されたとき
                if (de.Key == "PC")
                {
                    this.MMView.SecondaryHighlightedIndex = (int)((ushort)de.Data);
                }

                // N、Zが書き換えられたとき
                if (de.Key == "N" || de.Key == "Z")
                {
                    this.RegistersTable.Rows.Find(de.Key)["値"] = Convert.ToInt32((bool)de.Data).ToString();
                }
                // その他(PCの更新も含む)
                else if (de.Key != "Controller")
                {
                    this.RegistersTable.Rows.Find(de.Key)["値"] = "0x" + Convert.ToString((ushort)de.Data, 16);
                }
            }));
        }
        private void DidDataAccessed(Common.DataEventArgs de)
        {

        }
        private void DidMemoryChanged(Common.MemoryEventArgs me)
        {
            // 変更されたメモリの内容を解析
            for (int i = me.StartAddress; i <= me.EndAddress; i++ )
            {
                ushort line = 0;
                ushort opecode = 0;
                ushort operand = 0;

                line = me.Memory[i];
                opecode = (ushort)(line >> 12); // オペコード取り出す
                operand = (ushort)(line & 4095); // オペランドを取り出す

                // ニーモニックに変換して更新
                // HLTの場合は、オペランドを省く
                if (Common.Defines.ToOPECODE((int)opecode) == Common.Defines.OPECODE.HLT) {
                    this.MMTable.Rows[i]["ニーモニック"] = Common.Defines.ToString((int)opecode);
                } else {
                    this.MMTable.Rows[i]["ニーモニック"] = Common.Defines.ToString((int)opecode) + " 0x" + Convert.ToString(operand, 16);
                }
                
                // バイナリ列に二進数を書き込み
                this.MMTable.Rows[i]["バイナリ"] = Convert.ToString(line, 2).PadLeft(16, '0');
            }
        }
        private void DidMemoryAccessed(Common.MemoryEventArgs me)
        {

        }

        #endregion

    }
}
