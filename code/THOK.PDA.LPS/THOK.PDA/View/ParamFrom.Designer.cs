namespace AS.PDA.View
{
    partial class ParamFrom
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
            this.txtpositionName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtHttpStr = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.btnReturn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtpositionName
            // 
            this.txtpositionName.Location = new System.Drawing.Point(100, 105);
            this.txtpositionName.Name = "txtpositionName";
            this.txtpositionName.Size = new System.Drawing.Size(194, 23);
            this.txtpositionName.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.Text = "Position Name:";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Info;
            this.button2.Location = new System.Drawing.Point(204, 187);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 40);
            this.button2.TabIndex = 32;
            this.button2.Text = "Save";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Info;
            this.button1.Location = new System.Drawing.Point(11, 187);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 40);
            this.button1.TabIndex = 31;
            this.button1.Text = "Keyboard";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtHttpStr
            // 
            this.txtHttpStr.Location = new System.Drawing.Point(100, 45);
            this.txtHttpStr.Name = "txtHttpStr";
            this.txtHttpStr.ReadOnly = true;
            this.txtHttpStr.Size = new System.Drawing.Size(194, 23);
            this.txtHttpStr.TabIndex = 30;
            // 
            // lblIP
            // 
            this.lblIP.Location = new System.Drawing.Point(4, 48);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(100, 20);
            this.lblIP.Text = "Web Address:";
            // 
            // btnReturn
            // 
            this.btnReturn.BackColor = System.Drawing.SystemColors.Info;
            this.btnReturn.Location = new System.Drawing.Point(108, 187);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(90, 40);
            this.btnReturn.TabIndex = 29;
            this.btnReturn.Text = "Exit";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // ParamFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(318, 275);
            this.Controls.Add(this.txtpositionName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtHttpStr);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.btnReturn);
            this.Name = "ParamFrom";
            this.Text = "Parameter settings";
            this.Load += new System.EventHandler(this.ParamFrom_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ParamFrom_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
        private System.Windows.Forms.TextBox txtpositionName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtHttpStr;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Button btnReturn;
    }
}