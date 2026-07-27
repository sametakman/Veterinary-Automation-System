using System;
using System.Data;
using System.Data.SQLite;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;
using System.Text;

namespace WinFormsApp8
{
    public partial class Form6 : Form
    {
        private static readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";
        private DataTable originalDataTable; // DataGridView1 için orijinal verileri saklamak
        private DataTable originalPregnancyDataTable; // DataGridView2 için orijinal verileri saklamak

        public Form6()
        {
            InitializeComponent();
            LoadData();  // DataGridView1 için verileri yükle
            LoadPregnancyData();  // DataGridView2 için verileri yükle
            textBox2.TextChanged += TextBox2_TextChanged; // DataGridView1 için TextChanged olayını ekle
            textBox3.TextChanged += TextBox3_TextChanged; // DataGridView2 için TextChanged olayını ekle
        }

        // Verileri DataGridView1'e yüklemek için method
        private void LoadData()
        {
            using (SQLiteConnection con = new SQLiteConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT KupeNo, Cinsi, Adres, HayvanSahibi, Telefon, HayvanID FROM HayvanKayit LIMIT 1000";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, con);
                    originalDataTable = new DataTable();
                    da.Fill(originalDataTable);

                    // Verileri DataGridView1'e yükle
                    dataGridView1.DataSource = originalDataTable;

                    // DataGridView1 özelleştirmeleri
                    CustomizeDataGridView1();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        // DataGridView1 düzenlemeleri
        private void CustomizeDataGridView1()
        {
            dataGridView1.Columns["KupeNo"].Width = 100;
            dataGridView1.Columns["Cinsi"].Width = 150;
            dataGridView1.Columns["Adres"].Width = 200;
            dataGridView1.Columns["HayvanSahibi"].Width = 150;
            dataGridView1.Columns["Telefon"].Width = 120;
            dataGridView1.Columns["HayvanID"].Width = 80;

            dataGridView1.Columns["KupeNo"].HeaderText = "Kupe No";
            dataGridView1.Columns["Cinsi"].HeaderText = "Cinsi";
            dataGridView1.Columns["Adres"].HeaderText = "Adres";
            dataGridView1.Columns["HayvanSahibi"].HeaderText = "Hayvan Sahibi";
            dataGridView1.Columns["Telefon"].HeaderText = "Telefon";
            dataGridView1.Columns["HayvanID"].HeaderText = "Hayvan ID";

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        // Verileri DataGridView2'ye yüklemek için method
        private void LoadPregnancyData()
        {
            using (SQLiteConnection con = new SQLiteConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Join ile Cinsi sütununu HayvanKayit tablosundan alıyoruz
                    string query = @"
                        SELECT g.ID, g.KupeNo, h.Cinsi, g.GebelikDurumu, g.KayitTarihi, g.DogumTarihi, g.DogumaKalanSure 
                        FROM Gebelik g
                        LEFT JOIN HayvanKayit h ON g.KupeNo = h.KupeNo 
                        LIMIT 1000";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(query, con);
                    originalPregnancyDataTable = new DataTable();
                    da.Fill(originalPregnancyDataTable);

                    // Verileri DataGridView2'ye yükle
                    dataGridView2.DataSource = originalPregnancyDataTable;

                    // DataGridView2 özelleştirmeleri
                    CustomizeDataGridView2();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        // DataGridView2 düzenlemeleri
        private void CustomizeDataGridView2()
        {
            dataGridView2.Columns["ID"].Width = 80;
            dataGridView2.Columns["KupeNo"].Width = 100;
            dataGridView2.Columns["Cinsi"].Width = 150;
            dataGridView2.Columns["GebelikDurumu"].Width = 150;
            dataGridView2.Columns["KayitTarihi"].Width = 120;
            dataGridView2.Columns["DogumTarihi"].Width = 120;
            dataGridView2.Columns["DogumaKalanSure"].Width = 120;

            dataGridView2.Columns["ID"].HeaderText = "ID";
            dataGridView2.Columns["KupeNo"].HeaderText = "Kupe No";
            dataGridView2.Columns["Cinsi"].HeaderText = "Cinsi";
            dataGridView2.Columns["GebelikDurumu"].HeaderText = "Gebelik Durumu";
            dataGridView2.Columns["KayitTarihi"].HeaderText = "Kayıt Tarihi";
            dataGridView2.Columns["DogumTarihi"].HeaderText = "Doğum Tarihi";
            dataGridView2.Columns["DogumaKalanSure"].HeaderText = "Doğuma Kalan Süre";

            dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        // TextBox2 ile filtreleme işlemi (DataGridView1) - Sadece Telefon sütununa göre
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (originalDataTable != null)
            {
                string filterText = textBox2.Text.Trim();
                DataTable filteredTable = originalDataTable.Clone();

                foreach (DataRow row in originalDataTable.Rows)
                {
                    if (row["Telefon"].ToString().Contains(filterText))
                    {
                        filteredTable.ImportRow(row);
                    }
                }
                dataGridView1.DataSource = filteredTable;
            }
        }

        // TextBox3 ile filtreleme işlemi (DataGridView2) - Sadece KupeNo sütununa göre
        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (originalPregnancyDataTable != null)
            {
                string filterText = textBox3.Text.Trim();
                DataTable filteredTable = originalPregnancyDataTable.Clone();

                foreach (DataRow row in originalPregnancyDataTable.Rows)
                {
                    if (row["KupeNo"].ToString().Contains(filterText))
                    {
                        filteredTable.ImportRow(row);
                    }
                }
                dataGridView2.DataSource = filteredTable;
            }
        }

        // Ana sayfaya dön butonu
        private void pictureBox1_Click(object sender, EventArgs e)
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

        // Email gönderme işlemi
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Gönderici e-posta ve şifresini mail tablosundan al
                string gondericiEmail = "";
                string uygulamaSifresi = "";

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT email, password FROM mail LIMIT 1";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            gondericiEmail = reader["email"].ToString();
                            uygulamaSifresi = reader["password"].ToString();
                        }
                    }
                }

                if (string.IsNullOrEmpty(gondericiEmail) || string.IsNullOrEmpty(uygulamaSifresi))
                {
                    MessageBox.Show("Gönderici e-posta veya şifre veritabanından çekilemedi.");
                    return;
                }

                // 2. Alıcı e-posta adresini textBox1'den al
                string aliciEmail = textBox1.Text.Trim();
                if (string.IsNullOrEmpty(aliciEmail))
                {
                    MessageBox.Show("Lütfen alıcı e-posta adresini girin (textBox1).");
                    return;
                }

                // 3. Hangi RadioButton seçiliyse o tabloyu al
                string selectedQuery = "";
                string tableName = "";

                if (radioButton1.Checked)
                {
                    selectedQuery = "SELECT HayvanID, HayvanSahibi, Telefon, KupeNo, Cinsi, Adres, GebelikDurumu FROM HayvanKayit LIMIT 1000";
                    tableName = "Hayvan Kayıt";
                }
                else if (radioButton2.Checked)
                {
                    selectedQuery = "SELECT id, urun_adi, urun_no, adet, alis_fiyati, satis_fiyati, kazanc FROM stok LIMIT 1000";
                    tableName = "Stok";
                }
                else if (radioButton3.Checked)
                {
                    selectedQuery = "SELECT ID, AdSoyad, TelefonNo, YapilanIslem, IslemUcreti, OdemeSekli, KayitTarihi, Borç FROM Islemler LIMIT 1000";
                    tableName = "İşlemler";
                }
                else if (radioButton4.Checked)
                {
                    // Gebelik tablosu için join ile Cinsi sütununu alıyoruz
                    selectedQuery = @"
                        SELECT g.ID, g.KupeNo, h.Cinsi, g.GebelikDurumu, g.KayitTarihi, g.DogumTarihi, g.DogumaKalanSure 
                        FROM Gebelik g
                        LEFT JOIN HayvanKayit h ON g.KupeNo = h.KupeNo 
                        LIMIT 1000";
                    tableName = "Gebelik";
                }
                else if (radioButton5.Checked)
                {
                    selectedQuery = "SELECT AşıAdı, Periyot, HayvanID, HayvanSahibi, Telefon, KupeNo, Cinsi, Adres, Durum, DigerDoz, KayitTarihi FROM AsiBilgisi LIMIT 1000";
                    tableName = "Aşı Bilgisi";
                }
                else
                {
                    MessageBox.Show("Lütfen bir tablo seçin (RadioButton).");
                    return;
                }

                // 4. Veriyi al ve e-posta gövdesine ekle
                StringBuilder emailBody = new StringBuilder();
                emailBody.AppendLine("Merhaba,");
                emailBody.AppendLine();
                emailBody.AppendLine("Bu e-posta, veteriner otomasyon sisteminden seçilen tabloya ait verileri içermektedir.");
                emailBody.AppendLine($"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                emailBody.AppendLine($"Seçilen Tablo: {tableName}");
                emailBody.AppendLine();
                emailBody.AppendLine("Bu rapor, veteriner otomasyon sisteminden seçilen tablodaki verileri içermektedir. Aşağıda, seçtiğiniz tablodaki veriler detaylı bir şekilde listelenmiştir. Bu veriler, sistemdeki güncel kayıtları yansıtmaktadır ve raporun oluşturulduğu tarih ve saat yukarıda belirtilmiştir.");
                emailBody.AppendLine();

                // Veriyi al
                DataTable tableData = new DataTable();
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    SQLiteDataAdapter da = new SQLiteDataAdapter(selectedQuery, conn);
                    da.Fill(tableData);
                }

                // Veri kontrolü
                if (tableData.Columns.Count == 0 || tableData.Rows.Count == 0)
                {
                    emailBody.AppendLine("Tabloda veri bulunamadı.");
                }
                else
                {
                    // Sütun başlıklarını ekle
                    emailBody.AppendLine("Tablo Verileri:");
                    emailBody.AppendLine(new string('-', 50));
                    StringBuilder headerLine = new StringBuilder();
                    foreach (DataColumn column in tableData.Columns)
                    {
                        headerLine.Append($"{column.ColumnName,-20} | ");
                    }
                    emailBody.AppendLine(headerLine.ToString().TrimEnd(' ', '|'));
                    emailBody.AppendLine(new string('-', 50));

                    // Satırları ekle
                    foreach (DataRow row in tableData.Rows)
                    {
                        StringBuilder rowLine = new StringBuilder();
                        foreach (var item in row.ItemArray)
                        {
                            string cellValue = item?.ToString() ?? "NULL";
                            // Özel karakterleri temizle
                            cellValue = cellValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Trim();
                            rowLine.Append($"{cellValue,-20} | ");
                        }
                        emailBody.AppendLine(rowLine.ToString().TrimEnd(' ', '|'));
                    }
                    emailBody.AppendLine(new string('-', 50));
                }

                emailBody.AppendLine();
                emailBody.AppendLine("İyi günler,");
                emailBody.AppendLine("Veteriner Otomasyon Ekibi");

                // 5. E-posta gönder
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential(gondericiEmail, uygulamaSifresi);
                    smtpClient.EnableSsl = true;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(gondericiEmail);
                        mailMessage.To.Add(aliciEmail);
                        mailMessage.Subject = $"Veteriner Otomasyon Raporu - {tableName} - {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
                        mailMessage.Body = emailBody.ToString();
                        mailMessage.IsBodyHtml = false;

                        smtpClient.Send(mailMessage);
                    }
                }

                MessageBox.Show("E-posta başarıyla gönderildi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("E-posta gönderilirken hata oluştu: " + ex.Message + "\nStackTrace: " + ex.StackTrace);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form11 form11 = Application.OpenForms["Form11"] as Form11;
            if (form11 != null)
            {
                form11.Show();
            }
            else
            {
                form11 = new Form11();
                form11.Show();
            }
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // button4 işlevselliği buraya eklenebilir
        }

        private void pictureBox1_Click_1(object sender, EventArgs e) // Temizle butonu
        {
            // TextBox'ları temizle
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();

            // RadioButton'ları sıfırla
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            // Başlık yazı tipi ve rengi
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Kenarlıklar
            dataGridView1.EnableHeadersVisualStyles = false;

            // Satır veri yazı tipi ve stilleri
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Regular);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Başlık yazı tipi ve rengi
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Kenarlıklar
            dataGridView2.EnableHeadersVisualStyles = false;

            // Satır veri yazı tipi ve stilleri
            dataGridView2.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Regular);
            dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView2.DefaultCellStyle.BackColor = Color.White;
            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            // Boş bırakıldı
        }
    }
}