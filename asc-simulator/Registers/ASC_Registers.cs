/*
 * ASC_Registers.cs
 * ASCのレジスタをシミュレートするクラス
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Simulator.Common;

namespace Simulator.Registers
{
    public class ASC_Registers
    {
        // レジスタが変更されたときに発生するイベント
        public event DataEventHandler DataChanged;
        public event DataEventHandler DataAccessed;


        // レジスタ参照・書換のイベントを発生させるか否か
        public bool IsEventEnabled = true;


        // Rレジスタ
        public ushort R
        {
            get
            {
                this.OnDataAccessed(new DataEventArgs(this._R, "R"));
                return this._R;
            }
            set
            {
                this._R = value;
                this.OnDataChanged(new DataEventArgs(this._R, "R"));
            }
        }

        // PCレジスタ
        public ushort PC
        {
            get
            {
                this.OnDataAccessed(new DataEventArgs(this._PC, "PC"));
                return this._PC;
            }
            set
            {
                this._PC = value;
                this.OnDataChanged(new DataEventArgs(this._PC, "PC"));
            }
        }

        // IRレジスタ
        public ushort IR
        {
            get
            {
                this.OnDataAccessed(new DataEventArgs(this._IR, "IR"));
                return this._IR;
            }
            set
            {
                this._IR = value;
                this.OnDataChanged(new DataEventArgs(this._IR, "IR"));
            }
        }

        // MARレジスタ
        public ushort MAR
        {
            get
            {
                this.OnDataAccessed(new DataEventArgs(this._MAR, "MAR"));
                return this._MAR;
            }
            set
            {
                this._MAR = value;
                this.OnDataChanged(new DataEventArgs(this._MAR, "MAR"));
            }
        }

        // Nレジスタ
        public bool N
        {
            get
            {
                this.OnDataAccessed(new DataEventArgs(this._N, "N"));
                return this._N;
            }
            set
            {
                this._N = value;
                this.OnDataChanged(new DataEventArgs(this._N, "N"));
            }
        }

        // Zレジスタ
        public bool Z
        {
            get
            {
                this.OnDataAccessed(new DataEventArgs(this._Z, "Z"));
                return this._Z;
            }
            set
            {
                this._Z = value;
                this.OnDataChanged(new DataEventArgs(this._Z, "Z"));
            }
        }

        // 便宜上の制御部(値の格納とイベントの発生を管理するだけ)
        public Common.Defines.OPECODE Controller
        {
            get
            {
                this.OnDataAccessed(new DataEventArgs(this._Controller, "Controller"));
                return this._Controller;
            }
            set
            {
                this._Controller = value;
                this.OnDataChanged(new DataEventArgs(this._Controller, "Controller"));
            }
        }


        public ASC_Registers()
        {
            this.DataAccessed = (de) => { };
            this.DataChanged = (de) => { };
            Init();
        }
        ~ASC_Registers()
        {

        }

        // レジスタの内容を初期化する
        public void Init()
        {
            this.R = 0;
            this.PC = 0;
            this.IR = 0;
            this.MAR = 0;
            this.N = false;
            this.Z = false;
        }

        #region イベント
        // レジスタが変更・参照されたときに発生するイベント
        protected void OnDataChanged(DataEventArgs de)
        {
            if (this.IsEventEnabled)
                this.DataChanged(de);
        }
        protected void OnDataAccessed(DataEventArgs de)
        {
            if (this.IsEventEnabled)
                this.DataAccessed(de);
        }
        #endregion

        private ushort _R;
        private ushort _PC;
        private ushort _IR;
        private ushort _MAR;
        private bool _N;
        private bool _Z;
        private Common.Defines.OPECODE _Controller;
    }
}
