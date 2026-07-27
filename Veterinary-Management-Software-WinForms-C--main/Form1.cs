using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;

namespace WinFormsApp8
{
    public partial class Form1 : Form
    {
        private static readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;";
        private System.Windows.Forms.Timer? emailTimer;
        private const int TwentyFourHoursInMilliseconds = 24 * 60 * 60 * 1000;
        private bool hasSentToday = false;

        public Form1()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form oluşturulurken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeEmailTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeEmailTimer()
        {
            try
            {
                emailTimer = new System.Windows.Forms.Timer();

                DateTime now = DateTime.Now;
                DateTime nextRunTime = now.Date.AddHours(9); // 9 AM today
                if (now > nextRunTime)
                {
                    nextRunTime = nextRunTime.AddDays(1); // If past 9 AM, use tomorrow
                }

                TimeSpan timeUntilNextRun = nextRunTime - now;
                int initialInterval = Math.Max((int)timeUntilNextRun.TotalMilliseconds, 1000); // Prevent negative interval

                emailTimer.Interval = initialInterval;
                emailTimer.Tick += async (s, e) =>
                {
                    try
                    {
                        if (!hasSentToday)
                        {
                            await SmsGonderAsync();
                            hasSentToday = true;
                        }

                        // Reset at midnight
                        if (DateTime.Now.Date > now.Date)
                        {
                            hasSentToday = false;
                        }

                        emailTimer.Interval = TwentyFourHoursInMilliseconds; // 24 hours after first run
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Timer hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                emailTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Timer başlatılırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form6>();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form8>();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form7>();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form9>();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form15>();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form16>();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form5>();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form4>();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form2>();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form3>();
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form20>();
        }

        private void pictureBox12_Click_1(object sender, EventArgs e)
        {
            NavigateToForm<Form17>();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form17>();
        }

        private void pictureBox13_Click_1(object sender, EventArgs e)
        {
            NavigateToForm<Form17>();
        }

        private void pictureBox13_Click_2(object sender, EventArgs e)
        {
            this.Hide();
            
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            NavigateToForm<Form20>();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Placeholder for future implementation
        }

        private void NavigateToForm<T>() where T : Form, new()
        {
            try
            {
                this.Hide();
                var form = Application.OpenForms[typeof(T).Name] as T ?? new T();
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form geçişinde hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SmsGonderAsync()
        {
            try
            {
                string dbPath = @"C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db";
                if (!File.Exists(dbPath))
                {
                    throw new Exception($"Veritabanı dosyası bulunamadı: {dbPath}");
                }

                string gondericiEmail = string.Empty, uygulamaSifresi = string.Empty, aliciEmail = string.Empty;
                using var conn = new SqliteConnection(connectionString);
                await conn.OpenAsync();

                // Fetch sender email and password
                using (var cmd = new SqliteCommand("SELECT email, password FROM mail LIMIT 1", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        gondericiEmail = reader["email"]?.ToString() ?? string.Empty;
                        uygulamaSifresi = reader["password"]?.ToString() ?? string.Empty;
                    }
                }

                // Fetch recipient email
                using (var cmd = new SqliteCommand("SELECT email FROM mail LIMIT 1", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        aliciEmail = reader["email"]?.ToString() ?? string.Empty;
                    }
                }

                if (string.IsNullOrEmpty(gondericiEmail) || string.IsNullOrEmpty(uygulamaSifresi))
                {
                    throw new Exception("Gönderici e-posta veya şifre eksik.");
                }

                if (string.IsNullOrEmpty(aliciEmail))
                {
                    throw new Exception("Alıcı e-posta adresi eksik.");
                }

                var sorgular = new List<(string Query, string Baslik, string Aciklama)>
                {
                    ("SELECT ID, Cinsi, GebelikDurumu, KayitTarihi, DogumTarihi, Telefon, HayvanSahibi, DogumaKalanSure FROM kedi_gebelik WHERE DogumTarihi IS NOT NULL AND CAST(SUBSTR(DogumaKalanSure, 1, INSTR(DogumaKalanSure, ' gün') - 1) AS INTEGER) < 15",
                     "Kedi Gebelik",
                     "Kedi Gebelik tablosu, gebelik süresi 15 günden az olan kedilere ait bilgileri içerir. Gebelik süresi 5 gün ve altında olanlar kırmızı, 6-14 gün arasında olanlar sarı ile işaretlenmiştir."),
                    ("SELECT ID, Cinsi, GebelikDurumu, KayitTarihi, DogumTarihi, Telefon, HayvanSahibi, DogumaKalanSure FROM kopek_gebelik WHERE DogumTarihi IS NOT NULL AND CAST(SUBSTR(DogumaKalanSure, 1, INSTR(DogumaKalanSure, ' gün') - 1) AS INTEGER) < 15",
                     "Köpek Gebelik",
                     "Köpek Gebelik tablosu, gebelik süresi 15 günden az olan köpeklere ait bilgileri içerir. Gebelik süresi 5 gün ve altında olanlar kırmızı, 6-14 gün arasında olanlar sarı ile işaretlenmiştir."),
                    ("SELECT * FROM Gebelik WHERE DogumTarihi IS NOT NULL AND CAST(SUBSTR(DogumaKalanSure, 1, INSTR(DogumaKalanSure, ' gün') - 1) AS INTEGER) < 15",
                     "Genel Gebelik",
                     "Genel Gebelik tablosu, gebelik süresi 15 günden az olan hayvanlara ait bilgileri içerir. Gebelik süresi 5 gün ve altında olanlar kırmızı, 6-14 gün arasında olanlar sarı ile işaretlenmiştir."),
                    ("SELECT * FROM kopek_asi WHERE AsiSuresi IS NOT NULL AND CAST(AsiSuresi AS INTEGER) < 3",
                     "Köpek Aşı",
                     "Köpek Aşı tablosu, aşı süresi 3 günden az olan köpeklere ait bilgileri içerir. Aşı süresi 0 gün olanlar kırmızı, 1-2 gün olanlar sarı ile işaretlenmiştir."),
                    ("SELECT * FROM kedi_asi WHERE AsiSuresi IS NOT NULL AND CAST(AsiSuresi AS INTEGER) < 3",
                     "Kedi Aşı",
                     "Kedi Aşı tablosu, aşı süresi 3 günden az olan kedilere ait bilgileri içerir. Aşı süresi 0 gün olanlar kırmızı, 1-2 gün olanlar sarı ile işaretlenmiştir."),
                    ("SELECT * FROM AsiBilgisi WHERE DigerDoz IS NOT NULL AND CAST(DigerDoz AS INTEGER) < 5",
                     "Aşı Bilgisi",
                     "Aşı Bilgisi tablosu, diğer doz süresi 5 günden az olan aşılara ait bilgileri içerir. Doz süresi 0 gün olanlar kırmızı, 1-4 gün olanlar sarı ile işaretlenmiştir."),
                    ("SELECT * FROM Hatirlatmalar WHERE DATETIME(saat) <= DATETIME('now', '+3 hours') AND DATETIME(saat) >= DATETIME('now')",
                     "Hatırlatmalar (3 Saat veya Daha Az)",
                     "Hatırlatmalar tablosu, kalan süresi 3 saatten az olan hatırlatmaları içerir. Kalan süresi 1 saatten az olanlar kırmızı, 1-3 saat arasında olanlar sarı ile işaretlenmiştir.")
                };

                var htmlBody = new StringBuilder();
                htmlBody.AppendLine("<!DOCTYPE html>");
                htmlBody.AppendLine("<html lang='tr'>");
                htmlBody.AppendLine("<head>");
                htmlBody.AppendLine("<meta charset='UTF-8'>");
                htmlBody.AppendLine("<style>");
                htmlBody.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
                htmlBody.AppendLine("h1 { color: #2c3e50; }");
                htmlBody.AppendLine("h2 { color: #34495e; border-bottom: 2px solid #3498db; padding-bottom: 5px; }");
                htmlBody.AppendLine("p { margin: 10px 0; }");
                htmlBody.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
                htmlBody.AppendLine("th, td { padding: 10px; border: 1px solid #ddd; text-align: left; }");
                htmlBody.AppendLine("th { background-color: #3498db; color: white; }");
                htmlBody.AppendLine("tr.alert-red { background-color: #ffcccc; }");
                htmlBody.AppendLine("tr.alert-yellow { background-color: #ffffcc; }");
                htmlBody.AppendLine("tr.alert-green { background-color: #ccffcc; }");
                htmlBody.AppendLine("tr:hover { background-color: #f1f1f1; }");
                htmlBody.AppendLine("</style>");
                htmlBody.AppendLine("</head>");
                htmlBody.AppendLine("<body>");

                htmlBody.AppendLine("<h1>Veteriner Otomasyon Uyarıları</h1>");
                htmlBody.AppendLine($"<p><strong>Tarih:</strong> {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");

                int totalRecords = 0;
                var tableSummaries = new List<(string TableName, int RecordCount)>();

                foreach (var (query, tableName, aciklama) in sorgular)
                {
                    try
                    {
                        // Check if table exists
                        string tableNameClean = tableName.Replace(" (3 Saat veya Daha Az)", "").ToLower();
                        using (var cmd = new SqliteCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableNameClean}'", conn))
                        {
                            var result = await cmd.ExecuteScalarAsync();
                            if (result == null)
                            {
                                continue;
                            }
                        }

                        var dt = new DataTable();
                        using (var cmd = new SqliteCommand(query, conn))
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }

                        if (dt.Rows.Count == 0) continue;

                        totalRecords += dt.Rows.Count;
                        tableSummaries.Add((tableName, dt.Rows.Count));

                        htmlBody.AppendLine($"<h2>{tableName} ({dt.Rows.Count} Kayıt)</h2>");
                        htmlBody.AppendLine($"<p>{aciklama}</p>");
                        htmlBody.AppendLine("<table>");
                        htmlBody.AppendLine("<tr>");
                        foreach (DataColumn col in dt.Columns)
                        {
                            htmlBody.AppendLine($"<th>{col.ColumnName}</th>");
                        }
                        htmlBody.AppendLine("</tr>");

                        foreach (DataRow row in dt.Rows)
                        {
                            string rowClass = "alert-green";
                            if (tableName == "Kedi Gebelik" || tableName == "Köpek Gebelik" || tableName == "Genel Gebelik")
                            {
                                string dogumaKalanSure = row["DogumaKalanSure"]?.ToString() ?? "";
                                int daysRemaining = ParseDaysFromDogumaKalanSure(dogumaKalanSure);
                                if (daysRemaining <= 5) rowClass = "alert-red";
                                else if (daysRemaining <= 14) rowClass = "alert-yellow";
                            }
                            else if (tableName == "Köpek Aşı" || tableName == "Kedi Aşı")
                            {
                                if (int.TryParse(row["AsiSuresi"]?.ToString(), out int asiSuresi))
                                {
                                    if (asiSuresi <= 0) rowClass = "alert-red";
                                    else if (asiSuresi <= 2) rowClass = "alert-yellow";
                                }
                            }
                            else if (tableName == "Aşı Bilgisi")
                            {
                                if (int.TryParse(row["DigerDoz"]?.ToString(), out int digerDoz))
                                {
                                    if (digerDoz <= 0) rowClass = "alert-red";
                                    else if (digerDoz <= 4) rowClass = "alert-yellow";
                                }
                                else if (DateTime.TryParse(row["DigerDoz"]?.ToString(), out DateTime digerDozDate))
                                {
                                    TimeSpan timeUntilDoz = digerDozDate - DateTime.Now;
                                    int daysUntilDoz = (int)timeUntilDoz.TotalDays;
                                    if (daysUntilDoz <= 0) rowClass = "alert-red";
                                    else if (daysUntilDoz <= 4) rowClass = "alert-yellow";
                                }
                            }
                            else if (tableName == "Hatırlatmalar (3 Saat veya Daha Az)")
                            {
                                if (DateTime.TryParse(row["saat"]?.ToString(), out DateTime reminderTime))
                                {
                                    TimeSpan timeUntilReminder = reminderTime - DateTime.Now;
                                    double hoursRemaining = timeUntilReminder.TotalHours;
                                    if (hoursRemaining <= 1) rowClass = "alert-red";
                                    else if (hoursRemaining <= 3) rowClass = "alert-yellow";
                                }
                            }

                            htmlBody.AppendLine($"<tr class='{rowClass}'>");
                            foreach (var item in row.ItemArray)
                            {
                                string val = item?.ToString()?.Replace("\r", "")?.Replace("\n", "")?.Trim() ?? "NULL";
                                if (DateTime.TryParse(val, out DateTime dateValue))
                                {
                                    val = dateValue.ToString("dd.MM.yyyy HH:mm:ss");
                                }
                                htmlBody.AppendLine($"<td>{val}</td>");
                            }
                            htmlBody.AppendLine("</tr>");
                        }
                        htmlBody.AppendLine("</table>");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                if (totalRecords == 0)
                {
                    MessageBox.Show("Gönderilecek veri bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                htmlBody.Insert(htmlBody.ToString().IndexOf("<h1>"), $"<p>Bu e-posta, veteriner otomasyon sisteminden gelen günlük uyarıları içermektedir. Aşağıda, toplam {totalRecords} kayıt bulunmaktadır:</p><ul>");
                foreach (var (tableName, recordCount) in tableSummaries)
                {
                    htmlBody.Insert(htmlBody.ToString().IndexOf("<h1>"), $"<li>{tableName}: {recordCount} Kayıt</li>");
                }
                htmlBody.Insert(htmlBody.ToString().IndexOf("<h1>"), "</ul>");

                htmlBody.AppendLine("</body>");
                htmlBody.AppendLine("</html>");

                using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(gondericiEmail, uygulamaSifresi),
                    EnableSsl = true,
                    Timeout = 30000
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(gondericiEmail),
                    Subject = $"Veteriner Otomasyon Uyarıları ({DateTime.Now:dd.MM.yyyy})",
                    Body = htmlBody.ToString(),
                    IsBodyHtml = true
                };
                mail.To.Add(aliciEmail);

                await smtpClient.SendMailAsync(mail);

                DateTime nextEmailTime = DateTime.Now.Date.AddDays(1).AddHours(9);
                TimeSpan timeRemaining = nextEmailTime - DateTime.Now;

                string timeRemainingText = timeRemaining.TotalHours >= 1
                    ? $"{(int)timeRemaining.TotalHours} saat {timeRemaining.Minutes} dakika"
                    : $"{(int)timeRemaining.TotalMinutes} dakika {timeRemaining.Seconds} saniye";

                MessageBox.Show($"Tek bir e-posta başarıyla gönderildi.\nBir sonraki e-posta {timeRemainingText} sonra gönderilecek.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"E-posta gönderilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static int ParseDaysFromDogumaKalanSure(string dogumaKalanSure)
        {
            try
            {
                if (string.IsNullOrEmpty(dogumaKalanSure)) return int.MaxValue;
                var parts = dogumaKalanSure.Split(' ');
                if (parts.Length > 0 && int.TryParse(parts[0], out int days))
                {
                    return days;
                }
                return int.MaxValue;
            }
            catch (Exception)
            {
                return int.MaxValue;
            }
        }
    }
}