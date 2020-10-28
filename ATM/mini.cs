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
    public partial class mini : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='F:\Finger + OTP\ATM\Database1.mdf';Integrated Security=True");
        string Name1 = "", Bal = "", BankAC = "", BankName = "", Pin = "";
        public mini()
        {
            InitializeComponent();
        }

        public mini(string Name1, string Bal, string BankAC, string BankName, string Pin)
        {
            InitializeComponent();
            this.Name1 = Name1;
            this.Bal = Bal;
            this.BankAC = BankAC;
            this.BankName = BankName;
            this.Pin = Pin;
        }

        private void mini_Load(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT Date, Debit, Credit, Bal FROM [Transaction] where Name = '"+Name1+"' AND BankAC = '"+BankAC+"' AND BankName = '"+BankName+"' Order By Date Desc",con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            DataGridViewColumn date = dataGridView1.Columns[0];
            date.Width = 181;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
