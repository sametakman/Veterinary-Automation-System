using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinFormsApp8
{
    public partial class Form17 : Form
    {
        private readonly string dbPath = @"C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db";
        private readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;";
        private readonly System.Windows.Forms.Timer timer;
        private readonly HashSet<string> notifiedRecords = new HashSet<string>();
        private DataTable dataTable; // Store DataTable as a class-level variable to avoid repeated database queries in timer

        public Form17()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            InitializeTimer();
            Load += Form17_Load;
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
        }

        private void InitializeTimer()
        {
            timer.Interval = 1000; // Update every second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateDataGridViewCountdown();
        }

        private void Form17_Load(object? sender, EventArgs e)
        {
            try
            {
                CheckDatabaseFile();
                CreateTableIfNotExists();
                LoadDataGridView();
                StyleDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(135, 206, 250);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.GridColor = Color.FromArgb(200, 200, 200);
            dataGridView1.BorderStyle = BorderStyle.FixedSingle;
        }

        private void CheckDatabaseFile()
        {
            if (!File.Exists(dbPath))
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open(); // Creates the database file
                }
                MessageBox.Show("Veritabanı dosyası oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CreateTableIfNotExists()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Hatirlatmalar (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        hatirlatma_adi TEXT NOT NULL,
                        icerik TEXT NOT NULL,
                        saat DATETIME NOT NULL,
                        kalan_saat TEXT NOT NULL,
                        gun INTEGER
                    )";
                using (SqliteCommand cmd = new SqliteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadDataGridView()
        {
            try
            {
                dataTable = new DataTable();
                dataTable.Columns.Add("id", typeof(long));
                dataTable.Columns.Add("hatirlatma_adi", typeof(string));
                dataTable.Columns.Add("icerik", typeof(string));
                dataTable.Columns.Add("saat", typeof(string));
                dataTable.Columns.Add("kalan_saat", typeof(string));
                dataTable.Columns.Add("gun", typeof(int));
                dataTable.Columns.Add("kalan_saat_display", typeof(string)); // Display column

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, hatirlatma_adi, icerik, saat, kalan_saat, gun FROM Hatirlatmalar ORDER BY saat ASC";
                    using (SqliteCommand cmd = new SqliteCommand(query, conn))
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["id"] = reader.GetInt64(0);
                            row["hatirlatma_adi"] = reader.GetString(1);
                            row["icerik"] = reader.GetString(2);
                            row["saat"] = reader.GetString(3);
                            row["kalan_saat"] = reader.GetString(4);
                            row["gun"] = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

                            if (DateTime.TryParse(row["saat"]?.ToString(), out DateTime reminderTime))
                            {
                                TimeSpan remainingTime = reminderTime - DateTime.Now;
                                row["kalan_saat_display"] = remainingTime.TotalSeconds > 0
                                    ? $"{remainingTime.Days:D2} Gün {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}"
                                    : "Hatırlatma süresi bitti!";
                            }
                            else
                            {
                                row["kalan_saat_display"] = "Geçersiz Saat";
                            }

                            dataTable.Rows.Add(row);
                        }
                    }
                }

                dataGridView1.DataSource = dataTable;

                // Configure columns
                if (dataGridView1.Columns["kalan_saat"] != null)
                    dataGridView1.Columns["kalan_saat"].Visible = false;
                if (dataGridView1.Columns["kalan_saat_display"] != null)
                {
                    dataGridView1.Columns["kalan_saat_display"].HeaderText = "Kalan Süre";
                    dataGridView1.Columns["kalan_saat_display"].Width = 120;
                }
                if (dataGridView1.Columns["hatirlatma_adi"] != null)
                    dataGridView1.Columns["hatirlatma_adi"].HeaderText = "Hatırlatma Adı";
                if (dataGridView1.Columns["icerik"] != null)
                    dataGridView1.Columns["icerik"].HeaderText = "İçerik";
                if (dataGridView1.Columns["saat"] != null)
                    dataGridView1.Columns["saat"].HeaderText = "Saat";
                if (dataGridView1.Columns["gun"] != null)
                    dataGridView1.Columns["gun"].HeaderText = "Gün";

                UpdateDataGridViewRowColors(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDataGridViewCountdown()
        {
            if (dataTable == null) return;

            bool hasExpiredRecords = false;
            var messageBuilder = new StringBuilder();

            foreach (DataRow row in dataTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                if (DateTime.TryParse(row["saat"]?.ToString(), out DateTime reminderTime))
                {
                    TimeSpan remainingTime = reminderTime - DateTime.Now;
                    string recordKey = $"{row["id"]}";

                    if (remainingTime.TotalSeconds <= 0)
                    {
                        row["kalan_saat_display"] = "Hatırlatma süresi bitti!";
                        if (!notifiedRecords.Contains(recordKey))
                        {
                            messageBuilder.AppendLine($"⚠️ {row["hatirlatma_adi"]} hatırlatmasının süresi bitmiştir!");
                            hasExpiredRecords = true;
                            notifiedRecords.Add(recordKey);
                        }
                    }
                    else if (remainingTime.TotalHours <= 3 && !notifiedRecords.Contains(recordKey))
                    {
                        row["kalan_saat_display"] = $"{remainingTime.Days:D2} Gün {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                        notifiedRecords.Add(recordKey);
                    }
                    else
                    {
                        row["kalan_saat_display"] = $"{remainingTime.Days:D2} Gün {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                    }

                    row["kalan_saat"] = remainingTime.TotalSeconds > 0
                        ? $"{(int)remainingTime.TotalHours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}"
                        : "00:00:00";
                }
                else
                {
                    row["kalan_saat_display"] = "Geçersiz Saat";
                }
            }

            if (hasExpiredRecords)
            {
                MessageBox.Show(messageBuilder.ToString(), "Hatırlatma Süresi Bildirimi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            UpdateDataGridViewRowColors(false);
            dataGridView1.Refresh();
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

                    if (rowView.Row["saat"] != DBNull.Value && DateTime.TryParse(rowView.Row["saat"]?.ToString(), out DateTime reminderTime))
                    {
                        TimeSpan remainingTime = reminderTime - DateTime.Now;
                        if (remainingTime.TotalSeconds <= 0)
                        {
                            gridRow.DefaultCellStyle.BackColor = Color.FromArgb(255, 99, 71); // Tomato
                            gridRow.DefaultCellStyle.ForeColor = Color.White;
                        }
                        else if (remainingTime.TotalHours <= 3)
                        {
                            gridRow.DefaultCellStyle.BackColor = Color.Red; // Red for 3 hours or less
                            gridRow.DefaultCellStyle.ForeColor = Color.White;
                        }
                        else
                        {
                            gridRow.DefaultCellStyle.BackColor = Color.Green; // Green for more than 3 hours
                            gridRow.DefaultCellStyle.ForeColor = Color.Black;
                        }
                    }
                }
            }
        }

        private void DataGridView1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.DataSource is DataTable dt)
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

        private void button1_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Hatırlatma adı boş olamaz!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(richTextBox1.Text))
                {
                    MessageBox.Show("İçerik boş olamaz!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Saat boş olamaz!");
                    return;
                }

                string girilen = textBox2.Text.Trim().Replace(".", ":").Replace(",", ":").Replace("-", ":");
                girilen = Regex.Replace(girilen, @"\s+", "");

                if (Regex.IsMatch(girilen, @"^\d{1,2}$"))
                {
                    girilen = girilen.PadLeft(2, '0') + ":00";
                }
                else if (Regex.IsMatch(girilen, @"^\d{3,4}$"))
                {
                    girilen = girilen.PadLeft(4, '0');
                    girilen = girilen.Insert(2, ":");
                }

                if (!girilen.Contains(":"))
                {
                    MessageBox.Show("Saat formatı geçersiz!");
                    return;
                }

                string tarihliSaat = dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + girilen;

                if (!DateTime.TryParse(tarihliSaat, out DateTime saat))
                {
                    MessageBox.Show("Saat formatı geçersiz! (örn: 14:30)");
                    return;
                }

                TimeSpan kalanSure = saat - DateTime.Now;
                if (kalanSure.TotalSeconds < 0)
                {
                    MessageBox.Show("Girilen saat geçmişte olamaz!");
                    return;
                }

                string kalanSaat = $"{(int)kalanSure.TotalHours:D2}:{kalanSure.Minutes:D2}:{kalanSure.Seconds:D2}";
                int gun = dateTimePicker1.Value.Day;

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    string insertQuery = @"
                        INSERT INTO Hatirlatmalar (hatirlatma_adi, icerik, saat, kalan_saat, gun)
                        VALUES (@hatirlatma_adi, @icerik, @saat, @kalan_saat, @gun)";
                    using (SqliteCommand cmd = new SqliteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@hatirlatma_adi", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@icerik", richTextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@saat", saat.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@kalan_saat", kalanSaat);
                        cmd.Parameters.AddWithValue("@gun", gun);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Kayıt başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBox1.Clear();
                richTextBox1.Clear();
                textBox2.Clear();
                dateTimePicker1.Value = DateTime.Now;

                LoadDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            textBox1.Clear();
            richTextBox1.Clear();
            textBox2.Clear();
            dateTimePicker1.Value = DateTime.Now;
            notifiedRecords.Clear();
        }

        private void pictureBoxSave_Click(object? sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            if (dataGridView1.DataSource is not DataTable dt) return;

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    int updatedRows = 0, deletedRows = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted)
                        {
                            long id = Convert.ToInt64(row["id", DataRowVersion.Original]);
                            using (SqliteCommand cmd = new SqliteCommand("DELETE FROM Hatirlatmalar WHERE id = @id", conn))
                            {
                                cmd.Parameters.AddWithValue("@id", id);
                                if (cmd.ExecuteNonQuery() > 0) deletedRows++;
                            }
                            continue;
                        }

                        long rowId = Convert.ToInt64(row["id"]);
                        string? hatirlatmaAdi = row["hatirlatma_adi"]?.ToString()?.Trim();
                        string? icerik = row["icerik"]?.ToString()?.Trim();
                        string? saatStr = row["saat"]?.ToString()?.Trim();
                        string? kalanSaat = row["kalan_saat"]?.ToString()?.Trim();
                        int gun = row["gun"] != DBNull.Value ? Convert.ToInt32(row["gun"]) : 0;

                        if (string.IsNullOrEmpty(hatirlatmaAdi) || string.IsNullOrEmpty(icerik) || string.IsNullOrEmpty(saatStr)) continue;

                        using (SqliteCommand checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Hatirlatmalar WHERE id = @id", conn))
                        {
                            checkCmd.Parameters.AddWithValue("@id", rowId);
                            if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                            {
                                using (SqliteCommand cmd = new SqliteCommand(
                                    "UPDATE Hatirlatmalar SET hatirlatma_adi = @hatirlatma_adi, icerik = @icerik, saat = @saat, kalan_saat = @kalan_saat, gun = @gun WHERE id = @id", conn))
                                {
                                    cmd.Parameters.AddWithValue("@id", rowId);
                                    cmd.Parameters.AddWithValue("@hatirlatma_adi", hatirlatmaAdi);
                                    cmd.Parameters.AddWithValue("@icerik", icerik);
                                    cmd.Parameters.AddWithValue("@saat", saatStr);
                                    cmd.Parameters.AddWithValue("@kalan_saat", kalanSaat);
                                    cmd.Parameters.AddWithValue("@gun", gun);
                                    if (cmd.ExecuteNonQuery() > 0) updatedRows++;
                                }
                            }
                        }
                    }

                    dt.AcceptChanges();
                    LoadDataGridView();

                    MessageBox.Show(updatedRows == 0 && deletedRows == 0
                        ? "Değişiklik yapılmadı!"
                        : $"Değişiklikler kaydedildi!\nGüncellenen: {updatedRows}, Silinen: {deletedRows}",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    notifiedRecords.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                Form1 form1 = Application.OpenForms["Form1"] as Form1 ?? new Form1();
                form1.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form1 açılırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Clear();             // TextBox1 temizlenir
            richTextBox1.Clear();         // RichTextBox1 temizlenir
            textBox2.Clear();             // TextBox2 temizlenir
            dateTimePicker1.Value = DateTime.Now; // DateTimePicker bugünün tarihi olarak ayarlanır
        }
    }
}