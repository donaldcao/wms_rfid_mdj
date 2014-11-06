namespace AS.PDA.View
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnParam = new System.Windows.Forms.Button();
            this.btnPallet = new System.Windows.Forms.Button();
            this.lblSystem = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(31, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 17);
            this.label1.Text = "www.SkySeaOK.Com";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.btnExit.Location = new System.Drawing.Point(12, 168);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(288, 40);
            this.btnExit.TabIndex = 12;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnParam
            // 
            this.btnParam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.btnParam.Location = new System.Drawing.Point(12, 122);
            this.btnParam.Name = "btnParam";
            this.btnParam.Size = new System.Drawing.Size(288, 40);
            this.btnParam.TabIndex = 11;
            this.btnParam.Text = "Parameter settings";
            this.btnParam.Click += new System.EventHandler(this.btnParam_Click);
            // 
            // btnPallet
            // 
            this.btnPallet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.btnPallet.Location = new System.Drawing.Point(12, 76);
            this.btnPallet.Name = "btnPallet";
            this.btnPallet.Size = new System.Drawing.Size(288, 40);
            this.btnPallet.TabIndex = 10;
            this.btnPallet.Text = "Start warehousing";
            this.btnPallet.Click += new System.EventHandler(this.btnPallet_Click);
            // 
            // lblSystem
            // 
            this.lblSystem.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblSystem.Location = new System.Drawing.Point(54, 19);
            this.lblSystem.Name = "lblSystem";
            this.lblSystem.Size = new System.Drawing.Size(204, 39);
            this.lblSystem.Text = "Storage System";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(318, 275);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnParam);
            this.Controls.Add(this.btnPallet);
            this.Controls.Add(this.lblSystem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Text = "Main Menu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnParam;
        private System.Windows.Forms.Button btnPallet;
        private System.Windows.Forms.Label lblSystem;
    }
}

