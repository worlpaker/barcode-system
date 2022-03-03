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
namespace WindowsFormsApplication4
{
    public partial class Form3 : Form
    {
        SqlConnection baglanti;
        SqlCommand komut;
        SqlCommand komut2;
        SqlCommand komut3;
        SqlDataAdapter liste;
        SqlDataAdapter listesatis;
        public Form3()
        {
            InitializeComponent(); 
            baglanti = new SqlConnection(@"server=.\SQLEXPRESS; Initial Catalog=BarcodeProg;Integrated Security=SSPI");
            timer1.Start();

        }
        DataTable iadetable = new DataTable();

        private void Form3_Load(object sender, EventArgs e)
        { 
            Satisgetir();
            Barcodegetir();
            this.ActiveControl = Barcheck;
            iadetable.Columns.Add("Satış Numarası", typeof(string));
            iadetable.Columns.Add("Ürün Çeşidi", typeof(string));
            iadetable.Columns.Add("Cinsiyet", typeof(string));
            iadetable.Columns.Add("Etiket Adı", typeof(string));
            iadetable.Columns.Add("Fiyat(₺)", typeof(string));
            iadetable.Columns.Add("Barcode No", typeof(string));
            iadetable.Columns.Add("Stok Durumu", typeof(string));
            iadetable.Columns.Add("İade Tarihi", typeof(string));
     //       iadetable.Columns.Add("Gosterme", typeof(string));
     //       iadetable.Columns.Add("Gosterme2", typeof(string));

        }
        string gunluksatisshow = null;
        string showsumsatis = null;
        decimal sumsatis = 0;
        int stoksatis = 0;
        int number = 0;
        int number2 = 0;
        int rowcount = 0;
        int[] noarray = new int[0];
        int[] stokarray = new int[0];
        int[] barcodearray = new int[0];
        string[] cesitarray = new string[0];
        string[] cinsiyetarray = new string[0];
        string[] etiketarray = new string[0];
        decimal[] satisarray = new decimal[0];
        void Satisgetir()
        {
            baglanti.Open();
            listesatis = new SqlDataAdapter("Select *From SatisTable", baglanti);
            DataTable tablesatis = new DataTable();
            listesatis.Fill(tablesatis);
            dataGridView1.DataSource = tablesatis;
            baglanti.Close();
        }
        void Barcodegetir()
        {
            baglanti.Open();
            liste = new SqlDataAdapter("Select *From BarcodeTables", baglanti);
            DataTable tables = new DataTable();
            liste.Fill(tables);
            dataGridView3.DataSource = tables;
            baglanti.Close();
        }

        void gunlukurun()
        {
            gunluksatisshow = satisarray[number2].ToString();
           gunluksatisshow = gunluksatisshow.Insert(gunluksatisshow.Length - 2, ".");
            iadetable.Rows.Add(noarray[number],
               cesitarray[number],
               cinsiyetarray[number],
               etiketarray[number],
               gunluksatisshow,
               barcodearray[number],
               stokarray[number].ToString(),
               timerBox.Text);
           dataGridView2.DataSource = iadetable;
        }
        private void Barcheck_TextChanged(object sender, EventArgs e)
        {
            if (Barcheck.Text.Length == 8)
            {
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Barcodeno LIKE '%{0}%'", Barcheck.Text);
                (dataGridView3.DataSource as DataTable).DefaultView.RowFilter = string.Format("Barcodeno LIKE '%{0}%'", Barcheck.Text);
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Ürün Kodu Bulunumadı! Lütfen Tekrar Deneyiniz.", "Hata!");
                }
                else
                {
                    Array.Resize<int>(ref barcodearray, barcodearray.Length + 1);
                    Array.Resize<int>(ref stokarray, stokarray.Length + 1);
                    Array.Resize<decimal>(ref satisarray, satisarray.Length + 1);
                    Array.Resize<string>(ref cesitarray, cesitarray.Length + 1);
                    Array.Resize<string>(ref cinsiyetarray, cinsiyetarray.Length + 1);
                    Array.Resize<string>(ref etiketarray, etiketarray.Length + 1);
                    Array.Resize<int>(ref noarray, noarray.Length + 1);
                    stoksatis = Convert.ToInt32(dataGridView3.Rows[0].Cells[6].Value);
                    stokarray[number] = Convert.ToInt32(dataGridView3.Rows[0].Cells[6].Value);
                    cesitarray[number] = dataGridView1.Rows[0].Cells[1].Value.ToString();
                    cinsiyetarray[number] = dataGridView1.Rows[0].Cells[2].Value.ToString();
                    etiketarray[number] = dataGridView1.Rows[0].Cells[3].Value.ToString();
                    untextBox3.Text = dataGridView1.Rows[0].Cells[4].Value.ToString();
                    satisarray[number2] = Convert.ToDecimal(untextBox3.Text);
                    int check;
                    check = Convert.ToInt32(Barcheck.Text);
                    for (int i = 0; i < barcodearray.Length; i++)
                    {
                        if (check == barcodearray[i])
                        {
                            number = i;
                            rowcount = rowcount + 1;
                        }
                    }
                    if (rowcount == dataGridView1.Rows.Count)
                    {
                        MessageBox.Show("Ürün Satılanlar Listesinde Bulunamadı!");
                        rowcount = 0;
                    }
                    else
                    {
                        noBox.Text = dataGridView1.Rows[rowcount].Cells[0].Value.ToString();
                        noarray[number] = Convert.ToInt32(noBox.Text);
                        barcodearray[number] = Convert.ToInt32(Barcheck.Text);
                        sumsatis = satisarray.Sum();
                        showsumsatis = sumsatis.ToString();
                        if (stoksatis == 0)
                        {
                            MessageBox.Show("Ürün Satılanlar Listesinde Bulunamadı!");
                        }
                        else
                        stokarray[number] = stokarray[number] + 1;
                        textBox2.Text = stokarray[number].ToString();
                        gunlukurun();
                        number++;
                        number2++;
                        Barcheck.Text = "";
                        this.ActiveControl = Barcheck;
                        Satisgetir();
                        Barcodegetir();
                       showsumsatis = showsumsatis.Insert(showsumsatis.Length - 2, ".");
                        label9.Text = showsumsatis;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {           
                showsumsatis = sumsatis.ToString();
                    //   this.Visible = false;
                    baglanti.Open();
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        string StrQuery = "INSERT INTO RefundTables VALUES ('" + dataGridView2.Rows[i].Cells[0].Value + "',' "
                            + dataGridView2.Rows[i].Cells[1].Value + "', '"
                            + dataGridView2.Rows[i].Cells[2].Value + "', '"
                            + dataGridView2.Rows[i].Cells[3].Value + "',' "
                            + dataGridView2.Rows[i].Cells[4].Value + "',' "
                            + dataGridView2.Rows[i].Cells[5].Value + "',' "
                            + dataGridView2.Rows[i].Cells[6].Value + "',' "
                            + dataGridView2.Rows[i].Cells[7].Value + "')";
                        komut = new SqlCommand(StrQuery, baglanti);
                        komut.ExecuteNonQuery();
                        komut.Parameters.Clear();
                        string sorgu = "Update BarcodeTables Set Stok=@Stok Where Barcodeno=@Barcodeno";
                        komut2 = new SqlCommand(sorgu, baglanti);
                        komut2.Parameters.AddWithValue("@Barcodeno", dataGridView2.Rows[i].Cells[5].Value);
                        komut2.Parameters.AddWithValue("@Stok", dataGridView2.Rows[i].Cells[6].Value);
                        komut2.ExecuteNonQuery();
                        komut2.Parameters.Clear();
                        string sorgu3 = "Delete From SatisTable Where Satisno=@Satisno";
                        komut3 = new SqlCommand(sorgu3, baglanti);
                        komut3.Parameters.AddWithValue("@Satisno", Convert.ToInt32(dataGridView2.Rows[i].Cells[0].Value));
                        komut3.ExecuteNonQuery();
                        komut3.Parameters.Clear();
                    }

                    baglanti.Close();
                    Satisgetir();
                    Barcodegetir();
                    MessageBox.Show(label9.Text + " TL İade Edildi.");
                    this.Visible = false;     

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            timerBox.Text = dateTime.ToString();
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
          //  this.dataGridView2.Columns["Gosterme"].Visible = false;
          //  this.dataGridView2.Columns["Gosterme2"].Visible = false;
        }

        private void Barcheck_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar)
         && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.ActiveControl = Barcheck;

            } 
        }




    }
}
