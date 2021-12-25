/*
 * MachineDisplay.cs
 * ASCの状態を表示するコントロール
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Simulator.MachineDisplay
{
    public partial class MachineDisplay : UserControl
    {
        #region Connectorクラス、ICクラスなど
        // レジスタ間を繋ぐコネクタ(緑色の線)
        public class Connector
        {
            // コネクタの状態
            // ToDo: Connectorに変更？
            public enum ConnectState
            {
                Invisible,    // 見えないコネクタ
                Visible,      // 通常状態のコネクタ
                OnElectricity // 値の移動があったコネクタ
            }


            // コネクタの形状
            public GraphicsPath Path;

            // コネクタの状態
            public ConnectState State;


            public Connector()
            {
                this.Path = new GraphicsPath();
                this.State = ConnectState.Visible;
            }

        }

        // レジスタなどのデバイスを表すクラス
        public class IC
        {
            // 縦、横の拡大比率
            public class RatioPair
            {
                public RatioPair(double w_ratio, double h_ratio)
                {
                    this.WidthRatio = w_ratio;
                    this.HeightRatio = h_ratio;
                }

                public double WidthRatio;
                public double HeightRatio;
            }

            // ICの位置とサイズを表すクラス
            public class RatioRectangle
            {
                // ICの位置
                public int X;
                public int Y;
                // ICのサイズ
                public int Width;
                public int Height;

                // 拡大比率
                public RatioPair Ratios {
                    get
                    {
                        return this._Ratios;
                    }
                    set
                    {
                        this._Ratios = value;

                        // 新しく設定された比率に合わせて座標とサイズを書き換える
                        this._Left   = (int)(this.X * this._Ratios.WidthRatio);
                        this._Right  = (int)((this.X+this.Width) * this._Ratios.WidthRatio);
                        this._Top    = (int)(this.Y * this._Ratios.HeightRatio);
                        this._Bottom = (int)((this.Y+this.Height) * this._Ratios.HeightRatio);
                        this._Center = new Point((this._Left+this._Right)/2, (this._Top+this._Bottom)/2);
                        this._Rect = new Rectangle(this._Left, this._Top, (int)(this.Width * this._Ratios.WidthRatio), (int)(this.Height * this._Ratios.HeightRatio));
                    }
                }

                // ICの右端のx座標
                public int Right
                {
                    get
                    {
                        return this._Right;
                    }
                }

                // ICの左端のx座標
                public int Left
                {
                    get
                    {
                        return this._Left;
                    }
                }

                // ICの上辺のy座標
                public int Top
                {
                    get
                    {
                        return this._Top;
                    }
                }

                // ICの底辺のy座標
                public int Bottom
                {
                    get
                    {
                        return this._Bottom;
                    }
                }

                // 中心の座標
                public Point Center
                {
                    get
                    {
                        return this._Center;
                    }
                }

                // Rectangleインスタンス
                public Rectangle Rect
                {
                    get
                    {
                        return this._Rect;
                    }
                }



                public RatioRectangle(int x, int y, int wid, int hei)
                {
                    this.X = x;
                    this.Y = y;
                    this.Width = wid;
                    this.Height = hei;

                    // デフォルトで横1.0, 縦1.0
                    this.Ratios = new RatioPair(1.0, 1.0);
                }
                public RatioRectangle()
                    : this(0, 0, 0, 0)
                {
                }


                private int _Left;
                private int _Top;
                private int _Right;
                private int _Bottom;
                private Point _Center;
                private RatioPair _Ratios;
                private Rectangle _Rect;
            }


            // 現在の値
            public string Data = "";

            // ICが書き換えられたか、参照されたかなどの状態
            public ICState State = ICState.Normal;

            // ICのRectangle
            public RatioRectangle Frame = new RatioRectangle();

            // 形状などが通常のICか否か
            public bool AllowAutomaticDraw = true;

        }

        // ICが書き換えられたか、参照されたかなどの状態を表す列挙体
        public enum ICState {
            Normal,   // 通常
            Reffered, // 参照された
            Changed   // 書き換えられた
        }


        #endregion

        public MachineDisplay(Simulator.Common.ApplicationSettings settings)
        {
            // asc-simulatorと共有する設定を確保
            this._Settings = settings;

            #region ICの登録

            // 登録されたICは自動的に描画される
            this._ICs.Add("Controller", new IC()
            {
                AllowAutomaticDraw = false,
                Frame = new IC.RatioRectangle(260, 30, 100, 40)
            });
            this._ICs.Add("IR", new IC()
            {
                Frame = new IC.RatioRectangle(15, 100, 140, 40)
            });
            this._ICs.Add("PC", new IC()
            {
                Frame = new IC.RatioRectangle(130, 220, 140, 40)
            });
            this._ICs.Add("R", new IC()
            {
                Frame = new IC.RatioRectangle(390, 220, 140, 40)
            });
            this._ICs.Add("MAR", new IC()
            {
                Frame = new IC.RatioRectangle(70, 330, 140, 40)
            });
            this._ICs.Add("ALU", new IC()
            {
                AllowAutomaticDraw = false,
                Frame = new IC.RatioRectangle(320, 330, 200, 60)
            });
            this._ICs.Add("ALURight", new IC()
            {
                AllowAutomaticDraw = false,
                Frame = new IC.RatioRectangle(440, 330, 50, 20)
            });
            this._ICs.Add("ALULeft", new IC()
            {
                AllowAutomaticDraw = false,
                Frame = new IC.RatioRectangle(this._ICs["ALU"].Frame.X, this._ICs["ALU"].Frame.Y, 50, 20)
            });
            this._ICs.Add("ALUOperator", new IC()
            {
                AllowAutomaticDraw = false,
                Frame = new IC.RatioRectangle(520, 420, 20, 20)
            });
            this._ICs.Add("1", new IC()
            {
                AllowAutomaticDraw = false,
                Frame = new IC.RatioRectangle(475, 270, 20, 20)
            });
            this._ICs.Add("Z", new IC()
            {
                Frame = new IC.RatioRectangle(285, 390, 30, 40)
            });
            this._ICs.Add("N", new IC()
            {
                Frame = new IC.RatioRectangle(325, 390, 30, 40)
            });
            this._ICs.Add("MM", new IC()
            {
                Frame = new IC.RatioRectangle(50, 390, 180, 60)
            });
            this._ICs.Add("OV", new IC()
            {
                AllowAutomaticDraw = false,
                Frame = new IC.RatioRectangle(355, 460, 200, 40)
            });

            #endregion

            #region コネクタの登録

            // コネクタ名はDataMoveイベントのSender, Recieverを"to"で繋いだもの
            this._Connectors.Add("IRtoController", new Connector());
            this._Connectors.Add("IRtoMAR", new Connector());
            this._Connectors.Add("IRtoPC", new Connector());
            this._Connectors.Add("PCtoMAR", new Connector());
            this._Connectors.Add("PCtoALULeft", new Connector());
            this._Connectors.Add("RtoALURight", new Connector());
            this._Connectors.Add("RtoMM", new Connector());
            this._Connectors.Add("MMtoIR", new Connector());
            this._Connectors.Add("MMtoR", new Connector());
            this._Connectors.Add("MMtoALULeft", new Connector());
            this._Connectors.Add("1toALURight", new Connector());
            this._Connectors.Add("ALUtoZ", new Connector());
            this._Connectors.Add("ALUtoN", new Connector());
            this._Connectors.Add("ALUtoR", new Connector());
            this._Connectors.Add("ALUtoPC", new Connector());
            #endregion

            #region ICとコネクタの見た目の設定
            // コネクタの状態に対応したPenを登録
            // コネクタの色、太さなどを設定
            this._ConnectorPen.Add(Connector.ConnectState.Invisible, Pens.Transparent);
            this._ConnectorPen.Add(Connector.ConnectState.Visible, new Pen(ColorTranslator.FromHtml(this._Settings.ConnectorColorNormal), this._Settings.ConnectorWidth));
            this._ConnectorPen.Add(Connector.ConnectState.OnElectricity, new Pen(ColorTranslator.FromHtml(this._Settings.ConnectorColorActive), this._Settings.ConnectorWidth));

            // コネクタの角と、端の形状を設定
            this._ConnectorPen[Connector.ConnectState.Visible].LineJoin = LineJoin.Round;
            this._ConnectorPen[Connector.ConnectState.Visible].SetLineCap(LineCap.RoundAnchor, LineCap.ArrowAnchor, DashCap.Round);
            this._ConnectorPen[Connector.ConnectState.OnElectricity].LineJoin = LineJoin.Round;
            this._ConnectorPen[Connector.ConnectState.OnElectricity].SetLineCap(LineCap.RoundAnchor, LineCap.ArrowAnchor, DashCap.Round);

            // ICの状態に対応したBrushを登録
            this._ICBrush.Add(ICState.Normal, new SolidBrush(ColorTranslator.FromHtml(this._Settings.RegisterColorNormal)));
            this._ICBrush.Add(ICState.Reffered, new SolidBrush(ColorTranslator.FromHtml(this._Settings.RegisterColorReference)));
            this._ICBrush.Add(ICState.Changed, new SolidBrush(ColorTranslator.FromHtml(this._Settings.RegisterColorModify)));

            // ICの状態に対応したPenを登録
            // 文字の描画に使われる
            this._ICPen.Add(ICState.Normal, new Pen(this._ICBrush[ICState.Normal], this._Settings.ConnectorWidth)
            {

            });
            this._ICPen.Add(ICState.Reffered, new Pen(this._ICBrush[ICState.Reffered], this._Settings.ConnectorWidth)
            {

            });
            this._ICPen.Add(ICState.Changed, new Pen(this._ICBrush[ICState.Changed], this._Settings.ConnectorWidth)
            {

            });

            // フォントを登録
            this._ICFont = new Font("Terminal", 11);

            #endregion

            // MachineDisplayの基準となるサイズ(RatioPairが1.0, 1.0のとき)を指定
            // このサイズとRatioPairを掛けたものが実際のサイズになる
            this._BaseSize = new Size(730, 540);

            InitializeComponent();

            this.Enabled = true;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserMouse, true);
            this.SetStyle(ControlStyles.StandardClick, true);

        }

        // ICの状態を全てNormalに戻す
        public void ResetHighlights()
        {
            this._ResetCycleVariables();
        }

        #region イベント

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // アンチエイリアシングを有効に
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 背景を白で塗りつぶす
            Brush background = new SolidBrush(Color.FromKnownColor(KnownColor.White));
            e.Graphics.FillRectangle(background, 0, 0, this.Width, this.Height);

            #region ICの描画
            // AllowAutomaticDrawがtrueのICを描画する
            foreach (KeyValuePair<string, IC> pair in this._ICs)
            {
                if (pair.Value.AllowAutomaticDraw)
                {
                    // ICの枠を表示
                    e.Graphics.DrawRectangle(this._ICPen[pair.Value.State], pair.Value.Frame.Rect);

                    // 文字を表示
                    e.Graphics.DrawString(pair.Key, this._ICFont, this._ICBrush[pair.Value.State], pair.Value.Frame.Left + 2, pair.Value.Frame.Top + 4);
                    e.Graphics.DrawString(pair.Value.Data, this._ICFont, this._ICBrush[pair.Value.State], pair.Value.Frame.Left + 2, pair.Value.Frame.Top + (int)(20*this._HeightRatio));
                }
            }

            // 独自に描画する(AlloAutomaticDrawがfalse)のICを描画する
            //制御部
            e.Graphics.DrawRectangle(this._ICPen[this._ICs["Controller"].State], this._ICs["Controller"].Frame.Rect);
            e.Graphics.DrawString("制御部", this._ICFont, this._ICBrush[this._ICs["Controller"].State], this._ICs["Controller"].Frame.Left + 5, this._ICs["Controller"].Frame.Top + 5);
            e.Graphics.DrawString(this._ICs["Controller"].Data, this._ICFont, this._ICBrush[this._ICs["Controller"].State], this._ICs["Controller"].Frame.Left + 5, this._ICs["Controller"].Frame.Top + (int)(20*this._HeightRatio));

            //ALU
            GraphicsPath aluPath = new GraphicsPath();
            aluPath.StartFigure();
            aluPath.AddLine(this._ICs["ALULeft"].Frame.Left, this._ICs["ALULeft"].Frame.Top, this._ICs["ALULeft"].Frame.Right, this._ICs["ALULeft"].Frame.Top);
            aluPath.AddLine(this._ICs["ALULeft"].Frame.Right, this._ICs["ALULeft"].Frame.Top, (this._ICs["ALULeft"].Frame.Right + this._ICs["ALURight"].Frame.Left) / 2, this._ICs["ALU"].Frame.Top + (int)(20*this._HeightRatio));
            aluPath.AddLine((this._ICs["ALULeft"].Frame.Right + this._ICs["ALURight"].Frame.Left) / 2, this._ICs["ALU"].Frame.Top + (int)(20*this._HeightRatio), this._ICs["ALURight"].Frame.Left, this._ICs["ALURight"].Frame.Top);
            aluPath.AddLine(this._ICs["ALURight"].Frame.Left, this._ICs["ALURight"].Frame.Top, this._ICs["ALURight"].Frame.Right, this._ICs["ALURight"].Frame.Top);
            aluPath.AddLine(this._ICs["ALURight"].Frame.Right, this._ICs["ALURight"].Frame.Top, this._ICs["ALURight"].Frame.Left, this._ICs["ALU"].Frame.Bottom);
            aluPath.AddLine(this._ICs["ALURight"].Frame.Left, this._ICs["ALU"].Frame.Bottom, this._ICs["ALULeft"].Frame.Right, this._ICs["ALU"].Frame.Bottom);
            aluPath.AddLine(this._ICs["ALULeft"].Frame.Right, this._ICs["ALU"].Frame.Bottom, this._ICs["ALULeft"].Frame.Left, this._ICs["ALULeft"].Frame.Top);

            e.Graphics.DrawPath(this._ICPen[this._ICs["ALU"].State], aluPath);

            var aluDetailLeft = (int)(this._ICs["ALU"].Frame.Right + 50 * this._WidthRatio);
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("ALU", this._ICFont, this._ICBrush[ICState.Normal], this._ICs["ALU"].Frame.Left + this._ICs["ALU"].Frame.Rect.Width / 4, this._ICs["ALU"].Frame.Top + this._ICs["ALU"].Frame.Rect.Height / 2-10, stringFormat);
            e.Graphics.DrawString("ALU", this._ICFont, this._ICBrush[ICState.Normal], aluDetailLeft, this._ICs["ALU"].Frame.Top);

            //ALUOperator
            e.Graphics.DrawString(this._ICs["ALUOperator"].Data, this._ICFont, this._ICBrush[this._ICs["ALUOperator"].State], this._ICs["ALU"].Frame.Right + (int)(40*this._WidthRatio), this._ICs["ALU"].Frame.Top+(int)(40*this._HeightRatio));
            
            //ALULeft
            e.Graphics.DrawString(this._ICs["ALULeft"].Data, this._ICFont, this._ICBrush[this._ICs["ALULeft"].State], aluDetailLeft, this._ICs["ALURight"].Frame.Top+(int)(20*this._HeightRatio));
            //ALURight
            e.Graphics.DrawString(this._ICs["ALURight"].Data, this._ICFont, this._ICBrush[this._ICs["ALURight"].State], aluDetailLeft, this._ICs["ALU"].Frame.Top+(int)(40*this._HeightRatio));
           
            e.Graphics.DrawLine(this._ICPen[ICState.Normal], aluDetailLeft, this._ICs["ALU"].Frame.Top + (int)(60*this._HeightRatio), this._ICs["ALU"].Frame.Right + (int)(200*this._WidthRatio), this._ICs["ALU"].Frame.Top + (int)(60*this._HeightRatio));

            e.Graphics.DrawString(this._ICs["ALU"].Data, this._ICFont, this._ICBrush[this._ICs["ALU"].State], this._ICs["ALU"].Frame.Right + (int)(50*this._WidthRatio), this._ICs["ALU"].Frame.Top + (int)(60*this._HeightRatio));

            // 1
            e.Graphics.DrawString("1", this._ICFont, this._ICBrush[ICState.Normal], this._ICs["1"].Frame.Rect);

            // OV
            if (this._ICs["OV"].State == ICState.Changed)
            {
                e.Graphics.DrawString("オーバフローが発生しています", this._ICFont, this._ICBrush[ICState.Changed], this._ICs["OV"].Frame.Rect);
            }

            #endregion


            #region コネクタの描画
            // OnElectricityのコネクタを最も上に描画したいため、二回に分けている

            // 通常状態のコネクタを描画
            foreach (Connector connector in this._Connectors.Values)
            {
                if (connector.State == Connector.ConnectState.Visible)
                {
                    e.Graphics.DrawPath(this._ConnectorPen[connector.State], connector.Path);
                }
            }
            // 値の移動があったコネクタを描画
            foreach (Connector connector in this._Connectors.Values)
            {
                if (connector.State == Connector.ConnectState.OnElectricity)
                {
                    e.Graphics.DrawPath(this._ConnectorPen[connector.State], connector.Path);
                }
            }
            #endregion
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 最小化されていなければ
            if ((this.Size.Width > 0) && (this.Size.Height > 0))
            {
                // 拡大比率を計算
                this._WidthRatio = (double)this.Size.Width / (double)this._BaseSize.Width;
                this._HeightRatio = (double)this.Size.Height / (double)this._BaseSize.Height;

                #region ICのサイズ更新
                // 全てのICのRatiosを更新する
                foreach (string icName in this._ICs.Keys.ToArray())
                {
                    this._ICs[icName].Frame.Ratios = new IC.RatioPair(this._WidthRatio, this._HeightRatio);
                }
                #endregion

                #region コネクタのサイズ更新
                var xMargin = (int)(this._Settings.RegisterMargin * this._WidthRatio);
                var yMargin = (int)(this._Settings.RegisterMargin * this._HeightRatio);

                // 全てのコネクタの形状を登録する
                var IC1 = this._ICs["IR"];
                var IC2 = this._ICs["Controller"];
                this._Connectors["IRtoController"].Path = new GraphicsPath();
                this._Connectors["IRtoController"].Path.StartFigure();
                this._Connectors["IRtoController"].Path.AddLine(IC1.Frame.Center.X - xMargin, IC1.Frame.Top - 10, IC1.Frame.Center.X - xMargin, IC2.Frame.Center.Y);
                this._Connectors["IRtoController"].Path.AddLine(IC1.Frame.Center.X - xMargin, IC2.Frame.Center.Y, IC2.Frame.Left - 5, IC2.Frame.Center.Y);

                IC2 = this._ICs["MAR"];
                this._Connectors["IRtoMAR"].Path = new GraphicsPath();
                this._Connectors["IRtoMAR"].Path.StartFigure();
                this._Connectors["IRtoMAR"].Path.AddLine(IC1.Frame.Center.X + xMargin, IC1.Frame.Bottom + 10, IC1.Frame.Center.X + xMargin, IC2.Frame.Top - 5);

                IC2 = this._ICs["PC"];
                this._Connectors["IRtoPC"].Path = new GraphicsPath();
                this._Connectors["IRtoPC"].Path.StartFigure();
                this._Connectors["IRtoPC"].Path.AddLine(IC1.Frame.Center.X + xMargin, IC1.Frame.Bottom + 10, IC1.Frame.Center.X + xMargin, IC2.Frame.Top - yMargin);
                this._Connectors["IRtoPC"].Path.AddLine(IC1.Frame.Center.X + xMargin, IC2.Frame.Top - yMargin, IC2.Frame.Left + 20, IC2.Frame.Top - yMargin);
                this._Connectors["IRtoPC"].Path.AddLine(IC2.Frame.Left + 20, IC2.Frame.Top - yMargin, IC2.Frame.Left + 20, IC2.Frame.Top - 5);

                int tmp;
                IC1 = this._ICs["PC"];
                IC2 = this._ICs["MAR"];
                this._Connectors["PCtoMAR"].Path = new GraphicsPath();
                this._Connectors["PCtoMAR"].Path.StartFigure();
                this._Connectors["PCtoMAR"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 10, IC1.Frame.Center.X, IC2.Frame.Top - yMargin);
                this._Connectors["PCtoMAR"].Path.AddLine(IC1.Frame.Center.X, IC2.Frame.Top - yMargin, IC2.Frame.Right - 20, IC2.Frame.Top - yMargin);
                this._Connectors["PCtoMAR"].Path.AddLine(IC2.Frame.Right - 20, IC2.Frame.Top - yMargin, IC2.Frame.Right - 20, IC2.Frame.Top - 5);

                IC2 = this._ICs["ALULeft"];
                this._Connectors["PCtoALULeft"].Path = new GraphicsPath();
                this._Connectors["PCtoALULeft"].Path.StartFigure();
                this._Connectors["PCtoALULeft"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 10, IC1.Frame.Center.X, IC2.Frame.Top - yMargin);
                this._Connectors["PCtoALULeft"].Path.AddLine(IC1.Frame.Center.X, IC2.Frame.Top - yMargin, IC2.Frame.Center.X, IC2.Frame.Top - yMargin);
                this._Connectors["PCtoALULeft"].Path.AddLine(IC2.Frame.Center.X, IC2.Frame.Top - yMargin, IC2.Frame.Center.X, IC2.Frame.Top - 5);

                IC1 = this._ICs["R"];
                this._Connectors["RtoALURight"].Path = new GraphicsPath();
                this._Connectors["RtoALURight"].Path.StartFigure();
                this._Connectors["RtoALURight"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 10, IC1.Frame.Center.X, IC2.Frame.Top - 5);

                IC2 = this._ICs["MM"];
                var mmRight = (int)(IC2.Frame.Right + 40 * this._WidthRatio);
                this._Connectors["RtoMM"].Path = new GraphicsPath();
                this._Connectors["RtoMM"].Path.StartFigure();
                this._Connectors["RtoMM"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 10, IC1.Frame.Center.X, IC1.Frame.Bottom + 20);
                this._Connectors["RtoMM"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 20, mmRight, IC1.Frame.Bottom + 20);
                this._Connectors["RtoMM"].Path.AddLine(mmRight, IC1.Frame.Bottom + 20, mmRight, IC2.Frame.Center.Y);
                this._Connectors["RtoMM"].Path.AddLine(mmRight, IC2.Frame.Center.Y, IC2.Frame.Right + 5, IC2.Frame.Center.Y);

                IC1 = this._ICs["MM"];
                IC2 = this._ICs["IR"];
                this._Connectors["MMtoIR"].Path = new GraphicsPath();
                this._Connectors["MMtoIR"].Path.StartFigure();
                this._Connectors["MMtoIR"].Path.AddLine(IC1.Frame.Left - 10, IC1.Frame.Center.Y, IC2.Frame.Left + 10, IC1.Frame.Center.Y);
                this._Connectors["MMtoIR"].Path.AddLine(IC2.Frame.Left + 10, IC1.Frame.Center.Y, IC2.Frame.Left + 10, IC2.Frame.Bottom + 5);

                IC2 = this._ICs["R"];
                this._Connectors["MMtoR"].Path = new GraphicsPath();
                this._Connectors["MMtoR"].Path.StartFigure();
                this._Connectors["MMtoR"].Path.AddLine(IC1.Frame.Left - 10, IC1.Frame.Center.Y, tmp = this._ICs["IR"].Frame.Left + 10, IC1.Frame.Center.Y);
                this._Connectors["MMtoR"].Path.AddLine(tmp, IC1.Frame.Center.Y, tmp, this._ICs["IR"].Frame.Bottom + yMargin);
                this._Connectors["MMtoR"].Path.AddLine(tmp, this._ICs["IR"].Frame.Bottom + yMargin, IC2.Frame.Left + 20, this._ICs["IR"].Frame.Bottom + yMargin);
                this._Connectors["MMtoR"].Path.AddLine(IC2.Frame.Left + 20, this._ICs["IR"].Frame.Bottom + yMargin, IC2.Frame.Left + 20, IC2.Frame.Top - 5);

                IC2 = this._ICs["ALULeft"];
                this._Connectors["MMtoALULeft"].Path = new GraphicsPath();
                this._Connectors["MMtoALULeft"].Path.StartFigure();
                this._Connectors["MMtoALULeft"].Path.AddLine(IC1.Frame.Left - 10, IC1.Frame.Center.Y, tmp = this._ICs["IR"].Frame.Left + 10, IC1.Frame.Center.Y);
                this._Connectors["MMtoALULeft"].Path.AddLine(tmp, IC1.Frame.Center.Y, tmp, this._ICs["IR"].Frame.Bottom + yMargin);
                this._Connectors["MMtoALULeft"].Path.AddLine(tmp, this._ICs["IR"].Frame.Bottom + yMargin, IC2.Frame.Right - 5, this._ICs["IR"].Frame.Bottom + yMargin);
                this._Connectors["MMtoALULeft"].Path.AddLine(IC2.Frame.Right - 5, this._ICs["IR"].Frame.Bottom + yMargin, IC2.Frame.Right - 5, IC2.Frame.Top - 5);

                IC1 = this._ICs["1"];
                IC2 = this._ICs["ALURight"];
                this._Connectors["1toALURight"].Path = new GraphicsPath();
                this._Connectors["1toALURight"].Path.StartFigure();
                this._Connectors["1toALURight"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 10, IC1.Frame.Center.X, IC2.Frame.Top - 5);

                IC1 = this._ICs["ALU"];
                IC2 = this._ICs["Z"];
                var aluLeft = (int)(IC1.Frame.Left + 20 * this._WidthRatio);
                this._Connectors["ALUtoZ"].Path = new GraphicsPath();
                this._Connectors["ALUtoZ"].Path.StartFigure();
                this._Connectors["ALUtoZ"].Path.AddLine(aluLeft, IC1.Frame.Center.Y, IC2.Frame.Center.X, IC1.Frame.Center.Y);
                this._Connectors["ALUtoZ"].Path.AddLine(IC2.Frame.Center.X, IC1.Frame.Center.Y, IC2.Frame.Center.X, IC2.Frame.Top - 5);

                IC2 = this._ICs["N"];
                this._Connectors["ALUtoN"].Path = new GraphicsPath();
                this._Connectors["ALUtoN"].Path.StartFigure();
                this._Connectors["ALUtoN"].Path.AddLine(aluLeft, IC1.Frame.Center.Y, IC2.Frame.Center.X, IC2.Frame.Top - 5);

                IC2 = this._ICs["R"];
                this._Connectors["ALUtoR"].Path = new GraphicsPath();
                this._Connectors["ALUtoR"].Path.StartFigure();
                this._Connectors["ALUtoR"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 10, IC1.Frame.Center.X, IC1.Frame.Bottom + 40);
                this._Connectors["ALUtoR"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 40, IC1.Frame.Right + xMargin, IC1.Frame.Bottom + 40);
                this._Connectors["ALUtoR"].Path.AddLine(IC1.Frame.Right + xMargin, IC1.Frame.Bottom + 40, IC1.Frame.Right + xMargin, IC2.Frame.Top - 20);
                this._Connectors["ALUtoR"].Path.AddLine(IC1.Frame.Right + xMargin, IC2.Frame.Top - 20, IC2.Frame.Right - 10, IC2.Frame.Top - 20);
                this._Connectors["ALUtoR"].Path.AddLine(IC2.Frame.Right - 10, IC2.Frame.Top - 20, IC2.Frame.Right - 10, IC2.Frame.Top - 5);

                IC2 = this._ICs["PC"];
                this._Connectors["ALUtoPC"].Path = new GraphicsPath();
                this._Connectors["ALUtoPC"].Path.StartFigure();
                this._Connectors["ALUtoPC"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 10, IC1.Frame.Center.X, IC1.Frame.Bottom + 40);
                this._Connectors["ALUtoPC"].Path.AddLine(IC1.Frame.Center.X, IC1.Frame.Bottom + 40, IC1.Frame.Right + xMargin, IC1.Frame.Bottom + 40);
                this._Connectors["ALUtoPC"].Path.AddLine(IC1.Frame.Right + xMargin, IC1.Frame.Bottom + 40, IC1.Frame.Right + xMargin, IC2.Frame.Top - 20);
                this._Connectors["ALUtoPC"].Path.AddLine(IC1.Frame.Right + xMargin, IC2.Frame.Top - 20, IC2.Frame.Right - 10, IC2.Frame.Top - 20);
                this._Connectors["ALUtoPC"].Path.AddLine(IC2.Frame.Right - 10, IC2.Frame.Top - 20, IC2.Frame.Right - 10, IC2.Frame.Top - 5);

                #endregion

                // フォントサイズを更新する
                this._ICFont = new Font("Terminal", (float)(14.0 * this._WidthRatio), GraphicsUnit.Pixel);

                // 画面を書き換える
                this.Invalidate();
            }
        }

#endregion

        #region イベントハンドラ

        // GUIによって割り当てられるイベントハンドラ
        public void DidDataChanged(Common.DataEventArgs de)
        {
            // ICの状態をChangedへ変更する
            this._ICs[de.Key].State = ICState.Changed;

            // ICの種類ごとに値を更新する
            if (de.Key == "N" || de.Key == "Z")
            {
                this._ICs[de.Key].Data = Convert.ToInt32((bool)de.Data).ToString();
            }
            else if (de.Key == "ALUOperator")
            {
                this._ICs[de.Key].Data = (string)de.Data;
            }
            else if (de.Key == "Controller")
            {
                this._ICs[de.Key].Data = Common.Defines.ToString((Common.Defines.OPECODE)de.Data);
            } else
            {
                this._ICs[de.Key].Data = Convert.ToString((ushort)de.Data, 2).PadLeft(16, '0');
            }
        }
        public void DidDataAccessed(Common.DataEventArgs de)
        {
            // ICの状態をRefferedへ
            this._ICs[de.Key].State = ICState.Reffered;
        }
        public void DidMemoryChanged(Common.MemoryEventArgs me)
        {
            // メモリの状態をChangedへ更新
            this._ICs["MM"].State = ICState.Changed;
            this._ICs["MM"].Data = "";

            // メモリの値を更新
            // もしもStartAddress, EndAddressともに0なら(初期化のときなど)
            if ((me.StartAddress == 0) && (me.EndAddress == 0))
            {
                this._ICs["MM"].Data = "0x0:0000000000000000" + Environment.NewLine;
                return;
            }

            // メモリの値を文字列へ変換して更新
            for (ushort i = me.StartAddress; i <= me.EndAddress; i++)
            {
                // 変更件数が多いときははしょる
                if (i >= me.StartAddress + 1) { this._ICs["MM"].Data += "...";  break; }

                // 更新
                this._ICs["MM"].Data += "0x" + Convert.ToString(i, 16) + ":" + Convert.ToString(me.Memory[i], 2).PadLeft(16, '0') + Environment.NewLine;
            }
        }

        public void DidMemoryAccessed(Common.MemoryEventArgs me)
        {
            // メモリの状態をRefferedへ変更
            this._ICs["MM"].State = ICState.Reffered;
            this._ICs["MM"].Data = "";

            // メモリの内容を更新
            for (ushort i = me.StartAddress; i <= me.EndAddress; i++)
            {
                // 変更件数が多いときははしょる
                if (i >= me.StartAddress + 1) { this._ICs["MM"].Data += "...";  break; }

                // 更新
                this._ICs["MM"].Data += "0x" + Convert.ToString(i, 16) + ":" + Convert.ToString(me.Memory[i], 2).PadLeft(16, '0') + Environment.NewLine;
            }
        }

        public void DidPreDataMoved(Common.DataMovedEventArgs dme)
        {
            // コネクタ名を生成
            string connector_name = dme.Sender + "to" + dme.Reciever;


            // コネクタ名がコネクタとして登録されているか
            if (this._Connectors.ContainsKey(connector_name))
            {
                // コネクタの状態をOnElectricityへ
                this._Connectors[connector_name].State = Connector.ConnectState.OnElectricity;
            }
            else
            {
                // エラー
                Console.WriteLine(connector_name+ ", NotFound!!");
            }

        }

        // マシンの実行状態に合わせた処理
        public void DidCycleBegin()
        {
            //this._ResetCycleVariables();
        }
        public void DidCycleEnd()
        {
            //this._ResetCycleVariables();
        }
        public void DidCycleDecode()
        {
            //this._ResetCycleVariables();
        }
        public void DidCycleUpdateIR()
        {
            //this._ResetCycleVariables();
        }
        public void DidCycleOpecode()
        {
            //this._ResetCycleVariables();
        }
        public void DidCycleUpdatePC()
        {
            //this._ResetCycleVariables();
        }

        // オーバフロー発生時
        public void DidOverflowed(Common.OverflowedEventArgs ove)
        {
            this._ICs["OV"].State = ICState.Changed;
        }


        #endregion

        #region プライベート

        private Simulator.Common.ApplicationSettings _Settings;

        private Dictionary<Connector.ConnectState, Pen> _ConnectorPen = new Dictionary<Connector.ConnectState,Pen>();
        private Dictionary<ICState, Brush> _ICBrush = new Dictionary<ICState,Brush>();
        private Dictionary<ICState, Pen> _ICPen = new Dictionary<ICState,Pen>();
        private Font _ICFont;
        private SortedDictionary<string, Connector> _Connectors = new SortedDictionary<string,Connector>();
        private Dictionary<string, IC> _ICs = new Dictionary<string,IC>();

        private Size _BaseSize;
        private Double _WidthRatio;
        private Double _HeightRatio;

        // ICとコネクタの状態を初期状態に戻す
        // 表示している値などはそのまま
        private void _ResetCycleVariables()
        {
            foreach (string item in this._ICs.Keys.ToArray())
            {
                this._ICs[item].State = ICState.Normal;
            }
            foreach (string connector in this._Connectors.Keys.ToArray())
            {
                this._Connectors[connector].State = Connector.ConnectState.Visible;
            }
        }


        #endregion

    }
}
