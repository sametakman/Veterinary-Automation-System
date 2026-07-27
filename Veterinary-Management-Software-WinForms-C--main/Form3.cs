using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp8
{
    public partial class Form3 : Form
    {
        private DataTable tablo = new DataTable();
        private static readonly string dbPath = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";

        public Form3()
        {
            InitializeComponent();
            VeriYenile();
        }

        // Veritabanından verileri çekip DataGridView1'e yükler
        public void VeriYenile()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(dbPath))
                {
                    connection.Open();
                    string query = "SELECT HayvanID, HayvanSahibi, Telefon, KupeNo, Cinsi, Adres, GebelikDurumu FROM HayvanKayit";
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                    {
                        tablo.Clear(); // Önceki verileri temizler
                        adapter.Fill(tablo); // Yeni verilerle tabloyu doldurur
                        dataGridView1.DataSource = tablo; // DataGridView1'e tabloyu bağlar
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yenileme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DataGridView1'de satır silme işlemi (Delete tuşu ile)
        private void DataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                if (e.Row.DataBoundItem == null) return;

                DialogResult result = MessageBox.Show("Bu satırı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Silme işlemini iptal eder
                }
                else
                {
                    using (SQLiteConnection connection = new SQLiteConnection(dbPath))
                    {
                        connection.Open();
                        DataRow row = ((DataRowView)e.Row.DataBoundItem).Row;
                        string query = "DELETE FROM HayvanKayit WHERE HayvanID = @HayvanID";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@HayvanID", row["HayvanID"]);
                            command.ExecuteNonQuery(); // Veritabanından satırı siler
                        }
                    }
                    // Yerel tabloda da satırı sil
                    DataRowView drv = e.Row.DataBoundItem as DataRowView;
                    drv.Row.Delete();
                    tablo.AcceptChanges(); // Silme işlemini yerel tabloda onaylar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Silme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PictureBox1 tıklandığında güncelleme işlemini gerçekleştirir ve tabloyu kaydeder
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(dbPath))
                {
                    connection.Open();
                    DataTable changes = tablo.GetChanges(); // Değişiklikleri kontrol eder
                    if (changes == null)
                    {
                        MessageBox.Show("Herhangi bir değişiklik yapılmadı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Güncelleme işlemi
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT HayvanID, HayvanSahibi, Telefon, KupeNo, Cinsi, Adres, GebelikDurumu FROM HayvanKayit", connection))
                    {
                        SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
                        adapter.UpdateCommand = builder.GetUpdateCommand(); // Otomatik UPDATE komutu oluşturur
                        adapter.Update(tablo); // Değişiklikleri veritabanına kaydeder
                    }

                    tablo.AcceptChanges(); // Yerel tabloyu günceller
                }

                MessageBox.Show("Değişiklikler başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataGridView1.DataSource = tablo; // Güncel tabloyu DataGridView1'e bağlar
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Güncelleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Uygulamayı kapatır
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Birinci forma geçiş yapar
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form1 form1 = Application.OpenForms["Form1"] as Form1 ?? new Form1();
                form1.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form geçiş hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
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
        }
    }
}