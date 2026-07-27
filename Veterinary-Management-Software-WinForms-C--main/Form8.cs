using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Collections.Generic;
using System.Data.SQLite;

namespace WinFormsApp8
{
    public partial class Form8 : Form
    {
        private readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;";
        private readonly System.Windows.Forms.Timer timer;
        private DataTable? originalDataTable;
        private DataTable? currentDataTable;
        private bool isSaving = false;
        private NotifyIcon notifyIcon;
        private Dictionary<string, DateTime> lastEmailSentTimes;

        public Form8()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += Timer_Tick;
            timer.Start();
            dataGridView1.AllowUserToDeleteRows = true;

            // Initialize CheckedListBox
            checkedListBox1.Items.AddRange(new object[] { "Gebe", "Boş" });

            // Initialize NotifyIcon
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true
            };
            notifyIcon.BalloonTipClicked += (s, e) => notifyIcon.Visible = false;

            // Initialize dictionary
            lastEmailSentTimes = new Dictionary<string, DateTime>();
        }

        private async void Form8_Load(object? sender, EventArgs e)
        {
            await UpdateDataTable();
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (isSaving) return;

            label8.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            await UpdateCountdownAndSendEmails();
        }

        private async Task UpdateDataTable()
        {
            timer.Stop();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();
                    string query = @"
                        SELECT 
                            g.KupeNo, 
                            h.Cinsi, 
                            g.GebelikDurumu, 
                            g.DogumTarihi, 
                            h.Telefon, 
                            h.Adres, 
                            h.HayvanSahibi,
                            g.DogumaKalanSure
                        FROM Gebelik g
                        LEFT JOIN HayvanKayit h ON g.KupeNo = h.KupeNo";

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        Console.WriteLine($"UpdateDataTable: Fetched {dt.Rows.Count} rows from database.");

                        // If no rows are returned, show a notification
                        if (dt.Rows.Count == 0)
                        {
                            ShowSystemTrayNotification("Bilgi", "Veri bulunamadı. Gebelik veya HayvanKayit tablosunda veri olmayabilir.", ToolTipIcon.Info);
                        }

                        // Ensure DogumaKalanSure column exists
                        if (!dt.Columns.Contains("DogumaKalanSure"))
                            dt.Columns.Add("DogumaKalanSure", typeof(string));

                        foreach (DataRow row in dt.Rows)
                        {
                            // Update DogumaKalanSure
                            if (row["DogumTarihi"] != DBNull.Value)
                            {
                                DateTime dogumTarihi = Convert.ToDateTime(row["DogumTarihi"]);
                                TimeSpan kalanSure = dogumTarihi - DateTime.Now;
                                row["DogumaKalanSure"] = kalanSure.TotalSeconds > 0
                                    ? $"{kalanSure.Days} gün {kalanSure.Hours} saat {kalanSure.Minutes} dakika"
                                    : "Doğum zamanı geldi!";
                            }
                            else
                            {
                                row["DogumaKalanSure"] = "Bilinmiyor";
                            }

                            // Transform Adres: if "bilinemeyn@gmail.", display as "Bilinmiyor"
                            if (row["Adres"] != DBNull.Value && row["Adres"]?.ToString() == "bilinemeyn@gmail.")
                            {
                                row["Adres"] = "Bilinmiyor";
                            }
                            // If Adres is null (due to LEFT JOIN), set it to "Bilinmiyor"
                            else if (row["Adres"] == DBNull.Value)
                            {
                                row["Adres"] = "Bilinmiyor";
                            }

                            // If other columns from HayvanKayit are null, set default values
                            if (row["Cinsi"] == DBNull.Value) row["Cinsi"] = "Bilinmiyor";
                            if (row["Telefon"] == DBNull.Value) row["Telefon"] = "Bilinmiyor";
                            if (row["HayvanSahibi"] == DBNull.Value) row["HayvanSahibi"] = "Bilinmiyor";
                        }

                        originalDataTable = dt.Copy();
                        currentDataTable = dt.Copy();
                        Invoke((MethodInvoker)(() =>
                        {
                            dataGridView1.DataSource = currentDataTable;
                            Console.WriteLine($"UpdateDataTable: DataGridView updated with {currentDataTable.Rows.Count} rows.");
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Veri güncellenirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"UpdateDataTable Error: {ex.Message}");
            }
            finally
            {
                timer.Start();
            }
        }

        private async Task UpdateCountdownAndSendEmails()
        {
            await Task.Run(() =>
            {
                Invoke((MethodInvoker)(() =>
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow || row.DataBoundItem == null) continue;

                        if (row.Cells["DogumTarihi"].Value != null && row.Cells["DogumTarihi"].Value != DBNull.Value)
                        {
                            if (DateTime.TryParse(row.Cells["DogumTarihi"].Value?.ToString(), out DateTime dogumTarihi))
                            {
                                TimeSpan kalanSure = dogumTarihi - DateTime.Now;
                                row.Cells["DogumaKalanSure"].Value = kalanSure.TotalSeconds > 0
                                    ? $"{kalanSure.Days} gün {kalanSure.Hours} saat {kalanSure.Minutes} dakika"
                                    : "Doğum zamanı geldi!";

                                if (kalanSure.TotalDays <= 15 && kalanSure.TotalSeconds > 0)
                                {
                                    row.DefaultCellStyle.BackColor = Color.Red;
                                    row.DefaultCellStyle.ForeColor = Color.White;

                                    string? kupeNo = row.Cells["KupeNo"].Value?.ToString();
                                    if (!string.IsNullOrEmpty(kupeNo))
                                    {
                                        if (!lastEmailSentTimes.ContainsKey(kupeNo) ||
                                            (DateTime.Now - lastEmailSentTimes[kupeNo]).TotalHours >= 24)
                                        {
                                            _ = SendEmailNotification(row);
                                            lastEmailSentTimes[kupeNo] = DateTime.Now;

                                            ShowSystemTrayNotification("Bilgilendirme",
                                                $"Küpe No {kupeNo} için e-posta gönderildi. Doğuma kalan süre: {row.Cells["DogumaKalanSure"].Value}",
                                                ToolTipIcon.Info);
                                        }
                                    }
                                }
                                else if (kalanSure.TotalSeconds > 0)
                                {
                                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                                    row.DefaultCellStyle.ForeColor = Color.Black;
                                }
                                else
                                {
                                    row.DefaultCellStyle.BackColor = Color.Gray;
                                    row.DefaultCellStyle.ForeColor = Color.White;
                                }
                            }
                        }
                        else
                        {
                            row.Cells["DogumaKalanSure"].Value = "Bilinmiyor";
                            row.DefaultCellStyle.BackColor = Color.White;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }
                    }
                    dataGridView1.Refresh();
                }));
            });
        }

        private void ShowSystemTrayNotification(string title, string message, ToolTipIcon icon)
        {
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.ShowBalloonTip(3000);
        }

        private async Task SendEmailNotification(DataGridViewRow row)
        {
            try
            {
                string gondericiEmail = "";
                string uygulamaSifresi = "";

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT email, password FROM mail LIMIT 1";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = (SQLiteDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            gondericiEmail = reader["email"]?.ToString() ?? "";
                            uygulamaSifresi = reader["password"]?.ToString() ?? "";
                        }
                    }
                }

                if (string.IsNullOrEmpty(gondericiEmail) || string.IsNullOrEmpty(uygulamaSifresi))
                {
                    ShowSystemTrayNotification("Hata", "Gönderici e-posta veya şifre veritabanından çekilemedi.", ToolTipIcon.Error);
                    return;
                }

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential(gondericiEmail, uygulamaSifresi);
                    smtpClient.EnableSsl = true;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(gondericiEmail);
                        mailMessage.To.Add("otomasyonveteriner@gmail.com");
                        mailMessage.Subject = $"Doğum Uyarısı - Küpe No: {row.Cells["KupeNo"].Value}";
                        mailMessage.Body = $"Küpe No: {row.Cells["KupeNo"].Value}\n" +
                                         $"Hayvan Sahibi: {row.Cells["HayvanSahibi"].Value}\n" +
                                         $"Doğuma Kalan Süre: {row.Cells["DogumaKalanSure"].Value}\n" +
                                         $"Telefon: {row.Cells["Telefon"].Value}\n" +
                                         $"Adres: {row.Cells["Adres"].Value}";
                        mailMessage.IsBodyHtml = false;

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                PlayNotificationSound(@"C:\Users\ridva\Desktop\dutdut\my-project\veteriner_otomasyon\inek.wav");
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"E-posta gönderilirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"SendEmailNotification Error: {ex.Message}");
            }
        }

        private void PlayNotificationSound(string filePath)
        {
            try
            {
                SoundPlayer player = new SoundPlayer(filePath);
                player.Play();
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Ses Hatası", $"Ses çalınırken hata oluştu: {ex.Message}", ToolTipIcon.Warning);
                Console.WriteLine($"PlayNotificationSound Error: {ex.Message}");
            }
        }

        private async void pictureBox2_Click(object? sender, EventArgs e)
        {
            try
            {
                string? kupeNo = textBox1.Text?.Trim();
                string? cinsi = textBox2.Text?.Trim();
                string gebelikDurumu = checkedListBox1.CheckedItems.Cast<object>()
                    .Any(item => item.ToString().Trim().Equals("Gebe", StringComparison.OrdinalIgnoreCase)) ? "Gebe" : "Boş";

                // Validate inputs
                if (string.IsNullOrWhiteSpace(kupeNo) || string.IsNullOrWhiteSpace(cinsi))
                {
                    ShowSystemTrayNotification("Hata", "Küpe No ve Cinsi boş bırakılamaz!", ToolTipIcon.Warning);
                    return;
                }

                // Check if KupeNo exists in HayvanKayit and save to Gebelik
                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();

                    string checkHayvanQuery = "SELECT COUNT(*) FROM HayvanKayit WHERE KupeNo = @KupeNo";
                    using (SQLiteCommand checkHayvanCmd = new SQLiteCommand(checkHayvanQuery, con))
                    {
                        checkHayvanCmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                        object? result = await checkHayvanCmd.ExecuteScalarAsync();
                        long hayvanCount = result != null ? Convert.ToInt64(result) : 0;
                        if (hayvanCount == 0)
                        {
                            ShowSystemTrayNotification("Hata", $"Küpe No '{kupeNo}' HayvanKayit tablosunda bulunamadı!", ToolTipIcon.Error);
                            return;
                        }
                    }

                    // Check if KupeNo already exists in Gebelik
                    string checkGebelikQuery = "SELECT COUNT(*) FROM Gebelik WHERE KupeNo = @KupeNo";
                    using (SQLiteCommand checkGebelikCmd = new SQLiteCommand(checkGebelikQuery, con))
                    {
                        checkGebelikCmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                        object? result = await checkGebelikCmd.ExecuteScalarAsync();
                        long gebelikCount = result != null ? Convert.ToInt64(result) : 0;
                        if (gebelikCount > 0)
                        {
                            ShowSystemTrayNotification("Hata", $"Küpe No '{kupeNo}' zaten Gebelik tablosunda kayıtlı!", ToolTipIcon.Warning);
                            return;
                        }
                    }

                    // Insert into Gebelik table
                    string insertQuery = @"
                        INSERT INTO Gebelik (KupeNo, GebelikDurumu, KayitTarihi, DogumTarihi, DogumaKalanSure)
                        VALUES (@KupeNo, @GebelikDurumu, @KayitTarihi, @DogumTarihi, @DogumaKalanSure)";

                    using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, con))
                    {
                        object dogumTarihi = gebelikDurumu == "Gebe" ? (object)DateTime.Now.AddDays(279).ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value;
                        string dogumaKalanSure = gebelikDurumu == "Gebe" ? "279 gün 0 saat 0 dakika" : "Bilinmiyor";

                        insertCmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                        insertCmd.Parameters.AddWithValue("@GebelikDurumu", gebelikDurumu);
                        insertCmd.Parameters.AddWithValue("@KayitTarihi", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCmd.Parameters.AddWithValue("@DogumTarihi", dogumTarihi);
                        insertCmd.Parameters.AddWithValue("@DogumaKalanSure", dogumaKalanSure);

                        await insertCmd.ExecuteNonQueryAsync();
                    }
                }

                // Refresh the DataGridView by reloading data from the database
                await UpdateDataTable();

                // Clear input fields
                textBox1.Clear();
                textBox2.Clear();
                checkedListBox1.ClearSelected();

                ShowSystemTrayNotification("Bilgi", "Kayıt başarıyla eklendi!", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Kayıt eklenirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"pictureBox2_Click Error: {ex.Message}");
            }
        }

        private void dataGridView1_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Check if the clicked cell is in the GebelikDurumu column
                if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["GebelikDurumu"].Index)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    string? currentGebelikDurumu = row.Cells["GebelikDurumu"].Value?.ToString();

                    if (!string.IsNullOrEmpty(currentGebelikDurumu))
                    {
                        // Toggle GebelikDurumu
                        string newGebelikDurumu = currentGebelikDurumu == "Gebe" ? "Boş" : "Gebe";
                        object newDogumTarihi = newGebelikDurumu == "Gebe" ? (object)DateTime.Now.AddDays(279) : DBNull.Value;
                        string newDogumaKalanSure = newGebelikDurumu == "Gebe" ? "279 gün 0 saat 0 dakika" : "Bilinmiyor";

                        // Update the DataGridView row
                        row.Cells["GebelikDurumu"].Value = newGebelikDurumu;
                        row.Cells["DogumTarihi"].Value = newDogumTarihi;
                        row.Cells["DogumaKalanSure"].Value = newDogumaKalanSure;

                        ShowSystemTrayNotification("Bilgi", $"Gebelik Durumu '{newGebelikDurumu}' olarak değiştirildi. Güncellemek için Güncelle butonuna tıklayın.", ToolTipIcon.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Gebelik Durumu değiştirilirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"dataGridView1_CellContentClick Error: {ex.Message}");
            }
        }

        private async void pictureBox1_Click(object? sender, EventArgs e)
        {
            try
            {
                // Get values from textboxes and checkedListBox
                string? kupeNo = textBox1.Text?.Trim();
                string? cinsi = textBox2.Text?.Trim();
                string newGebelikDurumu = checkedListBox1.CheckedItems.Cast<object>()
                    .Any(item => item.ToString().Trim().Equals("Gebe", StringComparison.OrdinalIgnoreCase)) ? "Gebe" : "Boş";

                // Validate inputs
                if (string.IsNullOrWhiteSpace(kupeNo) || string.IsNullOrWhiteSpace(cinsi) || string.IsNullOrWhiteSpace(newGebelikDurumu))
                {
                    ShowSystemTrayNotification("Hata", "Küpe No, Cinsi veya Gebelik Durumu boş olamaz!", ToolTipIcon.Warning);
                    return;
                }

                // Calculate DogumTarihi and DogumaKalanSure based on GebelikDurumu
                object newDogumTarihi = newGebelikDurumu == "Gebe" ? (object)DateTime.Now.AddDays(279) : DBNull.Value;
                string newDogumaKalanSure = newGebelikDurumu == "Gebe" ? "279 gün 0 saat 0 dakika" : "Bilinmiyor";

                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();

                    // Check if KupeNo exists in Gebelik table
                    string checkGebelikQuery = "SELECT COUNT(*) FROM Gebelik WHERE KupeNo = @KupeNo";
                    using (SQLiteCommand checkGebelikCmd = new SQLiteCommand(checkGebelikQuery, con))
                    {
                        checkGebelikCmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                        object? result = await checkGebelikCmd.ExecuteScalarAsync();
                        long gebelikCount = result != null ? Convert.ToInt64(result) : 0;
                        if (gebelikCount == 0)
                        {
                            ShowSystemTrayNotification("Hata", $"Küpe No '{kupeNo}' Gebelik tablosunda bulunamadı!", ToolTipIcon.Error);
                            return;
                        }
                    }

                    // Update GebelikDurumu, DogumTarihi, and DogumaKalanSure in the Gebelik table
                    string updateGebelikQuery = @"
                        UPDATE Gebelik 
                        SET GebelikDurumu = @GebelikDurumu, DogumTarihi = @DogumTarihi, DogumaKalanSure = @DogumaKalanSure
                        WHERE KupeNo = @KupeNo";

                    using (SQLiteCommand cmd = new SQLiteCommand(updateGebelikQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                        cmd.Parameters.AddWithValue("@GebelikDurumu", newGebelikDurumu);
                        cmd.Parameters.AddWithValue("@DogumTarihi", newDogumTarihi == DBNull.Value ? DBNull.Value : Convert.ToDateTime(newDogumTarihi).ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@DogumaKalanSure", newDogumaKalanSure);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        Console.WriteLine($"pictureBox1_Click: Updated Gebelik for KupeNo '{kupeNo}', New GebelikDurumu: '{newGebelikDurumu}', Rows Affected: {rowsAffected}");

                        if (rowsAffected == 0)
                        {
                            ShowSystemTrayNotification("Hata", $"Gebelik durumu güncellenemedi, kayıt bulunamadı!", ToolTipIcon.Error);
                            return;
                        }
                    }

                    // Update Cinsi in the HayvanKayit table
                    string updateCinsiQuery = "UPDATE HayvanKayit SET Cinsi = @Cinsi WHERE KupeNo = @KupeNo";
                    using (SQLiteCommand cmd = new SQLiteCommand(updateCinsiQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Cinsi", cinsi);
                        cmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Refresh the DataGridView by reloading data from the database
                await UpdateDataTable();

                // Clear input fields
                textBox1.Clear();
                textBox2.Clear();
                checkedListBox1.ClearSelected();

                ShowSystemTrayNotification("Bilgi", $"Küpe No '{kupeNo}' için Gebelik Durumu '{newGebelikDurumu}' olarak güncellendi!", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Güncelleme sırasında hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"pictureBox1_Click Error: {ex.Message}");
            }
        }

        private async Task SaveDataGridViewChanges()
        {
            try
            {
                isSaving = true;
                timer.Stop();

                dataGridView1.EndEdit();
                if (currentDataTable != null)
                {
                    currentDataTable.AcceptChanges();
                }

                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();

                    // Handle deleted rows
                    if (originalDataTable != null && currentDataTable != null)
                    {
                        var deletedRows = originalDataTable.AsEnumerable()
                            .Where(origRow => !currentDataTable.AsEnumerable()
                                .Any(currRow => currRow["KupeNo"].ToString() == origRow["KupeNo"].ToString()))
                            .ToList();

                        foreach (var deletedRow in deletedRows)
                        {
                            string? kupeNo = deletedRow["KupeNo"]?.ToString();
                            if (string.IsNullOrEmpty(kupeNo)) continue;

                            string deleteQuery = "DELETE FROM Gebelik WHERE KupeNo = @KupeNo";
                            using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Handle updated rows
                    if (currentDataTable != null)
                    {
                        var activeRows = currentDataTable.AsEnumerable()
                            .Where(row => row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                            .ToList();

                        foreach (var row in activeRows)
                        {
                            string? kupeNo = row["KupeNo"]?.ToString();
                            string? gebelikDurumu = row["GebelikDurumu"]?.ToString();
                            DateTime? dogumTarihi = row["DogumTarihi"] != DBNull.Value ? (DateTime?)row["DogumTarihi"] : null;
                            string? dogumaKalanSure = row["DogumaKalanSure"]?.ToString();

                            if (string.IsNullOrEmpty(kupeNo) || string.IsNullOrEmpty(gebelikDurumu)) continue;

                            string updateQuery = @"
                                UPDATE Gebelik 
                                SET GebelikDurumu = @GebelikDurumu, DogumTarihi = @DogumTarihi, DogumaKalanSure = @DogumaKalanSure
                                WHERE KupeNo = @KupeNo";

                            using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@KupeNo", kupeNo);
                                cmd.Parameters.AddWithValue("@GebelikDurumu", gebelikDurumu);
                                cmd.Parameters.AddWithValue("@DogumTarihi", dogumTarihi.HasValue ? dogumTarihi.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);
                                cmd.Parameters.AddWithValue("@DogumaKalanSure", dogumaKalanSure ?? "Bilinmiyor");

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    await UpdateDataTable();
                }

                ShowSystemTrayNotification("Bilgi", "Değişiklikler ve silme işlemleri başarıyla kaydedildi.", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Değişiklikler kaydedilirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"SaveDataGridViewChanges Error: {ex.Message}");
            }
            finally
            {
                isSaving = false;
                timer.Start();
            }
        }

        private void pictureBox3_Click(object? sender, EventArgs e)
        {
            this.Hide();
            Form1? form9 = Application.OpenForms["Form9"] as Form1 ?? new Form1();
            form9.Show();
        }

        private void groupBox4_Enter(object? sender, EventArgs e)
        {
            ShowSystemTrayNotification("Bilgi", "GroupBox4'e girildi.", ToolTipIcon.Info);
        }

        private void pictureBox5_Click(object? sender, EventArgs e)
        {
            this.Hide();
            Form19 form19 = Application.OpenForms["Form19"] as Form19;
            if (form19 != null)
            {
                form19.Show();
            }
            else
            {
                form19 = new Form19();
                form19.Show();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            base.OnFormClosing(e);
        }
    }
}