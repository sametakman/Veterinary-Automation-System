using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Drawing;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.Threading;

namespace WinFormsApp8
{
    public partial class Form14 : Form
    {
        private readonly string dbPath = @"C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db";
        private readonly string connectionString;
        private SQLiteConnection? sharedConnection;
        private DataTable originalDataTable;
        private DataTable currentDataTable;
        private readonly System.Windows.Forms.Timer timer;
        private bool isSaving = false;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public Form14()
        {
            InitializeComponent();
            connectionString = $"Data Source={dbPath};Version=3;";
            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += Timer_Tick;
            originalDataTable = new DataTable();
            currentDataTable = new DataTable();

            this.Load += async (s, e) => await InitializeFormAsync();
        }

        private async Task InitializeFormAsync()
        {
            await InitializeSharedConnectionAsync();
            await EnsureDatabaseSchemaAsync();
            await UpdateExistingRecordsAsync();
            InitializeDataGridView();
            SetupButton();
            SetupRadioButtonEvents();
            SetupCheckBoxEvents();
            SetupTextBoxEvents();
            await UpdateDataTableAsync();
            await UpdateCountdown();
            timer.Start();
        }

        private async Task InitializeSharedConnectionAsync()
        {
            try
            {
                sharedConnection = new SQLiteConnection(connectionString);
                await sharedConnection.OpenAsync();
                using (var command = new SQLiteCommand("PRAGMA journal_mode=WAL;", sharedConnection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Bağlantı hatası: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                sharedConnection = null;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            timer.Stop();
            if (sharedConnection != null && sharedConnection.State == ConnectionState.Open)
            {
                sharedConnection.Close();
                sharedConnection.Dispose();
            }
            semaphore.Dispose();
        }

        private async Task EnsureDatabaseSchemaAsync()
        {
            if (sharedConnection == null) return;

            try
            {
                string createHayvanKayitQuery = @"
                    CREATE TABLE IF NOT EXISTS HayvanKayit (
                        HayvanID INTEGER PRIMARY KEY AUTOINCREMENT,
                        HayvanSahibi TEXT NOT NULL,
                        Telefon TEXT,
                        KupeNo TEXT,
                        Cinsi TEXT,
                        Adres TEXT NOT NULL,
                        GebelikDurumu TEXT
                    );";
                using (var command = new SQLiteCommand(createHayvanKayitQuery, sharedConnection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                string createKopekAsiQuery = @"
                    CREATE TABLE IF NOT EXISTS kopek_asi (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        IsimSoyisim TEXT NOT NULL,
                        Telefon TEXT NOT NULL,
                        Cins TEXT,
                        AsiAdi TEXT,
                        AsiSuresi TEXT,
                        DozSayisi INTEGER,
                        IcParazit INTEGER CHECK(IcParazit IN (0, 1)),
                        DisParazit INTEGER CHECK(DisParazit IN (0, 1)),
                        YapilmaDurumu TEXT CHECK(YapilmaDurumu IN ('Yapildi', 'Yapilmadi')),
                        YasAraligi TEXT CHECK(YasAraligi IN ('6-8 HAFTA', '9-11 HAFTA', '12-14 HAFTA', '13-15 HAFTA', '4-6 AYLIK', '12 AYLIK (1 YIL)')),
                        Doz INTEGER,
                        asisayisi INTEGER,
                        Digerdoz INTEGER,
                        AsiSonTarihi TEXT,
                        KayitTarihi TEXT,
                        FOREIGN KEY (Telefon) REFERENCES HayvanKayit(Telefon)
                    );";
                using (var command = new SQLiteCommand(createKopekAsiQuery, sharedConnection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                string createMailQuery = @"
                    CREATE TABLE IF NOT EXISTS mail (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        email TEXT NOT NULL,
                        password TEXT NOT NULL
                    );";
                using (var command = new SQLiteCommand(createMailQuery, sharedConnection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                string checkColumnQuery = "PRAGMA table_info(kopek_asi);";
                bool hasKayitTarihiColumn = false;
                bool hasAsiSonTarihiColumn = false;
                using (var command = new SQLiteCommand(checkColumnQuery, sharedConnection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string? columnName = reader["name"]?.ToString();
                            if (columnName == "KayitTarihi") hasKayitTarihiColumn = true;
                            if (columnName == "AsiSonTarihi") hasAsiSonTarihiColumn = true;
                        }
                    }
                }

                if (!hasKayitTarihiColumn)
                {
                    string alterTableQuery = "ALTER TABLE kopek_asi ADD COLUMN KayitTarihi TEXT;";
                    using (var command = new SQLiteCommand(alterTableQuery, sharedConnection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

                if (!hasAsiSonTarihiColumn)
                {
                    string alterTableQuery = "ALTER TABLE kopek_asi ADD COLUMN AsiSonTarihi TEXT;";
                    using (var command = new SQLiteCommand(alterTableQuery, sharedConnection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Şema güncelleme hatası: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task UpdateExistingRecordsAsync()
        {
            if (sharedConnection == null) return;

            try
            {
                string updateQuery = @"
                    UPDATE HayvanKayit 
                    SET Adres = @DefaultEmail 
                    WHERE Adres IS NULL OR Adres = '' OR Adres NOT LIKE '%@%.%';";
                using (var command = new SQLiteCommand(updateQuery, sharedConnection))
                {
                    command.Parameters.AddWithValue("@DefaultEmail", "bilinmeyen@ornek.com");
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Mevcut kayıtları güncellerken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private void SetupButton()
        {
            if (button2 != null)
            {
                button2.Enabled = true;
                button2.Visible = true;
                button2.Text = "Kaydet";
                button2.Click += async (s, e) => await Button2_ClickAsync(s, e);
            }

            if (button1 != null)
            {
                button1.Enabled = true;
                button1.Visible = true;
                button1.Text = "E-posta Gönder";
                button1.Click += async (s, e) => await Button1_ClickAsync(s, e);
            }

            if (button4 != null)
            {
                button4.Enabled = true;
                button4.Visible = true;
                button4.Text = "Değişiklikleri Kaydet";
                button4.Click += async (s, e) => await Button4_ClickAsync(s, e);
            }
        }

        private void SetupRadioButtonEvents()
        {
            // Radio buttons are mutually exclusive by design
        }

        private void SetupCheckBoxEvents()
        {
            // CheckBox events set in designer
        }

        private void SetupTextBoxEvents()
        {
            if (textBox2 != null)
            {
                textBox2.KeyPress += (s, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                        e.Handled = true;
                };
            }

            if (textBox7 != null)
            {
                textBox7.KeyPress += (s, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                        e.Handled = true;
                };
            }

            if (textBox5 != null)
            {
                textBox5.TextChanged += async (s, e) => await FilterDataGridViewAsync();
            }

            if (textBox8 != null)
            {
                textBox8.TextChanged += (s, e) =>
                {
                    string email = textBox8.Text?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                    {
                        textBox8.BackColor = Color.LightPink;
                    }
                    else
                    {
                        textBox8.BackColor = Color.White;
                    }
                };
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void InitializeDataGridView()
        {
            if (dataGridView1 == null) return;

            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.Visible = true;
            dataGridView1.ReadOnly = false;

            // Attach event handlers
            dataGridView1.UserDeletingRow += DataGridView1_UserDeletingRow;
            dataGridView1.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Delete)
                {
                    // Check if there are rows to delete
                    if (dataGridView1.Rows.Count == 0 || dataGridView1.RowCount == 0 || currentDataTable == null || currentDataTable.Rows.Count == 0)
                    {
                        e.Handled = true; // Prevent the delete operation
                        await InvokeAsync(() => MessageBox.Show("Silinecek kayıt yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                        return;
                    }

                    // Check if a row is selected
                    if (dataGridView1.SelectedRows.Count == 0)
                    {
                        e.Handled = true;
                        await InvokeAsync(() => MessageBox.Show("Lütfen silmek için bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                        return;
                    }
                }
            };

            // Define columns
            string[] columns = {
                "Id", "IsimSoyisim", "Telefon", "Cins", "AsiAdi", "AsiSuresi", "DozSayisi",
                "IcParazit", "DisParazit", "YapilmaDurumu", "YasAraligi", "Doz", "asisayisi",
                "Digerdoz", "HayvanSahibi", "KayitTarihi", "AsiSonTarihi", "GeriSayim"
            };

            foreach (var column in columns)
            {
                originalDataTable.Columns.Add(column, column == "Id" || column == "DozSayisi" || column == "Doz" || column == "asisayisi" || column == "Digerdoz" ? typeof(int) : typeof(string));
                currentDataTable.Columns.Add(column, column == "Id" || column == "DozSayisi" || column == "Doz" || column == "asisayisi" || column == "Digerdoz" ? typeof(int) : typeof(string));
            }

            dataGridView1.DataSource = currentDataTable;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Styling
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

            // Column headers
            dataGridView1.Columns["Id"].HeaderText = "KupeNo";
            dataGridView1.Columns["IsimSoyisim"].HeaderText = "Hayvan Sahibi";
            dataGridView1.Columns["Cins"].HeaderText = "Cinsi";
            dataGridView1.Columns["AsiAdi"].HeaderText = "Aşı Adı";
            dataGridView1.Columns["AsiSuresi"].HeaderText = "Aşı Süresi (Gün)";
            dataGridView1.Columns["DozSayisi"].HeaderText = "Doz Sayısı";
            dataGridView1.Columns["IcParazit"].HeaderText = "İç Parazit";
            dataGridView1.Columns["DisParazit"].HeaderText = "Dış Parazit";
            dataGridView1.Columns["YapilmaDurumu"].HeaderText = "Durum";
            dataGridView1.Columns["YasAraligi"].HeaderText = "Yaş Aralığı";
            dataGridView1.Columns["Doz"].HeaderText = "Doz";
            dataGridView1.Columns["asisayisi"].HeaderText = "Aşı Sayısı";
            dataGridView1.Columns["Digerdoz"].HeaderText = "Diğer Doz";
            dataGridView1.Columns["HayvanSahibi"].HeaderText = "Hayvan Sahibi";
            dataGridView1.Columns["KayitTarihi"].HeaderText = "Kayıt Tarihi";
            dataGridView1.Columns["AsiSonTarihi"].HeaderText = "Son Aşı Tarihi";
            dataGridView1.Columns["GeriSayim"].HeaderText = "Geri Sayım";

            // Column widths
            dataGridView1.Columns["Id"].Width = 100;
            dataGridView1.Columns["IsimSoyisim"].Width = 150;
            dataGridView1.Columns["Telefon"].Width = 100;
            dataGridView1.Columns["Cins"].Width = 100;
            dataGridView1.Columns["AsiAdi"].Width = 100;
            dataGridView1.Columns["AsiSuresi"].Width = 100;
            dataGridView1.Columns["DozSayisi"].Width = 100;
            dataGridView1.Columns["IcParazit"].Width = 100;
            dataGridView1.Columns["DisParazit"].Width = 100;
            dataGridView1.Columns["YapilmaDurumu"].Width = 100;
            dataGridView1.Columns["YasAraligi"].Width = 150;
            dataGridView1.Columns["Doz"].Width = 100;
            dataGridView1.Columns["asisayisi"].Width = 100;
            dataGridView1.Columns["Digerdoz"].Width = 100;
            dataGridView1.Columns["HayvanSahibi"].Width = 150;
            dataGridView1.Columns["KayitTarihi"].Width = 150;
            dataGridView1.Columns["AsiSonTarihi"].Width = 150;
            dataGridView1.Columns["GeriSayim"].Width = 150;
        }

        private async void DataGridView1_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                // Check if DataGridView or DataTable is empty
                if (dataGridView1 == null || dataGridView1.Rows.Count <= 1 || currentDataTable == null || currentDataTable.Rows.Count == 0)
                {
                    e.Cancel = true;
                    await InvokeAsync(() => MessageBox.Show("Silinecek kayıt yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                if (e.Row?.DataBoundItem is not DataRowView rowView || rowView.Row == null)
                {
                    e.Cancel = true;
                    await InvokeAsync(() => MessageBox.Show("Silinecek satır geçersiz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    return;
                }

                DataRow row = rowView.Row;
                if (row.RowState == DataRowState.Deleted)
                {
                    e.Cancel = true;
                    await InvokeAsync(() => MessageBox.Show("Bu satır zaten silinmiş!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                string? id = row["Id"]?.ToString();
                if (string.IsNullOrEmpty(id))
                {
                    e.Cancel = true;
                    await InvokeAsync(() => MessageBox.Show("Silinecek satırın kimliği bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    return;
                }

                // Remove the row from currentDataTable
                await semaphore.WaitAsync();
                try
                {
                    // Find the row in currentDataTable by Id
                    DataRow? rowToDelete = currentDataTable.AsEnumerable()
                        .FirstOrDefault(r => r.RowState != DataRowState.Deleted && r["Id"].ToString() == id);

                    if (rowToDelete == null)
                    {
                        e.Cancel = true;
                        await InvokeAsync(() => MessageBox.Show("Silinecek satır bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                        return;
                    }

                    currentDataTable.Rows.Remove(rowToDelete);
                }
                finally
                {
                    semaphore.Release();
                }

                // Remove the corresponding row from originalDataTable
                await semaphore.WaitAsync();
                try
                {
                    DataRow? origRow = originalDataTable.AsEnumerable()
                        .FirstOrDefault(r => r.RowState != DataRowState.Deleted && r["Id"].ToString() == id);

                    if (origRow != null)
                    {
                        originalDataTable.Rows.Remove(origRow);
                    }
                }
                finally
                {
                    semaphore.Release();
                }

                // Delete from the database
                if (sharedConnection != null)
                {
                    string deleteQuery = "DELETE FROM kopek_asi WHERE Id = @Id";
                    using (var cmd = new SQLiteCommand(deleteQuery, sharedConnection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Refresh the DataGridView
                await InvokeAsync(() =>
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = currentDataTable;
                    dataGridView1.Refresh();
                });

                await InvokeAsync(() => MessageBox.Show("Kayıt başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information));
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                await InvokeAsync(() => MessageBox.Show($"Satır silinirken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task UpdateDataTableAsync()
        {
            timer.Stop();
            try
            {
                if (sharedConnection == null) return;

                string query = @"
                    SELECT k.Id, k.IsimSoyisim, k.Telefon, k.Cins, k.AsiAdi, k.AsiSuresi, k.DozSayisi, 
                           k.IcParazit, k.DisParazit, k.YapilmaDurumu, k.YasAraligi, k.Doz, k.asisayisi, k.Digerdoz, 
                           h.HayvanSahibi, k.KayitTarihi, k.AsiSonTarihi
                    FROM kopek_asi k
                    LEFT JOIN HayvanKayit h ON k.Telefon = h.Telefon";
                using (var command = new SQLiteCommand(query, sharedConnection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            originalDataTable.Clear();
                            currentDataTable.Clear();
                            while (reader.Read())
                            {
                                string icParazitDisplay = reader["IcParazit"] != DBNull.Value && reader.GetInt32(reader.GetOrdinal("IcParazit")) == 1 ? "Yapıldı" : "Yapılmadı";
                                string disParazitDisplay = reader["DisParazit"] != DBNull.Value && reader.GetInt32(reader.GetOrdinal("DisParazit")) == 1 ? "Yapıldı" : "Yapılmadı";

                                var row = originalDataTable.NewRow();
                                row["Id"] = reader["Id"] != DBNull.Value ? reader["Id"] : DBNull.Value;
                                row["IsimSoyisim"] = reader["IsimSoyisim"] != DBNull.Value ? reader["IsimSoyisim"] : string.Empty;
                                row["Telefon"] = reader["Telefon"] != DBNull.Value ? reader["Telefon"] : string.Empty;
                                row["Cins"] = reader["Cins"] != DBNull.Value ? reader["Cins"] : string.Empty;
                                row["AsiAdi"] = reader["AsiAdi"] != DBNull.Value ? reader["AsiAdi"] : string.Empty;
                                row["AsiSuresi"] = reader["AsiSuresi"] != DBNull.Value ? reader["AsiSuresi"] : "0";
                                row["DozSayisi"] = reader["DozSayisi"] != DBNull.Value ? reader["DozSayisi"] : 1;
                                row["IcParazit"] = icParazitDisplay;
                                row["DisParazit"] = disParazitDisplay;
                                row["YapilmaDurumu"] = reader["YapilmaDurumu"] != DBNull.Value ? reader["YapilmaDurumu"] : string.Empty;
                                row["YasAraligi"] = reader["YasAraligi"] != DBNull.Value ? reader["YasAraligi"] : string.Empty;
                                row["Doz"] = reader["Doz"] != DBNull.Value ? reader["Doz"] : 1;
                                row["asisayisi"] = reader["asisayisi"] != DBNull.Value ? reader["asisayisi"] : 1;
                                row["Digerdoz"] = reader["Digerdoz"] != DBNull.Value ? reader["Digerdoz"] : 0;
                                row["HayvanSahibi"] = reader["HayvanSahibi"] != DBNull.Value ? reader["HayvanSahibi"] : string.Empty;
                                row["KayitTarihi"] = reader["KayitTarihi"] != DBNull.Value ? reader["KayitTarihi"] : DateTime.Now.ToString("o");
                                row["AsiSonTarihi"] = reader["AsiSonTarihi"] != DBNull.Value ? reader["AsiSonTarihi"] : string.Empty;
                                row["GeriSayim"] = "Hesaplanıyor...";

                                originalDataTable.Rows.Add(row);
                                var currentRow = currentDataTable.NewRow();
                                foreach (DataColumn col in originalDataTable.Columns)
                                {
                                    currentRow[col.ColumnName] = row[col.ColumnName];
                                }
                                currentDataTable.Rows.Add(currentRow);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                }

                await InvokeAsync(() =>
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = currentDataTable;
                });
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"DataGridView yüklenirken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
            finally
            {
                timer.Start();
            }
        }

        private async Task FilterDataGridViewAsync()
        {
            timer.Stop();
            try
            {
                if (sharedConnection == null) return;

                string telefonFilter = textBox5.Text?.Trim() ?? string.Empty;
                string query = string.IsNullOrWhiteSpace(telefonFilter) ?
                    @"SELECT k.Id, k.IsimSoyisim, k.Telefon, k.Cins, k.AsiAdi, k.AsiSuresi, k.DozSayisi, 
                             k.IcParazit, k.DisParazit, k.YapilmaDurumu, k.YasAraligi, k.Doz, k.asisayisi, k.Digerdoz, 
                             h.HayvanSahibi, k.KayitTarihi, k.AsiSonTarihi
                      FROM kopek_asi k
                      LEFT JOIN HayvanKayit h ON k.Telefon = h.Telefon" :
                    @"SELECT k.Id, k.IsimSoyisim, k.Telefon, k.Cins, k.AsiAdi, k.AsiSuresi, k.DozSayisi, 
                             k.IcParazit, k.DisParazit, k.YapilmaDurumu, k.YasAraligi, k.Doz, k.asisayisi, k.Digerdoz, 
                             h.HayvanSahibi, k.KayitTarihi, k.AsiSonTarihi
                      FROM kopek_asi k
                      LEFT JOIN HayvanKayit h ON k.Telefon = h.Telefon
                      WHERE k.Telefon = @Telefon";

                using (var command = new SQLiteCommand(query, sharedConnection))
                {
                    if (!string.IsNullOrWhiteSpace(telefonFilter))
                        command.Parameters.AddWithValue("@Telefon", telefonFilter);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            currentDataTable.Clear();
                            while (reader.Read())
                            {
                                string icParazitDisplay = reader["IcParazit"] != DBNull.Value && reader.GetInt32(reader.GetOrdinal("IcParazit")) == 1 ? "Yapıldı" : "Yapılmadı";
                                string disParazitDisplay = reader["DisParazit"] != DBNull.Value && reader.GetInt32(reader.GetOrdinal("DisParazit")) == 1 ? "Yapıldı" : "Yapılmadı";

                                var row = currentDataTable.NewRow();
                                row["Id"] = reader["Id"] != DBNull.Value ? reader["Id"] : DBNull.Value;
                                row["IsimSoyisim"] = reader["IsimSoyisim"] != DBNull.Value ? reader["IsimSoyisim"] : string.Empty;
                                row["Telefon"] = reader["Telefon"] != DBNull.Value ? reader["Telefon"] : string.Empty;
                                row["Cins"] = reader["Cins"] != DBNull.Value ? reader["Cins"] : string.Empty;
                                row["AsiAdi"] = reader["AsiAdi"] != DBNull.Value ? reader["AsiAdi"] : string.Empty;
                                row["AsiSuresi"] = reader["AsiSuresi"] != DBNull.Value ? reader["AsiSuresi"] : "0";
                                row["DozSayisi"] = reader["DozSayisi"] != DBNull.Value ? reader["DozSayisi"] : 1;
                                row["IcParazit"] = icParazitDisplay;
                                row["DisParazit"] = disParazitDisplay;
                                row["YapilmaDurumu"] = reader["YapilmaDurumu"] != DBNull.Value ? reader["YapilmaDurumu"] : string.Empty;
                                row["YasAraligi"] = reader["YasAraligi"] != DBNull.Value ? reader["YasAraligi"] : string.Empty;
                                row["Doz"] = reader["Doz"] != DBNull.Value ? reader["Doz"] : 1;
                                row["asisayisi"] = reader["asisayisi"] != DBNull.Value ? reader["asisayisi"] : 1;
                                row["Digerdoz"] = reader["Digerdoz"] != DBNull.Value ? reader["Digerdoz"] : 0;
                                row["HayvanSahibi"] = reader["HayvanSahibi"] != DBNull.Value ? reader["HayvanSahibi"] : string.Empty;
                                row["KayitTarihi"] = reader["KayitTarihi"] != DBNull.Value ? reader["KayitTarihi"] : DateTime.Now.ToString("o");
                                row["AsiSonTarihi"] = reader["AsiSonTarihi"] != DBNull.Value ? reader["AsiSonTarihi"] : string.Empty;
                                row["GeriSayim"] = "Hesaplanıyor...";
                                currentDataTable.Rows.Add(row);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                }

                await InvokeAsync(() =>
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = currentDataTable;
                });

                await UpdateCountdown();
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"DataGridView filtrelenirken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
            finally
            {
                timer.Start();
            }
        }

        private async Task<(string? email, string? password)?> GetEmailCredentialsAsync()
        {
            if (sharedConnection == null) return null;

            try
            {
                string query = "SELECT email, password FROM mail LIMIT 1";
                using (var command = new SQLiteCommand(query, sharedConnection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string? email = reader["email"]?.ToString();
                            string? password = reader["password"]?.ToString();
                            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                            {
                                return null;
                            }
                            return (email, password);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"E-posta kimlik bilgileri alınırken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return null;
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false, int retryCount = 3)
        {
            var credentials = await GetEmailCredentialsAsync();
            if (credentials == null || string.IsNullOrEmpty(credentials.Value.email) || string.IsNullOrEmpty(credentials.Value.password))
            {
                await InvokeAsync(() => MessageBox.Show("E-posta gönderici kimlik bilgileri bulunamadı! Lütfen mail tablosuna geçerli bir e-posta ve şifre ekleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return;
            }

            if (!IsValidEmail(toEmail))
            {
                await InvokeAsync(() => MessageBox.Show($"Geçersiz e-posta adresi: {toEmail}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return;
            }

            int attempts = 0;
            while (attempts < retryCount)
            {
                try
                {
                    using (var client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(credentials.Value.email, credentials.Value.password);
                        client.Timeout = 30000;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(credentials.Value.email, "Veteriner Otomasyon Sistemi"),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = isBodyHtml
                        };
                        mailMessage.To.Add(toEmail);

                        await client.SendMailAsync(mailMessage);
                        await InvokeAsync(() => MessageBox.Show($"E-posta başarıyla gönderildi: {toEmail}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information));
                        return;
                    }
                }
                catch (SmtpException smtpEx)
                {
                    attempts++;
                    if (attempts == retryCount)
                    {
                        await InvokeAsync(() => MessageBox.Show($"SMTP hatası: {smtpEx.Message}\nE-posta gönderilemedi. Lütfen Gmail ayarlarınızı kontrol edin (örneğin, 2FA için App Password kullanın).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    }
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    attempts++;
                    if (attempts == retryCount)
                    {
                        await InvokeAsync(() => MessageBox.Show($"E-posta gönderilirken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    }
                    await Task.Delay(2000);
                }
            }
        }

        private async Task UpdateCountdown()
        {
            await Task.Run(async () =>
            {
                await InvokeAsync(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        if (dataGridView1.Rows.Count == 0 || currentDataTable == null || currentDataTable.Rows.Count == 0) return;

                        foreach (DataGridViewRow gridRow in dataGridView1.Rows)
                        {
                            if (gridRow.IsNewRow || gridRow.DataBoundItem == null) continue;

                            DataRowView? rowView = gridRow.DataBoundItem as DataRowView;
                            if (rowView == null || rowView.Row == null || rowView.Row.RowState == DataRowState.Deleted || rowView.Row.Table == null) continue;

                            DataRow row = rowView.Row;
                            DataRow clonedRow = row.Table.NewRow();
                            foreach (DataColumn col in row.Table.Columns)
                            {
                                clonedRow[col.ColumnName] = row[col.ColumnName];
                            }

                            string? asiSuresiStr = clonedRow["AsiSuresi"]?.ToString() ?? "0";
                            string? dozSayisiStr = clonedRow["DozSayisi"]?.ToString() ?? "1";
                            string? kayitTarihiStr = clonedRow["KayitTarihi"]?.ToString() ?? DateTime.Now.ToString("o");
                            string? asiSonTarihiStr = clonedRow["AsiSonTarihi"]?.ToString();
                            string? digerDozStr = clonedRow["Digerdoz"]?.ToString() ?? "0";

                            if (!int.TryParse(asiSuresiStr, out int asiSuresi) || !int.TryParse(dozSayisiStr, out int dozSayisi) || !int.TryParse(digerDozStr, out int digerDoz))
                            {
                                clonedRow["GeriSayim"] = "Hatalı Veri";
                                gridRow.DefaultCellStyle.BackColor = Color.White;
                                gridRow.DefaultCellStyle.ForeColor = Color.Black;
                                continue;
                            }

                            if (!DateTime.TryParse(kayitTarihiStr, out DateTime kayitTarihi))
                            {
                                clonedRow["GeriSayim"] = "Hatalı Tarih";
                                gridRow.DefaultCellStyle.BackColor = Color.White;
                                gridRow.DefaultCellStyle.ForeColor = Color.Black;
                                continue;
                            }

                            if (dozSayisi <= 1 || digerDoz <= 0)
                            {
                                clonedRow["GeriSayim"] = "Tamamlandı";
                                gridRow.DefaultCellStyle.BackColor = Color.Gray;
                                gridRow.DefaultCellStyle.ForeColor = Color.White;
                                continue;
                            }

                            DateTime asiSonTarihi;
                            if (!string.IsNullOrEmpty(asiSonTarihiStr) && DateTime.TryParse(asiSonTarihiStr, out DateTime parsedAsiSonTarihi))
                            {
                                asiSonTarihi = parsedAsiSonTarihi;
                            }
                            else
                            {
                                double intervalDays = asiSuresi / (double)(dozSayisi - 1);
                                double elapsedDays = (DateTime.Now - kayitTarihi).TotalDays;
                                int currentDose = Math.Min((int)(elapsedDays / intervalDays) + 1, dozSayisi);
                                asiSonTarihi = kayitTarihi.AddDays(intervalDays * currentDose);
                            }

                            TimeSpan timeUntilNextDose = asiSonTarihi - DateTime.Now;

                            if (timeUntilNextDose.TotalSeconds <= 0)
                            {
                                clonedRow["GeriSayim"] = "Süre Doldu";
                                gridRow.DefaultCellStyle.BackColor = Color.Gray;
                                gridRow.DefaultCellStyle.ForeColor = Color.White;
                                continue;
                            }

                            clonedRow["GeriSayim"] = $"{timeUntilNextDose.Days} gün {timeUntilNextDose.Hours} saat {timeUntilNextDose.Minutes} dakika";

                            if (timeUntilNextDose.TotalDays >= 15)
                            {
                                gridRow.DefaultCellStyle.BackColor = Color.LightGreen;
                                gridRow.DefaultCellStyle.ForeColor = Color.Black;
                            }
                            else
                            {
                                gridRow.DefaultCellStyle.BackColor = Color.Red;
                                gridRow.DefaultCellStyle.ForeColor = Color.White;
                            }

                            foreach (DataColumn col in row.Table.Columns)
                            {
                                row[col.ColumnName] = clonedRow[col.ColumnName];
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                    dataGridView1.Refresh();
                });
            });
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (isSaving) return;
            _ = UpdateCountdown();
        }

        private async Task<bool> TelefonExistsAsync(string telefon)
        {
            if (sharedConnection == null) return false;

            try
            {
                string query = "SELECT COUNT(*) FROM HayvanKayit WHERE Telefon = @Telefon";
                using (var command = new SQLiteCommand(query, sharedConnection))
                {
                    command.Parameters.AddWithValue("@Telefon", telefon ?? string.Empty);
                    long count = (long)(await command.ExecuteScalarAsync() ?? 0);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Telefon varlığı kontrol edilirken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return false;
            }
        }

        private async Task Button1_ClickAsync(object? sender, EventArgs e)
        {
            try
            {
                string toEmail = textBox8.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    await InvokeAsync(() => MessageBox.Show("textBox8 (E-posta) boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                if (!IsValidEmail(toEmail))
                {
                    await InvokeAsync(() => MessageBox.Show($"Geçersiz e-posta adresi: {toEmail}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                string subject = "Veteriner Otomasyon - Aşı Kayıtları (3 Gün ve Altı)";
                string body = await GenerateKopekAsiTableHtmlWithCountdownFilterAsync();
                await SendEmailAsync(toEmail, subject, body, true);
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"E-posta gönderilirken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task Button2_ClickAsync(object? sender, EventArgs e)
        {
            if (sharedConnection == null)
            {
                await InvokeAsync(() => MessageBox.Show("Veritabanı bağlantısı kurulamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return;
            }

            try
            {
                string isimSoyisim = textBox1.Text?.Trim() ?? string.Empty;
                string telefon = textBox3.Text?.Trim() ?? string.Empty;
                string asiSuresiStr = textBox2.Text?.Trim() ?? string.Empty;
                string dozSayisiStr = textBox7.Text?.Trim() ?? string.Empty;
                string cins = textBox4.Text?.Trim() ?? string.Empty;
                string email = textBox8.Text?.Trim() ?? string.Empty;

                // Validate required fields (excluding email)
                if (string.IsNullOrWhiteSpace(isimSoyisim))
                {
                    await InvokeAsync(() => MessageBox.Show("textBox1 (Isim Soyisim) boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }
                if (string.IsNullOrWhiteSpace(telefon))
                {
                    await InvokeAsync(() => MessageBox.Show("textBox3 (Telefon) boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }
                if (string.IsNullOrWhiteSpace(asiSuresiStr))
                {
                    await InvokeAsync(() => MessageBox.Show("textBox2 (Aşı Süresi) boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }
                if (string.IsNullOrWhiteSpace(dozSayisiStr))
                {
                    await InvokeAsync(() => MessageBox.Show("textBox7 (Doz Sayısı) boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }
                if (string.IsNullOrWhiteSpace(cins))
                {
                    await InvokeAsync(() => MessageBox.Show("textBox4 (Cins) boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                // Validate email format if provided
                if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                {
                    await InvokeAsync(() => MessageBox.Show("textBox8 (E-posta) geçerli bir e-posta formatında olmalı! Örnek: ornek@domain.com", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                if (!int.TryParse(asiSuresiStr, out int asiSuresi) || asiSuresi <= 0)
                {
                    await InvokeAsync(() => MessageBox.Show($"textBox2 (Aşı Süresi) pozitif bir sayı olmalı! Girdi: {asiSuresiStr}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }
                if (!int.TryParse(dozSayisiStr, out int dozSayisi) || dozSayisi <= 0)
                {
                    await InvokeAsync(() => MessageBox.Show($"textBox7 (Doz Sayısı) pozitif bir sayı olmalı! Girdi: {dozSayisiStr}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                // Set default email if empty
                string emailToUse = string.IsNullOrWhiteSpace(email) ? "bilinmeyen@ornek.com" : email;

                int digerDoz = dozSayisi - 1;
                DateTime kayitTarihi = DateTime.Now;
                double intervalDays = asiSuresi / (double)(dozSayisi - 1);
                string asiSonTarihi = kayitTarihi.AddDays(intervalDays).ToString("o");

                string yasAraligi = string.Empty;
                if (radioButton1?.Checked == true)
                    yasAraligi = "6-8 HAFTA";
                else if (radioButton2?.Checked == true)
                    yasAraligi = "9-11 HAFTA";
                else if (radioButton3?.Checked == true)
                    yasAraligi = "12-14 HAFTA";
                else if (radioButton4?.Checked == true)
                    yasAraligi = "13-15 HAFTA";
                else if (radioButton5?.Checked == true)
                    yasAraligi = "4-6 AYLIK";
                else
                {
                    await InvokeAsync(() => MessageBox.Show("Lütfen bir yaş aralığı seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    return;
                }

                int icParazit = checkBox1.Checked ? 1 : 0;
                int disParazit = checkBox2.Checked ? 1 : 0;
                string yapilmaDurumu = "Yapilmadi";
                if (radioButton7?.Checked == true)
                    yapilmaDurumu = "Yapildi";

                string kayitTarihiStr = kayitTarihi.ToString("o");
                string icParazitDisplay = icParazit == 1 ? "Yapıldı" : "Yapılmadı";
                string disParazitDisplay = disParazit == 1 ? "Yapıldı" : "Yapılmadı";
                string asiAdi = string.IsNullOrWhiteSpace(textBox5.Text) ? "Varsayılan Aşı" : textBox5.Text?.Trim() ?? "Varsayılan Aşı";

                long insertedId = 0;
                using (var transaction = sharedConnection.BeginTransaction())
                {
                    if (!await TelefonExistsAsync(telefon))
                    {
                        string hayvanKayitQuery = "INSERT INTO HayvanKayit (HayvanSahibi, Telefon, Adres) VALUES (@HayvanSahibi, @Telefon, @Adres)";
                        using (var command = new SQLiteCommand(hayvanKayitQuery, sharedConnection))
                        {
                            command.Parameters.AddWithValue("@HayvanSahibi", isimSoyisim);
                            command.Parameters.AddWithValue("@Telefon", telefon);
                            command.Parameters.AddWithValue("@Adres", emailToUse);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        string updateQuery = "UPDATE HayvanKayit SET Adres = @Adres WHERE Telefon = @Telefon AND (Adres IS NULL OR Adres = '' OR Adres NOT LIKE '%@%.%')";
                        using (var command = new SQLiteCommand(updateQuery, sharedConnection))
                        {
                            command.Parameters.AddWithValue("@Adres", emailToUse);
                            command.Parameters.AddWithValue("@Telefon", telefon);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    string kopekAsiQuery = @"
                        INSERT INTO kopek_asi 
                        (IsimSoyisim, Telefon, Cins, AsiAdi, AsiSuresi, DozSayisi, IcParazit, DisParazit, YapilmaDurumu, YasAraligi, Doz, asisayisi, Digerdoz, KayitTarihi, AsiSonTarihi) 
                        VALUES 
                        (@IsimSoyisim, @Telefon, @Cins, @AsiAdi, @AsiSuresi, @DozSayisi, @IcParazit, @DisParazit, @YapilmaDurumu, @YasAraligi, @Doz, @asisayisi, @Digerdoz, @KayitTarihi, @AsiSonTarihi);
                        SELECT last_insert_rowid();";
                    using (var command = new SQLiteCommand(kopekAsiQuery, sharedConnection))
                    {
                        command.Parameters.AddWithValue("@IsimSoyisim", isimSoyisim);
                        command.Parameters.AddWithValue("@Telefon", telefon);
                        command.Parameters.AddWithValue("@Cins", cins);
                        command.Parameters.AddWithValue("@AsiAdi", asiAdi);
                        command.Parameters.AddWithValue("@AsiSuresi", asiSuresi.ToString());
                        command.Parameters.AddWithValue("@DozSayisi", dozSayisi);
                        command.Parameters.AddWithValue("@IcParazit", icParazit);
                        command.Parameters.AddWithValue("@DisParazit", disParazit);
                        command.Parameters.AddWithValue("@YapilmaDurumu", yapilmaDurumu);
                        command.Parameters.AddWithValue("@YasAraligi", yasAraligi);
                        command.Parameters.AddWithValue("@Doz", dozSayisi);
                        command.Parameters.AddWithValue("@asisayisi", dozSayisi);
                        command.Parameters.AddWithValue("@Digerdoz", digerDoz);
                        command.Parameters.AddWithValue("@KayitTarihi", kayitTarihiStr);
                        command.Parameters.AddWithValue("@AsiSonTarihi", asiSonTarihi);
                        insertedId = (long)(await command.ExecuteScalarAsync() ?? 0);
                    }

                    await transaction.CommitAsync();
                }

                await semaphore.WaitAsync();
                try
                {
                    var newRow = currentDataTable.NewRow();
                    newRow["Id"] = insertedId;
                    newRow["IsimSoyisim"] = isimSoyisim;
                    newRow["Telefon"] = telefon;
                    newRow["Cins"] = cins;
                    newRow["AsiAdi"] = asiAdi;
                    newRow["AsiSuresi"] = asiSuresi.ToString();
                    newRow["DozSayisi"] = dozSayisi;
                    newRow["IcParazit"] = icParazitDisplay;
                    newRow["DisParazit"] = disParazitDisplay;
                    newRow["YapilmaDurumu"] = yapilmaDurumu;
                    newRow["YasAraligi"] = yasAraligi;
                    newRow["Doz"] = dozSayisi;
                    newRow["asisayisi"] = dozSayisi;
                    newRow["Digerdoz"] = digerDoz;
                    newRow["HayvanSahibi"] = isimSoyisim;
                    newRow["KayitTarihi"] = kayitTarihiStr;
                    newRow["AsiSonTarihi"] = asiSonTarihi;
                    newRow["GeriSayim"] = "Hesaplanıyor...";
                    currentDataTable.Rows.Add(newRow);

                    var origRow = originalDataTable.NewRow();
                    foreach (DataColumn col in currentDataTable.Columns)
                    {
                        origRow[col.ColumnName] = newRow[col.ColumnName];
                    }
                    originalDataTable.Rows.Add(origRow);
                }
                finally
                {
                    semaphore.Release();
                }

                await InvokeAsync(() => MessageBox.Show("Kayıt başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information));

                await InvokeAsync(() =>
                {
                    if (textBox1 != null) textBox1.Text = string.Empty;
                    if (textBox2 != null) textBox2.Text = string.Empty;
                    if (textBox3 != null) textBox3.Text = string.Empty;
                    if (textBox4 != null) textBox4.Text = string.Empty;
                    if (textBox5 != null) textBox5.Text = string.Empty;
                    if (textBox7 != null) textBox7.Text = string.Empty;
                    if (textBox8 != null) textBox8.Text = string.Empty;
                    if (checkBox1 != null) checkBox1.Checked = false;
                    if (checkBox2 != null) checkBox2.Checked = false;
                    if (radioButton1 != null) radioButton1.Checked = false;
                    if (radioButton2 != null) radioButton2.Checked = false;
                    if (radioButton3 != null) radioButton3.Checked = false;
                    if (radioButton4 != null) radioButton4.Checked = false;
                    if (radioButton5 != null) radioButton5.Checked = false;
                    if (radioButton7 != null) radioButton7.Checked = false;
                });

                await FilterDataGridViewAsync();
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task Button4_ClickAsync(object? sender, EventArgs e)
        {
            try
            {
                await SaveDataGridViewChanges();
                await InvokeAsync(() => MessageBox.Show("Değişiklikler başarıyla veritabanına kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information));
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Değişiklikler kaydedilirken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task<string> GenerateKopekAsiTableHtmlWithCountdownFilterAsync()
        {
            if (sharedConnection == null)
                return "Veritabanı bağlantısı yok.";

            try
            {
                var htmlBuilder = new StringBuilder();
                htmlBuilder.AppendLine("<html><body>");
                htmlBuilder.AppendLine("<h2>Köpek Aşı Kayıtları (Geri Sayım 3 Gün ve Altı)</h2>");
                htmlBuilder.AppendLine("<p>Bu e-posta Veteriner Otomasyon Sistemi’nden gönderilmiştir. Aşağıda geri sayımı 3 gün ve altında olan kayıtlar listelenmiştir:</p>");

                bool hasRecords = false;

                htmlBuilder.AppendLine("<table border='1' style='border-collapse: collapse; width: 100%; font-family: Arial;'>");
                htmlBuilder.AppendLine("<tr style='background-color: #f2f2f2;'>");
                string[] columns = { "Id", "IsimSoyisim", "Telefon", "Cins", "AsiAdi", "AsiSuresi", "DozSayisi", "IcParazit", "DisParazit", "YapilmaDurumu", "YasAraligi", "Doz", "asisayisi", "Digerdoz", "AsiSonTarihi", "KayitTarihi", "GeriSayim" };
                foreach (var column in columns)
                    htmlBuilder.AppendLine($"<th style='padding: 8px;'>{column}</th>");
                htmlBuilder.AppendLine("</tr>");

                string query = "SELECT * FROM kopek_asi";
                using (var command = new SQLiteCommand(query, sharedConnection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string? asiSuresiStr = reader["AsiSuresi"]?.ToString() ?? "0";
                            string? dozSayisiStr = reader["DozSayisi"]?.ToString() ?? "1";
                            string? kayitTarihiStr = reader["KayitTarihi"]?.ToString() ?? DateTime.Now.ToString("o");
                            string? asiSonTarihiStr = reader["AsiSonTarihi"]?.ToString();
                            string? digerDozStr = reader["Digerdoz"]?.ToString() ?? "0";

                            if (!int.TryParse(asiSuresiStr, out int asiSuresi) || !int.TryParse(dozSayisiStr, out int dozSayisi) || !int.TryParse(digerDozStr, out int digerDoz))
                                continue;

                            if (!DateTime.TryParse(kayitTarihiStr, out DateTime kayitTarihi))
                                continue;

                            if (dozSayisi <= 1 || digerDoz <= 0)
                                continue;

                            DateTime asiSonTarihi;
                            if (!string.IsNullOrEmpty(asiSonTarihiStr) && DateTime.TryParse(asiSonTarihiStr, out DateTime parsedAsiSonTarihi))
                            {
                                asiSonTarihi = parsedAsiSonTarihi;
                            }
                            else
                            {
                                double intervalDays = asiSuresi / (double)(dozSayisi - 1);
                                double elapsedDays = (DateTime.Now - kayitTarihi).TotalDays;
                                int currentDose = Math.Min((int)(elapsedDays / intervalDays) + 1, dozSayisi);
                                asiSonTarihi = kayitTarihi.AddDays(intervalDays * currentDose);
                            }

                            TimeSpan timeUntilNextDose = asiSonTarihi - DateTime.Now;

                            if (timeUntilNextDose.TotalDays <= 3 || timeUntilNextDose.TotalSeconds <= 0)
                            {
                                hasRecords = true;
                                htmlBuilder.AppendLine("<tr>");
                                foreach (var column in columns)
                                {
                                    if (column == "GeriSayim")
                                    {
                                        string geriSayim = timeUntilNextDose.TotalSeconds <= 0
                                            ? "Süre Doldu"
                                            : $"{timeUntilNextDose.Days} gün {timeUntilNextDose.Hours} saat {timeUntilNextDose.Minutes} dakika";
                                        htmlBuilder.AppendLine($"<td style='padding: 8px;'>{geriSayim}</td>");
                                    }
                                    else
                                    {
                                        string? value = reader[column]?.ToString();
                                        if (column == "IcParazit" || column == "DisParazit")
                                            value = value == "1" ? "Yapıldı" : "Yapılmadı";
                                        htmlBuilder.AppendLine($"<td style='padding: 8px;'>{value ?? "N/A"}</td>");
                                    }
                                }
                                htmlBuilder.AppendLine("</tr>");
                            }
                        }
                    }
                }

                htmlBuilder.AppendLine("</table>");

                if (!hasRecords)
                    htmlBuilder.AppendLine("<p><b>Uyarı:</b> Geri sayımı 3 gün ve altında olan kayıt yok.</p>");

                htmlBuilder.AppendLine("<br><p>Sağlıklı günler,<br>Veteriner Otomasyon Ekibi</p>");
                htmlBuilder.AppendLine("</body></html>");

                return htmlBuilder.ToString();
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Tablo oluşturma hatası: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return $"<html><body><p>Tablo oluşturma hatası: {ex.Message}</p></body></html>";
            }
        }

        private async Task SaveDataGridViewChanges()
        {
            try
            {
                isSaving = true;
                timer.Stop();

                await InvokeAsync(() => dataGridView1.EndEdit());
                await semaphore.WaitAsync();
                try
                {
                    currentDataTable.AcceptChanges();
                }
                finally
                {
                    semaphore.Release();
                }

                if (sharedConnection == null)
                {
                    await InvokeAsync(() => MessageBox.Show("Veritabanı bağlantısı bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    return;
                }

                using (var transaction = sharedConnection.BeginTransaction())
                {
                    var deletedRows = originalDataTable.AsEnumerable()
                        .Where(origRow => origRow.RowState != DataRowState.Deleted &&
                                          (currentDataTable == null || !currentDataTable.AsEnumerable()
                                              .Any(currRow => currRow.RowState != DataRowState.Deleted &&
                                                              currRow["Id"].ToString() == origRow["Id"].ToString())))
                        .ToList();

                    foreach (var deletedRow in deletedRows)
                    {
                        string? id = deletedRow["Id"]?.ToString();
                        if (string.IsNullOrEmpty(id)) continue;

                        string deleteQuery = "DELETE FROM kopek_asi WHERE Id = @Id";
                        using (var cmd = new SQLiteCommand(deleteQuery, sharedConnection))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        await semaphore.WaitAsync();
                        try
                        {
                            deletedRow.Delete();
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }

                    var activeRows = currentDataTable.AsEnumerable()
                        .Where(row => row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                        .ToList();

                    foreach (var row in activeRows)
                    {
                        string? id = row["Id"]?.ToString();
                        if (string.IsNullOrEmpty(id)) continue;

                        string updateQuery = @"
                            UPDATE kopek_asi 
                            SET IsimSoyisim = @IsimSoyisim, Telefon = @Telefon, Cins = @Cins, AsiAdi = @AsiAdi,
                                AsiSuresi = @AsiSuresi, DozSayisi = @DozSayisi, IcParazit = @IcParazit,
                                DisParazit = @DisParazit, YapilmaDurumu = @YapilmaDurumu, YasAraligi = @YasAraligi,
                                Doz = @Doz, asisayisi = @asisayisi, Digerdoz = @Digerdoz, KayitTarihi = @KayitTarihi,
                                AsiSonTarihi = @AsiSonTarihi
                            WHERE Id = @Id";

                        using (var cmd = new SQLiteCommand(updateQuery, sharedConnection))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.Parameters.AddWithValue("@IsimSoyisim", row["IsimSoyisim"]?.ToString() ?? string.Empty);
                            cmd.Parameters.AddWithValue("@Telefon", row["Telefon"]?.ToString() ?? string.Empty);
                            cmd.Parameters.AddWithValue("@Cins", row["Cins"]?.ToString() ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AsiAdi", row["AsiAdi"]?.ToString() ?? string.Empty);
                            cmd.Parameters.AddWithValue("@AsiSuresi", row["AsiSuresi"]?.ToString() ?? "0");
                            cmd.Parameters.AddWithValue("@DozSayisi", Convert.ToInt32(row["DozSayisi"] ?? 1));
                            cmd.Parameters.AddWithValue("@IcParazit", row["IcParazit"]?.ToString() == "Yapıldı" ? 1 : 0);
                            cmd.Parameters.AddWithValue("@DisParazit", row["DisParazit"]?.ToString() == "Yapıldı" ? 1 : 0);
                            cmd.Parameters.AddWithValue("@YapilmaDurumu", row["YapilmaDurumu"]?.ToString() ?? string.Empty);
                            cmd.Parameters.AddWithValue("@YasAraligi", row["YasAraligi"]?.ToString() ?? string.Empty);
                            cmd.Parameters.AddWithValue("@Doz", Convert.ToInt32(row["Doz"] ?? 1));
                            cmd.Parameters.AddWithValue("@asisayisi", Convert.ToInt32(row["asisayisi"] ?? 1));
                            cmd.Parameters.AddWithValue("@Digerdoz", Convert.ToInt32(row["Digerdoz"] ?? 0));
                            cmd.Parameters.AddWithValue("@KayitTarihi", row["KayitTarihi"]?.ToString() ?? DateTime.Now.ToString("o"));
                            cmd.Parameters.AddWithValue("@AsiSonTarihi", row["AsiSonTarihi"]?.ToString() ?? string.Empty);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    await transaction.CommitAsync();
                    await UpdateDataTableAsync();
                }
            }
            catch (Exception ex)
            {
                await InvokeAsync(() => MessageBox.Show($"Değişiklikler kaydedilirken hata oluştu: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
            finally
            {
                isSaving = false;
                timer.Start();
            }
        }

        private async Task InvokeAsync(Action action)
        {
            if (InvokeRequired)
            {
                await Task.Run(() => Invoke(action));
            }
            else
            {
                action();
            }
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form13 form13 = Application.OpenForms["Form13"] as Form13 ?? new Form13();
                form13.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form13 açılırken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object? sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form13 form13 = Application.OpenForms["Form13"] as Form13 ?? new Form13();
                form13.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form13 açılırken hata: {ex.Message}\nStackTrace: {ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}