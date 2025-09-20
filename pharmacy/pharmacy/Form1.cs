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
        SqlCommand cmd1;
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

        public void SalesSave()
        {
            String total = txttotal.Text;
            String pay = txtpay.Text;
            String bal = txtbal.Text;

            String sql1;
            String sql2;

            sql1 = "Insert into sales(subtotal,pay,balance) values ( @subtotal, @pay, @balance) select @@identity;";

            con.Open();
            cmd = new SqlCommand(sql1, con);
            cmd.Parameters.AddWithValue("@subtotal", total);
            cmd.Parameters.AddWithValue("@pay", pay);
            cmd.Parameters.AddWithValue("@balance", bal);
            int lastid = int.Parse(cmd.ExecuteScalar().ToString());

            string dname;
            int price = 0 ;
            int qty = 0 ;
            int tot = 0;

            for (int row = 0; row < dataGridView1.Rows.Count; row++)
            {
                dname = dataGridView1.Rows[row].Cells[1].Value.ToString();
                price = int.Parse(dataGridView1.Rows[row].Cells[2].Value.ToString());
                qty = int.Parse(dataGridView1.Rows[row].Cells[3].Value.ToString());
                tot = int.Parse(dataGridView1.Rows[row].Cells[4].Value.ToString());

                sql2 = "insert into sales_product (sales_id , drugname, price, qty, total)values (@sales_id, @drugname, @price, @qty, @total)";
                cmd1 = new SqlCommand(sql2, con);
                cmd1.Parameters.AddWithValue("@sales_id", lastid);
                cmd1.Parameters.AddWithValue("@drugname", dname);
                cmd1.Parameters.AddWithValue("@price", price);
                cmd1.Parameters.AddWithValue("@qty", qty);
                cmd1.Parameters.AddWithValue("@total", tot);
                cmd1.ExecuteNonQuery();
            }
            MessageBox.Show("Sales Completed !");
            con.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            double total = double.Parse(txttotal.Text);
            double pay = double.Parse(txtpay.Text);
            double bal = pay - total;
            txtbal.Text = bal.ToString();

            SalesSave();
        }
    }
}
