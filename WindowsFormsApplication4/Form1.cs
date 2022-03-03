// Barcode System, Market Automation-Sales Program
// Features: 1.Create barcode for product, add to inventory with properties and print barcode.
//           2.Sell product by reading barcode, refund option also available.
//           3.Check every day how much sales you make and also check stock quantities of your products.
//           4.See profits, statistics day by day.
//           Contact: Furkanozalp@gmail.com
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
using System.Drawing.Printing;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        BarcodeLib.Barcode b = new BarcodeLib.Barcode();
        SqlConnection baglanti;
        SqlCommand komut;
        SqlDataAdapter liste;
        SqlDataAdapter listesatis;
        SqlDataAdapter listestok;
        SqlDataAdapter listeiade;
        SqlDataAdapter listegunluk;

        public Form1()
        {
            InitializeComponent();
            baglanti = new SqlConnection(@"server=.\SQLEXPRESS; Initial Catalog=BarcodeProg;Integrated Security=SSPI");
            timer1.Start();

        }

       Int64 number = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            Barcodegetir();
            Satilanurunler();
            Stokgetir();
            iadegetir();
            gunlukurun();
            gunlukkazanc();


        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((string.IsNullOrWhiteSpace(cesitBox.Text)) || comboBox2.SelectedIndex == -1 || (string.IsNullOrWhiteSpace(textBox2.Text)) || (string.IsNullOrWhiteSpace(textBox3.Text)) || (textBox4.Text.Length < 3))
            {
                MessageBox.Show("Lütfen Gerekli Tüm Alanları Doldurunuz!");
            }
            
            else
            {
                Random r1 = new Random();
                number = r1.Next(10000000, 99999999);
                textBox1.Text = number.ToString();
                writeonimage();
                Barcodeolustur();
                {
                    string sorgu = "Insert into BarcodeTables (Cesit,Cinsiyet,Etiket,Fiyat,Barcodeno,Stok,Tarih) values (@Cesit,@Cinsiyet,@Etiket,@Fiyat,@Barcodeno,@Stok,@Tarih)";
                    komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@Cesit", cesitBox.Text);
                    komut.Parameters.AddWithValue("@Cinsiyet", comboBox2.SelectedItem.ToString());
                    komut.Parameters.AddWithValue("@Etiket", textBox3.Text);
                    komut.Parameters.AddWithValue("@Fiyat", textBox4.Text);
                    komut.Parameters.AddWithValue("@Barcodeno", textBox1.Text);
                    komut.Parameters.AddWithValue("@Stok", textBox2.Text);
                    komut.Parameters.AddWithValue("@Tarih", textBox6.Text);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                    komut.Parameters.Clear();
                    //    string sorgusatis = "Insert into SatisTables (Fiyat,Barcodeno) values (@Fiyat,@Barcodeno)";
                    //   komut = new SqlCommand(sorgusatis, baglanti);
                    //   komut.Parameters.AddWithValue("@Fiyat", textBox4.Text);
                    //   komut.Parameters.AddWithValue("@Barcodeno", textBox1.Text);
                    //   komut.ExecuteNonQuery();
                    baglanti.Close();
                    Barcodegetir();
                    Stokgetir();
                    //     Satisgetir();

                }
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
                }

            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void writeonimage()
        {
            var image = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            var font = new Font("TimesNewRoman", 22, FontStyle.Bold, GraphicsUnit.Pixel);
            var font2 = new Font("Arial", 17, FontStyle.Italic, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(image);
            //       graphics.DrawString("Ürün Çeşidi: " + textBox3.Text, font, Brushes.Black, new Point(0, 0));
            graphics.DrawString("Fiyat : " + textBox4.Text + " ₺", font2, Brushes.Black, new Point(120, 15));
            this.pictureBox2.Image = image;

        }
        void Barcodegetir()
        {
            b.Alignment = BarcodeLib.AlignmentPositions.CENTER;
            baglanti.Open();
            liste = new SqlDataAdapter("Select *From BarcodeTables", baglanti);
            DataTable table = new DataTable();
            liste.Fill(table);
            dataGridView1.DataSource = table;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            baglanti.Close();
        }
        void gunlukurun()
        {
            baglanti.Open();
            listegunluk = new SqlDataAdapter("Select *From SatisTable", baglanti);
            DataTable tablegunluk = new DataTable();
            listegunluk.Fill(tablegunluk);
            dataGridView5.DataSource = tablegunluk;
            baglanti.Close();
            gunlukkazanc();

        }
        void Satilanurunler()
        {
            baglanti.Open();
            listesatis = new SqlDataAdapter("Select *From SatisTable", baglanti);
            DataTable tablesatis = new DataTable();
            listesatis.Fill(tablesatis);
            dataGridView3.DataSource = tablesatis;
          dataGridView3.Sort(dataGridView3.Columns[9], ListSortDirection.Ascending);
            baglanti.Close();
            decimal sumsatilanlar = 0;
            decimal sumindirim = 0;
            decimal sumtoplam = 0;
            for (int i = 0; i < dataGridView3.Rows.Count; ++i)
            {
                sumsatilanlar += Convert.ToDecimal(dataGridView3.Rows[i].Cells[4].Value);
                sumindirim += Convert.ToDecimal(dataGridView3.Rows[i].Cells[5].Value); 
                sumtoplam = sumsatilanlar - sumindirim;
            }
            label20.Text = sumsatilanlar.ToString().Insert(sumsatilanlar.ToString().Length - 2, ".") + " - " + sumindirim.ToString().Insert(sumindirim.ToString().Length - 2, ".") + " = " + sumtoplam.ToString().Insert(sumtoplam.ToString().Length - 2, ".") + " TL ";
        }
        void Stokgetir()
        {
            baglanti.Open();
            listestok = new SqlDataAdapter("Select *From BarcodeTables", baglanti);
            DataTable tablestok = new DataTable();
            listestok.Fill(tablestok);
            dataGridView2.DataSource = tablestok;
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
            baglanti.Close();
        }

        void iadegetir()
        {
            baglanti.Open();
            listeiade = new SqlDataAdapter("Select *From RefundTables", baglanti);
            DataTable tableiade = new DataTable();
            listeiade.Fill(tableiade);
            dataGridView4.DataSource = tableiade;
           dataGridView4.Sort(dataGridView4.Columns[7], ListSortDirection.Ascending);
            baglanti.Close();

        }
        void Barcodeolustur()
        {
            BarcodeLib.TYPE type = BarcodeLib.TYPE.EAN8;
            try
            {
                if (type != BarcodeLib.TYPE.UNSPECIFIED)
                {
                    b.IncludeLabel = true;
                    b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                    pictureBox1.BackgroundImage = b.Encode(type, this.textBox1.Text.Trim(), Color.Black, Color.White, pictureBox1.Width, pictureBox1.Height);
                }
                pictureBox1.Location = new Point((this.pictureBox1.Location.X + this.pictureBox1.Width / 2) - pictureBox1.Width / 2, (this.pictureBox1.Location.Y + this.pictureBox1.Height / 2) - pictureBox1.Height / 2);
            }
            catch (Exception)
            { MessageBox.Show("Hata"); }

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif";
            sfd.FilterIndex = 3;
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
                pictureBox2.DrawToBitmap(bmp, new Rectangle(0, 0, pictureBox2.Width, pictureBox2.Height));
                bmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }//if

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seçmiş olduğunuz barkodu silmek istediğinize emin misiniz?", "Remove Row", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sorgu = "Delete From BarcodeTables Where No=@No";
                komut = new SqlCommand(sorgu, baglanti);
                baglanti.Open();
                komut.Parameters.AddWithValue("@No", textBox5.Text);
                komut.ExecuteNonQuery();
                komut.Parameters.Clear();
                //         string sorgu2 = "Delete From SatisTables Where Barcodeno=@Barcodeno";
                //         komut = new SqlCommand(sorgu2, baglanti);
                //         komut.Parameters.AddWithValue("@Barcodeno", textBox1.Text);
                //         komut.ExecuteNonQuery();
                baglanti.Close();
                Barcodegetir();
                Stokgetir();
                clear();
            }

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            textBox5.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            //    comboBox1.SelectedItem = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            //    comboBox2.SelectedItem = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            //    textBox3.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            //        textBox4.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBox1.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            //    textBox2.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            //    textBox6.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            //    writeonimage();
            //     Barcodeolustur();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            textBox6.Text = dateTime.ToString();
            System.DateTime.Now.DayOfWeek.ToString();
            //localized version
            label22.Text = System.DateTime.Now.ToString("dddd");
            label28.Text = dateTime.ToString("dd.MM.yyyy");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += printDocument1_PrintPage;
            pd.Document = doc;
            if (pd.ShowDialog() == DialogResult.OK)
                doc.Print();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            pictureBox2.DrawToBitmap(bmp, new Rectangle(0, 0, pictureBox2.Width, pictureBox2.Height));
            e.Graphics.DrawImage(bmp, 0, 0);
            bmp.Dispose();
        }
        void clear()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            cesitBox.Text = "";
            comboBox2.SelectedIndex = -1;
            pictureBox1.BackgroundImage = null;
            pictureBox2.Image = null;

        }

        private void Satis_Click(object sender, EventArgs e)
        {
            Form Form2 = new Form2();
            Form2.Show();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            Barcodegetir();
            Satilanurunler();
            Stokgetir();
            iadegetir();
            gunlukurun();
            gunlukkazanc();

        }

        private void iadebutton_Click(object sender, EventArgs e)
        {
            Form Form3 = new Form3();
            Form3.Show();
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = string.Format("Barcodeno LIKE '%{0}%'", textBox9.Text);

        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            stoksearch();
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            stoksearch();

        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {  }
            else
            {
                stoksearch();
            }

        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
        private void tabPage2_Click(object sender, EventArgs e)
        {


        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1)
            { }
            else
            {
                saturunlersearch();
                if (dataGridView3.Rows.Count != 0)
                { satilanlarsearch(); }
            }
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            saturunlersearch();
            if (dataGridView3.Rows.Count != 0)
                satilanlarsearch();
        }

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            saturunlersearch();
            if (dataGridView3.Rows.Count != 0)
                satilanlarsearch();
        }

        private void stoksearch()
        {
            if (comboBox1.SelectedIndex == -1)
            {
                ((DataTable)dataGridView2.DataSource).DefaultView.RowFilter =
                    "Etiket like '%" + textBox8.Text + "%' AND Cesit like '%" + textBox7.Text + "%'";
            }
            else
            {
                ((DataTable)dataGridView2.DataSource).DefaultView.RowFilter =
                  "Etiket like '%" + textBox8.Text + "%' AND Cesit like '%" + textBox7.Text + "%' AND Cinsiyet like '%" + comboBox1.SelectedItem.ToString() + "%'";
            }
        }

        private void saturunlersearch()
        {
            if (comboBox3.SelectedIndex == -1)
            {
                ((DataTable)dataGridView3.DataSource).DefaultView.RowFilter =
                    "Etiket like '%" + textBox12.Text + "%' AND Cesit like '%" + textBox11.Text + "%'";
            }
            else
            {
                ((DataTable)dataGridView3.DataSource).DefaultView.RowFilter =
                    "Etiket like '%" + textBox12.Text + "%' AND Cesit like '%" + textBox11.Text + "%' AND Cinsiyet like '%" + comboBox3.SelectedItem.ToString() + "%'";
            }

        }
        private void satilanlarsearch()
        {
            decimal sumsatilanlar = 0;
            decimal sumindirim = 0;
            decimal sumtoplam = 0;
            for (int i = 0; i < dataGridView3.Rows.Count; ++i)
            {
                    sumsatilanlar += Convert.ToDecimal(dataGridView3.Rows[i].Cells[4].Value);
                    sumindirim += Convert.ToDecimal(dataGridView3.Rows[i].Cells[5].Value);
                    sumtoplam = sumsatilanlar - sumindirim;                
            }
            if (sumindirim == 0)
            {
                if (sumtoplam == 0)
                { label20.Text = " 0 TL"; }
                else { label20.Text = sumsatilanlar.ToString().Insert(sumsatilanlar.ToString().Length - 2, ".") + " - 0 " + " = " + sumtoplam.ToString().Insert(sumtoplam.ToString().Length - 2, ".") + " TL "; }
            }
            else
            {
                label20.Text = sumsatilanlar.ToString().Insert(sumsatilanlar.ToString().Length - 2, ".") + " - " + sumindirim.ToString().Insert(sumindirim.ToString().Length - 2, ".") + " = " + sumtoplam.ToString().Insert(sumtoplam.ToString().Length - 2, ".") + " TL ";
            }
        }

        private void gunlukkazanc()
        {
            
            decimal sumnakit = 0;
            decimal indirimnakit = 0;
            decimal sumkredi = 0;
            decimal indirimkredi = 0;
            decimal sumnakittoplam = 0;
            decimal sumkreditoplam = 0;
            decimal sumboth = 0;
            for (int i = 0; i < dataGridView5.Rows.Count; ++i)
            {
                ((DataTable)dataGridView5.DataSource).DefaultView.RowFilter =
                    "Odeme like '%" + label23.Text + "%' AND Tarih like '%" + label28.Text + "%'";
                if (dataGridView5.Rows.Count == 0) { }
                else
                {
                    sumnakit += Convert.ToDecimal(dataGridView5.Rows[i].Cells[4].Value);
                    indirimnakit += Convert.ToDecimal(dataGridView5.Rows[i].Cells[5].Value);
                    sumnakittoplam = sumnakit - indirimnakit;
                }                
            }
            for (int i = 0; i < dataGridView5.Rows.Count; ++i)
            {
                ((DataTable)dataGridView5.DataSource).DefaultView.RowFilter =
                    "Odeme like '%" + label24.Text + "%' AND Tarih like '%" + label28.Text + "%'";
                if (dataGridView5.Rows.Count == 0) { }
                else
                {
                    sumkredi += Convert.ToDecimal(dataGridView5.Rows[i].Cells[4].Value);
                    indirimkredi += Convert.ToDecimal(dataGridView5.Rows[i].Cells[5].Value);
                    sumkreditoplam = sumkredi - indirimkredi;
                }
            }

            if (indirimnakit == 0)
            {
                if (sumnakittoplam == 0)
                { label25.Text = " 0 TL"; }
                else { label25.Text = sumnakittoplam.ToString().Insert(sumnakittoplam.ToString().Length - 2, ".") + " TL ";}
            }
            else
            {
                label25.Text = sumnakit.ToString().Insert(sumnakit.ToString().Length - 2, ".") + " - " + indirimnakit.ToString().Insert(indirimnakit.ToString().Length - 2, ".") + " = " + sumnakittoplam.ToString().Insert(sumnakittoplam.ToString().Length - 2, ".") + " TL ";
            }
// KREDİ GÜNLÜK
            if (indirimkredi == 0)
            {
                if (sumkreditoplam == 0)
                { label26.Text = " 0 TL"; }
                else { label26.Text = sumkreditoplam.ToString().Insert(sumkreditoplam.ToString().Length - 2, ".") + " TL "; }
            }
            else
            {
                label26.Text = sumkredi.ToString().Insert(sumkredi.ToString().Length - 2, ".") + " - " + indirimkredi.ToString().Insert(indirimkredi.ToString().Length - 2, ".") + " = " + sumkreditoplam.ToString().Insert(sumkreditoplam.ToString().Length - 2, ".") + " TL ";
            }
            sumboth = sumnakittoplam+sumkreditoplam;
            if (sumboth == 0)
            { label29.Text = " Toplam Kazanç : 0 TL"; }
            else { label29.Text ="Toplam Kazanç : " + sumboth.ToString().Insert(sumboth.ToString().Length - 2, ".") + " TL "; }

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            string theDate = dateTimePicker1.Value.ToString("dd.MM.yyyy");
            (dataGridView3.DataSource as DataTable).DefaultView.RowFilter = string.Format("Tarih LIKE '%{0}%'", theDate);
            satilanlarsearch();

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            gunlukkazanc();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Barcodegetir();
                Satilanurunler();
                Stokgetir();
                iadegetir();
                gunlukurun();
                gunlukkazanc();
            }
            if (e.KeyCode == Keys.F1)
            {
                Form Form2 = new Form2();
                Form2.Show();
            } 
           if (e.KeyCode == Keys.F3)
            {
                Form Form3 = new Form3();
                Form3.Show();
            } 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Barcodegetir();
            Satilanurunler();
            Stokgetir();
            iadegetir();
            gunlukurun();
            gunlukkazanc();
        }
        

    }



}