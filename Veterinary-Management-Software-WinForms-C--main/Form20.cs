using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.IO; // Added for file existence check

namespace WinFormsApp8
{
    public partial class Form20 : Form
    {
        private readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;";
        private Panel? label5Panel;

        public Form20()
        {
            InitializeComponent();

            // Check if the database file exists
            if (!File.Exists(@"C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db"))
            {
                MessageBox.Show("Veritabanı dosyası bulunamadı! Lütfen dosyanın şu konumda olduğundan emin olun: " +
                                @"C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CreateTableIfNotExists();
            LoadDataGridView();

            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            checkBox2.CheckedChanged += CheckBox2_CheckedChanged;
            button1.Click += button1_Click;
            button2.Click += button2_Click;

            SetupLabel5Panel();
            this.AutoScroll = true;
            label5.Text = "Sonuçlar burada görüntülenecek. Lütfen belirtileri arayın.";
        }

        private void SetupLabel5Panel()
        {
            Point label5Location = new Point(dataGridView1.Location.X, dataGridView1.Location.Y + dataGridView1.Height + 10);
            Size label5Size = new Size(500, 100);

            label5Panel = new Panel
            {
                Location = label5Location,
                Size = label5Size,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            label5.Location = new Point(0, 0);
            label5.AutoSize = false;
            label5.Size = new Size(label5Panel.ClientSize.Width, 80);
            label5.BackColor = Color.LightGray;
            label5.ForeColor = Color.DarkBlue;
            label5.Font = new Font("Consolas", 12, FontStyle.Regular);
            label5.Padding = new Padding(5, 5, 0, 5);
            label5.TextAlign = ContentAlignment.TopLeft;

            label5Panel.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, label5Panel.Width - 1, label5Panel.Height - 1);
                }
            };

            label5Panel.Controls.Add(label5);
            this.Controls.Add(label5Panel);
        }

        private void CheckBox1_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkBox1.Checked) checkBox2.Checked = false;
        }

        private void CheckBox2_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkBox2.Checked) checkBox1.Checked = false;
        }

        private void CreateTableIfNotExists()
        {
            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        CREATE TABLE IF NOT EXISTS HayvanHastaliklari (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            kedi_hastalik_adi TEXT,
                            kedi_belirtiler TEXT,
                            kedi_tedavi TEXT,
                            kopek_hastalik_adi TEXT,
                            kopek_belirtiler TEXT,
                            kopek_tedavi TEXT
                        )";
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Tablo oluşturulamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataGridView()
        {
            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM HayvanHastaliklari";
                    using (var cmd = new SqliteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Veri yüklenemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(richTextBox1.Text) ||
                string.IsNullOrWhiteSpace(richTextBox2.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!checkBox1.Checked && !checkBox2.Checked)
            {
                MessageBox.Show("Lütfen hayvan türünü seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = checkBox1.Checked
                ? "INSERT INTO HayvanHastaliklari (kedi_hastalik_adi, kedi_belirtiler, kedi_tedavi) VALUES (@adi, @belirti, @tedavi)"
                : "INSERT INTO HayvanHastaliklari (kopek_hastalik_adi, kopek_belirtiler, kopek_tedavi) VALUES (@adi, @belirti, @tedavi)";

            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqliteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@adi", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@belirti", richTextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@tedavi", richTextBox2.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Kayıt eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataGridView();

                textBox1.Clear();
                richTextBox1.Clear();
                richTextBox2.Clear();
                checkBox1.Checked = false;
                checkBox2.Checked = false;
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Kayıt eklenemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            if (!checkBox1.Checked && !checkBox2.Checked)
            {
                MessageBox.Show("Lütfen kedi veya köpek seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string girilenBelirti = richTextBox1.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(girilenBelirti))
            {
                MessageBox.Show("Belirti giriniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string hayvanTuru = checkBox1.Checked ? "Kedi" : "Köpek";
            string query = "SELECT * FROM HayvanHastaliklari";
            var sonucListesi = new System.Collections.Generic.List<(string Hastalik, double Yuzde, string? Tedavi)>();

            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqliteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string? hastalikAdi = null;
                            string? belirtiler = null;
                            string? tedavi = null;

                            if (checkBox1.Checked)
                            {
                                hastalikAdi = reader["kedi_hastalik_adi"]?.ToString();
                                belirtiler = reader["kedi_belirtiler"]?.ToString();
                                tedavi = reader["kedi_tedavi"]?.ToString();
                            }
                            else
                            {
                                hastalikAdi = reader["kopek_hastalik_adi"]?.ToString();
                                belirtiler = reader["kopek_belirtiler"]?.ToString();
                                tedavi = reader["kopek_tedavi"]?.ToString();
                            }

                            if (!string.IsNullOrWhiteSpace(hastalikAdi) && !string.IsNullOrWhiteSpace(belirtiler))
                            {
                                string[] girilenKelimeler = girilenBelirti.Split(new[] { ' ', ',', '.', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                int ortak = girilenKelimeler.Count(k => belirtiler.ToLower().Contains(k));
                                double yuzde = (double)ortak / girilenKelimeler.Length * 100;

                                if (yuzde > 0)
                                {
                                    sonucListesi.Add((hastalikAdi, yuzde, tedavi));
                                }
                            }
                        }
                    }
                }

                UpdateLabel5(hayvanTuru, girilenBelirti, sonucListesi);
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Arama sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label5.Text = "Arama sırasında hata oluştu.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label5.Text = "Beklenmeyen hata oluştu.";
            }
        }

        private void UpdateLabel5(string hayvanTuru, string girilenBelirti, System.Collections.Generic.List<(string Hastalik, double Yuzde, string? Tedavi)> sonucListesi)
        {
            if (label5Panel == null) return;

            StringBuilder rapor = new StringBuilder();
            rapor.AppendLine("=================================");
            rapor.AppendLine("      HASTALIK TESPİT RAPORU     ");
            rapor.AppendLine("=================================");
            rapor.AppendLine($"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            rapor.AppendLine($"Hayvan Türü: {hayvanTuru}");
            rapor.AppendLine($"Girilen Belirtiler: {girilenBelirti}");
            rapor.AppendLine();

            if (!sonucListesi.Any())
            {
                rapor.AppendLine("---------------------------------");
                rapor.AppendLine("SONUÇ: Benzer hastalık bulunamadı.");
                rapor.AppendLine("---------------------------------");
            }
            else
            {
                var top3Sonuclar = sonucListesi.OrderByDescending(x => x.Yuzde).Take(3).ToList();
                rapor.AppendLine("---------------------------------");
                rapor.AppendLine("En Yüksek İhtimalle 3 Hastalık ve Tedaviler");
                rapor.AppendLine("---------------------------------");

                int maxHastalikLength = top3Sonuclar.Max(x => x.Hastalik.Length);
                int maxYuzdeLength = top3Sonuclar.Max(x => $"{x.Yuzde:F1}".Length);
                maxHastalikLength = Math.Max(maxHastalikLength, "Hastalik Adı".Length);
                maxYuzdeLength = Math.Max(maxYuzdeLength, "Olasılık (%)".Length);

                rapor.AppendLine($"{("Hastalik Adı").PadRight(maxHastalikLength)} {("Olasılık (%)").PadRight(maxYuzdeLength)} Tedavi");
                rapor.AppendLine(new string('-', maxHastalikLength + maxYuzdeLength + " Tedavi".Length + 2));

                foreach (var (hastalik, yuzde, tedavi) in top3Sonuclar)
                {
                    string yuzdeStr = $"{yuzde:F1}";
                    string tedaviStr = tedavi ?? "Tedavi bilgisi yok";
                    rapor.AppendLine($"{hastalik.PadRight(maxHastalikLength)} {yuzdeStr.PadRight(maxYuzdeLength)} {tedaviStr}");
                }
            }

            rapor.AppendLine();
            rapor.AppendLine("=================================");
            rapor.AppendLine("Veteriner Otomasyon Sistemi");
            rapor.AppendLine("=================================");

            int baseHeight = 100;
            int lineHeight = 20;
            int numberOfLines = rapor.ToString().Split('\n').Length;
            int newHeight = baseHeight + (numberOfLines * lineHeight);
            int maxHeight = 600;
            newHeight = Math.Min(newHeight, maxHeight);

            string[] lines = rapor.ToString().Split('\n');
            using (Graphics g = label5.CreateGraphics())
            {
                int newWidth = 0;
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    SizeF textSize = g.MeasureString(line, label5.Font);
                    newWidth = Math.Max(newWidth, (int)textSize.Width);
                }
                newWidth += 10;
                newWidth = Math.Max(500, newWidth);
                newWidth = Math.Min(newWidth, 800);
                label5Panel.Size = new Size(newWidth, newHeight);
                label5.Size = new Size(newWidth, newHeight);
            }

            label5.Text = rapor.ToString();
            label5Panel.Invalidate();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = Application.OpenForms["Form1"] as Form1;
            if (form1 != null)
            {
                form1.Show();
            }
            else
            {
                form1 = new Form1();
                form1.Show();
            }
        }
    }
}