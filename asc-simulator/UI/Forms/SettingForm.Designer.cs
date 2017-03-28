namespace Simulator.UI.Forms
{
    partial class SettingForm
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
            this.SettingTab = new System.Windows.Forms.TabControl();
            this.GeneralPage = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.AppearencePage = new System.Windows.Forms.TabPage();
            this.RegisterColorModifyInput = new System.Windows.Forms.TextBox();
            this.RegisterColorRefecenceInput = new System.Windows.Forms.TextBox();
            this.RegisterColorNormalInput = new System.Windows.Forms.TextBox();
            this.ConnectorColorActiveInput = new System.Windows.Forms.TextBox();
            this.ConnectorColorNormalInput = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ConnectorWidthUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.RegisterMarginUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.AttitudePage = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.StartupInitializeCheck = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BaseSpeedUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.SettingTab.SuspendLayout();
            this.GeneralPage.SuspendLayout();
            this.AppearencePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConnectorWidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterMarginUpDown)).BeginInit();
            this.AttitudePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BaseSpeedUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // SettingTab
            // 
            this.SettingTab.Controls.Add(this.GeneralPage);
            this.SettingTab.Controls.Add(this.AppearencePage);
            this.SettingTab.Controls.Add(this.AttitudePage);
            this.SettingTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingTab.ItemSize = new System.Drawing.Size(80, 70);
            this.SettingTab.Location = new System.Drawing.Point(0, 0);
            this.SettingTab.Name = "SettingTab";
            this.SettingTab.SelectedIndex = 0;
            this.SettingTab.Size = new System.Drawing.Size(753, 458);
            this.SettingTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.SettingTab.TabIndex = 0;
            // 
            // GeneralPage
            // 
            this.GeneralPage.Controls.Add(this.button1);
            this.GeneralPage.Location = new System.Drawing.Point(4, 74);
            this.GeneralPage.Name = "GeneralPage";
            this.GeneralPage.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralPage.Size = new System.Drawing.Size(745, 380);
            this.GeneralPage.TabIndex = 0;
            this.GeneralPage.Text = "一般";
            this.GeneralPage.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(8, 328);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(731, 46);
            this.button1.TabIndex = 0;
            this.button1.Text = "全ての設定を元に戻す";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AppearencePage
            // 
            this.AppearencePage.Controls.Add(this.label15);
            this.AppearencePage.Controls.Add(this.RegisterColorModifyInput);
            this.AppearencePage.Controls.Add(this.RegisterColorRefecenceInput);
            this.AppearencePage.Controls.Add(this.RegisterColorNormalInput);
            this.AppearencePage.Controls.Add(this.ConnectorColorActiveInput);
            this.AppearencePage.Controls.Add(this.ConnectorColorNormalInput);
            this.AppearencePage.Controls.Add(this.label14);
            this.AppearencePage.Controls.Add(this.label12);
            this.AppearencePage.Controls.Add(this.label13);
            this.AppearencePage.Controls.Add(this.label11);
            this.AppearencePage.Controls.Add(this.label10);
            this.AppearencePage.Controls.Add(this.label9);
            this.AppearencePage.Controls.Add(this.label8);
            this.AppearencePage.Controls.Add(this.ConnectorWidthUpDown);
            this.AppearencePage.Controls.Add(this.label7);
            this.AppearencePage.Controls.Add(this.label6);
            this.AppearencePage.Controls.Add(this.RegisterMarginUpDown);
            this.AppearencePage.Controls.Add(this.label5);
            this.AppearencePage.Location = new System.Drawing.Point(4, 74);
            this.AppearencePage.Name = "AppearencePage";
            this.AppearencePage.Padding = new System.Windows.Forms.Padding(3);
            this.AppearencePage.Size = new System.Drawing.Size(745, 380);
            this.AppearencePage.TabIndex = 1;
            this.AppearencePage.Text = "見た目";
            this.AppearencePage.UseVisualStyleBackColor = true;
            // 
            // RegisterColorModifyInput
            // 
            this.RegisterColorModifyInput.Location = new System.Drawing.Point(334, 187);
            this.RegisterColorModifyInput.Name = "RegisterColorModifyInput";
            this.RegisterColorModifyInput.Size = new System.Drawing.Size(100, 19);
            this.RegisterColorModifyInput.TabIndex = 20;
            this.RegisterColorModifyInput.TextChanged += new System.EventHandler(this.RegisterColorModifyInput_TextChanged);
            // 
            // RegisterColorRefecenceInput
            // 
            this.RegisterColorRefecenceInput.Location = new System.Drawing.Point(334, 161);
            this.RegisterColorRefecenceInput.Name = "RegisterColorRefecenceInput";
            this.RegisterColorRefecenceInput.Size = new System.Drawing.Size(100, 19);
            this.RegisterColorRefecenceInput.TabIndex = 19;
            this.RegisterColorRefecenceInput.TextChanged += new System.EventHandler(this.RegisterColorRefecenceInput_TextChanged);
            // 
            // RegisterColorNormalInput
            // 
            this.RegisterColorNormalInput.Location = new System.Drawing.Point(334, 136);
            this.RegisterColorNormalInput.Name = "RegisterColorNormalInput";
            this.RegisterColorNormalInput.Size = new System.Drawing.Size(100, 19);
            this.RegisterColorNormalInput.TabIndex = 18;
            this.RegisterColorNormalInput.TextChanged += new System.EventHandler(this.RegisterColorNormalInput_TextChanged);
            // 
            // ConnectorColorActiveInput
            // 
            this.ConnectorColorActiveInput.Location = new System.Drawing.Point(334, 98);
            this.ConnectorColorActiveInput.Name = "ConnectorColorActiveInput";
            this.ConnectorColorActiveInput.Size = new System.Drawing.Size(100, 19);
            this.ConnectorColorActiveInput.TabIndex = 17;
            this.ConnectorColorActiveInput.TextChanged += new System.EventHandler(this.ConnectorColorActiveInput_TextChanged);
            // 
            // ConnectorColorNormalInput
            // 
            this.ConnectorColorNormalInput.Location = new System.Drawing.Point(334, 73);
            this.ConnectorColorNormalInput.Name = "ConnectorColorNormalInput";
            this.ConnectorColorNormalInput.Size = new System.Drawing.Size(100, 19);
            this.ConnectorColorNormalInput.TabIndex = 16;
            this.ConnectorColorNormalInput.TextChanged += new System.EventHandler(this.ConnectorColorNormalInput_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label14.Location = new System.Drawing.Point(197, 190);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 16);
            this.label14.TabIndex = 15;
            this.label14.Text = "書換時";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label12.Location = new System.Drawing.Point(197, 164);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 16);
            this.label12.TabIndex = 14;
            this.label12.Text = "参照時";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label13.Location = new System.Drawing.Point(197, 139);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 16);
            this.label13.TabIndex = 13;
            this.label13.Text = "通常時";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label11.Location = new System.Drawing.Point(9, 139);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 16);
            this.label11.TabIndex = 12;
            this.label11.Text = "レジスタの色";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(197, 101);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 16);
            this.label10.TabIndex = 11;
            this.label10.Text = "アクティブ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label9.Location = new System.Drawing.Point(197, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 16);
            this.label9.TabIndex = 10;
            this.label9.Text = "通常時";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(9, 76);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 16);
            this.label8.TabIndex = 9;
            this.label8.Text = "コネクタの色";
            // 
            // ConnectorWidthUpDown
            // 
            this.ConnectorWidthUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ConnectorWidthUpDown.Location = new System.Drawing.Point(197, 38);
            this.ConnectorWidthUpDown.Name = "ConnectorWidthUpDown";
            this.ConnectorWidthUpDown.Size = new System.Drawing.Size(120, 23);
            this.ConnectorWidthUpDown.TabIndex = 8;
            this.ConnectorWidthUpDown.ValueChanged += new System.EventHandler(this.ConnectorWidthUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(9, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 16);
            this.label7.TabIndex = 7;
            this.label7.Text = "線の太さ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(335, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(407, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "画面を拡大した時などに、レジスタ同士を繋ぐ線が見づらいときに値を増やしてください。";
            // 
            // RegisterMarginUpDown
            // 
            this.RegisterMarginUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RegisterMarginUpDown.Location = new System.Drawing.Point(197, 10);
            this.RegisterMarginUpDown.Name = "RegisterMarginUpDown";
            this.RegisterMarginUpDown.Size = new System.Drawing.Size(120, 23);
            this.RegisterMarginUpDown.TabIndex = 1;
            this.RegisterMarginUpDown.ValueChanged += new System.EventHandler(this.RegisterMarginUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(9, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "レジスタのマージン";
            // 
            // AttitudePage
            // 
            this.AttitudePage.Controls.Add(this.label4);
            this.AttitudePage.Controls.Add(this.StartupInitializeCheck);
            this.AttitudePage.Controls.Add(this.label3);
            this.AttitudePage.Controls.Add(this.label2);
            this.AttitudePage.Controls.Add(this.BaseSpeedUpDown);
            this.AttitudePage.Controls.Add(this.label1);
            this.AttitudePage.Location = new System.Drawing.Point(4, 74);
            this.AttitudePage.Name = "AttitudePage";
            this.AttitudePage.Padding = new System.Windows.Forms.Padding(3);
            this.AttitudePage.Size = new System.Drawing.Size(745, 380);
            this.AttitudePage.TabIndex = 2;
            this.AttitudePage.Text = "動作";
            this.AttitudePage.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(324, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "次回起動時から有効になります。";
            // 
            // StartupInitializeCheck
            // 
            this.StartupInitializeCheck.AutoSize = true;
            this.StartupInitializeCheck.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.StartupInitializeCheck.Location = new System.Drawing.Point(12, 14);
            this.StartupInitializeCheck.Name = "StartupInitializeCheck";
            this.StartupInitializeCheck.Size = new System.Drawing.Size(222, 20);
            this.StartupInitializeCheck.TabIndex = 4;
            this.StartupInitializeCheck.Text = "起動時にレジスタを初期化する";
            this.StartupInitializeCheck.UseVisualStyleBackColor = true;
            this.StartupInitializeCheck.CheckedChanged += new System.EventHandler(this.StartupInitializeCheck_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 12);
            this.label3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(324, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(296, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ASCマシンの実行速度を最も遅くした時の、実行の間隔です。";
            // 
            // BaseSpeedUpDown
            // 
            this.BaseSpeedUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BaseSpeedUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.BaseSpeedUpDown.Location = new System.Drawing.Point(182, 40);
            this.BaseSpeedUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.BaseSpeedUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.BaseSpeedUpDown.Name = "BaseSpeedUpDown";
            this.BaseSpeedUpDown.Size = new System.Drawing.Size(120, 23);
            this.BaseSpeedUpDown.TabIndex = 1;
            this.BaseSpeedUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.BaseSpeedUpDown.ValueChanged += new System.EventHandler(this.BaseSpeedUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(9, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "実行の間隔[ms]";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label15.Location = new System.Drawing.Point(335, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(158, 12);
            this.label15.TabIndex = 21;
            this.label15.Text = "プログラムの再起動が必要です。";
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 458);
            this.Controls.Add(this.SettingTab);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingForm";
            this.Text = "設定";
            this.SettingTab.ResumeLayout(false);
            this.GeneralPage.ResumeLayout(false);
            this.AppearencePage.ResumeLayout(false);
            this.AppearencePage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConnectorWidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterMarginUpDown)).EndInit();
            this.AttitudePage.ResumeLayout(false);
            this.AttitudePage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BaseSpeedUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl SettingTab;
        private System.Windows.Forms.TabPage GeneralPage;
        private System.Windows.Forms.TabPage AppearencePage;
        private System.Windows.Forms.TabPage AttitudePage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown BaseSpeedUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox StartupInitializeCheck;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox RegisterColorModifyInput;
        private System.Windows.Forms.TextBox RegisterColorRefecenceInput;
        private System.Windows.Forms.TextBox RegisterColorNormalInput;
        private System.Windows.Forms.TextBox ConnectorColorActiveInput;
        private System.Windows.Forms.TextBox ConnectorColorNormalInput;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown ConnectorWidthUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown RegisterMarginUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label15;
    }
}