namespace Simulator.UI.Forms
{
    partial class JumpBox
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
            this.JumpButton = new System.Windows.Forms.Button();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.AddressUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.AddressUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // JumpButton
            // 
            this.JumpButton.Location = new System.Drawing.Point(138, 44);
            this.JumpButton.Name = "JumpButton";
            this.JumpButton.Size = new System.Drawing.Size(124, 23);
            this.JumpButton.TabIndex = 0;
            this.JumpButton.Text = "移動";
            this.JumpButton.UseVisualStyleBackColor = true;
            this.JumpButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddressLabel.Location = new System.Drawing.Point(11, 14);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(51, 15);
            this.AddressLabel.TabIndex = 2;
            this.AddressLabel.Text = "アドレス";
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(19, 44);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(113, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "キャンセル";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // AddressUpDown
            // 
            this.AddressUpDown.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddressUpDown.Hexadecimal = true;
            this.AddressUpDown.Location = new System.Drawing.Point(68, 12);
            this.AddressUpDown.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.AddressUpDown.Name = "AddressUpDown";
            this.AddressUpDown.Size = new System.Drawing.Size(194, 22);
            this.AddressUpDown.TabIndex = 4;
            // 
            // JumpBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 77);
            this.Controls.Add(this.AddressUpDown);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.AddressLabel);
            this.Controls.Add(this.JumpButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "JumpBox";
            this.Text = "指定したアドレスへ移動";
            ((System.ComponentModel.ISupportInitialize)(this.AddressUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button JumpButton;
        private System.Windows.Forms.Label AddressLabel;
        private new System.Windows.Forms.Button CancelButton;
        public System.Windows.Forms.NumericUpDown AddressUpDown;
    }
}