using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecuGen.FDxSDKPro.Windows;

namespace ATM
{
    public partial class Form1 : Form
    {

        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void registerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reg r = new Reg();
            DisposeAllButThis(r);
            r.MdiParent = this;
            r.Show();
        }

        public void DisposeAll(Form form)
        {
            foreach (Form frm in this.MdiChildren)
            {
                frm.Close();
            }
            return;
        }

        public void DisposeAllButThis(Form form)
        {
            foreach (Form frm in this.MdiChildren)
            {
                if (frm != form)
                {
                    frm.Close();
                }
            }
            return;
        }

        private void fingerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Finger f = new Finger();
            DisposeAllButThis(f);
            f.MdiParent = this;
            f.Show();
        }

        private void oTPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OTP f = new OTP();
            DisposeAllButThis(f);
            f.MdiParent = this;
            f.Show();
        }
    }
}
