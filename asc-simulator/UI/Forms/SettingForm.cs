/*
 * SettingForm.cs
 * 設定ダイアログ
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

namespace Simulator.UI.Forms
{
    public partial class SettingForm : Form
    {
        public SettingForm(Simulator.Common.ApplicationSettings settings)
        {
            InitializeComponent();

            // ApplicationSettingsを格納
            this.Settings = settings;

            // 設定項目を反映させる
            this.RefreshComponents();
        }
        public Simulator.Common.ApplicationSettings Settings;

        // 設定項目を更新する
        public void RefreshComponents() {
            this.BaseSpeedUpDown.Value = (decimal)this.Settings.MachineBaseSpeed;
            this.StartupInitializeCheck.Checked = this.Settings.MachineShouldInitializeWithStartup;
            this.RegisterColorNormalInput.Text = this.Settings.RegisterColorNormal;
            this.RegisterColorRefecenceInput.Text = this.Settings.RegisterColorReference;
            this.RegisterColorModifyInput.Text = this.Settings.RegisterColorModify;
            this.RegisterMarginUpDown.Value = this.Settings.RegisterMargin;
            this.ConnectorColorActiveInput.Text = this.Settings.ConnectorColorActive;
            this.ConnectorColorNormalInput.Text = this.Settings.ConnectorColorNormal;
            this.ConnectorWidthUpDown.Value = this.Settings.ConnectorWidth;
        }

        // それぞれの設定項目とApplicationSettingsを割り当てる
        private void StartupInitializeCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.Settings.MachineShouldInitializeWithStartup = this.StartupInitializeCheck.Checked;
        }

        private void BaseSpeedUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.Settings.MachineBaseSpeed = (int)this.BaseSpeedUpDown.Value;
        }

        private void ConnectorColorNormalInput_TextChanged(object sender, EventArgs e)
        {
            this.Settings.ConnectorColorNormal = this.ConnectorColorNormalInput.Text;
        }

        private void RegisterMarginUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.Settings.RegisterMargin = (int)this.RegisterMarginUpDown.Value;
        }

        private void ConnectorWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.Settings.ConnectorWidth = (int)ConnectorWidthUpDown.Value;
        }

        private void ConnectorColorActiveInput_TextChanged(object sender, EventArgs e)
        {
            this.Settings.ConnectorColorActive = this.ConnectorColorActiveInput.Text;
        }

        private void RegisterColorNormalInput_TextChanged(object sender, EventArgs e)
        {
            this.Settings.RegisterColorNormal = this.RegisterColorNormalInput.Text;
        }

        private void RegisterColorRefecenceInput_TextChanged(object sender, EventArgs e)
        {
            this.Settings.RegisterColorReference = this.RegisterColorRefecenceInput.Text;
        }

        private void RegisterColorModifyInput_TextChanged(object sender, EventArgs e)
        {
            this.Settings.RegisterColorModify = this.RegisterColorModifyInput.Text;
        }

        // ToDo: 名前を変更する
        private void button1_Click(object sender, EventArgs e)
        {
            this.Settings = new Common.ApplicationSettings();
            this.RefreshComponents();
        }

        // ToDo: 名前を変更する
        // ToDo: そもそも関連付けに対応させるかどうか
        private void button2_Click(object sender, EventArgs e)
        {
            //関連付ける拡張子
            string extension = ".asco";
			//実行するコマンドライン
			string commandline = "\"" + Application.ExecutablePath + "\" %1";
			//ファイルタイプ名
			string fileType = Application.ProductName;
			//説明（「ファイルの種類」として表示される）
			//（必要なし）
			string description = "ASC Simulator";
			//動詞
			string verb = "open";
			//動詞の説明（エクスプローラのコンテキストメニューに表示される）
			//（必要なし）
			string verb_description = "ASC Simulatorで開く(&O)";
			//アイコンのパスとインデックス
			string iconPath =Application.ExecutablePath;
			int iconIndex = 0;

			//ファイルタイプを登録
			Microsoft.Win32.RegistryKey regkey =
			    Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(extension);
			regkey.SetValue("", fileType);
			regkey.Close();

			//ファイルタイプとその説明を登録
			Microsoft.Win32.RegistryKey shellkey =
			    Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(fileType);
			shellkey.SetValue("", description);

			//動詞とその説明を登録
			shellkey = shellkey.CreateSubKey("shell\\" + verb);
			shellkey.SetValue("", verb_description);

			//コマンドラインを登録
			shellkey = shellkey.CreateSubKey("command");
			shellkey.SetValue("", commandline);
			shellkey.Close();

			//アイコンの登録
			Microsoft.Win32.RegistryKey iconkey =
			    Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(
			    fileType + "\\DefaultIcon");
			iconkey.SetValue("", iconPath + "," + iconIndex.ToString());
			iconkey.Close();
        }

        // ToDo: 名前を変える
        private void button3_Click(object sender, EventArgs e)
        {
            //拡張子
            string extension = ".asco";
			//ファイルタイプ名
			string fileType = Application.ProductName;

			//レジストリキーを削除
			Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(extension);
			Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(fileType);
        }

    }
}
