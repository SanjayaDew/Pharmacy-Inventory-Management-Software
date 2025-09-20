using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace pharmacy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=Pharmacy;Integrated Security=True;TrustServerCertificate=True");
        SqlCommand cmd;
        SqlDataAdapter dt;
        SqlDataReader read;

        private void txtdcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                cmd = new SqlCommand("Select * from product where id = '" + txtdcode.Text + "'", con);
                con.Open();
                read = cmd.ExecuteReader();

                if (read.Read())
                {
                    String pname;
                    String price;

                    pname = read["prodname"].ToString();
                    price = read["price"].ToString();

                    txtdname.Text = pname;
                    txtprice.Text = price;
                }
                else
                {
                    MessageBox.Show("No barcode found");
                }
                con.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String dcode = txtdcode.Text;
            String dname = txtdname.Text;
            double price = Convert.ToDouble(txtprice.Text);
            double qty = Convert.ToDouble(txtqty.Text);

            double tot = price * qty;

            this.dataGridView1.Rows.Add(dcode, dname, price, qty, tot);

            int sum = 0;

            for (int row = 0; row < dataGridView1.Rows.Count; row++)
            {
                sum = sum + Convert.ToInt32(dataGridView1.Rows[row].Cells[4].Value);                
            }

            txttotal.Text = sum.ToString();

            txtdcode.Clear();
            txtdname.Clear();
            txtprice.Clear();
            txtqty.Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["delete"].Index && e.RowIndex >= 0)
            {
                dataGridView1.Rows.Remove(dataGridView1.Rows[e.RowIndex]);

                int sum = 0;

                for (int row = 0; row < dataGridView1.Rows.Count; row++)
                {
                    sum = sum + Convert.ToInt32(dataGridView1.Rows[row].Cells[4].Value);
                }

                txttotal.Text = sum.ToString();
            }
        }
    }
}
