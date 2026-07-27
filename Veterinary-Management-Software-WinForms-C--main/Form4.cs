using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp8
{
    public partial class Form4 : Form
    {
        private static readonly string dbPath = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";
        private DataTable originalDataTable; // Orijinal veriyi saklamak için

        public Form4()
        {
            InitializeComponent();
            StokListele();
            this.dataGridView1.KeyDown += new KeyEventHandler(this.dataGridView1_KeyDown);
            this.dataGridView1.CellEndEdit += new DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.textBox6.TextChanged += new EventHandler(this.textBox6_TextChanged);
        }

        // Stokları DataGridView'de göster
        private void StokListele()
        {
            try
            {
                if (dataGridView1.IsCurrentCellInEditMode)
                {
                    dataGridView1.EndEdit();
                }

                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;

                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    string query = "SELECT id, urun_adi, urun_no, adet, alis_fiyati, satis_fiyati, kazanc FROM stok";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, con))
                    {
                        originalDataTable = new DataTable();
                        da.Fill(originalDataTable);

                        dataGridView1.DataSource = originalDataTable;
                        dataGridView1.Columns["kazanc"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["alis_fiyati"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["satis_fiyati"].DefaultCellStyle.Format = "C2";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stoklar listelenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TextBox6'daki metne göre filtreleme (urun_adi veya urun_no)
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        // Filtreleme işlemini gerçekleştiren metot (sadece textBox6 için)
        private void Filtrele()
        {
            try
            {
                if (dataGridView1.IsCurrentCellInEditMode)
                {
                    dataGridView1.EndEdit();
                }

                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;

                string filterText6 = textBox6.Text.Trim();

                DataView dv = originalDataTable.DefaultView;
                string filterExpression = "";

                if (!string.IsNullOrEmpty(filterText6))
                {
                    // Case-insensitive search using LIKE with wildcards
                    filterExpression = $"urun_adi LIKE '%{filterText6}%' OR urun_no LIKE '%{filterText6}%'";
                }

                if (string.IsNullOrEmpty(filterExpression))
                {
                    dv.RowFilter = string.Empty;
                    dataGridView1.DataSource = originalDataTable;
                }
                else
                {
                    dv.RowFilter = filterExpression;
                    dataGridView1.DataSource = dv;
                }

                if (dv.Count == 0)
                {
                    MessageBox.Show("Arama kriterlerine uygun veri bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filtreleme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Textbox'ları temizle
        private void TextboxlariTemizle()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }

        // Delete tuşuyla seçili satırı silme
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                try
                {
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        int seciliSatirId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                        DialogResult result = MessageBox.Show("Bu ürünü silmek istediğinize emin misiniz?", "Silme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            using (SQLiteConnection con = new SQLiteConnection(dbPath))
                            {
                                con.Open();
                                string deleteQuery = "DELETE FROM stok WHERE id = @id";
                                using (SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, con))
                                {
                                    deleteCmd.Parameters.AddWithValue("@id", seciliSatirId);
                                    deleteCmd.ExecuteNonQuery();
                                }
                            }
                            MessageBox.Show("Ürün başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            StokListele();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lütfen silmek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Hücre düzenleme tamamlandığında veritabanını güncelle
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowIndex = e.RowIndex;
                int columnIndex = e.ColumnIndex;

                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["id"].Value);
                string columnName = dataGridView1.Columns[columnIndex].Name;
                object newValue = dataGridView1.Rows[rowIndex].Cells[columnIndex].Value;

                if (newValue == null || string.IsNullOrWhiteSpace(newValue.ToString()))
                {
                    MessageBox.Show("Lütfen geçerli bir değer girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    StokListele();
                    return;
                }

                if (columnName == "adet" && !int.TryParse(newValue.ToString(), out _))
                {
                    MessageBox.Show("Adet için geçerli bir sayı girin (Örnek: 10).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    StokListele();
                    return;
                }
                else if ((columnName == "alis_fiyati" || columnName == "satis_fiyati" || columnName == "kazanc") && !decimal.TryParse(newValue.ToString(), out _))
                {
                    MessageBox.Show("Fiyat veya kazanç için geçerli bir sayı girin (Örnek: 50.00).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    StokListele();
                    return;
                }

                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    string updateQuery = $"UPDATE stok SET {columnName} = @newValue WHERE id = @id";
                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@newValue", newValue);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Değişiklik başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                StokListele();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Değişiklik kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StokListele();
            }
        }

        // PictureBox1: Ana forma dönme
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

        // PictureBox3: Yeni stok ekleme veya mevcut stoğu güncelleme
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox3.Text) ||
                    string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    MessageBox.Show("Lütfen ürün adı, miktar, alış fiyatı ve satış fiyatı alanlarını doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(textBox3.Text, out int miktar) ||
                    !decimal.TryParse(textBox4.Text, out decimal alisFiyati) ||
                    !decimal.TryParse(textBox5.Text, out decimal satisFiyati))
                {
                    MessageBox.Show("Lütfen geçerli bir miktar ve fiyat girin (Örnek: Miktar: 10, Fiyat: 50.00).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    string checkQuery = "SELECT COUNT(*) FROM stok WHERE TRIM(LOWER(urun_no)) = TRIM(LOWER(@urunNo)) OR (urun_no IS NULL AND @urunNo IS NULL) AND TRIM(LOWER(urun_adi)) = TRIM(LOWER(@urunAdi))";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@urunNo", string.IsNullOrWhiteSpace(textBox2.Text) ? (object)DBNull.Value : textBox2.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@urunAdi", textBox1.Text.Trim());
                        int urunVarMi = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (urunVarMi == 0)
                        {
                            string insertQuery = "INSERT INTO stok (urun_adi, urun_no, adet, alis_fiyati, satis_fiyati, kazanc) " +
                                                 "VALUES (@urunAdi, @urunNo, @miktar, @alisFiyati, @satisFiyati, 0)";
                            using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, con))
                            {
                                insertCmd.Parameters.AddWithValue("@urunAdi", textBox1.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@urunNo", string.IsNullOrWhiteSpace(textBox2.Text) ? (object)DBNull.Value : textBox2.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@miktar", miktar);
                                insertCmd.Parameters.AddWithValue("@alisFiyati", alisFiyati);
                                insertCmd.Parameters.AddWithValue("@satisFiyati", satisFiyati);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string updateQuery = "UPDATE stok SET adet = adet + @miktar, alis_fiyati = @alisFiyati, satis_fiyati = @satisFiyati WHERE TRIM(LOWER(urun_no)) = TRIM(LOWER(@urunNo)) OR (urun_no IS NULL AND @urunNo IS NULL) AND TRIM(LOWER(urun_adi)) = TRIM(LOWER(@urunAdi))";
                            using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, con))
                            {
                                updateCmd.Parameters.AddWithValue("@urunNo", string.IsNullOrWhiteSpace(textBox2.Text) ? (object)DBNull.Value : textBox2.Text.Trim());
                                updateCmd.Parameters.AddWithValue("@urunAdi", textBox1.Text.Trim());
                                updateCmd.Parameters.AddWithValue("@miktar", miktar);
                                updateCmd.Parameters.AddWithValue("@alisFiyati", alisFiyati);
                                updateCmd.Parameters.AddWithValue("@satisFiyati", satisFiyati);
                                updateCmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Stok başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        StokListele();
                        TextboxlariTemizle();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stok güncellenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PictureBox2: Ürün satışı ve kazanç hesaplama
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) && string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Lütfen ürün adı veya ürün numarasını girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Lütfen satış miktarı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(textBox3.Text, out int satilacakMiktar) || satilacakMiktar <= 0)
                {
                    MessageBox.Show("Lütfen geçerli bir satış miktarı girin (Örnek: 10).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    string checkQuery = "SELECT adet, alis_fiyati, satis_fiyati, kazanc FROM stok WHERE TRIM(LOWER(urun_no)) = TRIM(LOWER(@urunNo)) OR (urun_no IS NULL AND @urunNo IS NULL) AND TRIM(LOWER(urun_adi)) = TRIM(LOWER(@urunAdi))";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@urunNo", string.IsNullOrWhiteSpace(textBox2.Text) ? (object)DBNull.Value : textBox2.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@urunAdi", string.IsNullOrWhiteSpace(textBox1.Text) ? (object)DBNull.Value : textBox1.Text.Trim());
                        using (SQLiteDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Ürün stokta bulunamadı! Lütfen ürün adı veya ürün numarasını doğru girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            int stokMiktari = Convert.ToInt32(reader["adet"]);
                            decimal alisFiyati = Convert.ToDecimal(reader["alis_fiyati"]);
                            decimal satisFiyati = Convert.ToDecimal(reader["satis_fiyati"]);
                            decimal mevcutKazanc = Convert.ToDecimal(reader["kazanc"]);
                            reader.Close();

                            if (stokMiktari < satilacakMiktar)
                            {
                                MessageBox.Show($"Yetersiz stok! Mevcut: {stokMiktari}, Satış İstediğiniz: {satilacakMiktar}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            decimal saleProfit = (satisFiyati - alisFiyati) * satilacakMiktar;
                            decimal yeniKazanc = mevcutKazanc + saleProfit;

                            string updateQuery = "UPDATE stok SET adet = adet - @satilacakMiktar, kazanc = @yeniKazanc WHERE TRIM(LOWER(urun_no)) = TRIM(LOWER(@urunNo)) OR (urun_no IS NULL AND @urunNo IS NULL) AND TRIM(LOWER(urun_adi)) = TRIM(LOWER(@urunAdi))";
                            using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, con))
                            {
                                updateCmd.Parameters.AddWithValue("@satilacakMiktar", satilacakMiktar);
                                updateCmd.Parameters.AddWithValue("@yeniKazanc", yeniKazanc);
                                updateCmd.Parameters.AddWithValue("@urunNo", string.IsNullOrWhiteSpace(textBox2.Text) ? (object)DBNull.Value : textBox2.Text.Trim());
                                updateCmd.Parameters.AddWithValue("@urunAdi", string.IsNullOrWhiteSpace(textBox1.Text) ? (object)DBNull.Value : textBox1.Text.Trim());
                                updateCmd.ExecuteNonQuery();
                            }

                            MessageBox.Show($"Satış işlemi başarılı!\nBu satıştan elde edilen kazanç: {saleProfit:C2}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            StokListele();
                            TextboxlariTemizle();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satış işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PictureBox5: DataGridView'deki tüm değişiklikleri kaydet
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(dbPath))
                {
                    con.Open();
                    bool hasError = false;

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int id = Convert.ToInt32(row.Cells["id"].Value);
                        string urunAdi = row.Cells["urun_adi"].Value?.ToString();
                        string urunNo = row.Cells["urun_no"].Value?.ToString();
                        string adetStr = row.Cells["adet"].Value?.ToString();
                        string alisFiyatiStr = row.Cells["alis_fiyati"].Value?.ToString();
                        string satisFiyatiStr = row.Cells["satis_fiyati"].Value?.ToString();
                        string kazancStr = row.Cells["kazanc"].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(urunAdi))
                        {
                            MessageBox.Show($"Satır {row.Index + 1}: Ürün adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            hasError = true;
                            continue;
                        }
                        if (!int.TryParse(adetStr, out int adet))
                        {
                            MessageBox.Show($"Satır {row.Index + 1}: Adet için geçerli bir sayı girin (Örnek: 10).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            hasError = true;
                            continue;
                        }
                        if (!decimal.TryParse(alisFiyatiStr, out decimal alisFiyati))
                        {
                            MessageBox.Show($"Satır {row.Index + 1}: Alış fiyatı için geçerli bir sayı girin (Örnek: 50.00).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            hasError = true;
                            continue;
                        }
                        if (!decimal.TryParse(satisFiyatiStr, out decimal satisFiyati))
                        {
                            MessageBox.Show($"Satır {row.Index + 1}: Satış fiyatı için geçerli bir sayı girin (Örnek: 75.00).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            hasError = true;
                            continue;
                        }
                        if (!decimal.TryParse(kazancStr, out decimal kazanc))
                        {
                            MessageBox.Show($"Satır {row.Index + 1}: Kazanç için geçerli bir sayı girin (Örnek: 25.00).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            hasError = true;
                            continue;
                        }

                        string updateQuery = "UPDATE stok SET urun_adi = @urunAdi, urun_no = @urunNo, adet = @adet, " +
                                             "alis_fiyati = @alisFiyati, satis_fiyati = @satisFiyati, kazanc = @kazanc WHERE id = @id";
                        using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@id", id);
                            updateCmd.Parameters.AddWithValue("@urunAdi", urunAdi);
                            updateCmd.Parameters.AddWithValue("@urunNo", string.IsNullOrWhiteSpace(urunNo) ? (object)DBNull.Value : urunNo);
                            updateCmd.Parameters.AddWithValue("@adet", adet);
                            updateCmd.Parameters.AddWithValue("@alisFiyati", alisFiyati);
                            updateCmd.Parameters.AddWithValue("@satisFiyati", satisFiyati);
                            updateCmd.Parameters.AddWithValue("@kazanc", kazanc);
                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    if (hasError)
                    {
                        MessageBox.Show("Bazı satırlarda hatalar bulundu. Lütfen düzeltip tekrar deneyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Tüm değişiklikler başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    StokListele();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Değişiklikler kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PictureBox4: TextBox'ları temizle
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            TextboxlariTemizle();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Regular);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Boş bırakıldı, gerekirse işlev eklenebilir
        }
    }
}