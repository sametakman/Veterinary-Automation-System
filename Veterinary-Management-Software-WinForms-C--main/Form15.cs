using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Net.Mail;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WinFormsApp8
{
    public partial class Form15 : Form
    {
        private readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";
        private System.Windows.Forms.Timer? gridUpdateTimer;
        private System.Windows.Forms.Timer? emailTimer;
        private readonly HashSet<string> notifiedRecords = new HashSet<string>();

        public Form15()
        {
            InitializeComponent();
            CheckAndCreateDoseNumberColumn();
            LoadDataGridView();
            InitializeTimers();
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            textBox3.TextChanged += TextBox3_TextChanged;
            textBox5.TextChanged += TextBox5_TextChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                gridUpdateTimer?.Stop();
                gridUpdateTimer?.Dispose();
                emailTimer?.Stop();
                emailTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void CheckAndCreateDoseNumberColumn()
        {
            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                using var command = new SQLiteCommand("PRAGMA table_info(AsiBilgisi)", connection);
                using var reader = command.ExecuteReader();
                bool doseNumberExists = false;
                while (reader.Read())
                {
                    if (reader["name"]?.ToString()?.Equals("DoseNumber", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        doseNumberExists = true;
                        break;
                    }
                }
                if (!doseNumberExists)
                {
                    using var alterCommand = new SQLiteCommand("ALTER TABLE AsiBilgisi ADD DoseNumber INTEGER DEFAULT 1", connection);
                    alterCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DoseNumber sütunu kontrol edilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeTimers()
        {
            gridUpdateTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            gridUpdateTimer.Tick += GridUpdateTimer_Tick;
            gridUpdateTimer.Start();

            emailTimer = new System.Windows.Forms.Timer { Interval = 24 * 60 * 60 * 1000 }; // 24 hours
            emailTimer.Tick += EmailTimer_Tick;
            emailTimer.Start();
        }

        private void TextBox3_TextChanged(object? sender, EventArgs e)
        {
            string? input = textBox3.Text?.Trim();
            if (string.IsNullOrEmpty(input)) return;

            string? suggestedAsiAdi = SuggestAsiAdi(input);
            if (!string.IsNullOrEmpty(suggestedAsiAdi))
            {
                if (MessageBox.Show($"Bunu mu kastediyorsunuz: {suggestedAsiAdi}?", "Aşı Adı Önerisi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    textBox3.Text = suggestedAsiAdi;
                }
            }
        }

        private void TextBox5_TextChanged(object? sender, EventArgs e)
        {
            LoadDataGridView();
        }

        private void DataGridView1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.DataSource is DataTable dataTable)
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow && row.DataBoundItem is DataRowView rowView)
                            rowView.Row.Delete();
                    }
                    dataGridView1.Refresh();
                    e.Handled = true;
                }
            }
        }

        private string? SuggestAsiAdi(string input)
        {
            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                using var command = new SQLiteCommand("SELECT AşıAdı FROM AsiBilgisi WHERE AşıAdı LIKE @Input || '%'", connection);
                command.Parameters.AddWithValue("@Input", input);
                using var reader = command.ExecuteReader();
                return reader.Read() ? reader["AşıAdı"]?.ToString() : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aşı adı önerisi alınırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void GridUpdateTimer_Tick(object? sender, EventArgs e)
        {
            UpdateDataGridViewCountdown();
        }

        private void EmailTimer_Tick(object? sender, EventArgs e)
        {
            SendEmailsForKupeNos();
        }

        private void UpdateDataGridViewCountdown()
        {
            if (dataGridView1.DataSource is not DataTable dataTable) return;

            bool isTextBox6LessThan5 = int.TryParse(textBox6.Text, out int doseCount) && doseCount < 5;
            var messageBuilder = new StringBuilder();
            bool hasExpiredRecords = false;

            foreach (DataRow row in dataTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                if (row["DigerDoz"] != DBNull.Value && DateTime.TryParse(row["DigerDoz"]?.ToString(), out DateTime targetDate))
                {
                    TimeSpan remainingTime = targetDate - DateTime.Now;
                    string recordKey = $"{row["KupeNo"]}_{row["AşıAdı"]}_{row["DoseNumber"]}";

                    if (remainingTime.TotalSeconds <= 0)
                    {
                        row["DigerDozDisplay"] = "Aşı periyotu bitti!";
                        if (!notifiedRecords.Contains(recordKey))
                        {
                            messageBuilder.AppendLine($"⚠️ {row["KupeNo"]} küpe numaralı hayvanın {row["AşıAdı"]} aşısının {row["DoseNumber"]}. dozu bitmiştir!");
                            messageBuilder.AppendLine("Lütfen yeni bir aşı tarihi belirleyiniz.\n");
                            hasExpiredRecords = true;

                            string emailBody = $"Sayın Veteriner Hekim,\n\n" +
                                              $"{row["HayvanSahibi"]} adlı müşterimizin {row["KupeNo"]} küpe numaralı {row["Cinsi"]} cinsindeki hayvanının " +
                                              $"{row["AşıAdı"]} aşısının {row["DoseNumber"]}. dozu bitmiştir.\n" +
                                              $"Son aşı tarihi: {targetDate:dd.MM.yyyy HH:mm:ss}\n" +
                                              $"Lütfen yeni bir aşı tarihi belirleyiniz.\n\n" +
                                              $"İletişim Bilgileri:\nMüşteri: {row["HayvanSahibi"]}\nTelefon: {row["Telefon"]}\nAdres: {row["Adres"]}\n\n" +
                                              "Veteriner Otomasyon Sistemi";
                            SendEmailNotification(emailBody, "otomasyonveteriner@gmail.com");
                            notifiedRecords.Add(recordKey);
                        }
                    }
                    else if (remainingTime.TotalDays < 5 && !notifiedRecords.Contains(recordKey))
                    {
                        row["DigerDozDisplay"] = $"{remainingTime.Days:D2} Gün {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                        string emailBody = $"Sayın Veteriner Hekim,\n\n" +
                                          $"{row["HayvanSahibi"]} adlı müşterimizin {row["KupeNo"]} küpe numaralı {row["Cinsi"]} cinsindeki hayvanının " +
                                          $"{row["AşıAdı"]} aşısının {row["DoseNumber"]}. dozu için {remainingTime.Days} gün {remainingTime.Hours} saat kaldı.\n" +
                                          $"Son aşı tarihi: {targetDate:dd.MM.yyyy HH:mm:ss}\n" +
                                          $"Lütfen aşı takvimini kontrol ediniz.\n\n" +
                                          $"İletişim Bilgileri:\nMüşteri: {row["HayvanSahibi"]}\nTelefon: {row["Telefon"]}\nAdres: {row["Adres"]}\n\n" +
                                          "Veteriner Otomasyon Sistemi";
                        SendEmailNotification(emailBody, "otomasyonveteriner@gmail.com");
                        notifiedRecords.Add(recordKey);
                    }
                    else
                    {
                        row["DigerDozDisplay"] = $"{remainingTime.Days:D2} Gün {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                    }
                }
                else
                {
                    row["DigerDozDisplay"] = "";
                }
            }

            if (hasExpiredRecords)
            {
                MessageBox.Show(messageBuilder.ToString(), "Aşı Süresi Bildirimi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            UpdateDataGridViewRowColors(isTextBox6LessThan5);
            dataGridView1.Refresh();
        }

        private void SendEmailsForKupeNos()
        {
            if (dataGridView1.DataSource is not DataTable dataTable) return;

            var kupeNoGroups = new Dictionary<string, List<DataRow>>();
            foreach (DataRow row in dataTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                string? rowKupeNo = row["KupeNo"]?.ToString();
                if (string.IsNullOrEmpty(rowKupeNo)) continue;
                if (!kupeNoGroups.ContainsKey(rowKupeNo))
                    kupeNoGroups[rowKupeNo] = new List<DataRow>();
                kupeNoGroups[rowKupeNo].Add(row);
            }

            foreach (var (groupKupeNo, rows) in kupeNoGroups)
            {
                var emailBodyBuilder = new StringBuilder();
                string hayvanSahibi = "", telefon = "", adres = "", cinsi = "";
                bool shouldSendEmail = false;

                foreach (DataRow row in rows)
                {
                    if (row["DigerDoz"] != DBNull.Value && DateTime.TryParse(row["DigerDoz"]?.ToString(), out DateTime targetDate))
                    {
                        TimeSpan remainingTime = targetDate - DateTime.Now;
                        string recordKey = $"{groupKupeNo}_{row["AşıAdı"]}_{row["DoseNumber"]}";

                        if (remainingTime.TotalSeconds > 0 && remainingTime.TotalDays <= 15 && !notifiedRecords.Contains(recordKey))
                        {
                            string? rowAsiAdi = row["AşıAdı"]?.ToString();
                            hayvanSahibi = row["HayvanSahibi"]?.ToString() ?? "";
                            telefon = row["Telefon"]?.ToString() ?? "";
                            adres = row["Adres"]?.ToString() ?? "";
                            cinsi = row["Cinsi"]?.ToString() ?? "";

                            emailBodyBuilder.AppendLine($"• {rowAsiAdi} aşısının {row["DoseNumber"]}. dozu için {remainingTime.Days} gün {remainingTime.Hours} saat kaldı.");
                            shouldSendEmail = true;
                            notifiedRecords.Add(recordKey);
                        }
                    }
                }

                if (shouldSendEmail)
                {
                    string emailBody = $"Sayın Veteriner Hekim,\n\n" +
                                      $"{hayvanSahibi} adlı müşterimizin {groupKupeNo} küpe numaralı {cinsi} cinsindeki hayvanı için:\n\n" +
                                      emailBodyBuilder.ToString() +
                                      $"\nİletişim Bilgileri:\nMüşteri: {hayvanSahibi}\nTelefon: {telefon}\nAdres: {adres}\n\n" +
                                      "Veteriner Otomasyon Sistemi";
                    SendEmailNotification(emailBody, "otomasyonveteriner@gmail.com");
                }
            }
        }

        private void SendEmailNotification(string emailBody, string recipientEmail)
        {
            try
            {
                string senderEmail = "", appPassword = "";
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    using var cmd = new SQLiteCommand("SELECT email, password FROM mail LIMIT 1", conn);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        senderEmail = reader["email"]?.ToString()?.Trim() ?? "";
                        appPassword = reader["password"]?.ToString()?.Trim() ?? "";
                    }
                }

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
                {
                    MessageBox.Show("E-posta veya şifre veritabanından alınamadı.", "E-posta Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!senderEmail.Contains('@'))
                {
                    MessageBox.Show($"Geçersiz e-posta adresi: {senderEmail}", "E-posta Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential(senderEmail, appPassword),
                    EnableSsl = true,
                    Timeout = 10000
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Veteriner Otomasyon - Aşı Süresi Bildirimi",
                    Body = emailBody,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(recipientEmail);

                smtpClient.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show($"E-posta gönderimi başarısız: {ex.Message}", "E-posta Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"E-posta gönderimi sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadDataGridView()
        {
            string query = @"
                SELECT a.KupeNo, a.Cinsi, a.AşıAdı, a.Periyot, a.Durum, a.DigerDoz, a.KayitTarihi, a.DoseNumber, 
                       h.HayvanSahibi, h.Telefon, h.Adres 
                FROM AsiBilgisi a
                INNER JOIN HayvanKayit h ON a.KupeNo = h.KupeNo";

            string? kupeNoFilter = textBox5.Text?.Trim();
            if (!string.IsNullOrEmpty(kupeNoFilter))
                query += " WHERE a.KupeNo LIKE @KupeNo || '%'";

            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                using var command = new SQLiteCommand(query, connection);
                if (!string.IsNullOrEmpty(kupeNoFilter))
                    command.Parameters.AddWithValue("@KupeNo", kupeNoFilter);

                using var dataAdapter = new SQLiteDataAdapter(command);
                var dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                if (!dataTable.Columns.Contains("DigerDozDisplay"))
                    dataTable.Columns.Add("DigerDozDisplay", typeof(string));

                foreach (DataRow row in dataTable.Rows)
                {
                    row["DigerDozDisplay"] = row["DigerDoz"] != DBNull.Value && DateTime.TryParse(row["DigerDoz"]?.ToString(), out DateTime targetDate)
                        ? (targetDate - DateTime.Now).TotalSeconds <= 0
                            ? "Aşı periyotu bitti!"
                            : $"{(targetDate - DateTime.Now).Days:D2} Gün {(targetDate - DateTime.Now).Hours:D2}:{(targetDate - DateTime.Now).Minutes:D2}:{(targetDate - DateTime.Now).Seconds:D2}"
                        : "";
                }

                dataGridView1.DataSource = dataTable;

                if (dataGridView1.Columns["DigerDoz"] != null)
                    dataGridView1.Columns["DigerDoz"].Visible = false;
                if (dataGridView1.Columns["DigerDozDisplay"] != null)
                {
                    dataGridView1.Columns["DigerDozDisplay"].HeaderText = "Diğer Doz";
                    dataGridView1.Columns["DigerDozDisplay"].Width = 120;
                }
                if (dataGridView1.Columns["DoseNumber"] != null)
                {
                    dataGridView1.Columns["DoseNumber"].HeaderText = "Doz Numarası";
                    dataGridView1.Columns["DoseNumber"].Width = 100;
                }

                UpdateDataGridViewRowColors(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDataGridViewRowColors(bool preserveColors)
        {
            if (dataGridView1.Rows.Count == 0) return;

            foreach (DataGridViewRow gridRow in dataGridView1.Rows)
            {
                if (gridRow.DataBoundItem is not DataRowView rowView || rowView.Row.RowState == DataRowState.Deleted) continue;

                if (!preserveColors)
                {
                    gridRow.DefaultCellStyle.BackColor = Color.White;
                    gridRow.DefaultCellStyle.ForeColor = Color.Black;

                    string? durum = rowView.Row["Durum"]?.ToString()?.Trim();
                    if (durum == "Yapıldı")
                    {
                        gridRow.DefaultCellStyle.BackColor = Color.FromArgb(144, 238, 144); // LightGreen
                    }
                    else if (durum == "Yapılmadı")
                    {
                        gridRow.DefaultCellStyle.BackColor = Color.White;
                    }

                    if (rowView.Row["DigerDoz"] != DBNull.Value && DateTime.TryParse(rowView.Row["DigerDoz"]?.ToString(), out DateTime targetDate))
                    {
                        TimeSpan remainingTime = targetDate - DateTime.Now;
                        if (remainingTime.TotalSeconds <= 0)
                        {
                            gridRow.DefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71); // Tomato
                            gridRow.DefaultCellStyle.ForeColor = Color.White;
                        }
                        else if (remainingTime.TotalDays <= 15)
                        {
                            gridRow.DefaultCellStyle.BackColor = Color.FromArgb(240, 128, 128); // LightCoral
                        }
                    }
                }
            }
        }

        private void pictureBox2_Click(object? sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            if (dataGridView1.DataSource is not DataTable currentTable) return;

            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                int updatedRows = 0, deletedRows = 0;

                foreach (DataRow row in currentTable.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        string? rowKupeNo = row["KupeNo", DataRowVersion.Original]?.ToString();
                        string? rowAsiAdi = row["AşıAdı", DataRowVersion.Original]?.ToString();
                        object? rowDoseNumber = row["DoseNumber", DataRowVersion.Original];

                        if (!string.IsNullOrEmpty(rowKupeNo) && !string.IsNullOrEmpty(rowAsiAdi))
                        {
                            using var cmd = new SQLiteCommand("DELETE FROM AsiBilgisi WHERE KupeNo = @KupeNo AND AşıAdı = @AsiAdi AND DoseNumber = @DoseNumber", connection);
                            cmd.Parameters.AddWithValue("@KupeNo", rowKupeNo);
                            cmd.Parameters.AddWithValue("@AsiAdi", rowAsiAdi);
                            cmd.Parameters.AddWithValue("@DoseNumber", rowDoseNumber ?? DBNull.Value);
                            if (cmd.ExecuteNonQuery() > 0) deletedRows++;
                        }
                        continue;
                    }

                    string? rowKupeNo2 = row["KupeNo"]?.ToString()?.Trim();
                    string? rowAsiAdi2 = row["AşıAdı"]?.ToString()?.Trim();
                    object? rowDoseNumber2 = row["DoseNumber"] ?? 1;

                    if (string.IsNullOrEmpty(rowKupeNo2) || string.IsNullOrEmpty(rowAsiAdi2)) continue;

                    string? durum = row["Durum"]?.ToString()?.Trim();
                    object digerDoz = row["DigerDoz"] == DBNull.Value ? DBNull.Value : Convert.ToDateTime(row["DigerDoz"]);
                    string? cinsi = row["Cinsi"]?.ToString()?.Trim();
                    object kayitTarihi = row["KayitTarihi"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(row["KayitTarihi"]);

                    using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM AsiBilgisi WHERE KupeNo = @KupeNo AND AşıAdı = @AsiAdi AND DoseNumber = @DoseNumber", connection))
                    {
                        checkCmd.Parameters.AddWithValue("@KupeNo", rowKupeNo2);
                        checkCmd.Parameters.AddWithValue("@AsiAdi", rowAsiAdi2);
                        checkCmd.Parameters.AddWithValue("@DoseNumber", rowDoseNumber2);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            using var cmd = new SQLiteCommand(
                                "UPDATE AsiBilgisi SET Cinsi = @Cinsi, Durum = @Durum, DigerDoz = @DigerDoz, KayitTarihi = @KayitTarihi " +
                                "WHERE KupeNo = @KupeNo AND AşıAdı = @AsiAdi AND DoseNumber = @DoseNumber", connection);
                            cmd.Parameters.AddWithValue("@KupeNo", rowKupeNo2);
                            cmd.Parameters.AddWithValue("@AsiAdi", rowAsiAdi2);
                            cmd.Parameters.AddWithValue("@DoseNumber", rowDoseNumber2);
                            cmd.Parameters.AddWithValue("@Cinsi", string.IsNullOrEmpty(cinsi) ? DBNull.Value : cinsi);
                            cmd.Parameters.AddWithValue("@Durum", string.IsNullOrEmpty(durum) ? DBNull.Value : durum);
                            cmd.Parameters.AddWithValue("@DigerDoz", digerDoz);
                            cmd.Parameters.AddWithValue("@KayitTarihi", kayitTarihi);
                            if (cmd.ExecuteNonQuery() > 0) updatedRows++;
                        }
                    }
                }

                currentTable.AcceptChanges();
                LoadDataGridView();

                MessageBox.Show(updatedRows == 0 && deletedRows == 0
                    ? "Değişiklik yapılmadı!"
                    : $"Değişiklikler kaydedildi!\nGüncellenen: {updatedRows}, Silinen: {deletedRows}",
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                notifiedRecords.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {
            HandleDatabaseOperation();
        }

        private void HandleDatabaseOperation()
        {
            string? kupeNo = textBox1.Text?.Trim();
            string? cinsi = textBox2.Text?.Trim();
            string? asiAdi = textBox3.Text?.Trim();

            if (string.IsNullOrWhiteSpace(kupeNo) || string.IsNullOrWhiteSpace(cinsi) || string.IsNullOrWhiteSpace(asiAdi))
            {
                MessageBox.Show("Küpe No, Cinsi ve Aşı Adı alanlarını doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Bir durum seçin.", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox4.Text?.Trim(), out int totalDays) || totalDays <= 0)
            {
                MessageBox.Show("Geçerli bir periyot süresi girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox6.Text?.Trim(), out int doseCount) || doseCount <= 0)
            {
                MessageBox.Show("Geçerli bir doz sayısı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double doseInterval = doseCount > 1 ? (double)totalDays / (doseCount - 1) : totalDays;

            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();

                int currentDose = 1;
                using (var checkCommand = new SQLiteCommand("SELECT MAX(DoseNumber) FROM AsiBilgisi WHERE KupeNo = @KupeNo AND AşıAdı = @AsiAdi", connection))
                {
                    checkCommand.Parameters.AddWithValue("@KupeNo", kupeNo);
                    checkCommand.Parameters.AddWithValue("@AsiAdi", asiAdi);
                    var result = checkCommand.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                        currentDose = Convert.ToInt32(result) + 1;
                }

                if (currentDose > doseCount)
                {
                    MessageBox.Show($"Tüm dozlar ({doseCount}) tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var command = new SQLiteCommand(
                    "INSERT OR REPLACE INTO AsiBilgisi (KupeNo, Cinsi, AşıAdı, Periyot, Durum, DigerDoz, KayitTarihi, DoseNumber) " +
                    "VALUES (@KupeNo, @Cinsi, @AsiAdi, @Periyot, @Durum, @DigerDoz, @KayitTarihi, @DoseNumber)", connection);
                command.Parameters.AddWithValue("@KupeNo", kupeNo);
                command.Parameters.AddWithValue("@Cinsi", cinsi);
                command.Parameters.AddWithValue("@AsiAdi", asiAdi);
                command.Parameters.AddWithValue("@Periyot", totalDays);
                command.Parameters.AddWithValue("@Durum", radioButton1.Checked ? "Yapıldı" : "Yapılmadı");
                command.Parameters.AddWithValue("@DigerDoz", radioButton1.Checked && currentDose < doseCount ? (object)DateTime.Now.AddDays(doseInterval) : DBNull.Value);
                command.Parameters.AddWithValue("@KayitTarihi", DateTime.Now);
                command.Parameters.AddWithValue("@DoseNumber", currentDose);

                command.ExecuteNonQuery();

                MessageBox.Show($"Aşı bilgisi kaydedildi: Doz {currentDose}/{doseCount}, Durum: {(radioButton1.Checked ? "Yapıldı" : "Yapılmadı")}, " +
                                $"Sonraki doz: {(radioButton1.Checked && currentDose < doseCount ? $"{doseInterval:F2} gün sonra" : "Yok")}.",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataGridView();
                notifiedRecords.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox3_Click(object? sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            notifiedRecords.Clear();
        }

        private void pictureBox13_Click(object? sender, EventArgs e)
        {
            this.Hide();
            var form1 = Application.OpenForms["Form1"] as Form1 ?? new Form1();
            form1.Show();
        }

        private void pictureBoxUpdateDigerDoz_Click(object? sender, EventArgs e)
        {
            string? kupeNo = textBox1.Text?.Trim();
            string? cinsi = textBox2.Text?.Trim();
            string? asiAdi = textBox3.Text?.Trim();

            if (string.IsNullOrWhiteSpace(kupeNo) || string.IsNullOrWhiteSpace(cinsi) || string.IsNullOrWhiteSpace(asiAdi))
            {
                MessageBox.Show("Küpe No, Cinsi ve Aşı Adı alanlarını doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox4.Text?.Trim(), out int totalDays) || totalDays <= 0)
            {
                MessageBox.Show("Geçerli bir periyot süresi girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox6.Text?.Trim(), out int doseCount) || doseCount <= 0)
            {
                MessageBox.Show("Geçerli bir doz sayısı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double doseInterval = doseCount > 1 ? (double)totalDays / (doseCount - 1) : totalDays;

            try
            {
                using var connection = new SQLiteConnection(connectionString);
                connection.Open();

                int currentDose = 1;
                using (var checkCommand = new SQLiteCommand("SELECT MAX(DoseNumber) FROM AsiBilgisi WHERE KupeNo = @KupeNo AND AşıAdı = @AsiAdi", connection))
                {
                    checkCommand.Parameters.AddWithValue("@KupeNo", kupeNo);
                    checkCommand.Parameters.AddWithValue("@AsiAdi", asiAdi);
                    var result = checkCommand.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                        currentDose = Convert.ToInt32(result) + 1;
                }

                if (currentDose > doseCount)
                {
                    MessageBox.Show($"Tüm dozlar ({doseCount}) tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var command = new SQLiteCommand(
                    "INSERT OR REPLACE INTO AsiBilgisi (KupeNo, Cinsi, AşıAdı, Periyot, Durum, DigerDoz, KayitTarihi, DoseNumber) " +
                    "VALUES (@KupeNo, @Cinsi, @AsiAdi, @Periyot, @Durum, @DigerDoz, @KayitTarihi, @DoseNumber)", connection);
                command.Parameters.AddWithValue("@KupeNo", kupeNo);
                command.Parameters.AddWithValue("@Cinsi", cinsi);
                command.Parameters.AddWithValue("@AsiAdi", asiAdi);
                command.Parameters.AddWithValue("@Periyot", totalDays);
                command.Parameters.AddWithValue("@Durum", "Yapıldı");
                command.Parameters.AddWithValue("@DigerDoz", currentDose < doseCount ? (object)DateTime.Now.AddDays(doseInterval) : DBNull.Value);
                command.Parameters.AddWithValue("@KayitTarihi", DateTime.Now);
                command.Parameters.AddWithValue("@DoseNumber", currentDose);

                int rowsAffected = command.ExecuteNonQuery();
                MessageBox.Show(rowsAffected > 0
                    ? $"{rowsAffected} satır etkilendi. Doz {currentDose}/{doseCount} güncellendi! Sonraki doz: {(currentDose < doseCount ? $"{doseInterval:F2} gün sonra" : "Yok")}"
                    : "Hiçbir satır etkilenmedi.",
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataGridView();
                notifiedRecords.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Güncelleme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form15_Load(object? sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Regular);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(135, 206, 250);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.GridColor = Color.FromArgb(200, 200, 200);
            dataGridView1.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBox6_Click_1(object? sender, EventArgs e)
        {
            this.Hide();
            var form13 = Application.OpenForms["Form13"] as Form13 ?? new Form13();
            form13.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form13 form13 = Application.OpenForms["Form13"] as Form13 ?? new Form13();
                form13.Show();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}