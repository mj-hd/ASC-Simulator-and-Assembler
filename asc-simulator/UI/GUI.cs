/*
 * GUI.cs 
 * MainFormを管理するクラス
 * 他の方法でUIを表示する場合にも対応できるように、ProgramクラスとFormクラスの間に挟んでいる
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.UI
{
    class GUI
    {
        public GUI()
        {
            this._MainForm = new Forms.MainForm();
        }

        // メインフォームを表示する
        public void ShowWindow()
        {
            System.Windows.Forms.Application.Run(this._MainForm);
        }

        // エラーを表示する
        // message: エラーメッセージ
        public void ShowError(string message)
        {
            System.Windows.Forms.MessageBox.Show("エラーが発生したため、プログラムを終了します。"+Environment.NewLine+"「"+message+"」", "エラー", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        // ファイルを開く
        // ToDo: うまいこと開けない
        public void OpenFile(string file)
        {
            this._MainForm.Shown += (sender, e) =>
            {
                this._MainForm.OpenFile(file);
            };
        }

        public Machine.ASC Machine
        {
            set
            {
                this._MainForm.Machine = value;

                // マシンのイベントに、MachineDisplayを割り当てる
                this._MainForm.Machine.CycleBegin += this._MainForm.Display.DidCycleBegin;
                this._MainForm.Machine.CycleEnd += this._MainForm.Display.DidCycleEnd;
                this._MainForm.Machine.CycleDecode += this._MainForm.Display.DidCycleDecode;
                this._MainForm.Machine.CycleOpecode += this._MainForm.Display.DidCycleOpecode;
                this._MainForm.Machine.CycleUpdateIR += this._MainForm.Display.DidCycleUpdateIR;
                this._MainForm.Machine.CycleUpdatePC += this._MainForm.Display.DidCycleUpdatePC;

                this._MainForm.Machine.PreDataMoved += this._MainForm.Display.DidPreDataMoved;

                this._MainForm.Machine.Registers.DataChanged += this._MainForm.Display.DidDataChanged;
                this._MainForm.Machine.Registers.DataAccessed += this._MainForm.Display.DidDataAccessed;
                this._MainForm.Machine.ALU.DataChanged += this._MainForm.Display.DidDataChanged;
                this._MainForm.Machine.ALU.DataAccessed += this._MainForm.Display.DidDataAccessed;
                this._MainForm.Machine.Memory.MemoryChanged += this._MainForm.Display.DidMemoryChanged;
                this._MainForm.Machine.Memory.MemoryAccessed += this._MainForm.Display.DidMemoryAccessed;

                this._MainForm.Machine.ALU.Overflowed += this._MainForm.Display.DidOverflowed;
            }
        }

        private UI.Forms.MainForm _MainForm;
    }
}
