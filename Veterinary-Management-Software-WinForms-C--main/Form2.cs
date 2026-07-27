using System;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp8
{
    public partial class Form2 : Form
    {
        // SQLite Bağlantı Dizesi
        private static readonly string dbPath = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";
        private SQLiteConnection baglanti = new SQLiteConnection(dbPath);

        public Form2()
        {
            InitializeComponent();
        }

        // Kayıt Ekleme Butonu
        private void button1_Click(object sender, EventArgs e)
        {
            string telefon = textBox2.Text;
            string cinsi = textBox4.Text;
            string adres = richTextBox1.Text;
            string gebelikDurumu = "Bilinmiyor"; // Gebelik durumu sabit bir değer olabilir ya da bir comboBox üzerinden alınabilir.

            // Veri doğrulama
            if (telefon.Length != 11)
            {
                MessageBox.Show("Telefon numarası 11 karakterden olmalıdır.");
                return;
            }

            if (cinsi.Length > 25)
            {
                MessageBox.Show("Cinsi 25 karakterden fazla olamaz.");
                return;
            }

            if (adres.Length > 255)
            {
                MessageBox.Show("Adres 255 karakterden fazla olamaz.");
                return;
            }

            // SQLite Kayıt ekleme
            AddHayvanKayit(textBox1.Text, telefon, textBox3.Text, cinsi, adres, gebelikDurumu);
        }

        // SQLite Kayıt Ekleme İşlemi
        private void AddHayvanKayit(string hayvanSahibi, string telefon, string kupeNo, string cinsi, string adres, string gebelikDurumu)
        {
            string query = "INSERT INTO HayvanKayit (HayvanSahibi, Telefon, KupeNo, Cinsi, Adres, GebelikDurumu) " +
                           "VALUES (@HayvanSahibi, @Telefon, @KupeNo, @Cinsi, @Adres, @GebelikDurumu)";

            using (SQLiteConnection connection = new SQLiteConnection(dbPath))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@HayvanSahibi", hayvanSahibi);
                command.Parameters.AddWithValue("@Telefon", telefon);
                command.Parameters.AddWithValue("@KupeNo", string.IsNullOrEmpty(kupeNo) ? (object)DBNull.Value : kupeNo);
                command.Parameters.AddWithValue("@Cinsi", cinsi);
                command.Parameters.AddWithValue("@Adres", adres);
                command.Parameters.AddWithValue("@GebelikDurumu", gebelikDurumu);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Kayıt başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // TextBox ve RichTextBox temizleme
                        Temizle();

                        // Form3'ü güncelle
                        Form3 form3 = Application.OpenForms["Form3"] as Form3;
                        if (form3 != null)
                        {
                            form3.VeriYenile(); // Kayıt sonrası veriyi yenile
                        }
                    }
                    else
                    {
                        MessageBox.Show("Kayıt eklenemedi. Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // TextBox ve RichTextBox Temizleme Butonu
        private void button3_Click(object sender, EventArgs e)
        {
            Temizle();
            MessageBox.Show("Temizlendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // TextBoxları temizleyen yardımcı metot
        private void Temizle()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            richTextBox1.Clear();
        }

        // Sadece rakam girilmesine izin ver (Telefon)
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // TextBox MaxLength ayarlamaları
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.MaxLength = 100;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.MaxLength = 11;

            if (!System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, "^[0-9]*$"))
            {
                MessageBox.Show("Sadece rakam girebilirsiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Text = new string(textBox2.Text.Where(char.IsDigit).ToArray());
                textBox2.SelectionStart = textBox2.Text.Length;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.MaxLength = 20;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox4.MaxLength = 25;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.MaxLength = 255;
        }

        // Formlar Arası Geçiş Butonu
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form9 = Application.OpenForms["Form1"] as Form1;

            if (form9 != null)
            {
                form9.Show();
            }
            else
            {
                form9 = new Form1();
                form9.Show();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string telefon = textBox2.Text;
            string cinsi = textBox4.Text;
            string adres = richTextBox1.Text;
            string gebelikDurumu = "Bilinmiyor"; // Gebelik durumu sabit bir değer olabilir ya da bir comboBox üzerinden alınabilir.

            // Veri doğrulama
            if (telefon.Length != 11)
            {
                MessageBox.Show("Telefon numarası 11 karakterden olmalıdır.");
                return;
            }

            if (cinsi.Length > 25)
            {
                MessageBox.Show("Cinsi 25 karakterden fazla olamaz.");
                return;
            }

            if (adres.Length > 255)
            {
                MessageBox.Show("Adres 255 karakterden fazla olamaz.");
                return;
            }

            // SQLite Kayıt ekleme
            AddHayvanKayit(textBox1.Text, telefon, textBox3.Text, cinsi, adres, gebelikDurumu);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form9 = Application.OpenForms["Form1"] as Form1;
            if (form9 != null)
            {
                form9.Show();
            }
            else
            {
                form9 = new Form1();
                form9.Show();
            }
        }
    }
}