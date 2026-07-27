using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WinFormsApp8
{
    public partial class Form13 : Form
    {
        private readonly string dbPath = @"C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db";
        private readonly string connectionString;
        private SQLiteConnection? sharedConnection;
        private DataTable? dataTable;
        private System.Windows.Forms.Timer countdownTimer;
        private NotifyIcon? notifyIcon;
        private Dictionary<string, DateTime>? lastEmailSentTimes;
        private List<int> deletedRowIds; // To track deleted rows for saving to database

        public Form13()
        {
            InitializeComponent();
            connectionString = $"Data Source={dbPath};Version=3;";
            deletedRowIds = new List<int>(); // Initialize the list for tracking deleted rows
            InitializeSharedConnection();
            EnsureDatabaseSchema();
            InitializeDataGridView();
            SetupRadioButtonEvents();
            SetupTextBoxEvents();
            LoadDataToGridView(); // Load all data initially
            if (textBox5 != null)
            {
                textBox5.PlaceholderText = "Telefon numarasına göre filtrele (örneğin: 5551234567)";
                textBox5.TextChanged += TextBox5_TextChanged; // Add event handler for filtering
            }

            countdownTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            countdownTimer.Tick += CountdownTimer_Tick;

            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true
            };
            notifyIcon.BalloonTipClicked += (s, e) => notifyIcon.Visible = false;

            lastEmailSentTimes = new Dictionary<string, DateTime>();
        }

        private void TextBox5_TextChanged(object sender, EventArgs e)
        {
            string filterText = textBox5.Text.Trim();
            LoadDataToGridView(filterText); // Reload data with the filter applied
        }

        private void InitializeSharedConnection()
        {
            try
            {
                sharedConnection = new SQLiteConnection(connectionString);
                sharedConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand("PRAGMA journal_mode=WAL;", sharedConnection))
                {
                    command.ExecuteNonQuery();
                }
                File.AppendAllText("debug.log", $"{DateTime.Now}: Shared connection opened and WAL mode enabled\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bağlantı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: Connection Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        private void EnsureDatabaseSchema()
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
                        Adres TEXT,
                        GebelikDurumu TEXT
                    );";
                using (SQLiteCommand command = new SQLiteCommand(createHayvanKayitQuery, sharedConnection))
                {
                    command.ExecuteNonQuery();
                    File.AppendAllText("debug.log", $"{DateTime.Now}: Ensured HayvanKayit table exists\n");
                }

                string createMailQuery = @"
                    CREATE TABLE IF NOT EXISTS mail (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        email TEXT NOT NULL,
                        password TEXT NOT NULL
                    );";
                using (SQLiteCommand command = new SQLiteCommand(createMailQuery, sharedConnection))
                {
                    command.ExecuteNonQuery();
                    File.AppendAllText("debug.log", $"{DateTime.Now}: Ensured mail table exists\n");
                }

                string createKediAsiQuery = @"
                    CREATE TABLE IF NOT EXISTS kedi_asi (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Telefon TEXT NOT NULL,
                        Cins TEXT,
                        AsiAdi TEXT,
                        Durum TEXT CHECK(Durum IN ('Yapildi', 'Yapilmadi')),
                        Yas TEXT CHECK(Yas IN ('6-8 HAFTA', '9-11 HAFTA', '12-14 HAFTA', '13-15 HAFTA', '4-6 AYLIK', '12 AYLIK (1 YIL)')),
                        AsiSuresi TEXT,
                        Doz INTEGER,
                        KalanDoz INTEGER,
                        Sure TEXT,
                        LastDoseDate TEXT,
                        AsiSonTarihi TEXT,
                        KayitTarihi TEXT,
                        FOREIGN KEY(Telefon) REFERENCES HayvanKayit(Telefon)
                    );";
                using (SQLiteCommand command = new SQLiteCommand(createKediAsiQuery, sharedConnection))
                {
                    command.ExecuteNonQuery();
                    File.AppendAllText("debug.log", $"{DateTime.Now}: Ensured kedi_asi table exists\n");
                }

                string checkColumnQuery = "PRAGMA table_info(kedi_asi);";
                bool lastDoseDateExists = false, asiSonTarihiExists = false, kayitTarihiExists = false;
                using (SQLiteCommand command = new SQLiteCommand(checkColumnQuery, sharedConnection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["name"].ToString() ?? string.Empty;
                            if (columnName == "LastDoseDate") lastDoseDateExists = true;
                            if (columnName == "AsiSonTarihi") asiSonTarihiExists = true;
                            if (columnName == "KayitTarihi") kayitTarihiExists = true;
                        }
                    }
                }

                if (!lastDoseDateExists)
                {
                    string alterTableQuery = "ALTER TABLE kedi_asi ADD COLUMN LastDoseDate TEXT;";
                    using (SQLiteCommand command = new SQLiteCommand(alterTableQuery, sharedConnection))
                    {
                        command.ExecuteNonQuery();
                        File.AppendAllText("debug.log", $"{DateTime.Now}: Added LastDoseDate column to kedi_asi table\n");
                    }
                }

                if (!asiSonTarihiExists)
                {
                    string alterTableQuery = "ALTER TABLE kedi_asi ADD COLUMN AsiSonTarihi TEXT;";
                    using (SQLiteCommand command = new SQLiteCommand(alterTableQuery, sharedConnection))
                    {
                        command.ExecuteNonQuery();
                        File.AppendAllText("debug.log", $"{DateTime.Now}: Added AsiSonTarihi column to kedi_asi table\n");
                    }
                }

                if (!kayitTarihiExists)
                {
                    string alterTableQuery = "ALTER TABLE kedi_asi ADD COLUMN KayitTarihi TEXT;";
                    using (SQLiteCommand command = new SQLiteCommand(alterTableQuery, sharedConnection))
                    {
                        command.ExecuteNonQuery();
                        File.AppendAllText("debug.log", $"{DateTime.Now}: Added KayitTarihi column to kedi_asi table\n");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şema güncelleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: Schema Update Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
            }
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            if (sharedConnection != null && sharedConnection.State == ConnectionState.Open)
            {
                sharedConnection.Close();
                sharedConnection.Dispose();
                File.AppendAllText("debug.log", $"{DateTime.Now}: Shared connection closed\n");
            }
        }

        private void SetupRadioButtonEvents()
        {
            if (radioButton7 != null)
                radioButton7.CheckedChanged += (s, e) => { if (radioButton7.Checked && radioButton8 != null) radioButton8.Checked = false; };
            if (radioButton8 != null)
                radioButton8.CheckedChanged += (s, e) => { if (radioButton8.Checked && radioButton7 != null) radioButton7.Checked = false; };
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
        }

        private void InitializeDataGridView()
        {
            if (dataGridView1 == null) return;

            dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Telefon", typeof(string));
            dataTable.Columns.Add("Cins", typeof(string));
            dataTable.Columns.Add("AsiAdi", typeof(string));
            dataTable.Columns.Add("Durum", typeof(string));
            dataTable.Columns.Add("Yas", typeof(string));
            dataTable.Columns.Add("AsiSuresi", typeof(string));
            dataTable.Columns.Add("Doz", typeof(int));
            dataTable.Columns.Add("KalanDoz", typeof(int));
            dataTable.Columns.Add("Sure", typeof(string));
            dataTable.Columns.Add("LastDoseDate", typeof(string));
            dataTable.Columns.Add("AsiSonTarihi", typeof(string));
            dataTable.Columns.Add("KayitTarihi", typeof(string));
            dataTable.Columns.Add("NextDoseCountdown", typeof(string));
            dataTable.Columns.Add("Adres", typeof(string));

            dataGridView1.DataSource = dataTable;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dataGridView1.Columns["ID"].HeaderText = "KupeNo";
            dataGridView1.Columns["Cins"].HeaderText = "Cinsi";
            dataGridView1.Columns["AsiAdi"].HeaderText = "AşıAdı";
            dataGridView1.Columns["AsiSuresi"].HeaderText = "Periyot";
            dataGridView1.Columns["Durum"].HeaderText = "Durum";
            dataGridView1.Columns["KayitTarihi"].HeaderText = "KayıtTarihi";
            dataGridView1.Columns["Doz"].HeaderText = "Doz Numarası";
            dataGridView1.Columns["NextDoseCountdown"].HeaderText = "Kalan Süre";
            dataGridView1.Columns["LastDoseDate"].HeaderText = "Son Doz Tarihi";

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

            dataGridView1.Columns["Durum"].DefaultCellStyle.BackColor = Color.LightGray;

            // Allow editing, but protect certain columns
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns["ID"].ReadOnly = true;
            dataGridView1.Columns["LastDoseDate"].ReadOnly = true;
            dataGridView1.Columns["AsiSonTarihi"].ReadOnly = true;
            dataGridView1.Columns["KayitTarihi"].ReadOnly = true;
            dataGridView1.Columns["NextDoseCountdown"].ReadOnly = true;
            dataGridView1.Columns["Adres"].ReadOnly = true;

            // Add key press event for deleting rows
            dataGridView1.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete && dataGridView1.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            int id = Convert.ToInt32(row.Cells["ID"].Value);
                            deletedRowIds.Add(id); // Track the ID for deletion
                            dataGridView1.Rows.Remove(row);
                        }
                    }
                }
            };
        }

        private void LoadDataToGridView(string filterText = "")
        {
            if (sharedConnection == null || dataTable == null) return;

            try
            {
                string query = @"
                    SELECT k.ID, k.Telefon, k.Cins, k.AsiAdi, k.Durum, 
                           k.Yas, k.AsiSuresi, k.Doz, k.KalanDoz, k.Sure, k.LastDoseDate, k.AsiSonTarihi, k.KayitTarihi, 
                           h.Adres
                    FROM kedi_asi k
                    LEFT JOIN HayvanKayit h ON k.Telefon = h.Telefon";

                // Add WHERE clause for filtering by Telefon if filterText is not empty
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    query += " WHERE k.Telefon LIKE @Telefon";
                }

                using (SQLiteCommand command = new SQLiteCommand(query, sharedConnection))
                {
                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        command.Parameters.AddWithValue("@Telefon", $"%{filterText}%"); // Case-insensitive partial match
                    }

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        dataTable.Clear();
                        while (reader.Read())
                        {
                            object lastDoseDateValue = reader["LastDoseDate"] != DBNull.Value ? reader["LastDoseDate"] : DBNull.Value;
                            object asiSonTarihiValue = reader["AsiSonTarihi"] != DBNull.Value ? reader["AsiSonTarihi"] : DBNull.Value;
                            object kayitTarihiValue = reader["KayitTarihi"] != DBNull.Value ? reader["KayitTarihi"] : DBNull.Value;
                            dataTable.Rows.Add(
                                reader["ID"],
                                reader["Telefon"],
                                reader["Cins"],
                                reader["AsiAdi"],
                                reader["Durum"],
                                reader["Yas"],
                                reader["AsiSuresi"],
                                reader["Doz"],
                                reader["KalanDoz"],
                                reader["Sure"],
                                lastDoseDateValue,
                                asiSonTarihiValue,
                                kayitTarihiValue,
                                "Calculating...",
                                reader["Adres"]
                            );
                        }
                    }
                }
                deletedRowIds.Clear(); // Clear the deleted rows list after reloading
                File.AppendAllText("debug.log", $"{DateTime.Now}: Data loaded into DataGridView from kedi_asi with filter '{filterText}'\n");
                UpdateCountdowns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: LoadDataToGridView Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        private bool TelefonExists(string telefon)
        {
            if (sharedConnection == null) return false;

            try
            {
                string query = "SELECT COUNT(*) FROM HayvanKayit WHERE Telefon = @Telefon";
                using (SQLiteCommand command = new SQLiteCommand(query, sharedConnection))
                {
                    command.Parameters.AddWithValue("@Telefon", telefon);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.log", $"{DateTime.Now}: TelefonExists Error - {ex.Message}\n{ex.StackTrace}\n");
                return false;
            }
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            UpdateCountdowns();
        }

        private async void UpdateCountdowns()
        {
            if (dataTable == null || dataGridView1 == null || dataGridView1.Rows.Count == 0) return;

            if (lastEmailSentTimes == null)
            {
                lastEmailSentTimes = new Dictionary<string, DateTime>();
                File.AppendAllText("debug.log", $"{DateTime.Now}: lastEmailSentTimes was null, reinitialized in UpdateCountdowns\n");
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row if it exists

                try
                {
                    int kalanDoz = row.Cells["KalanDoz"].Value != null ? Convert.ToInt32(row.Cells["KalanDoz"].Value) : 0;
                    if (kalanDoz <= 0)
                    {
                        row.Cells["NextDoseCountdown"].Value = "Dozlar tamamlandı";
                        row.Cells["Durum"].Style.BackColor = Color.FromArgb(144, 238, 144);
                        continue;
                    }

                    string? asiSonTarihiStr = row.Cells["AsiSonTarihi"].Value?.ToString();
                    if (string.IsNullOrEmpty(asiSonTarihiStr) || !DateTime.TryParse(asiSonTarihiStr, out DateTime asiSonTarihi))
                    {
                        row.Cells["NextDoseCountdown"].Value = "Tarih hatalı";
                        row.Cells["Durum"].Style.BackColor = Color.LightGray;
                        continue;
                    }

                    double sureDays;
                    if (row.Cells["Sure"].Value == null || !double.TryParse(row.Cells["Sure"].Value?.ToString(), out sureDays))
                    {
                        int asiSuresi = Convert.ToInt32(row.Cells["AsiSuresi"].Value);
                        sureDays = kalanDoz > 0 ? asiSuresi / (double)kalanDoz : 0;
                        row.Cells["Sure"].Value = sureDays.ToString("F2");
                    }

                    if (sureDays <= 0)
                    {
                        row.Cells["NextDoseCountdown"].Value = "Geçersiz süre";
                        row.Cells["Durum"].Style.BackColor = Color.LightGray;
                        continue;
                    }

                    TimeSpan interval = TimeSpan.FromDays(sureDays);
                    DateTime nextDoseDate = asiSonTarihi.Add(interval);
                    TimeSpan timeRemaining = nextDoseDate - DateTime.Now;

                    if (timeRemaining.TotalSeconds <= 0)
                    {
                        row.Cells["AsiSonTarihi"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string updateQuery = "UPDATE kedi_asi SET AsiSonTarihi = @AsiSonTarihi WHERE ID = @ID";
                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, sharedConnection))
                        {
                            command.Parameters.AddWithValue("@AsiSonTarihi", row.Cells["AsiSonTarihi"].Value);
                            command.Parameters.AddWithValue("@ID", row.Cells["ID"].Value);
                            command.ExecuteNonQuery();
                        }
                    }

                    if (timeRemaining.TotalSeconds <= 0)
                    {
                        row.Cells["NextDoseCountdown"].Value = "Doz zamanı geçti";
                        row.Cells["Durum"].Style.BackColor = Color.FromArgb(255, 99, 71);
                    }
                    else
                    {
                        row.Cells["NextDoseCountdown"].Value =
                            $"{timeRemaining.Days} gün {timeRemaining.Hours:D2} saat " +
                            $"{timeRemaining.Minutes:D2} dakika {timeRemaining.Seconds:D2} saniye";

                        if (timeRemaining.Days <= 5)
                        {
                            row.Cells["Durum"].Style.BackColor = Color.FromArgb(240, 128, 128);
                            string? id = row.Cells["ID"].Value?.ToString();
                            if (!string.IsNullOrEmpty(id) &&
                                (!lastEmailSentTimes.ContainsKey(id) || (DateTime.Now - lastEmailSentTimes[id]).TotalHours >= 24))
                            {
                                await SendEmailNotification(row);
                                if (lastEmailSentTimes.ContainsKey(id))
                                    lastEmailSentTimes[id] = DateTime.Now;
                                else
                                    lastEmailSentTimes.Add(id, DateTime.Now);
                            }
                        }
                        else
                        {
                            row.Cells["Durum"].Style.BackColor = Color.FromArgb(144, 238, 144);
                        }
                    }
                }
                catch (Exception ex)
                {
                    row.Cells["NextDoseCountdown"].Value = "Hata";
                    row.Cells["Durum"].Style.BackColor = Color.LightGray;
                    File.AppendAllText("debug.log", $"{DateTime.Now}: Countdown Error - {ex.Message}\n{ex.StackTrace}\n");
                }
            }

            UpdateDataGridViewRowColors(false);
        }

        private async Task SendEmailNotification(DataGridViewRow row)
        {
            try
            {
                if (lastEmailSentTimes == null)
                {
                    lastEmailSentTimes = new Dictionary<string, DateTime>();
                    File.AppendAllText("debug.log", $"{DateTime.Now}: lastEmailSentTimes was null, reinitialized in SendEmailNotification\n");
                }

                string? gondericiEmail = null;
                string? uygulamaSifresi = null;
                string? aliciEmail = null;

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT email, password FROM mail LIMIT 1";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    using (SQLiteDataReader reader = (SQLiteDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            gondericiEmail = reader["email"]?.ToString()?.Trim();
                            uygulamaSifresi = reader["password"]?.ToString()?.Trim();
                            aliciEmail = gondericiEmail;
                        }
                    }
                }

                if (string.IsNullOrEmpty(gondericiEmail) || string.IsNullOrEmpty(uygulamaSifresi) || string.IsNullOrEmpty(aliciEmail))
                {
                    ShowSystemTrayNotification("Hata", "E-posta, şifre veya alıcı e-posta veritabanından çekilemedi!", ToolTipIcon.Error);
                    File.AppendAllText("debug.log", $"{DateTime.Now}: SendEmailNotification Error - Email credentials missing\n");
                    return;
                }

                if (!gondericiEmail.Contains("@") || !aliciEmail.Contains("@"))
                {
                    ShowSystemTrayNotification("Hata", "Geçerli bir e-posta adresi bulunamadı!", ToolTipIcon.Error);
                    File.AppendAllText("debug.log", $"{DateTime.Now}: SendEmailNotification Error - Invalid email address\n");
                    return;
                }

                try
                {
                    using (var client = new System.Net.NetworkInformation.Ping())
                    {
                        var reply = client.Send("smtp.gmail.com", 1000);
                        if (reply.Status != System.Net.NetworkInformation.IPStatus.Success)
                        {
                            throw new Exception("SMTP sunucusuna bağlanılamadı. İnternet bağlantınızı kontrol edin.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowSystemTrayNotification("Hata", $"Ağ bağlantı hatası: {ex.Message}", ToolTipIcon.Error);
                    File.AppendAllText("debug.log", $"{DateTime.Now}: SendEmailNotification Network Error - {ex.Message}\n{ex.StackTrace}\n");
                    return;
                }

                int doz = Convert.ToInt32(row.Cells["Doz"].Value);
                int kalanDoz = Convert.ToInt32(row.Cells["KalanDoz"].Value);
                int nextDoseNumber = doz - kalanDoz + 1;

                string? asiSonTarihiStr = row.Cells["AsiSonTarihi"].Value?.ToString();
                if (!DateTime.TryParse(asiSonTarihiStr, out DateTime asiSonTarihi))
                {
                    File.AppendAllText("debug.log", $"{DateTime.Now}: SendEmailNotification Error - Invalid AsiSonTarihi format\n");
                    return;
                }

                if (!double.TryParse(row.Cells["Sure"].Value?.ToString(), out double sureDays))
                {
                    File.AppendAllText("debug.log", $"{DateTime.Now}: SendEmailNotification Error - Invalid Sure format\n");
                    return;
                }

                DateTime nextDoseDueDate = asiSonTarihi.AddDays(sureDays);
                string formattedDueDate = nextDoseDueDate.ToString("yyyy-MM-dd HH:mm:ss");

                string emailBody = $"Sayın Müşteri,\n\n" +
                                   $"Bu e-posta, {row.Cells["Cins"].Value} cinsindeki kediniz için bir aşı dozu hatırlatmasıdır. " +
                                   $"Kedinizin {row.Cells["AsiAdi"].Value} aşısının {nextDoseNumber}. dozu yaklaşmaktadır. " +
                                   $"Bir sonraki dozun uygulanması gereken tarih {formattedDueDate}'tir ve şu anda son {row.Cells["NextDoseCountdown"].Value} kalmıştır. " +
                                   $"Lütfen bu tarihte veteriner kliniğimize uğrayarak aşınızı yaptırınız. İletişim bilgileriniz: Telefon: {row.Cells["Telefon"].Value}, Adres: {row.Cells["Adres"].Value ?? "Belirtilmemiş"}. " +
                                   $"Herhangi bir sorunuz olursa bizimle iletişime geçmekten çekinmeyin.\n\n" +
                                   $"Saygılarımızla,\nVeteriner Kliniği Ekibi";

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential(gondericiEmail, uygulamaSifresi);
                    smtpClient.EnableSsl = true;
                    smtpClient.Timeout = 10000;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(gondericiEmail);
                        mailMessage.To.Add(aliciEmail);
                        mailMessage.Subject = $"Aşı Dozu Hatırlatma - ID: {row.Cells["ID"].Value}";
                        mailMessage.Body = emailBody;
                        mailMessage.IsBodyHtml = false;

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                MessageBox.Show($"ID {row.Cells["ID"].Value} için e-posta başarıyla gönderildi.\nSonraki doz kalan süre: {row.Cells["NextDoseCountdown"].Value}",
                    "E-posta Gönderildi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ShowSystemTrayNotification("Bilgilendirme",
                    $"ID {row.Cells["ID"].Value} için e-posta gönderildi. Sonraki doz kalan süre: {row.Cells["NextDoseCountdown"].Value}",
                    ToolTipIcon.Info);
                File.AppendAllText("debug.log", $"{DateTime.Now}: Email sent successfully for ID {row.Cells["ID"].Value}\n");

                PlayNotificationSound("C:\\Users\\ridva\\Desktop\\dutdut\\my-project\\veteriner_otomasyon\\inek.wav");
            }
            catch (SmtpException smtpEx)
            {
                ShowSystemTrayNotification("Hata", $"E-posta gönderilirken SMTP hatası oluştu: {smtpEx.Message}", ToolTipIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: SendEmailNotification SMTP Error - {smtpEx.Message}\nStatusCode: {smtpEx.StatusCode}\nInnerException: {smtpEx.InnerException?.Message}\n{smtpEx.StackTrace}\n");
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"E-posta gönderilirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: SendEmailNotification Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        private void ShowSystemTrayNotification(string title, string message, ToolTipIcon icon)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = true;
                notifyIcon.BalloonTipTitle = title;
                notifyIcon.BalloonTipText = message;
                notifyIcon.BalloonTipIcon = icon;
                notifyIcon.ShowBalloonTip(3000);
            }
        }

        private void PlayNotificationSound(string filePath)
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer(filePath))
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Ses Hatası", $"Ses çalınırken hata oluştu: {ex.Message}", ToolTipIcon.Warning);
                File.AppendAllText("debug.log", $"{DateTime.Now}: PlayNotificationSound Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        private void UpdateDataGridViewRowColors(bool preserveColors)
        {
            if (dataGridView1 == null || dataGridView1.Rows.Count == 0) return;

            bool hasColoredFirstDoneRow = false;

            foreach (DataGridViewRow gridRow in dataGridView1.Rows)
            {
                if (gridRow.IsNewRow || gridRow.DataBoundItem is not DataRowView rowView || rowView.Row.RowState == DataRowState.Deleted) continue;

                if (!preserveColors)
                {
                    foreach (DataGridViewCell cell in gridRow.Cells)
                    {
                        if (cell.OwningColumn.Name != "Durum")
                        {
                            cell.Style.BackColor = Color.White;
                            cell.Style.ForeColor = Color.Black;
                        }
                    }

                    string? durum = rowView.Row["Durum"]?.ToString();

                    if (durum == "Yapildi" && !hasColoredFirstDoneRow)
                    {
                        foreach (DataGridViewCell cell in gridRow.Cells)
                        {
                            if (cell.OwningColumn.Name != "Durum")
                            {
                                cell.Style.BackColor = Color.FromArgb(144, 238, 144);
                            }
                        }
                        hasColoredFirstDoneRow = true;
                    }
                    else if (durum == "Yapilmadi")
                    {
                        foreach (DataGridViewCell cell in gridRow.Cells)
                        {
                            if (cell.OwningColumn.Name != "Durum")
                            {
                                cell.Style.BackColor = Color.White;
                            }
                        }
                    }

                    if (rowView.Row["AsiSonTarihi"] != DBNull.Value && DateTime.TryParse(rowView.Row["AsiSonTarihi"]?.ToString(), out DateTime asiSonTarihi))
                    {
                        if (double.TryParse(rowView.Row["Sure"]?.ToString(), out double sureDays) && sureDays > 0)
                        {
                            TimeSpan interval = TimeSpan.FromDays(sureDays);
                            DateTime nextDoseDate = asiSonTarihi.Add(interval);
                            TimeSpan remainingTime = nextDoseDate - DateTime.Now;

                            if (remainingTime.TotalSeconds <= 0)
                            {
                                foreach (DataGridViewCell cell in gridRow.Cells)
                                {
                                    if (cell.OwningColumn.Name != "Durum")
                                    {
                                        cell.Style.BackColor = Color.FromArgb(255, 99, 71);
                                        cell.Style.ForeColor = Color.White;
                                    }
                                }
                            }
                            else if (remainingTime.TotalDays <= 15)
                            {
                                foreach (DataGridViewCell cell in gridRow.Cells)
                                {
                                    if (cell.OwningColumn.Name != "Durum")
                                    {
                                        cell.Style.BackColor = Color.FromArgb(240, 128, 128);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            countdownTimer.Start();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form15 form15 = Application.OpenForms["Form15"] as Form15 ?? new Form15();
                form15.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form geçişi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: pictureBox3_Click Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                Form14 form = new Form14();
                form.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form geçişi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: pictureBox1_Click Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (sharedConnection == null || dataTable == null)
            {
                MessageBox.Show("Veritabanı bağlantısı veya veri tablosu başlatılamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Lütfen zorunlu alanları doldurun (Telefon, Cins, Aşı Adı).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox2.Text, out int asiSuresi) || asiSuresi <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir aşı süresi (pozitif tam sayı gün cinsinden) girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox7.Text, out int doz) || doz <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir doz numarası (pozitif tam sayı) girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int kalanDoz = doz - 1;
            double sureDays = kalanDoz > 0 ? asiSuresi / (double)kalanDoz : 0;
            string sure = sureDays.ToString("F2");
            string kayitTarihi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string asiSonTarihi = kayitTarihi;

            string durum = radioButton7.Checked ? "Yapildi" : radioButton8.Checked ? "Yapilmadi" : string.Empty;
            if (string.IsNullOrEmpty(durum))
            {
                MessageBox.Show("Lütfen bir durum seçin (Yapıldı/Yapılmadı).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string yas = string.Empty;
            if (radioButton1.Checked) yas = "6-8 HAFTA";
            else if (radioButton2.Checked) yas = "9-11 HAFTA";
            else if (radioButton3.Checked) yas = "12-14 HAFTA";
            else if (radioButton4.Checked) yas = "13-15 HAFTA";
            else if (radioButton5.Checked) yas = "4-6 AYLIK";
            else if (radioButton6.Checked) yas = "12 AYLIK (1 YIL)";
            else
            {
                MessageBox.Show("Lütfen bir yaş aralığı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                long insertedId;
                using (var transaction = sharedConnection.BeginTransaction())
                {
                    if (!TelefonExists(textBox3.Text))
                    {
                        string hayvanKayitQuery = "INSERT INTO HayvanKayit (HayvanSahibi, Telefon, Cinsi) VALUES (@HayvanSahibi, @Telefon, @Cinsi)";
                        using (SQLiteCommand command = new SQLiteCommand(hayvanKayitQuery, sharedConnection))
                        {
                            command.Parameters.AddWithValue("@HayvanSahibi", textBox1.Text ?? "Bilinmiyor");
                            command.Parameters.AddWithValue("@Telefon", textBox3.Text);
                            command.Parameters.AddWithValue("@Cinsi", textBox4.Text);
                            command.ExecuteNonQuery();
                        }
                        File.AppendAllText("debug.log", $"{DateTime.Now}: Inserted into HayvanKayit\n");
                    }

                    string kediAsiQuery = @"
                        INSERT INTO kedi_asi 
                        (Telefon, Cins, AsiAdi, Durum, Yas, AsiSuresi, Doz, KalanDoz, Sure, LastDoseDate, AsiSonTarihi, KayitTarihi) 
                        VALUES (@Telefon, @Cins, @AsiAdi, @Durum, @Yas, @AsiSuresi, @Doz, @KalanDoz, @Sure, @LastDoseDate, @AsiSonTarihi, @KayitTarihi);
                        SELECT last_insert_rowid();";
                    using (SQLiteCommand command = new SQLiteCommand(kediAsiQuery, sharedConnection))
                    {
                        command.Parameters.AddWithValue("@Telefon", textBox3.Text);
                        command.Parameters.AddWithValue("@Cins", textBox4.Text);
                        command.Parameters.AddWithValue("@AsiAdi", textBox6.Text);
                        command.Parameters.AddWithValue("@Durum", durum);
                        command.Parameters.AddWithValue("@Yas", yas);
                        command.Parameters.AddWithValue("@AsiSuresi", asiSuresi.ToString());
                        command.Parameters.AddWithValue("@Doz", doz);
                        command.Parameters.AddWithValue("@KalanDoz", kalanDoz);
                        command.Parameters.AddWithValue("@Sure", sure);
                        command.Parameters.AddWithValue("@LastDoseDate", asiSonTarihi);
                        command.Parameters.AddWithValue("@AsiSonTarihi", asiSonTarihi);
                        command.Parameters.AddWithValue("@KayitTarihi", kayitTarihi);
                        insertedId = (long)command.ExecuteScalar();
                    }

                    transaction.Commit();
                    File.AppendAllText("debug.log", $"{DateTime.Now}: Transaction committed successfully\n");
                }

                dataTable.Rows.Add(
                    insertedId,
                    textBox3.Text,
                    textBox4.Text,
                    textBox6.Text,
                    durum,
                    yas,
                    asiSuresi.ToString(),
                    doz,
                    kalanDoz,
                    sure,
                    asiSonTarihi,
                    asiSonTarihi,
                    kayitTarihi,
                    "Calculating...",
                    DBNull.Value
                );

                MessageBox.Show("Kayıt başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                File.AppendAllText("debug.log", $"{DateTime.Now}: Record saved successfully\n");

                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                textBox3.Text = string.Empty;
                textBox4.Text = string.Empty;
                textBox5.Text = string.Empty;
                textBox6.Text = string.Empty;
                textBox7.Text = string.Empty;
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;

                LoadDataToGridView(); // Reload without filter to show all records after adding
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt ekleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: pictureBox5_Click Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (sharedConnection == null || dataTable == null)
            {
                MessageBox.Show("Veritabanı bağlantısı veya veri tablosu başlatılamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var transaction = sharedConnection.BeginTransaction())
                {
                    // Step 1: Handle deletions
                    foreach (int id in deletedRowIds)
                    {
                        string deleteQuery = "DELETE FROM kedi_asi WHERE ID = @ID";
                        using (SQLiteCommand command = new SQLiteCommand(deleteQuery, sharedConnection))
                        {
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();
                        }
                        File.AppendAllText("debug.log", $"{DateTime.Now}: Deleted record with ID {id} from kedi_asi\n");
                    }
                    deletedRowIds.Clear(); // Clear the list after deletion

                    // Step 2: Handle updates
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue; // Skip the new row if it exists

                        int id = Convert.ToInt32(row.Cells["ID"].Value);
                        string telefon = row.Cells["Telefon"].Value?.ToString() ?? string.Empty;
                        string cins = row.Cells["Cins"].Value?.ToString() ?? string.Empty;
                        string asiAdi = row.Cells["AsiAdi"].Value?.ToString() ?? string.Empty;
                        string durum = row.Cells["Durum"].Value?.ToString() ?? "Yapilmadi";
                        string yas = row.Cells["Yas"].Value?.ToString() ?? string.Empty;
                        string asiSuresi = row.Cells["AsiSuresi"].Value?.ToString() ?? "0";
                        int doz = Convert.ToInt32(row.Cells["Doz"].Value);
                        int kalanDoz = Convert.ToInt32(row.Cells["KalanDoz"].Value);
                        string sure = row.Cells["Sure"].Value?.ToString() ?? "0";

                        // Validate data before updating
                        if (string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(cins) || string.IsNullOrWhiteSpace(asiAdi))
                        {
                            MessageBox.Show($"ID {id} için zorunlu alanlar (Telefon, Cins, Aşı Adı) eksik.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                        if (!int.TryParse(asiSuresi, out int asiSuresiInt) || asiSuresiInt <= 0)
                        {
                            MessageBox.Show($"ID {id} için geçerli bir aşı süresi (pozitif tam sayı) girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                        if (doz <= 0)
                        {
                            MessageBox.Show($"ID {id} için geçerli bir doz numarası (pozitif tam sayı) girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                        if (!new[] { "Yapildi", "Yapilmadi" }.Contains(durum))
                        {
                            MessageBox.Show($"ID {id} için geçerli bir durum seçin (Yapildi/Yapilmadi).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                        if (!new[] { "6-8 HAFTA", "9-11 HAFTA", "12-14 HAFTA", "13-15 HAFTA", "4-6 AYLIK", "12 AYLIK (1 YIL)" }.Contains(yas))
                        {
                            MessageBox.Show($"ID {id} için geçerli bir yaş aralığı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                        string updateQuery = @"
                            UPDATE kedi_asi 
                            SET Telefon = @Telefon, Cins = @Cins, AsiAdi = @AsiAdi, Durum = @Durum, Yas = @Yas, 
                                AsiSuresi = @AsiSuresi, Doz = @Doz, KalanDoz = @KalanDoz, Sure = @Sure 
                            WHERE ID = @ID";
                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, sharedConnection))
                        {
                            command.Parameters.AddWithValue("@Telefon", telefon);
                            command.Parameters.AddWithValue("@Cins", cins);
                            command.Parameters.AddWithValue("@AsiAdi", asiAdi);
                            command.Parameters.AddWithValue("@Durum", durum);
                            command.Parameters.AddWithValue("@Yas", yas);
                            command.Parameters.AddWithValue("@AsiSuresi", asiSuresi);
                            command.Parameters.AddWithValue("@Doz", doz);
                            command.Parameters.AddWithValue("@KalanDoz", kalanDoz);
                            command.Parameters.AddWithValue("@Sure", sure);
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();
                        }
                        File.AppendAllText("debug.log", $"{DateTime.Now}: Updated record with ID {id} in kedi_asi\n");
                    }

                    transaction.Commit();
                    File.AppendAllText("debug.log", $"{DateTime.Now}: Changes saved successfully\n");
                    MessageBox.Show("Değişiklikler başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload the DataGridView with the current filter (if any)
                    string currentFilter = textBox5.Text.Trim();
                    LoadDataToGridView(currentFilter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Değişiklikleri kaydederken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("debug.log", $"{DateTime.Now}: pictureBox2_Click Error - {ex.Message}\n{ex.StackTrace}\n");
            }
        }
    }
}