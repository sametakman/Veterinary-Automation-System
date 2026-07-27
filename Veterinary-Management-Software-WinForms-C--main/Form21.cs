using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Collections.Generic;
using System.Data.SQLite;

namespace WinFormsApp8
{
    public partial class Form21 : Form
    {
        private readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";
        private readonly System.Windows.Forms.Timer timer;
        private DataTable? originalDataTable;
        private DataTable? currentDataTable;
        private bool isSaving = false;
        private bool isLoading = false;
        private NotifyIcon? notifyIcon;
        private Dictionary<string, DateTime> lastEmailSentTimes;
        private CancellationTokenSource? filterCancellationTokenSource;

        public Form21()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer { Interval = 5000 }; // Refresh every 5 seconds
            timer.Tick += Timer_Tick;
            timer.Start();
            dataGridView1.AllowUserToDeleteRows = true; // Enable row deletion
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Visible = true; // Ensure DataGridView is visible

            checkBox1.Text = "Gebe";
            checkBox2.Text = "Gebe Değil";
            checkBox1.CheckedChanged += (s, e) => { if (checkBox1.Checked) checkBox2.Checked = false; };
            checkBox2.CheckedChanged += (s, e) => { if (checkBox2.Checked) checkBox1.Checked = false; };

            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true
            };
            notifyIcon.BalloonTipClicked += (s, e) => notifyIcon.Visible = false;

            lastEmailSentTimes = new Dictionary<string, DateTime>();

            InitializeTextBoxEvents();
            InitializeDataGridViewEvents();
        }

        private void InitializeTextBoxEvents()
        {
            textBox4.TextChanged += async (s, e) =>
            {
                // Cancel previous filter operation
                filterCancellationTokenSource?.Cancel();
                filterCancellationTokenSource = new CancellationTokenSource();

                try
                {
                    // Debounce: wait 300ms before filtering
                    await Task.Delay(300, filterCancellationTokenSource.Token);
                    FilterDataGridView();
                }
                catch (TaskCanceledException)
                {
                    // Ignore cancellation
                }
            };
        }

        private void InitializeDataGridViewEvents()
        {
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Delete && dataGridView1.SelectedRows.Count > 0)
                {
                    try
                    {
                        DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                        if (selectedRow.DataBoundItem is DataRowView rowView && rowView.Row != null)
                        {
                            string? id = rowView.Row["ID"]?.ToString();
                            if (string.IsNullOrEmpty(id))
                            {
                                ShowSystemTrayNotification("Hata", "Seçili satırın kimliği bulunamadı!", ToolTipIcon.Error);
                                return;
                            }

                            DialogResult result = MessageBox.Show(
                                $"ID '{id}' olan kayıt silinecek. Devam etmek istiyor musunuz?",
                                "Kayıt Silme Onayı",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                                timer.Stop();

                                // Delete from database
                                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                                {
                                    await conn.OpenAsync();
                                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                                    {
                                        string deleteQuery = "DELETE FROM kedi_gebelik WHERE ID = @ID";
                                        using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, conn, transaction))
                                        {
                                            cmd.Parameters.AddWithValue("@ID", id);
                                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                                            if (rowsAffected == 0)
                                            {
                                                ShowSystemTrayNotification("Hata", $"ID '{id}' silinemedi, kayıt bulunamadı!", ToolTipIcon.Error);
                                                transaction.Rollback();
                                                return;
                                            }
                                        }
                                        transaction.Commit();
                                    }
                                }

                                // Remove from currentDataTable
                                rowView.Row.Delete();
                                currentDataTable?.AcceptChanges();

                                // Refresh DataGridView
                                await UpdateDataTable();

                                ShowSystemTrayNotification("Bilgi", $"ID '{id}' başarıyla silindi!", ToolTipIcon.Info);
                            }
                            e.Handled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowSystemTrayNotification("Hata", $"Satır silinirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                        Console.WriteLine($"KeyDown Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    }
                    finally
                    {
                        dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
                        timer.Start();
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    MessageBox.Show("Lütfen silmek için bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Handled = true;
                }
            };
        }

        private async void Form21_Load(object? sender, EventArgs e)
        {
            isLoading = true;
            try
            {
                dataGridView1.Visible = true; // Ensure DataGridView is visible
                await UpdateDataTable(); // Load data
                StyleDataGridView(); // Apply styling
                if (currentDataTable == null || currentDataTable.Rows.Count == 0)
                {
                    ShowSystemTrayNotification("Bilgi", "Veri bulunamadı. kedi_gebelik tablosunda kayıt yok.", ToolTipIcon.Info);
                }
                else
                {
                    ShowSystemTrayNotification("Bilgi", $"{currentDataTable.Rows.Count} kayıt yüklendi.", ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Form yüklenirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"Form21_Load Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (isSaving) return;

            label8.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            await UpdateDataTable(); // Periodic refresh
            await UpdateCountdownAndSendEmails();
        }

        private async Task UpdateDataTable(string filter = "")
        {
            timer.Stop();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();
                    string query = @"
                        SELECT 
                            ID, 
                            Cinsi, 
                            GebelikDurumu, 
                            KayitTarihi, 
                            DogumTarihi, 
                            Telefon, 
                            HayvanSahibi,
                            DogumaKalanSure
                        FROM kedi_gebelik";
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        query += " WHERE LOWER(Telefon) LIKE '%' || LOWER(@Telefon) || '%'";
                    }

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        if (!string.IsNullOrWhiteSpace(filter))
                        {
                            cmd.Parameters.AddWithValue("@Telefon", filter);
                        }

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Console.WriteLine($"UpdateDataTable: Veritabanından {dt.Rows.Count} satır çekildi.");

                            if (dt.Rows.Count == 0)
                            {
                                ShowSystemTrayNotification("Bilgi", string.IsNullOrWhiteSpace(filter) ?
                                    "Veri bulunamadı. kedi_gebelik tablosunda veri olmayabilir." :
                                    $"Telefon '{filter}' için kayıt bulunamadı.", ToolTipIcon.Info);
                            }
                            else if (!string.IsNullOrWhiteSpace(filter))
                            {
                                ShowSystemTrayNotification("Bilgi", $"{dt.Rows.Count} kayıt filtrelendi.", ToolTipIcon.Info);
                            }

                            if (!dt.Columns.Contains("DogumaKalanSure"))
                                dt.Columns.Add("DogumaKalanSure", typeof(string));

                            foreach (DataRow row in dt.Rows)
                            {
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

                                if (row["Cinsi"] == DBNull.Value) row["Cinsi"] = "Bilinmiyor";
                                if (row["Telefon"] == DBNull.Value) row["Telefon"] = "Bilinmiyor";
                                if (row["HayvanSahibi"] == DBNull.Value) row["HayvanSahibi"] = "Bilinmiyor";
                            }

                            originalDataTable = dt.Copy();
                            currentDataTable = dt.Copy();
                            Invoke((MethodInvoker)(() =>
                            {
                                dataGridView1.DataSource = null;
                                dataGridView1.DataSource = currentDataTable;
                                if (currentDataTable != null && currentDataTable.Rows.Count > 0)
                                {
                                    dataGridView1.ClearSelection();
                                    if (dataGridView1.Rows.Count > 0)
                                    {
                                        dataGridView1.Rows[0].Selected = true;
                                    }
                                }
                                dataGridView1.Refresh();
                                dataGridView1.Visible = true; // Ensure visibility
                                Console.WriteLine($"UpdateDataTable: DataGridView {currentDataTable?.Rows.Count ?? 0} satır ile güncellendi.");
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Veri güncellenirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"UpdateDataTable Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
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
                    if (dataGridView1.Rows.Count == 0 || currentDataTable == null || currentDataTable.Rows.Count == 0)
                    {
                        return;
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows.Cast<DataGridViewRow>().ToList())
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

                                if (kalanSure.TotalDays <= 7 && kalanSure.TotalSeconds > 0)
                                {
                                    row.DefaultCellStyle.BackColor = Color.Red;
                                    row.DefaultCellStyle.ForeColor = Color.White;

                                    string? id = row.Cells["ID"].Value?.ToString();
                                    if (!string.IsNullOrEmpty(id))
                                    {
                                        if (!lastEmailSentTimes.ContainsKey(id) ||
                                            (DateTime.Now - lastEmailSentTimes[id]).TotalHours >= 24)
                                        {
                                            _ = SendEmailNotification(row);
                                            lastEmailSentTimes[id] = DateTime.Now;

                                            ShowSystemTrayNotification("Bilgilendirme",
                                                $"ID {id} için e-posta gönderildi. Doğuma kalan süre: {row.Cells["DogumaKalanSure"].Value}",
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
            if (notifyIcon == null) return;
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
                string? gondericiEmail = null;
                string? uygulamaSifresi = null;

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT email, password FROM mail LIMIT 1";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = (SQLiteDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            gondericiEmail = reader["email"]?.ToString() ?? string.Empty;
                            uygulamaSifresi = reader["password"]?.ToString() ?? string.Empty;
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
                        mailMessage.Subject = $"Doğum Uyarısı (Kedi) - ID: {row.Cells["ID"].Value?.ToString() ?? "Bilinmeyen"}";
                        mailMessage.Body = $"ID: {row.Cells["ID"].Value?.ToString() ?? "Bilinmeyen"}\n" +
                                         $"Hayvan Sahibi: {row.Cells["HayvanSahibi"].Value?.ToString() ?? "Bilinmeyen"}\n" +
                                         $"Doğuma Kalan Süre: {row.Cells["DogumaKalanSure"].Value?.ToString() ?? "Bilinmeyen"}\n" +
                                         $"Telefon: {row.Cells["Telefon"].Value?.ToString() ?? "Bilinmeyen"}\n" +
                                         $"Cinsi: {row.Cells["Cinsi"].Value?.ToString() ?? "Bilinmeyen"}";
                        mailMessage.IsBodyHtml = false;

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                PlayNotificationSound(@"C:\Users\ridva\Desktop\dutdut\my-project\veteriner_otomasyon\kedi.wav");
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"E-posta gönderilirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"SendEmailNotification Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
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
                Console.WriteLine($"PlayNotificationSound Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private void FilterDataGridView()
        {
            string filter = textBox4.Text?.Trim() ?? string.Empty;
            _ = UpdateDataTable(filter);
        }

        private void StyleDataGridView()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Regular);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            if (dataGridView1.Columns.Count > 0)
            {
                if (dataGridView1.Columns.Contains("ID")) dataGridView1.Columns["ID"].Width = 80;
                if (dataGridView1.Columns.Contains("Telefon")) dataGridView1.Columns["Telefon"].Width = 100;
                if (dataGridView1.Columns.Contains("Cinsi")) dataGridView1.Columns["Cinsi"].Width = 100;
                if (dataGridView1.Columns.Contains("HayvanSahibi")) dataGridView1.Columns["HayvanSahibi"].Width = 150;
                if (dataGridView1.Columns.Contains("GebelikDurumu")) dataGridView1.Columns["GebelikDurumu"].Width = 100;
                if (dataGridView1.Columns.Contains("KayitTarihi")) dataGridView1.Columns["KayitTarihi"].Width = 150;
                if (dataGridView1.Columns.Contains("DogumTarihi")) dataGridView1.Columns["DogumTarihi"].Width = 150;
                if (dataGridView1.Columns.Contains("DogumaKalanSure")) dataGridView1.Columns["DogumaKalanSure"].Width = 150;
            }

            dataGridView1.Visible = true; // Ensure visibility
        }

        private void DataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            // No auto-fill for textBox1, textBox2, textBox3, checkBox1, checkBox2
        }

        private async void button1_Click(object? sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                string? telefon = textBox1.Text?.Trim();
                string? cinsi = textBox2.Text?.Trim();
                string? hayvanSahibi = textBox3.Text?.Trim();
                string gebelikDurumu = checkBox1.Checked ? "Gebe" : checkBox2.Checked ? "Gebe Değil" : string.Empty;

                if (string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(cinsi) || string.IsNullOrWhiteSpace(hayvanSahibi) || string.IsNullOrEmpty(gebelikDurumu))
                {
                    ShowSystemTrayNotification("Hata", "Telefon, Cinsi, Hayvan Sahibi ve Gebelik Durumu boş bırakılamaz!", ToolTipIcon.Warning);
                    return;
                }

                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();
                    using (SQLiteTransaction transaction = con.BeginTransaction())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM kedi_gebelik WHERE Telefon = @Telefon";
                        using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Telefon", telefon);
                            long count = (long)(await checkCmd.ExecuteScalarAsync() ?? 0);
                            if (count > 0)
                            {
                                ShowSystemTrayNotification("Hata", $"Telefon numarası '{telefon}' zaten kayıtlı!", ToolTipIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                        }

                        string insertQuery = @"
                            INSERT INTO kedi_gebelik (Telefon, Cinsi, HayvanSahibi, GebelikDurumu, KayitTarihi, DogumTarihi, DogumaKalanSure)
                            VALUES (@Telefon, @Cinsi, @HayvanSahibi, @GebelikDurumu, @KayitTarihi, @DogumTarihi, @DogumaKalanSure)";

                        using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, con, transaction))
                        {
                            object dogumTarihi = gebelikDurumu == "Gebe" ? (object)DateTime.Now.AddDays(63).ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value;
                            string dogumaKalanSure = gebelikDurumu == "Gebe" ? "63 gün 0 saat 0 dakika" : "Bilinmiyor";

                            insertCmd.Parameters.AddWithValue("@Telefon", telefon);
                            insertCmd.Parameters.AddWithValue("@Cinsi", cinsi);
                            insertCmd.Parameters.AddWithValue("@HayvanSahibi", hayvanSahibi);
                            insertCmd.Parameters.AddWithValue("@GebelikDurumu", gebelikDurumu);
                            insertCmd.Parameters.AddWithValue("@KayitTarihi", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            insertCmd.Parameters.AddWithValue("@DogumTarihi", dogumTarihi);
                            insertCmd.Parameters.AddWithValue("@DogumaKalanSure", dogumaKalanSure);

                            await insertCmd.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                    }
                }

                await UpdateDataTable();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                checkBox1.Checked = false;
                checkBox2.Checked = false;

                ShowSystemTrayNotification("Bilgi", "Kayıt başarıyla eklendi!", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Kayıt eklenirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"button1_Click Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
            finally
            {
                timer.Start();
            }
        }

        private async void button2_Click(object? sender, EventArgs e)
        {
            try
            {
                isSaving = true;
                timer.Stop();

                if (dataGridView1.SelectedRows.Count == 0)
                {
                    ShowSystemTrayNotification("Hata", "Lütfen güncellemek için bir satır seçin!", ToolTipIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string? id = selectedRow.Cells["ID"].Value?.ToString();
                if (string.IsNullOrEmpty(id))
                {
                    ShowSystemTrayNotification("Hata", "Seçili satırın kimliği bulunamadı!", ToolTipIcon.Error);
                    return;
                }

                string? telefon = textBox1.Text?.Trim();
                string? cinsi = textBox2.Text?.Trim();
                string? hayvanSahibi = textBox3.Text?.Trim();
                string gebelikDurumu = checkBox1.Checked ? "Gebe" : checkBox2.Checked ? "Gebe Değil" : string.Empty;

                if (string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(cinsi) || string.IsNullOrWhiteSpace(hayvanSahibi) || string.IsNullOrEmpty(gebelikDurumu))
                {
                    ShowSystemTrayNotification("Hata", "Telefon, Cinsi, Hayvan Sahibi ve Gebelik Durumu boş bırakılamaz!", ToolTipIcon.Warning);
                    return;
                }

                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();
                    using (SQLiteTransaction transaction = con.BeginTransaction())
                    {
                        // Check for duplicate Telefon (excluding the current ID)
                        string checkQuery = "SELECT COUNT(*) FROM kedi_gebelik WHERE Telefon = @Telefon AND ID != @ID";
                        using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@Telefon", telefon);
                            checkCmd.Parameters.AddWithValue("@ID", id);
                            long count = (long)(await checkCmd.ExecuteScalarAsync() ?? 0);
                            if (count > 0)
                            {
                                ShowSystemTrayNotification("Hata", $"Telefon numarası '{telefon}' başka bir kayıtta mevcut!", ToolTipIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                        }

                        string updateQuery = @"
                            UPDATE kedi_gebelik 
                            SET Telefon = @Telefon, 
                                Cinsi = @Cinsi, 
                                HayvanSahibi = @HayvanSahibi, 
                                GebelikDurumu = @GebelikDurumu, 
                                DogumTarihi = @DogumTarihi, 
                                DogumaKalanSure = @DogumaKalanSure
                            WHERE ID = @ID";

                        using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, con, transaction))
                        {
                            object dogumTarihi = gebelikDurumu == "Gebe" ? (object)DateTime.Now.AddDays(63).ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value;
                            string dogumaKalanSure = gebelikDurumu == "Gebe" ? "63 gün 0 saat 0 dakika" : "Bilinmiyor";

                            cmd.Parameters.AddWithValue("@ID", id);
                            cmd.Parameters.AddWithValue("@Telefon", telefon);
                            cmd.Parameters.AddWithValue("@Cinsi", cinsi);
                            cmd.Parameters.AddWithValue("@HayvanSahibi", hayvanSahibi);
                            cmd.Parameters.AddWithValue("@GebelikDurumu", gebelikDurumu);
                            cmd.Parameters.AddWithValue("@DogumTarihi", dogumTarihi);
                            cmd.Parameters.AddWithValue("@DogumaKalanSure", dogumaKalanSure);

                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            if (rowsAffected == 0)
                            {
                                ShowSystemTrayNotification("Hata", $"ID '{id}' güncellenemedi, kayıt bulunamadı!", ToolTipIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                        }

                        transaction.Commit();
                    }
                }

                await UpdateDataTable();
                ShowSystemTrayNotification("Bilgi", $"ID '{id}' başarıyla güncellendi!", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Güncelleme sırasında hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"button2_Click Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
            finally
            {
                isSaving = false;
                timer.Start();
            }
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            // Disabled: Deletion is handled via Delete key
            ShowSystemTrayNotification("Bilgi", "Silme işlemi için lütfen satırı seçip Delete tuşuna basın.", ToolTipIcon.Info);
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form1? form1 = Application.OpenForms["Form1"] as Form1 ?? new Form1();
                form1.Show();
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Form geçişinde hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"button4_Click Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private async void button5_Click(object? sender, EventArgs e)
        {
            // Optional: Alternative save button for cell-based updates
            await SaveDataGridViewChanges();
        }

        private void pictureBox5_Click(object? sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form8? form8 = Application.OpenForms["Form8"] as Form8;
                if (form8 != null)
                {
                    form8.Show();
                }
                else
                {
                    form8 = new Form8();
                    form8.Show();
                }
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Form geçişinde hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"pictureBox5_Click Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private async Task SaveDataGridViewChanges()
        {
            try
            {
                isSaving = true;
                timer.Stop();

                dataGridView1.EndEdit();

                if (currentDataTable == null)
                {
                    ShowSystemTrayNotification("Hata", "Güncel veri tablosu boş!", ToolTipIcon.Error);
                    return;
                }

                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    await con.OpenAsync();
                    using (SQLiteTransaction transaction = con.BeginTransaction())
                    {
                        // Handle updated rows only (deletions are handled by KeyDown)
                        var activeRows = currentDataTable.AsEnumerable()
                            .Where(row => row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                            .ToList();

                        foreach (var row in activeRows)
                        {
                            string? id = row["ID"]?.ToString();
                            string? gebelikDurumu = row["GebelikDurumu"]?.ToString();
                            DateTime? dogumTarihi = row["DogumTarihi"] != DBNull.Value ? (DateTime?)row["DogumTarihi"] : null;
                            string? dogumaKalanSure = row["DogumaKalanSure"]?.ToString();

                            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(gebelikDurumu)) continue;

                            string updateQuery = @"
                                UPDATE kedi_gebelik 
                                SET GebelikDurumu = @GebelikDurumu, DogumTarihi = @DogumTarihi, DogumaKalanSure = @DogumaKalanSure
                                WHERE ID = @ID";

                            using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.Parameters.AddWithValue("@GebelikDurumu", gebelikDurumu);
                                cmd.Parameters.AddWithValue("@DogumTarihi", dogumTarihi.HasValue ? dogumTarihi.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);
                                cmd.Parameters.AddWithValue("@DogumaKalanSure", dogumaKalanSure ?? "Bilinmiyor");

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                    }

                    await UpdateDataTable(); // Refresh after saving
                }

                ShowSystemTrayNotification("Bilgi", "Değişiklikler başarıyla kaydedildi.", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Değişiklikler kaydedilirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"SaveDataGridViewChanges Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
            finally
            {
                isSaving = false;
                timer.Start();
            }
        }

        private void dataGridView1_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["GebelikDurumu"].Index)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    string? currentGebelikDurumu = row.Cells["GebelikDurumu"].Value?.ToString();

                    if (!string.IsNullOrEmpty(currentGebelikDurumu))
                    {
                        string newGebelikDurumu = currentGebelikDurumu == "Gebe" ? "Gebe Değil" : "Gebe";
                        object newDogumTarihi = newGebelikDurumu == "Gebe" ? (object)DateTime.Now.AddDays(63) : DBNull.Value;
                        string newDogumaKalanSure = newGebelikDurumu == "Gebe" ? "63 gün 0 saat 0 dakika" : "Bilinmiyor";

                        row.Cells["GebelikDurumu"].Value = newGebelikDurumu;
                        row.Cells["DogumTarihi"].Value = newDogumTarihi;
                        row.Cells["DogumaKalanSure"].Value = newDogumaKalanSure;

                        ShowSystemTrayNotification("Bilgi", $"Gebelik Durumu '{newGebelikDurumu}' olarak değiştirildi. Güncellemek için Kaydet butonuna tıklayın.", ToolTipIcon.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSystemTrayNotification("Hata", $"Gebelik Durumu değiştirilirken hata oluştu: {ex.Message}", ToolTipIcon.Error);
                Console.WriteLine($"dataGridView1_CellContentClick Hata: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            timer.Stop();
            timer.Dispose();
            filterCancellationTokenSource?.Dispose();
            base.OnFormClosing(e);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
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
    }
}