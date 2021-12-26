/*
 * ASC.cs
 * ASCのシミュレートを行うクラス
 * 
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Simulator.Machine
{
    #region ALUクラスとその他の型
    // マシンの実行モード
    public enum StopMode
    {
        DoNotStop, // 通常実行
        PerCycle, // 命令単位命令単位ステップラン
        PerStep,  // 状態単位ステップラン
        PerOperation // 操作単位ステップラン
    }

    // ALUクラス
    // ALUの処理をシミュレートする
    public class ALU
    {
        // N, Zが変更されたときに呼び出されるイベントハンドラ
        public delegate void AssignEventHandler();

        // N, Zレジスタを書き換えるべき時に発生するイベント
        // ALUクラスを利用するインスタンスは、このイベントハンドラを必ず割り当てるべき
        public event AssignEventHandler AssignNTrue;
        public event AssignEventHandler AssignNFalse;
        public event AssignEventHandler AssignZTrue;
        public event AssignEventHandler AssignZFalse;

        // ALUクラスにおいてデータが変更・参照された時に発生するイベント
        public event Common.DataEventHandler DataChanged;
        public event Common.DataEventHandler DataAccessed;

        // ALUクラスにおいてオーバフローが発生した時に発生するイベント
        public event Common.OverflowedEventHandler Overflowed;


        // N, Zレジスタを書き換えるか否か
        public bool IsAssignZFlagEnabled = true;
        public bool IsAssignNFlagEnabled = true;

        // ["+"]のように実行したい演算子を指定することで、
        // 左辺と右辺の値に基づいて計算結果を返す
        public ushort this[string ope]
        {
            get
            {
                ushort result = 0;

                switch (ope)
                {
                    case "+":
                        result = (ushort)(this._Left + this._Right);

                        if ((this._Left >> 15 == 0) && (this._Right >> 15 == 0) && (result >> 15 == 1))
                        {
                            this.Overflowed(new Common.OverflowedEventArgs("+"));
                        }
                        if ((this._Left >> 15 == 1) && (this._Right >> 15 == 1) && (result >> 15 == 0))
                        {
                            this.Overflowed(new Common.OverflowedEventArgs("+"));
                        }

                        break;
                    case "-":
                        result = (ushort)(this._Left - this._Right);

                        if ((this._Left >> 15 == 0) && (this._Right >> 15 == 1) && (result >> 15 == 1))
                        {
                            this.Overflowed(new Common.OverflowedEventArgs("-"));
                        }
                        if ((this._Left >> 15 == 1) && (this._Right >> 15 == 0) && (result >> 15 == 0))
                        {
                            this.Overflowed(new Common.OverflowedEventArgs("-"));
                        }

                        break;
                    case "*":
                        result = (ushort)(this._Left * this._Right);
                        break;
                    case "/":
                        result = (ushort)(this._Left / this._Right);
                        break;
                    case "&":
                        result = (ushort)(this._Left & this._Right);
                        this.IsAssignNFlagEnabled = false;
                        break;
                    case "|":
                        result = (ushort)(this._Left | this._Right);
                        this.IsAssignNFlagEnabled = false;
                        break;
                    case "^":
                        result = (ushort)(this._Left ^ this._Right);
                        this.IsAssignNFlagEnabled = false;
                        break;
                }

                // Z, Nフラグを書き換えさせる
                // 最上位ビットが立っていれば
                if (result >> 15 == 1)
                {
                    if (this.IsAssignNFlagEnabled) this.AssignNTrue();
                    if (this.IsAssignZFlagEnabled) this.AssignZFalse();
                }
                // 数値が0なら
                else if (result == 0)
                {
                    if (this.IsAssignNFlagEnabled) this.AssignNFalse();
                    if (this.IsAssignZFlagEnabled) this.AssignZTrue();
                }
                else
                {
                    if (this.IsAssignNFlagEnabled) this.AssignNFalse();
                    if (this.IsAssignZFlagEnabled) this.AssignZFalse();
                }
                

                // データ変更イベントを発生させる
                this.OnDataChanged(new Common.DataEventArgs(ope, "ALUOperator"));
                this.OnDataChanged(new Common.DataEventArgs(result, "ALU"));

                return result;
            }
        }

        // 演算子の左辺
        public ushort Left {
            get
            {
                this.OnDataAccessed(new Common.DataEventArgs(this._Left, "ALULeft"));
                return this._Left;
            }
            set
            {
                this._Left = value;
                this.OnDataChanged(new Common.DataEventArgs(this._Left, "ALULeft"));
            }
        }

        // 演算子の右辺
        public ushort Right
        {
            get
            {
                this.OnDataAccessed(new Common.DataEventArgs(this._Right, "ALURight"));
                return this._Right;
            }
            set
            {
                this._Right = value;
                this.OnDataChanged(new Common.DataEventArgs(this._Right, "ALURight"));
            }
        }


        public ALU()
        {
            this.AssignNFalse += () => { };
            this.AssignNTrue += () => { };
            this.AssignZFalse += () => { };
            this.AssignZTrue += () => { };
            this.DataAccessed += (de) => { };
            this.DataChanged += (de) => { };
            this.Overflowed += (ove) => { };
        }


        // データ変更・参照イベント
        protected void OnDataChanged(Common.DataEventArgs de)
        {
            this.DataChanged(de);
        }
        protected void OnDataAccessed(Common.DataEventArgs de)
        {
            this.DataAccessed(de);
        }

        private ushort _Right;
        private ushort _Left;
    }

    #endregion

    public class ASC : IDisposable
    {
        public interface ILoadable {
            ushort ToUShort();
        }

        // ニーモニックを格納する構造体
        public struct MNEMONIC: ILoadable
        {
            public Common.Defines.OPECODE Opecode;
            public ushort Operand; // ASCの場合は１つ。

            public ushort ToUShort()
            {
                return (ushort)(((ushort)Opecode << 12) | (Operand & 0xFFF));
            }
        }


        // 命令サイクル中に呼び出されるイベント
        public event Common.CycleEventHandler CycleBegin;
        public event Common.CycleEventHandler CycleEnd;
        public event Common.CycleEventHandler CycleDecode;
        public event Common.CycleEventHandler CycleUpdateIR;
        public event Common.CycleEventHandler CycleOpecode;
        public event Common.CycleEventHandler CycleUpdatePC;

        // ステップ実行が終了したとき
        public event Common.CycleEventHandler Stepped;

        // データ変更イベント
        public event Common.DataMovedEventHandler PreDataMoved;
        public event Common.DataMovedEventHandler DataMoved;

        public Registers.ASC_Registers Registers;
        public Memory.ASC_Memory Memory;
        public ALU ALU;

        // 実行モード
        public StopMode StopMode
        {
            get
            {
                return this._Mode;
            }
        }
        public ushort CurrentAddress
        {
            get
            {
                return this._CurrentAddress;
            }
        }


        public ASC()
        {
            this.Registers = new Registers.ASC_Registers();
            this.Memory = new Memory.ASC_Memory();
            this.ALU = new ALU();

            // N, Zレジスタを書き換えるイベントハンドラ
            this.ALU.AssignZTrue += () => {
                this.Registers.Z = true;
                this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "Z"));
            };
            this.ALU.AssignZFalse += () => {
                this.Registers.Z = false;
                this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "Z"));
            };
            this.ALU.AssignNTrue += () => {
                this.Registers.N = true;
                this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "N"));
            };
            this.ALU.AssignNFalse += () =>
            {
                this.Registers.N = false;
                this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "N"));
            };

            this.ALU.Overflowed += (ove) =>
            {
                // オーバーフロー発生時は次の状態で停止する
                this.HLT(StopMode.PerStep);
            };

            this.CycleBegin += (ce) => { };
            this.CycleEnd += (ce) => { };
            this.CycleDecode += (ce) => { };
            this.CycleUpdateIR += (ce) => { };
            this.CycleOpecode += (ce) => { };
            this.CycleUpdatePC += (ce) => { };

            this.Stepped += (ce) => { };

            this.PreDataMoved += (dme) => { };
            this.DataMoved += (dme) => { };

            // スレッドの実行を停止するためのResetEvent
            this._CycleResetEvent = new ManualResetEvent(false);
            this._StepResetEvent = new ManualResetEvent(false);
            this._OperateResetEvent = new ManualResetEvent(false);

            // 初期状態を命令単位ステップランに設定
            this._Mode = StopMode.PerCycle;

            this._CurrentAddress = 0;
            this._InstructionCount = 0;
            this._Initializing = true;

            // スレッドを作成
            this._LoopThread = new Thread(new ThreadStart(this._Loop));
            this._LoopThread.Start();
        }

        public void Dispose() {
            // _LoopThreadは自動でDisposeされるためここでは呼ばない
        }

        public void Init()
        {
            this.Registers.Init();

            this._CurrentAddress = 0;
            this._InstructionCount = 0;
            this._Initializing = true;

            // ループスレッドのリセット
            // DEPRECATED: Abortの使用を辞める
            this._LoopThread.Abort();
            this._LoopThread.Join();
            this._LoopThread = new Thread(new ThreadStart(this._Loop));
            this._LoopThread.Start();
        }

        // 通常実行
        public void Run(StopMode mode = StopMode.DoNotStop)
        {
            this._Mode = mode;

            this._CycleResetEvent.Set();
            this._OperateResetEvent.Set();
            this._StepResetEvent.Set();
        }

        // 命令単位ステップラン
        public void RunCycle()
        {
            this.Run(StopMode.PerCycle);
        }

        // 状態単位ステップランを実行
        public void RunStep()
        {
            this.Run(StopMode.PerStep);
        }

        // 操作単位ステップランを実行
        public void RunOperate()
        {
            this.Run(StopMode.PerOperation);
        }

        // マシンを停止する
        public void HLT(StopMode? nextMode = null)
        {
            // 実行モード別のResetEventをリセットしてスレッドを止める
            switch (nextMode ?? this._Mode)
            {
                case StopMode.DoNotStop:
                case StopMode.PerCycle:
                    this._CycleResetEvent.Reset();
                    break;
                case StopMode.PerStep:
                    this._StepResetEvent.Reset();
                    break;
                case StopMode.PerOperation:
                    this._OperateResetEvent.Reset();
                    break;
            }
        }

        // Streamからasco形式のバイナリを読み込む
        // r: asco形式のバイナリのStream
        // 戻り値としてDictionary<string, ushort>型のインスタンスを返す。
        // 戻り値には["begin"], ["org"], ["size"], ["end"]の四つの要素が含まれている。
        // begin: 読み込んだメモリ上の開始位置
        // end  : 読み込んだメモリ上の終了位置
        // size : 読み込んだサイズ
        // org  : 読み込みを開始した位置(beginと同じ値)
        public Dictionary<String, ushort> Load(Stream r)
        {
            Dictionary<String, ushort> result = new Dictionary<String,ushort>();

            BinaryReader br = new BinaryReader(r);
            r.Seek(0, SeekOrigin.Begin);

            #region ヘッダの解析
            ushort firstLine = br.ReadUInt16();
            ushort headerSize = (ushort)(firstLine >> 12);
            ushort org = (ushort)(firstLine - (headerSize << 12));

            /* 今後headerが追加された場合の処理 */

            #endregion

            // メモリを初期化して、バイナリを読み込む
            this.Memory.Init();
            this.Memory.Load(br, org);

            // 結果を格納
            result["begin"] = org;
            result["org"]  = org;
            result["size"] = (ushort)((r.Position - headerSize) / 2);
            result["end"]  = (ushort)(result["size"] + result["org"]);

            // PCレジスタをorgで書き換える
            this.Registers.PC = org;
            this._CurrentAddress = org;

            return result;
        }

        // Streamにasco形式でメモリを書き出す
        // w: 書き込むStream
        // 戻り値としてLoadメソッドと同じ形式のインスタンスを返す。
        public Dictionary<String, ushort> Save(Stream w)
        {
            Dictionary<String, ushort> result = new Dictionary<String, ushort>();

            BinaryWriter bw = new BinaryWriter(w);

            #region ヘッダの作成
            ushort org = 0;
            ushort headerSize = 2;
            ushort firstLine = (ushort)(org + (headerSize << 12));

            bw.Write(firstLine);

            /* 今後headerが追加された場合の処理 */

            #endregion

            // メモリの内容を書き出す
            this.Memory.Save(bw);

            // 結果を準備
            result["begin"] = org;
            result["org"] = org;
            result["size"] = (ushort)((w.Position - headerSize) / 2);
            result["end"] = (ushort)(result["size"] + result["org"]);

            return result;
        }


        #region プライベート

        private StopMode _Mode;
        private ushort _CurrentAddress;
        private Thread _LoopThread;
        private ManualResetEvent _CycleResetEvent;
        private ManualResetEvent _StepResetEvent;
        private ManualResetEvent _OperateResetEvent;
        private bool _Initializing;
        private uint _InstructionCount;

        // 実際にASCの処理をシミュレートするループ
        // このループを別スレッドで実行する
        private void _Loop()
        {
            // TODO: 全体的にMutexが必要
            try
            {
                while (true)
                {
                    #region CycleBegin
                    this.Registers.IsEventEnabled = false;
                    this._CurrentAddress = this.Registers.PC;

                    // 初期化後の初回実行時の処理
                    if (this._Initializing)
                    {
                        this._InstructionCount = 0;
                        this._Initializing = false;
                    }
                    else
                    {
                        this._InstructionCount += 1;
                    }

                    this.Registers.IsEventEnabled = true;
                    this.OnCycleBegin();

                    // ResetEventを待つ
                    this._CycleResetEvent.WaitOne();
                    this._StepResetEvent.WaitOne();
                    #endregion

                    // MARにPCを
                    this.Registers.MAR = this.Registers.PC;
                    #region PCtoMAR
                    this.OnDataMoved(new Common.DataMovedEventArgs("PC", "MAR"));
                    this._OperateResetEvent.WaitOne();
                    #endregion

                    // PCを更新
                    this.ALU.Right = this.Registers.PC;
                    #region PCtoALULeft
                    this.OnDataMoved(new Common.DataMovedEventArgs("PC", "ALULeft"));
                    this._OperateResetEvent.WaitOne();
                    #endregion
                    this.ALU.Left = 1;
                    #region 1toALURight
                    this.OnDataMoved(new Common.DataMovedEventArgs("1", "ALURight"));
                    this._OperateResetEvent.WaitOne();
                    #endregion

                    this.ALU.IsAssignNFlagEnabled = false;
                    this.ALU.IsAssignZFlagEnabled = false;
                    this.Registers.PC = this.ALU["+"];
                    this.ALU.IsAssignNFlagEnabled = true;
                    this.ALU.IsAssignZFlagEnabled = true;
                    #region ALUtoPC
                    this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "PC"));
                    this._OperateResetEvent.WaitOne();
                    #endregion

                    #region CycleUpdatePC
                    this.OnCycleUpdatePC();
                    this._StepResetEvent.WaitOne();
                    #endregion

                    // IRにメモリのMAR番地の内容を移動
                    this.Registers.IR = this.Memory[this.Registers.MAR];
                    #region MMtoIR
                    this.OnDataMoved(new Common.DataMovedEventArgs("MM", "IR"));
                    this._OperateResetEvent.WaitOne();
                    #endregion

                    #region CycleUpdateIR
                    this.OnCycleUpdateIR();
                    this._StepResetEvent.WaitOne();
                    #endregion

                    // IRの内容を解析して、MNEMONIC構造体へ格納
                    MNEMONIC mnemonic = this._Decode(this.Registers.IR);
                    this.Registers.MAR = mnemonic.Operand;
                    #region IRtoMAR
                    this.OnDataMoved(new Common.DataMovedEventArgs("IR", "MAR"));
                    #endregion
                    this.Registers.Controller = mnemonic.Opecode;
                    #region IRtoController
                    this.OnDataMoved(new Common.DataMovedEventArgs("IR", "Controller"));
                    this._OperateResetEvent.WaitOne();
                    #endregion

                    #region CycleDecode
                    this.OnCycleDecode();
                    this._StepResetEvent.WaitOne();
                    #endregion

                    switch (this.Registers.Controller)
                    {
                        case Common.Defines.OPECODE.LD:
                            this.Registers.R = this.Memory[this.Registers.MAR];
                            #region MMtoR
                            // データ変更イベントを発生させる
                            this.OnDataMoved(new Common.DataMovedEventArgs("MM", "R"));
                            // 操作単位ステップランのResetEventを待つ
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            break;
                        case Common.Defines.OPECODE.ST:
                            this.Memory[this.Registers.MAR] = this.Registers.R;
                            #region RtoMM
                            this.OnDataMoved(new Common.DataMovedEventArgs("R", "MM"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            break;
                        case Common.Defines.OPECODE.ADD:
                            this.ALU.Left = this.Registers.R;
                            #region RtoALURight
                            this.OnDataMoved(new Common.DataMovedEventArgs("R", "ALURight"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.ALU.Right = this.Memory[this.Registers.MAR];
                            #region MMtoALULeft
                            this.OnDataMoved(new Common.DataMovedEventArgs("MM", "ALULeft"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.Registers.R = this.ALU["+"];
                            #region ALUtoR
                            this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "R"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            break;
                        case Common.Defines.OPECODE.SUB:
                            this.ALU.Left = this.Registers.R;
                            #region RtoALURight
                            this.OnDataMoved(new Common.DataMovedEventArgs("R", "ALURight"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.ALU.Right = this.Memory[this.Registers.MAR];
                            #region MMtoALULeft
                            this.OnDataMoved(new Common.DataMovedEventArgs("MM", "ALULeft"));

                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.Registers.R = this.ALU["-"];
                            #region ALUtoR
                            this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "R"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            break;
                        case Common.Defines.OPECODE.AND:
                            this.ALU.Left = this.Registers.R;
                            #region RtoALURight
                            this.OnDataMoved(new Common.DataMovedEventArgs("R", "ALURight"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.ALU.Right = this.Memory[this.Registers.MAR];
                            #region MMtoALULeft
                            this.OnDataMoved(new Common.DataMovedEventArgs("MM", "ALULeft"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.Registers.R = this.ALU["&"];
                            #region ALUtoR
                            this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "R"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            break;
                        case Common.Defines.OPECODE.OR:
                            this.ALU.Left = this.Registers.R;
                            #region RtoALURight
                            this.OnDataMoved(new Common.DataMovedEventArgs("R", "ALURight"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.ALU.Right = this.Memory[this.Registers.MAR];
                            #region MMtoALULeft
                            this.OnDataMoved(new Common.DataMovedEventArgs("MM", "ALULeft"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            this.Registers.R = this.ALU["|"];
                            #region ALUtoR
                            this.OnDataMoved(new Common.DataMovedEventArgs("ALU", "R"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            break;
                        case Common.Defines.OPECODE.B:
                            // IRからオペランド部分をPCへ。
                            this.Registers.PC = (ushort)(this.Registers.IR & 4095);
                            #region IRtoPC
                            this.OnDataMoved(new Common.DataMovedEventArgs("IR", "PC"));
                            this._OperateResetEvent.WaitOne();
                            #endregion
                            break;
                        case Common.Defines.OPECODE.BZ:
                            if (this.Registers.Z)
                            {
                                this.Registers.PC = (ushort)(this.Registers.IR & 4095);
                                #region IRtoPC
                                this.OnDataMoved(new Common.DataMovedEventArgs("IR", "PC"));
                                this._OperateResetEvent.WaitOne();
                                #endregion
                            }
                            break;
                        case Common.Defines.OPECODE.BN:
                            if (this.Registers.N)
                            {
                                this.Registers.PC = (ushort)(this.Registers.IR & 4095);
                                #region IRtoPC
                                this.OnDataMoved(new Common.DataMovedEventArgs("IR", "PC"));
                                this._OperateResetEvent.WaitOne();
                                #endregion
                            }
                            break;
                        // 以下はダミーのオペコード
                        case Common.Defines.OPECODE.OPE1:
                            break;
                        case Common.Defines.OPECODE.OPE2:
                            break;
                        case Common.Defines.OPECODE.OPE3:
                            break;
                        case Common.Defines.OPECODE.OPE4:
                            break;
                        case Common.Defines.OPECODE.OPE5:
                            break;
                        case Common.Defines.OPECODE.OPE6:
                            break;

                        case Common.Defines.OPECODE.HLT:
                            this.HLT();
                            break;
                    }

                    #region CycleOpecode
                    this.OnCycleOpecode();
                    this._StepResetEvent.WaitOne();
                    #endregion

                    this.OnCycleEnd();
                    this._StepResetEvent.WaitOne();
                }
            } catch (ThreadAbortException)
            {
                return;
            }
        }

        // 16bitの数値をMNEMONIC構造体へ変換する
        private MNEMONIC _Decode(ushort line)
        {
            MNEMONIC mnemonic = new MNEMONIC();

            ushort opecode = (ushort)(line >> 12);

            mnemonic.Opecode = Common.Defines.ToOPECODE(opecode);
            mnemonic.Operand = (ushort)(line - ((line >> 12) << 12));

            return mnemonic;
        }

        #endregion

        #region イベント

        protected void OnCycleBegin()
        {
            this.CycleBegin(new Common.CycleEventArgs(this._InstructionCount));

            if ((this._Mode == Machine.StopMode.PerCycle) ||
                (this._Mode == Machine.StopMode.PerOperation) ||
                (this._Mode == Machine.StopMode.PerStep))
            {
                this.OnStepped();
            }
        }
        protected void OnCycleEnd()
        {
            this.CycleEnd(new Common.CycleEventArgs(this._InstructionCount));
        }
        protected void OnCycleDecode()
        {
            this.CycleDecode(new Common.CycleEventArgs(this._InstructionCount));

            if ((this._Mode == Machine.StopMode.PerStep) ||
                (this._Mode == Machine.StopMode.PerOperation))
            {
                this.OnStepped();
            }
        }
        protected void OnCycleOpecode()
        {
            this.CycleOpecode(new Common.CycleEventArgs(this._InstructionCount));

            if ((this._Mode == Machine.StopMode.PerStep) ||
                (this._Mode == Machine.StopMode.PerOperation))
            {
                this.OnStepped();
            }
        }
        protected void OnCycleUpdatePC()
        {
            this.CycleUpdatePC(new Common.CycleEventArgs(this._InstructionCount));

            if ((this._Mode == Machine.StopMode.PerStep) ||
                (this._Mode == Machine.StopMode.PerOperation))
            {
                this.OnStepped();
            }
        }
        protected void OnCycleUpdateIR()
        {
            this.CycleUpdateIR(new Common.CycleEventArgs(this._InstructionCount));

            if ((this._Mode == Machine.StopMode.PerStep) ||
                (this._Mode == Machine.StopMode.PerOperation))
            {
                this.OnStepped();
            }
        }

        protected void OnStepped()
        {
            this.Stepped(new Common.CycleEventArgs(this._InstructionCount));
        }


        protected void OnPreDataMoved(Common.DataMovedEventArgs dme)
        {
            this.PreDataMoved(dme);
        }
        protected void OnDataMoved(Common.DataMovedEventArgs dme)
        {
            this.OnPreDataMoved(dme);
            this.DataMoved(dme);

            if (this._Mode == Machine.StopMode.PerOperation)
            {
                this.OnStepped();
            }
        }

        #endregion
    }
}
