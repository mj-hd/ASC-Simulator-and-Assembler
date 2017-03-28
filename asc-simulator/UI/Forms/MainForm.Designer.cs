namespace Simulator.UI.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.プログラムを開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ファイルへ書き出すToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.動作ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.初期化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.メモリを初期化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ブレークポイントを初期化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.表示を更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.指定アドレスを表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.使い方ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.このプログラムについてToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.Strip = new System.Windows.Forms.ToolStrip();
            this.InitStripButton = new System.Windows.Forms.ToolStripButton();
            this.StripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.NextStepStripButton = new System.Windows.Forms.ToolStripButton();
            this.NextStripButton = new System.Windows.Forms.ToolStripButton();
            this.StartStripButton = new System.Windows.Forms.ToolStripButton();
            this.PauseStripButton = new System.Windows.Forms.ToolStripButton();
            this.StripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.StatusLabel = new System.Windows.Forms.ToolStripLabel();
            this.StripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.SpeedTrackBar = new Simulator.UI.Forms.ToolStripTrackBar();
            this.SpeedLabel = new System.Windows.Forms.ToolStripLabel();
            this.InformationLabel = new System.Windows.Forms.ToolStripLabel();
            this.ASCViewer = new System.Windows.Forms.GroupBox();
            this.Registers = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RegistersView = new Simulator.UI.Forms.MyDataGridView();
            this.MM = new System.Windows.Forms.GroupBox();
            this.MMView = new Simulator.UI.Forms.MyDataGridView();
            this.DisplayPanel = new System.Windows.Forms.Panel();
            this.MenuBar.SuspendLayout();
            this.Strip.SuspendLayout();
            this.ASCViewer.SuspendLayout();
            this.Registers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RegistersView)).BeginInit();
            this.MM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MMView)).BeginInit();
            this.SuspendLayout();
            // 
            // MenuBar
            // 
            this.MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.動作ToolStripMenuItem,
            this.ヘルプToolStripMenuItem});
            this.MenuBar.Location = new System.Drawing.Point(0, 0);
            this.MenuBar.Name = "MenuBar";
            this.MenuBar.Size = new System.Drawing.Size(1048, 24);
            this.MenuBar.TabIndex = 1;
            this.MenuBar.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            this.ファイルToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.プログラムを開くToolStripMenuItem,
            this.ファイルへ書き出すToolStripMenuItem,
            this.toolStripMenuItem3,
            this.設定ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.終了ToolStripMenuItem});
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.ファイルToolStripMenuItem.Text = "ファイル";
            // 
            // プログラムを開くToolStripMenuItem
            // 
            this.プログラムを開くToolStripMenuItem.Name = "プログラムを開くToolStripMenuItem";
            this.プログラムを開くToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.プログラムを開くToolStripMenuItem.Text = "ファイルを開く";
            this.プログラムを開くToolStripMenuItem.Click += new System.EventHandler(this.プログラムを開くToolStripMenuItem_Click);
            // 
            // ファイルへ書き出すToolStripMenuItem
            // 
            this.ファイルへ書き出すToolStripMenuItem.Name = "ファイルへ書き出すToolStripMenuItem";
            this.ファイルへ書き出すToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.ファイルへ書き出すToolStripMenuItem.Text = "ファイルへ書き出す";
            this.ファイルへ書き出すToolStripMenuItem.Click += new System.EventHandler(this.ファイルへ書き出すToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(158, 6);
            // 
            // 設定ToolStripMenuItem
            // 
            this.設定ToolStripMenuItem.Name = "設定ToolStripMenuItem";
            this.設定ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.設定ToolStripMenuItem.Text = "設定";
            this.設定ToolStripMenuItem.Click += new System.EventHandler(this.設定ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
            // 
            // 終了ToolStripMenuItem
            // 
            this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
            this.終了ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.終了ToolStripMenuItem.Text = "終了";
            this.終了ToolStripMenuItem.Click += new System.EventHandler(this.終了ToolStripMenuItem_Click);
            // 
            // 動作ToolStripMenuItem
            // 
            this.動作ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.初期化ToolStripMenuItem,
            this.メモリを初期化ToolStripMenuItem,
            this.ブレークポイントを初期化ToolStripMenuItem,
            this.toolStripMenuItem2,
            this.表示を更新ToolStripMenuItem,
            this.指定アドレスを表示ToolStripMenuItem});
            this.動作ToolStripMenuItem.Name = "動作ToolStripMenuItem";
            this.動作ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.動作ToolStripMenuItem.Text = "動作";
            // 
            // 初期化ToolStripMenuItem
            // 
            this.初期化ToolStripMenuItem.Name = "初期化ToolStripMenuItem";
            this.初期化ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.初期化ToolStripMenuItem.Text = "レジスタとフラグを初期化";
            this.初期化ToolStripMenuItem.Click += new System.EventHandler(this.初期化ToolStripMenuItem_Click);
            // 
            // メモリを初期化ToolStripMenuItem
            // 
            this.メモリを初期化ToolStripMenuItem.Name = "メモリを初期化ToolStripMenuItem";
            this.メモリを初期化ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.メモリを初期化ToolStripMenuItem.Text = "メモリを初期化";
            this.メモリを初期化ToolStripMenuItem.Click += new System.EventHandler(this.メモリを初期化ToolStripMenuItem_Click);
            // 
            // ブレークポイントを初期化ToolStripMenuItem
            // 
            this.ブレークポイントを初期化ToolStripMenuItem.Name = "ブレークポイントを初期化ToolStripMenuItem";
            this.ブレークポイントを初期化ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.ブレークポイントを初期化ToolStripMenuItem.Text = "ブレークポイントを初期化";
            this.ブレークポイントを初期化ToolStripMenuItem.Click += new System.EventHandler(this.ブレークポイントを初期化ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(188, 6);
            // 
            // 表示を更新ToolStripMenuItem
            // 
            this.表示を更新ToolStripMenuItem.Name = "表示を更新ToolStripMenuItem";
            this.表示を更新ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.表示を更新ToolStripMenuItem.Text = "表示を更新";
            this.表示を更新ToolStripMenuItem.Click += new System.EventHandler(this.表示を更新ToolStripMenuItem_Click);
            // 
            // 指定アドレスを表示ToolStripMenuItem
            // 
            this.指定アドレスを表示ToolStripMenuItem.Name = "指定アドレスを表示ToolStripMenuItem";
            this.指定アドレスを表示ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.指定アドレスを表示ToolStripMenuItem.Text = "指定アドレスへ移動";
            this.指定アドレスを表示ToolStripMenuItem.Click += new System.EventHandler(this.指定アドレスを表示ToolStripMenuItem_Click);
            // 
            // ヘルプToolStripMenuItem
            // 
            this.ヘルプToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.使い方ToolStripMenuItem,
            this.このプログラムについてToolStripMenuItem});
            this.ヘルプToolStripMenuItem.Name = "ヘルプToolStripMenuItem";
            this.ヘルプToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.ヘルプToolStripMenuItem.Text = "ヘルプ";
            // 
            // 使い方ToolStripMenuItem
            // 
            this.使い方ToolStripMenuItem.Name = "使い方ToolStripMenuItem";
            this.使い方ToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.使い方ToolStripMenuItem.Text = "使い方";
            this.使い方ToolStripMenuItem.Click += new System.EventHandler(this.使い方ToolStripMenuItem_Click);
            // 
            // このプログラムについてToolStripMenuItem
            // 
            this.このプログラムについてToolStripMenuItem.Name = "このプログラムについてToolStripMenuItem";
            this.このプログラムについてToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.このプログラムについてToolStripMenuItem.Text = "このプログラムについて";
            this.このプログラムについてToolStripMenuItem.Click += new System.EventHandler(this.このプログラムについてToolStripMenuItem_Click);
            // 
            // OpenDialog
            // 
            this.OpenDialog.AddExtension = false;
            this.OpenDialog.DefaultExt = "asco";
            this.OpenDialog.Filter = "ASCオブジェクトファイル|*.asco|すべてのファイル|*.*";
            this.OpenDialog.InitialDirectory = ".\\";
            this.OpenDialog.RestoreDirectory = true;
            this.OpenDialog.Title = "プログラムを開く";
            this.OpenDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenDialog_FileOk);
            // 
            // SaveDialog
            // 
            this.SaveDialog.AddExtension = false;
            this.SaveDialog.DefaultExt = "asco";
            this.SaveDialog.Filter = "ASCオブジェクトファイル|*.asco|全てのファイル|*.*";
            this.SaveDialog.InitialDirectory = ".\\";
            this.SaveDialog.RestoreDirectory = true;
            this.SaveDialog.Title = "メモリを保存";
            this.SaveDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveDialog_FileOk);
            // 
            // Strip
            // 
            this.Strip.AutoSize = false;
            this.Strip.CanOverflow = false;
            this.Strip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Strip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.Strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InitStripButton,
            this.StripSeparator1,
            this.NextStepStripButton,
            this.NextStripButton,
            this.StartStripButton,
            this.PauseStripButton,
            this.StripSeparator2,
            this.StatusLabel,
            this.StripSeparator3,
            this.SpeedTrackBar,
            this.SpeedLabel,
            this.InformationLabel});
            this.Strip.Location = new System.Drawing.Point(0, 564);
            this.Strip.Name = "Strip";
            this.Strip.Size = new System.Drawing.Size(1048, 50);
            this.Strip.TabIndex = 2;
            // 
            // InitStripButton
            // 
            this.InitStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.InitStripButton.Image = global::Simulator.Properties.Resources.init_old;
            this.InitStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.InitStripButton.Name = "InitStripButton";
            this.InitStripButton.Size = new System.Drawing.Size(36, 47);
            this.InitStripButton.Text = "初期化";
            this.InitStripButton.Click += new System.EventHandler(this.InitStripButton_Click);
            // 
            // StripSeparator1
            // 
            this.StripSeparator1.Name = "StripSeparator1";
            this.StripSeparator1.Size = new System.Drawing.Size(6, 50);
            // 
            // NextStepStripButton
            // 
            this.NextStepStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NextStepStripButton.Image = global::Simulator.Properties.Resources.step2;
            this.NextStepStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NextStepStripButton.Name = "NextStepStripButton";
            this.NextStepStripButton.Size = new System.Drawing.Size(36, 47);
            this.NextStepStripButton.Text = "通常実行";
            this.NextStepStripButton.Click += new System.EventHandler(this.NextStepStripButton_Click);
            // 
            // NextStripButton
            // 
            this.NextStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NextStripButton.Image = global::Simulator.Properties.Resources.step;
            this.NextStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NextStripButton.Name = "NextStripButton";
            this.NextStripButton.Size = new System.Drawing.Size(36, 47);
            this.NextStripButton.Text = "命令単位ステップラン";
            this.NextStripButton.Click += new System.EventHandler(this.ステップを進めるToolStripMenuItem_Click);
            // 
            // StartStripButton
            // 
            this.StartStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.StartStripButton.Image = global::Simulator.Properties.Resources.run;
            this.StartStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StartStripButton.Name = "StartStripButton";
            this.StartStripButton.Size = new System.Drawing.Size(36, 47);
            this.StartStripButton.Text = "状態単位ステップラン";
            this.StartStripButton.Click += new System.EventHandler(this.実行ToolStripMenuItem_Click);
            // 
            // PauseStripButton
            // 
            this.PauseStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PauseStripButton.Image = global::Simulator.Properties.Resources.stop;
            this.PauseStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PauseStripButton.Name = "PauseStripButton";
            this.PauseStripButton.Size = new System.Drawing.Size(36, 47);
            this.PauseStripButton.Text = "ブレーク";
            this.PauseStripButton.Click += new System.EventHandler(this.PauseStripButton_Click);
            // 
            // StripSeparator2
            // 
            this.StripSeparator2.Name = "StripSeparator2";
            this.StripSeparator2.Size = new System.Drawing.Size(6, 50);
            // 
            // StatusLabel
            // 
            this.StatusLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(55, 47);
            this.StatusLabel.Text = "準備完了";
            // 
            // StripSeparator3
            // 
            this.StripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.StripSeparator3.Name = "StripSeparator3";
            this.StripSeparator3.Size = new System.Drawing.Size(6, 50);
            // 
            // SpeedTrackBar
            // 
            this.SpeedTrackBar.Name = "SpeedTrackBar";
            this.SpeedTrackBar.Size = new System.Drawing.Size(104, 47);
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new System.Drawing.Size(90, 47);
            this.SpeedLabel.Text = "実行速度: 50%";
            // 
            // InformationLabel
            // 
            this.InformationLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.InformationLabel.Name = "InformationLabel";
            this.InformationLabel.Size = new System.Drawing.Size(0, 47);
            // 
            // ASCViewer
            // 
            this.ASCViewer.Controls.Add(this.Registers);
            this.ASCViewer.Controls.Add(this.MM);
            this.ASCViewer.Dock = System.Windows.Forms.DockStyle.Right;
            this.ASCViewer.Location = new System.Drawing.Point(693, 24);
            this.ASCViewer.Name = "ASCViewer";
            this.ASCViewer.Size = new System.Drawing.Size(355, 540);
            this.ASCViewer.TabIndex = 5;
            this.ASCViewer.TabStop = false;
            this.ASCViewer.Text = "ASCマシンの情報";
            // 
            // Registers
            // 
            this.Registers.Controls.Add(this.groupBox1);
            this.Registers.Controls.Add(this.RegistersView);
            this.Registers.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Registers.Location = new System.Drawing.Point(3, 358);
            this.Registers.Name = "Registers";
            this.Registers.Size = new System.Drawing.Size(349, 179);
            this.Registers.TabIndex = 2;
            this.Registers.TabStop = false;
            this.Registers.Text = "レジスタ";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(3, 182);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 97);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // RegistersView
            // 
            this.RegistersView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RegistersView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegistersView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.RegistersView.HighlightedIndex = -1;
            this.RegistersView.Location = new System.Drawing.Point(3, 15);
            this.RegistersView.Name = "RegistersView";
            this.RegistersView.RowHeadersVisible = false;
            this.RegistersView.RowTemplate.Height = 21;
            this.RegistersView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.RegistersView.SecondaryHighlightedIndex = -1;
            this.RegistersView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.RegistersView.Size = new System.Drawing.Size(343, 161);
            this.RegistersView.TabIndex = 0;
            this.RegistersView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.RegistersView_CellEndEdit);
            // 
            // MM
            // 
            this.MM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.MM.Controls.Add(this.MMView);
            this.MM.Location = new System.Drawing.Point(3, 15);
            this.MM.Name = "MM";
            this.MM.Size = new System.Drawing.Size(349, 337);
            this.MM.TabIndex = 1;
            this.MM.TabStop = false;
            this.MM.Text = "MM";
            // 
            // MMView
            // 
            this.MMView.AllowUserToAddRows = false;
            this.MMView.AllowUserToDeleteRows = false;
            this.MMView.AllowUserToResizeColumns = false;
            this.MMView.AllowUserToResizeRows = false;
            this.MMView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MMView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MMView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.MMView.HighlightedIndex = -1;
            this.MMView.Location = new System.Drawing.Point(3, 15);
            this.MMView.Name = "MMView";
            this.MMView.RowTemplate.Height = 21;
            this.MMView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MMView.SecondaryHighlightedIndex = -1;
            this.MMView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MMView.Size = new System.Drawing.Size(343, 319);
            this.MMView.TabIndex = 0;
            this.MMView.RowChecked += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MMView_RowChecked);
            this.MMView.RowUnchecked += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MMView_RowUnchecked);
            this.MMView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.MMView_CellEndEdit);
            this.MMView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.MMView_CellValidating);
            // 
            // DisplayPanel
            // 
            this.DisplayPanel.AutoSize = true;
            this.DisplayPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisplayPanel.Location = new System.Drawing.Point(0, 24);
            this.DisplayPanel.Name = "DisplayPanel";
            this.DisplayPanel.Size = new System.Drawing.Size(693, 540);
            this.DisplayPanel.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1048, 614);
            this.Controls.Add(this.DisplayPanel);
            this.Controls.Add(this.ASCViewer);
            this.Controls.Add(this.Strip);
            this.Controls.Add(this.MenuBar);
            this.MainMenuStrip = this.MenuBar;
            this.Name = "MainForm";
            this.Text = "ASC Simulator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.MenuBar.ResumeLayout(false);
            this.MenuBar.PerformLayout();
            this.Strip.ResumeLayout(false);
            this.Strip.PerformLayout();
            this.ASCViewer.ResumeLayout(false);
            this.Registers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RegistersView)).EndInit();
            this.MM.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MMView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuBar;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem プログラムを開くToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ファイルへ書き出すToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 動作ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 初期化ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ヘルプToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 使い方ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem このプログラムについてToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog OpenDialog;
        private System.Windows.Forms.SaveFileDialog SaveDialog;
        private System.Windows.Forms.ToolStrip Strip;
        private System.Windows.Forms.ToolStripButton InitStripButton;
        private System.Windows.Forms.ToolStripSeparator StripSeparator2;
        private System.Windows.Forms.ToolStripButton StartStripButton;
        private System.Windows.Forms.ToolStripButton PauseStripButton;
        private System.Windows.Forms.ToolStripButton NextStripButton;
        private System.Windows.Forms.ToolStripLabel StatusLabel;
        private System.Windows.Forms.ToolStripSeparator StripSeparator1;
        private Simulator.UI.Forms.ToolStripTrackBar SpeedTrackBar;

        private System.Windows.Forms.GroupBox ASCViewer;
        private System.Windows.Forms.GroupBox MM;
        private System.Windows.Forms.GroupBox Registers;
        private MyDataGridView MMView;
        private MyDataGridView RegistersView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripButton NextStepStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem 設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem メモリを初期化ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ブレークポイントを初期化ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 表示を更新ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator StripSeparator3;
        private System.Windows.Forms.ToolStripLabel InformationLabel;
        private System.Windows.Forms.ToolStripLabel SpeedLabel;
        private System.Windows.Forms.ToolStripMenuItem 指定アドレスを表示ToolStripMenuItem;
        private System.Windows.Forms.Panel DisplayPanel;
    }
}