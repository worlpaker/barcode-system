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
    public partial class Form2 : Form
    {
        SqlConnection baglanti;
        SqlCommand komut;
        SqlCommand komut2; 
        SqlCommand komut3;
        SqlCommand komut4;
        SqlDataAdapter listesatis;
        public Form2()
        {
            InitializeComponent();
            baglanti = new SqlConnection(@"server=.\SQLEXPRESS; Initial Catalog=BarcodeProg;Integrated Security=SSPI");
            timer1.Start();
        }
        DataTable gunluktable = new DataTable();
        int randomnumber = 0;
        private void Form2_Load(object sender, EventArgs e)
        {
           Satisgetir();
      //   Satilanurunler();
           this.ActiveControl = Barcheck;
           gunluktable.Columns.Add("Satış Numarası", typeof(string));
           gunluktable.Columns.Add("Ürün Çeşidi", typeof(string));
           gunluktable.Columns.Add("Cinsiyet", typeof(string));
           gunluktable.Columns.Add("Etiket Adı", typeof(string));
           gunluktable.Columns.Add("Fiyat(₺)", typeof(string));
           gunluktable.Columns.Add("İndirim(Varsa)", typeof(string));
           gunluktable.Columns.Add("Ödeme Şekli", typeof(string));
           gunluktable.Columns.Add("Barcode No", typeof(string));
           gunluktable.Columns.Add("Kalan Stok Sayisi", typeof(string));
           gunluktable.Columns.Add("Satış Tarihi", typeof(string));


        }
        string[] odemearray = new string[0];
        int arraylength = 0;
        string gunluksatisshow = null;
        string showsumsatis = null;
        decimal sumsatis = 0;
        decimal indsumsatis = 0;
        int stoksatis = 0;
        int number = 0;
        int number2 = 0;
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
            listesatis = new SqlDataAdapter("Select *From BarcodeTables", baglanti);
            DataTable tablesatis = new DataTable();
            listesatis.Fill(tablesatis);
            dataGridView2.DataSource = tablesatis;
            baglanti.Close();
        }
      //  void Satilanurunler()
      //  {
      //      baglanti.Open();
      //      listesatis = new SqlDataAdapter("Select *From SatisTable", baglanti);
      //      DataTable tablesatis2 = new DataTable();
      //      listesatis.Fill(tablesatis2);
      //      dataGridView3.DataSource = tablesatis2;
      //      baglanti.Close();
      //  }
    void gunlukurun()
        {

        arraylength = barcodearray.Length;
        gunluksatisshow = satisarray[number2].ToString();
        gunluksatisshow = gunluksatisshow.Insert(gunluksatisshow.Length - 2, ".");
        gunluktable.Rows.Add(randomnumber, 
            cesitarray[number],
            cinsiyetarray[number], 
            etiketarray[number],
            gunluksatisshow,
            indsumsatis, "",
            barcodearray[number],
            stokarray[number].ToString(),
            timerBox.Text);
        dataGridView1.DataSource = gunluktable;
        }
        private void Barcheck_TextChanged(object sender, EventArgs e)
        {   
            if (Barcheck.Text.Length == 8)
            {
                  Random r1 = new Random();
                 randomnumber = r1.Next(00000000, 99999999);
                (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = string.Format("Barcodeno LIKE '%{0}%'", Barcheck.Text);
                    if(dataGridView2.Rows.Count == 0)
                    {
                        MessageBox.Show("Ürün Kodu Bulunumadı! Lütfen Tekrar Deneyiniz.","Hata!");
                    }
                    else
                    {

                Array.Resize<int>(ref barcodearray, barcodearray.Length + 1);
                Array.Resize<int>(ref stokarray, stokarray.Length + 1);
                Array.Resize<decimal>(ref satisarray, satisarray.Length + 1);
                Array.Resize<string>(ref cesitarray, cesitarray.Length + 1);
                Array.Resize<string>(ref cinsiyetarray, cinsiyetarray.Length + 1);
                Array.Resize<string>(ref etiketarray, etiketarray.Length + 1);
                Array.Resize<string>(ref odemearray, odemearray.Length + 1);
                Array.Resize<int>(ref noarray, noarray.Length + 1);
                stoksatis = Convert.ToInt32(dataGridView2.Rows[0].Cells[6].Value);
                stokarray[number] = Convert.ToInt32(dataGridView2.Rows[0].Cells[6].Value);
                cesitarray[number] = dataGridView2.Rows[0].Cells[1].Value.ToString();
                cinsiyetarray[number] = dataGridView2.Rows[0].Cells[2].Value.ToString();
                etiketarray[number] = dataGridView2.Rows[0].Cells[3].Value.ToString();
                untextBox3.Text = dataGridView2.Rows[0].Cells[4].Value.ToString();
                satisarray[number2] = Convert.ToDecimal(untextBox3.Text);
                untextBox3.Text = satisarray[number2].ToString();
                int check;
                check = Convert.ToInt32(Barcheck.Text); 
                for (int i = 0; i < barcodearray.Length; i++)
                {
                    if (check == barcodearray[i])
                    {
                        number = i;
                   }
               }
                        barcodearray[number] = Convert.ToInt32(Barcheck.Text);
                        sumsatis = satisarray.Sum();
                        showsumsatis = sumsatis.ToString();
                        if (stoksatis == 0)
                        {
                            MessageBox.Show("Ürün Stokta Bulunmamakta!");
                        }
                        else
                          stokarray[number] = stokarray[number] - 1;
                          textBox2.Text = stokarray[number].ToString();
                          gunlukurun(); 
                          number++;
                          number2++;
                          Barcheck.Text = "";
                          this.ActiveControl = Barcheck;
                          Satisgetir();
                          showsumsatis = showsumsatis.Insert(showsumsatis.Length - 2, ".");
                          label9.Text = showsumsatis;
                    }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (odseklibox.SelectedIndex == -1 || label9.Text == "")
            {
                MessageBox.Show("Lütfen Ödeme Şeklini Seçiniz.", "Hata!");
            }
  
            else
            {
                if (string.IsNullOrWhiteSpace(indbox.Text))
                {
                    indbox.Text = "0.00";
                }
                else
                { indsumsatis = Convert.ToDecimal(indbox.Text); }
                sumsatis = sumsatis - indsumsatis;
                showsumsatis = sumsatis.ToString();
                if (string.IsNullOrWhiteSpace(Barcheck.Text))
                {  }
                else
               indbox.Text = indbox.Text.Insert(indbox.Text.Length - 2, ".");
               showsumsatis = showsumsatis.Insert(showsumsatis.Length - 2, ".");
               label9.Text = showsumsatis.ToString();
                //   this.Visible = false;
               if (indbox.Text.Length < 3)
               { MessageBox.Show("İndirim Fiyatı En Az 3 Haneli Olmalıdır. ÖRNEK : 9.90 veya 30.00"); }
               else
               {
                   baglanti.Open();
                   for (int i = 0; i < dataGridView1.Rows.Count; i++)
                   {
                       string StrQuery = "INSERT INTO SatisTable VALUES ('" + dataGridView1.Rows[i].Cells[0].Value + "',' "
                           + dataGridView1.Rows[i].Cells[1].Value + "', '"
                           + dataGridView1.Rows[i].Cells[2].Value + "', '"
                           + dataGridView1.Rows[i].Cells[3].Value + "',' "
                           + dataGridView1.Rows[i].Cells[4].Value + "',' "
                           + dataGridView1.Rows[i].Cells[5].Value + "',' "
                           + dataGridView1.Rows[i].Cells[6].Value + "',' "
                           + dataGridView1.Rows[i].Cells[7].Value + "',' "
                           + dataGridView1.Rows[i].Cells[8].Value + "',' "
                           + dataGridView1.Rows[i].Cells[9].Value + "')";
                       komut = new SqlCommand(StrQuery, baglanti);
                       komut.ExecuteNonQuery();
                       komut.Parameters.Clear();
                       string sorgu = "Update BarcodeTables Set Stok=@Stok Where Barcodeno=@Barcodeno";
                       komut2 = new SqlCommand(sorgu, baglanti);
                       komut2.Parameters.AddWithValue("@Barcodeno", barcodearray[i]);
                       komut2.Parameters.AddWithValue("@Stok", stokarray[i]);
                       komut2.ExecuteNonQuery();
                       komut2.Parameters.Clear();
                       string sorgu3 = "Update SatisTable Set Odeme=@Odeme Where Satisno=@Satisno";
                       komut3 = new SqlCommand(sorgu3, baglanti);
                       komut3.Parameters.AddWithValue("@Satisno", dataGridView1.Rows[i].Cells[0].Value);
                       komut3.Parameters.AddWithValue("@Odeme", odseklibox.SelectedItem.ToString());
                       komut3.ExecuteNonQuery();
                       komut3.Parameters.Clear();
                       string sorgu4 = "Update SatisTable Set Indirim=@Indirim Where Satisno=@Satisno";
                       komut4 = new SqlCommand(sorgu4, baglanti);
                       komut4.Parameters.AddWithValue("@Satisno", dataGridView1.Rows[0].Cells[0].Value);
                       komut4.Parameters.AddWithValue("@Indirim", indbox.Text);
                       komut4.ExecuteNonQuery();
                       komut4.Parameters.Clear();
                   }

                   baglanti.Close();
                   Satisgetir();
                   MessageBox.Show(label9.Text + " TL Satış Gerçekleştirdiniz.");
                   this.Visible = false;
               }
                // Satilanurunler();
            }
        }
        private void indbox_KeyPress(object sender, KeyPressEventArgs e)
        {            
            if (!char.IsControl(e.KeyChar)
         && !char.IsDigit(e.KeyChar)
         && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.'
                && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            timerBox.Text = dateTime.ToString();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.dataGridView1.Columns["Ödeme Şekli"].Visible = false;
            this.dataGridView1.Columns["İndirim(Varsa)"].Visible = false;


        }

        private void Barcheck_KeyPress(object sender, KeyPressEventArgs e)
        {
           
           if (!char.IsControl(e.KeyChar)
         && !char.IsDigit(e.KeyChar))
          {
               e.Handled = true;
            }
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.ActiveControl = Barcheck;

            } 
        }



        }



    }
