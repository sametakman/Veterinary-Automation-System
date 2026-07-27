using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;

namespace WinFormsApp8
{
    public partial class Form5 : Form
    {
        private static readonly string dbPath = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";

        public Form5()
        {
            InitializeComponent();
            checkedListBox1.ItemCheck += CheckedListBox1_ItemCheck;
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox2.KeyPress += TextBox2_KeyPress;
            textBox5.KeyUp += TextBox5_KeyUp;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadDataGrid();

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

            // Sütun genişliklerini ayarla
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["ID"].Width = 50;
                dataGridView1.Columns["AdSoyad"].Width = 150;
                dataGridView1.Columns["TelefonNo"].Width = 100;
                dataGridView1.Columns["YapilanIslem"].Width = 100;
                dataGridView1.Columns["IslemUcreti"].Width = 150;
                dataGridView1.Columns["OdemeSekli"].Width = 100;
                dataGridView1.Columns["KayitTarihi"].Width = 150;
                dataGridView1.Columns["Borç"].Width = 50;
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox2.Text.Length >= 11 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox1.SetItemChecked(i, false);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // İşlev eklenebilir
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // İşlev eklenebilir
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // İşlev eklenebilir
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // İşlev eklenebilir
        }

        private void LoadDataGrid(string filter = "")
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    string query = "SELECT ID, AdSoyad, TelefonNo, YapilanIslem, IslemUcreti, OdemeSekli, KayitTarihi, Borç FROM Islemler";

                    if (!string.IsNullOrEmpty(filter))
                    {
                        query += " WHERE AdSoyad LIKE @Filter OR TelefonNo LIKE @Filter";
                    }

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            cmd.Parameters.AddWithValue("@Filter", "%" + filter + "%");
                        }

                        using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void UncheckAllItems()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        private void TextBox5_KeyUp(object sender, KeyEventArgs e)
        {
            string filter = textBox5.Text.Trim();
            LoadDataGrid(filter);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
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
                MessageBox.Show($"Form geçişi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ClearFields();
            UncheckAllItems();
            MessageBox.Show("Temizlendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Lütfen ödeme şekli seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string telefonNo = textBox2.Text.Trim();
                if (telefonNo.Length != 11)
                {
                    MessageBox.Show("Telefon numarası 11 haneli olmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(textBox4.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal islemUcreti))
                {
                    MessageBox.Show("Geçersiz işlem ücreti. Lütfen geçerli bir sayı girin (Örnek: 100.00).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();

                    // Telefon numarası var mı kontrolü
                    string checkQuery = "SELECT COUNT(*) FROM Islemler WHERE TelefonNo = @TelefonNo";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@TelefonNo", telefonNo);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        string query;
                        if (count > 0) // Kayıt varsa güncelle
                        {
                            query = "UPDATE Islemler SET YapilanIslem = @YapilanIslem, IslemUcreti = @IslemUcreti, OdemeSekli = @OdemeSekli, Borç = Borç + @IslemUcreti WHERE TelefonNo = @TelefonNo";
                        }
                        else // Yoksa yeni kayıt ekle
                        {
                            query = "INSERT INTO Islemler (AdSoyad, TelefonNo, YapilanIslem, IslemUcreti, OdemeSekli, Borç, KayitTarihi) VALUES (@AdSoyad, @TelefonNo, @YapilanIslem, @IslemUcreti, @OdemeSekli, @Borç, CURRENT_TIMESTAMP)";
                        }

                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@AdSoyad", textBox1.Text.Trim());
                            cmd.Parameters.AddWithValue("@TelefonNo", telefonNo);
                            cmd.Parameters.AddWithValue("@YapilanIslem", textBox3.Text.Trim());
                            cmd.Parameters.AddWithValue("@IslemUcreti", islemUcreti);
                            cmd.Parameters.AddWithValue("@OdemeSekli", checkedListBox1.CheckedItems[0].ToString());
                            cmd.Parameters.AddWithValue("@Borç", islemUcreti);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Kayıt başarıyla eklendi veya güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadDataGrid();
                UncheckAllItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Lütfen ödeme şekli seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string telefonNo = textBox2.Text.Trim();
                if (!decimal.TryParse(textBox4.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal odenenTutar))
                {
                    MessageBox.Show("Geçersiz işlem ücreti. Lütfen geçerli bir sayı girin (Örnek: 100.00).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    string updateQuery = @"
                        UPDATE Islemler 
                        SET Borç = CASE 
                            WHEN Borç - @OdenenTutar < 0 THEN 0 
                            ELSE Borç - @OdenenTutar 
                        END, OdemeSekli = @OdemeSekli
                        WHERE TelefonNo = @TelefonNo";

                    using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@OdenenTutar", odenenTutar);
                        updateCmd.Parameters.AddWithValue("@OdemeSekli", checkedListBox1.CheckedItems[0].ToString());
                        updateCmd.Parameters.AddWithValue("@TelefonNo", telefonNo);
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show("Belirtilen telefon numarasına ait kayıt bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Borç kontrolü ve mesaj
                    string checkQuery = "SELECT Borç FROM Islemler WHERE TelefonNo = @TelefonNo";
                    decimal borc = 0;

                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@TelefonNo", telefonNo);
                        object result = checkCmd.ExecuteScalar();
                        borc = result != null ? Convert.ToDecimal(result) : 0;
                    }

                    if (borc == 0)
                    {
                        MessageBox.Show("Borcu kalmamıştır!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Borç güncellendi! Kalan borç: {borc:C2}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    LoadDataGrid();
                    ClearFields();
                    UncheckAllItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Borç güncelleme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}