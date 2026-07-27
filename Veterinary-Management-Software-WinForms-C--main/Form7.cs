using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp8
{
    public partial class Form7 : Form
    {
        private static readonly string dbPath = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";

        public Form7()
        {
            InitializeComponent();
            LoadData(); // Form açıldığında verileri yükle
            dataGridView1.KeyDown += DataGridView1_KeyDown; // Delete tuşu için olay ekle
        }

        private void Form7_Load(object sender, EventArgs e)
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

        private void LoadData()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    string query = "SELECT id, email, password FROM mail";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // DataGridView'e veriyi bağla ve sütun ayarlarını yap
                        dataGridView1.DataSource = dt;
                        if (dataGridView1.Columns.Count > 0)
                        {
                            dataGridView1.Columns["id"].ReadOnly = true; // ID sütununu salt okunur yap
                            dataGridView1.Columns["id"].Width = 100;
                            dataGridView1.Columns["email"].Width = 250;
                            dataGridView1.Columns["password"].Width = 150;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                string email = textBox1.Text.Trim();
                string password = textBox2.Text.Trim();

                // E-posta format kontrolü
                if (!email.Contains("@"))
                {
                    MessageBox.Show("Geçerli bir e-posta adresi girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Boş alan kontrolü
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("E-posta ve şifre alanları boş bırakılamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    string query = "INSERT INTO mail (email, password) VALUES (@email, @password)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Kayıt başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        catch (SQLiteException ex) when (ex.ErrorCode == 19) // SQLITE_CONSTRAINT (UNIQUE ihlali)
                        {
                            MessageBox.Show("Bu e-posta adresi zaten kayıtlı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                textBox1.Clear();
                textBox2.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt eklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    DialogResult result = MessageBox.Show("Seçili kayıtları silmek istediğinize emin misiniz?", "Silme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        using (SQLiteConnection con = new SQLiteConnection(dbPath))
                        {
                            con.Open();
                            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                            {
                                if (!row.IsNewRow && row.Cells["id"].Value != null)
                                {
                                    int id = Convert.ToInt32(row.Cells["id"].Value);
                                    string deleteQuery = "DELETE FROM mail WHERE id = @id";
                                    using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                                    {
                                        cmd.Parameters.AddWithValue("@id", id);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        MessageBox.Show("Seçili kayıtlar başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Verileri yenile
                    }
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            try
            {
                // DataGridView'deki düzenlemeleri tamamla
                dataGridView1.EndEdit();

                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    int updatedRows = 0;
                    int deletedRows = 0;

                    // Mevcut veritabanı kayıtlarını al
                    string selectQuery = "SELECT id FROM mail";
                    DataTable originalTable = new DataTable();
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(selectQuery, con))
                    {
                        da.Fill(originalTable);
                    }

                    // DataGridView'deki her satırı kontrol et ve güncelle
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow || row.Cells["id"].Value == null) continue;

                        int id = Convert.ToInt32(row.Cells["id"].Value);
                        string email = row.Cells["email"].Value?.ToString()?.Trim();
                        string password = row.Cells["password"].Value?.ToString()?.Trim();

                        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                        {
                            string deleteQuery = "DELETE FROM mail WHERE id = @id";
                            using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@id", id);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0) deletedRows++;
                            }
                        }
                        else
                        {
                            string updateQuery = "UPDATE mail SET email = @email, password = @password WHERE id = @id";
                            using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@password", password);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0) updatedRows++;
                            }
                        }
                    }

                    // Veritabanında olup DataGridView'de olmayan satırları sil
                    DataTable currentTable = (DataTable)dataGridView1.DataSource;
                    foreach (DataRow originalRow in originalTable.Rows)
                    {
                        int originalId = Convert.ToInt32(originalRow["id"]);
                        bool exists = false;

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.IsNewRow || row.Cells["id"].Value == null) continue;
                            if (Convert.ToInt32(row.Cells["id"].Value) == originalId)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            string deleteQuery = "DELETE FROM mail WHERE id = @id";
                            using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@id", originalId);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0) deletedRows++;
                            }
                        }
                    }

                    LoadData();

                    if (updatedRows == 0 && deletedRows == 0)
                    {
                        MessageBox.Show("Herhangi bir değişiklik yapılmadı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Değişiklikler başarıyla kaydedildi!\nGüncellenen: {updatedRows}, Silinen: {deletedRows}",
                            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Değişiklikler kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form1 form1 = Application.OpenForms["Form1"] as Form1 ?? new Form1();
                form1.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form geçişi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            // İşlev eklenebilir
        }
    }
}