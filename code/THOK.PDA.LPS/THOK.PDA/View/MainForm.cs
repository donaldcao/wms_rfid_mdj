using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AS.PDA.View
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnPallet_Click(object sender, EventArgs e)
        {
            ApplyForm bill = new ApplyForm();

           
            bill.ShowDialog();
        }

        private void btnParam_Click(object sender, EventArgs e)
        {
            ParamFrom param = new ParamFrom();
            param.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            Application.Exit();
        }
    }
}