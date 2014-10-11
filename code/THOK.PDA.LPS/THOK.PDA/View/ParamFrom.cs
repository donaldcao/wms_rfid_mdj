using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AS.PDA.Util;

namespace AS.PDA.View
{
    public partial class ParamFrom : Form
    {

        private ConfigUtil configUtil = new ConfigUtil();
        public ParamFrom()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            inputPanel1.Enabled = !inputPanel1.Enabled;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> httpStr = new Dictionary<string, string>();

                httpStr.Add("HttpConnStr", this.txtHttpStr.Text);
                httpStr.Add("positionName", this.txtpositionName.Text);

                configUtil.SaveConfig("HttpConnectionStr", httpStr);
                inputPanel1.Enabled = false;

                MessageBox.Show("Parameters saved successfully! Please reboot the system");
            }
            catch (Exception)
            {
                MessageBox.Show("Parameters save failed! Please try again");
            }
        }

        private void ParamFrom_Load(object sender, EventArgs e)
        {
            string HttpString = configUtil.GetConfig("HttpConnectionStr")["HttpConnStr"];
            string positionName = configUtil.GetConfig("HttpConnectionStr")["positionName"];
            this.txtHttpStr.Text = HttpString;
            this.txtpositionName.Text = positionName;
        }

        private void ParamFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Escape")
            {
                this.btnReturn_Click(null, null);
            }
            if (e.KeyCode.ToString() == "Return")
            {
                this.button2_Click(null, null);
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
             Close();

        }      
    }
}