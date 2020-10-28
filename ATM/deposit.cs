using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ATM
{
    public partial class deposit : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='F:\Finger + OTP\ATM\Database1.mdf';Integrated Security=True");
        string Name1 = "", Bal = "", BankAC = "", BankName = "", Pin = "";

        public deposit(string Name1, string Bal, string BankAC, string BankName, string Pin)
        {
            InitializeComponent();
            this.Name1 = Name1;
            this.Bal = Bal;
            this.BankAC = BankAC;
            this.BankName = BankName;
            this.Pin = Pin;
        }

        public deposit()
        {
            InitializeComponent();
        }

        private void deposit_Load(object sender, EventArgs e)
        {
            uname.Text = Name1;
            bal.Text = Bal;
            bankac.Text = BankAC;
            bankname.Text = BankName;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keypress = e.KeyChar;
            if (!(char.IsDigit(keypress) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                MessageBox.Show("Enter Numbers Only !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int amt = Convert.ToInt32(textBox1.Text);
            int b = Convert.ToInt32(bal.Text);

            b = b + amt;
            SqlCommand cmd = new SqlCommand("update Reg set bal = '" + b + "' where Name = '" + Name1 + "' AND BankAC = '" + BankAC + "' AND BankName = '" + BankName + "'", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            cmd = new SqlCommand("Insert Into [Transaction] Values ('" + Name1 + "','" + BankAC + "','" + BankName + "','0','"+amt+"','" + b + "','" + date + "')", con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            DialogResult d = MessageBox.Show("Transaction Successfull, Balance is : " + b, "Transaction Successfull !!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (d == DialogResult.OK)
            {
                this.Close();
            }
        }
    }
}
