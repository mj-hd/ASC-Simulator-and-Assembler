using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Simulator.UI
{
    public class CUI : IUI
    {
        public Machine.ASC Machine
        {
            set
            {
                this._Machine = value;
            }
        }

        public CUI()
        {
        }
        ~CUI()
        {
            if (this._OutFile != null)  this._OutFile.Close();
        }

        public void ShowWindow()
        {
            this._Machine.Init();
            this._Machine.CycleOpecode += new Machine.CycleEventHandler(this._CycleBeg);
            this._Machine.CycleUpdatePC += new Machine.CycleEventHandler(this._CycleEnd);

            //自分自身のAssemblyを取得
            System.Reflection.Assembly asm = 
            System.Reflection.Assembly.GetExecutingAssembly();

            System.Version ver = asm.GetName().Version;

            this._DrawLine();
            Console.WriteLine("ASC Simulator "+ver);
            Console.WriteLine("HELPと入力すると、コマンド一覧を表示します。");
            this._DrawLine();

            this._Loop();
        }
        public void ShowError(string message)
        {
            Console.WriteLine(message);
        }

        private Machine.ASC _Machine;
        private TextWriter _Out = Console.Out;
        private TextWriter _SOut = Console.Out;
        private FileStream _OutFile;
        private bool _IsTracing = false;
        private bool _IsBreaking = false;
        private bool _IsStepping = false;
        private List<ushort> _BreakPoints = new List<ushort>();

        private void _CycleBeg()
        {
            if (this._IsTracing)
            {
                Console.WriteLine("PC: 0x" + this._Machine.Registers.PC.ToString("x"));
                Console.WriteLine("ニーモニック: " + Common.Defines.ToString(this._Machine.Registers.IR) + " 0x" + this._Machine.Registers.MAR.ToString("x"));
            }


        }
        private void _CycleEnd()
        {
            if (this._IsTracing)
            {
                this._DUMP();
            }
            if (this._IsStepping)
            {
                this._IsStepping = false;
                this._Machine.HLT();
            }
            if (this._BreakPoints.Contains((ushort)(this._Machine.Registers.PC+1)))
            {
                this._Machine.HLT();
                this._IsBreaking = true;
            }
        }
        private void _DrawLine()
        {
            Console.WriteLine(new String('-', Console.BufferWidth));
        }
        private void _Loop()
        {
            String command = "";

            while (true)
            {
                Console.Write(">> ");
                command = Console.ReadLine();

                switch (command)
                {
                    case "HELP":
                        this._HELP();
                        break;
                    case "INIT":
                        this._INIT();
                        break;
                    case "LOAD":
                        this._LOAD();
                        break;
                    case "SAVE":
                        this._SAVE();
                        break;
                    case "OUT":
                        this._OUT();
                        break;
                    case "RUN":
                        this._RUN();
                        break;
                    case "SET":
                        this._SET();
                        break;
                    case "TRACE":
                        this._TRACE();
                        break;
                    case "DUMP":
                        this._DUMP();
                        break;
                    case "BREAK":
                        this._BREAK();
                        break;
                    case "STEP":
                        this._STEP();
                        break;
                    case "DIS":
                        this._DIS();
                        break;
                    case "END":
                        return;
                    default:
                        Console.WriteLine("コマンドが間違っています。");
                        Console.WriteLine("HELPコマンドで確認してください。");
                        break;
                }

                Console.WriteLine();
            }
        }
        private void _HELP()
        {
            Console.WriteLine("HELP コマンド一覧を表示");
            Console.WriteLine("INIT 全てのデバイスと設定を初期化");
            Console.WriteLine("LOAD ファイルをメモリに読み込む");
            Console.WriteLine("SAVE メモリの内容をファイルに保存");
            Console.WriteLine("OUT  TRACEやDUMPコマンドの出力先を指定");
            Console.WriteLine("RUN  実行");
            Console.WriteLine("SET  デバイスの値を設定");
            Console.WriteLine("TRACE デバイスの様子を出力しながら実行");
            Console.WriteLine("DUMP デバイスの状態を出力");
            Console.WriteLine("BREAK ブレークポイントを設定/解除");
            Console.WriteLine("STEP ステップを一つ進める");
            Console.WriteLine("DIS 逆アセンブルを表示する");
            Console.WriteLine("END  終了");
        }
        private void _INIT()
        {
            this._Machine.Init();

            Console.WriteLine("初期化しました。");
        }
        private void _LOAD()
        {
            String fname;

            Console.WriteLine("ファイル名を指定してください。");
            Console.Write("? ");
            fname = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine(fname + "を読み込みます。");

            FileStream f;
            try {
                f = new FileStream(fname, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine("ファイルを開けませんでした。");
                Console.WriteLine("("+e.Message+")");
                return;
            }

            Dictionary<String, ushort> address = this._Machine.Load(f);

            Console.WriteLine(String.Format("0x{0, 3:x3} 番地から 0x{1, 3:x3} 番地まで読み込みました。", address["begin"], address["end"]));

            Console.WriteLine();
            Console.WriteLine(String.Format("PCの値を 0x{0, 3:x3} に変更します。", address["org"]));

            this._Machine.Registers.PC = (ushort)address["org"];
            f.Close();
        }
        private void _SAVE()
        {
            String fname;

            Console.WriteLine("ファイル名を指定してください。");
            Console.Write("? ");
            fname = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine(fname + "として書き込みます。");

            FileStream f;
            try
            {
                f = new FileStream(fname, FileMode.Create);
            }
            catch (Exception e) {
                Console.WriteLine("ファイルに書き込めませんでした。");
                Console.WriteLine("("+e.Message+")");
                return;
            }

            this._Machine.Save(f);

            f.Close();
        }
        private void _OUT()
        {
            String dest;

            Console.WriteLine("出力先を入力してください。(空で標準出力)");
            Console.Write("? ");
            dest = Console.ReadLine();
            Console.WriteLine();

            if (dest == "")
            {
                Console.WriteLine("標準出力を使用します。");
                this._Out = this._SOut;
            }
            else
            {
                Console.WriteLine(dest+"に出力します。");
                this._OutFile = new FileStream(dest, FileMode.Append);
                this._Out     = new StreamWriter(this._OutFile);
            }
        }
        private void _RUN()
        {
            Console.WriteLine("実行を開始します。");

            this._IsBreaking = false;

            this._Machine.Run();

            Console.WriteLine();
            if (this._IsBreaking)
            {
                Console.WriteLine("0x"+this._Machine.Registers.PC.ToString("x") +"番地でブレークしました。");
            }
            else
            {
                Console.WriteLine("実行が完了しました。");
            }
        }
        private void _SET()
        {
            String device;

            Console.WriteLine("値を変更するデバイス名を指定してください。");
            Console.WriteLine("選択肢： PC MM R IR MAR Z N");
            Console.Write("? ");
            device = Console.ReadLine();

            Console.WriteLine();
            switch (device)
            {
                case "PC":
                    {
                        Console.WriteLine("変更後の値を入力してください。現在：0x" + this._Machine.Registers.PC.ToString("x"));
                        Console.Write("? ");
                        String value = Console.ReadLine();
                        ushort value_d;

                        if (!this._ToUShort(value, out value_d))
                        {
                            if (value != "")
                            {
                                Console.WriteLine("この値は不正です。");
                                break;
                            }
                        }

                        this._Machine.Registers.PC = value_d;
                        break;
                    }
                case "MM":
                    {
                        String address;
                        ushort address_d;
                        String value;
                        ushort value_d;

                        Console.WriteLine("変更するアドレスを指定してください。");
                        Console.Write("? ");
                        address = Console.ReadLine();

                        if (!this._ToUShort(address, out address_d))
                        {
                            Console.WriteLine("アドレスが不正です。");
                            break;
                        }

                        Console.WriteLine("値を入力してください。現在：0x" + this._Machine.Memory[address_d].ToString("x"));
                        Console.Write("? ");
                        value = Console.ReadLine();

                        if (!this._ToUShort(value, out value_d))
                        {
                            Console.WriteLine("値が不正です。");
                            break;
                        }

                        this._Machine.Memory[address_d] = value_d;
                        break;
                    }
                case "R":
                    {
                        Console.WriteLine("変更後の値を入力してください。現在：0x" + this._Machine.Registers.R.ToString("x"));
                        Console.Write("? ");
                        String value = Console.ReadLine();
                        ushort value_d;

                        if (!this._ToUShort(value, out value_d))
                        {
                            if (value != "")
                            {
                                Console.WriteLine("この値は不正です。");
                                break;
                            }
                        }

                        this._Machine.Registers.R = value_d;
                        break;
                    }
                case "IR":
                    {
                        Console.WriteLine("変更後の値を入力してください。現在：0x" + this._Machine.Registers.IR.ToString("x"));
                        Console.Write("? ");
                        String value = Console.ReadLine();
                        ushort value_d;

                        if (!this._ToUShort(value, out value_d))
                        {
                            if (value != "")
                            {
                                Console.WriteLine("この値は不正です。");
                                break;
                            }
                        }

                        this._Machine.Registers.IR = value_d;
                        break;
                    }
                case "MAR":
                    {
                        Console.WriteLine("変更後の値を入力してください。現在：0x" + this._Machine.Registers.MAR.ToString("x"));
                        Console.Write("? ");
                        String value = Console.ReadLine();
                        ushort value_d;

                        if (!this._ToUShort(value, out value_d))
                        {
                            if (value != "")
                            {
                                Console.WriteLine("この値は不正です。");
                                break;
                            }
                        }
                        
                         this._Machine.Registers.MAR = value_d;
                        break;
                    }
                case "Z":
                    {
                        Console.WriteLine("変更後の値を入力してください。現在：" + this._Machine.Registers.Z);
                        Console.Write("? ");
                        String value = Console.ReadLine();
                        bool value_b;

                        if (!bool.TryParse(value, out value_b))
                        {
                            if (value != "")
                            {
                                Console.WriteLine("この値は不正です。");
                            }
                        }
                        
                        this._Machine.Registers.Z = value_b;
                        break;
                    }
                case "N":
                    {
                        Console.WriteLine("変更後の値を入力してください。現在：" + this._Machine.Registers.N);
                        Console.Write("? ");
                        String value = Console.ReadLine();
                        bool value_b;

                        if (!bool.TryParse(value, out value_b))
                        {
                            if (value != "")
                            {
                                Console.WriteLine("この値は不正です。");
                            }
                        }
                        
                        this._Machine.Registers.N = value_b;
                        break;
                    }
            }
        }
        private void _TRACE()
        {
            this._IsTracing = true;
            this._IsBreaking = false;

            Console.WriteLine("トレースを開始します。");
            Console.WriteLine();

            Console.SetOut(this._Out);

            this._Machine.Run();

            Console.SetOut(this._SOut);

            this._IsTracing = false;

            if (this._IsBreaking)
            {
                Console.WriteLine("0x"+this._Machine.Registers.PC.ToString("x")+"番地でブレークしました。");
            }
            else
            {
                Console.WriteLine("トレースが完了しました。");
            }
        }
        private void _DUMP()
        {
            String line;

            this._DrawLine();
            line = String.Format(" {0, 5} {1, 3} {2, 5} {3, 5} Z N", "PC", "IR", "MAR", "R");
            Console.WriteLine(line);
            line = String.Format(" 0x{0, 3:x3} 0x{1, 1:x} 0x{2, 3:x3} 0x{3, 3:x3} {4, 1} {5, 1}", this._Machine.Registers.PC, this._Machine.Registers.IR, this._Machine.Registers.MAR, this._Machine.Registers.R, Convert.ToInt32(this._Machine.Registers.Z), Convert.ToInt32(this._Machine.Registers.N));
            Console.WriteLine(line);
            this._DrawLine();
        }
        private void _BREAK()
        {
            String address;
            ushort address_d;

            Console.WriteLine("何番地にブレークポイントを設置しますか？");
            Console.WriteLine("(同じ番地を指定すると解除できます。)");
            Console.Write("? ");
            address = Console.ReadLine();
            Console.WriteLine();

            if (!this._ToUShort(address, out address_d))
            {
                Console.WriteLine("入力された値が不正です。");
                return;
            }

            if (this._BreakPoints.Contains((ushort)address_d))
            {
                this._BreakPoints.Remove((ushort)address_d);
                Console.WriteLine("0x"+address_d.ToString("x")+"番地を解除しました。");
            }
            else
            {
                this._BreakPoints.Add((ushort)address_d);
                Console.WriteLine("0x"+address_d.ToString("x") + "番地を設定しました。");
            }
        }
        private void _STEP()
        {
            this._IsStepping = true;
            this._IsTracing = true;

            this._Machine.Run();

            this._IsTracing = false;
        }
        private void _DIS()
        {

            Console.WriteLine("何番地から逆アセンブルしますか？");
            Console.Write("? ");
            ushort start;
            if (!this._ToUShort(Console.ReadLine(), out start))
            {
                Console.WriteLine("入力された値が不正です。");
                return;
            }

            Console.WriteLine();

            Console.WriteLine("何番地まで逆アセンブルしますか？");
            Console.Write("? ");
            ushort end;
            if (!this._ToUShort(Console.ReadLine(), out end))
            {
                Console.WriteLine("入力された値が不正です。");
                return;
            }
            Console.WriteLine();

            Console.WriteLine("Address: Opecode Operand  Binary");
            for (int i = start; i <= end; i++)
            {
                ushort line = this._Machine.Memory[(ushort)i];
                Console.WriteLine("0x{0, 2:x2}:   {2, 3}      0x{3, 3:x3}    {4, 16}", i, (line>>12), Common.Defines.ToString(line>>12), line - ((line>>12)<<12), Convert.ToString(line, 2).PadLeft(16, '0'));
            }

        }
        private bool _ToUShort(String raw, out ushort result)
        {
            result = 0;

            if (raw.StartsWith("0x"))
            {
                try
                {
                    result = Convert.ToUInt16(raw.Substring(2, raw.Length - 2), 16);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                if (!ushort.TryParse(raw, out result)) {
                    return false;
                }
            }

            return true;
        }
    }
}
