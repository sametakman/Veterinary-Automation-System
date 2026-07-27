using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite; // SQLite için gerekli paket

namespace WinFormsApp8
{
    public partial class Form11 : Form
    {
        private readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;";
        private DataTable? originalIslemlerDataTable; // Nullable to avoid initialization warning
        private DataTable? originalAsiBilgisiDataTable; // Nullable to avoid initialization warning
        private readonly System.Windows.Forms.Timer refreshTimer; // Explicitly use Windows Forms Timer

        public Form11()
        {
            InitializeComponent();
            LoadIslemlerData();  // DataGridView1 için verileri yükle
            LoadAsiBilgisiData();  // DataGridView3 için verileri yükle
            textBox2.TextChanged += TextBox2_TextChanged; // DataGridView1 için TextChanged
            textBox1.TextChanged += TextBox1_TextChanged; // DataGridView3 için TextChanged

            // Timer'ı başlat
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // 5 saniyede bir yenile
            };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        // Timer her tetiklendiğinde verileri yenile
        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            LoadIslemlerData();  // DataGridView1 için verileri yenile
            LoadAsiBilgisiData();  // DataGridView3 için verileri yenile
        }

        // Verileri DataGridView1'e yüklemek için method (Islemler tablosu)
        private void LoadIslemlerData()
        {
            try
            {
                using (var con = new SqliteConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT ID, AdSoyad, TelefonNo, YapilanIslem, IslemUcreti, OdemeSekli, KayitTarihi, Borç FROM Islemler LIMIT 1000";
                    using (var cmd = new SqliteCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        // DataTable oluştur ve sütunları tanımla
                        originalIslemlerDataTable = new DataTable();
                        originalIslemlerDataTable.Columns.Add("ID", typeof(long));
                        originalIslemlerDataTable.Columns.Add("AdSoyad", typeof(string));
                        originalIslemlerDataTable.Columns.Add("TelefonNo", typeof(string));
                        originalIslemlerDataTable.Columns.Add("YapilanIslem", typeof(string));
                        originalIslemlerDataTable.Columns.Add("IslemUcreti", typeof(double));
                        originalIslemlerDataTable.Columns.Add("OdemeSekli", typeof(string));
                        originalIslemlerDataTable.Columns.Add("KayitTarihi", typeof(string));
                        originalIslemlerDataTable.Columns.Add("Borç", typeof(double));

                        // Verileri oku ve DataTable'a ekle
                        while (reader.Read())
                        {
                            var row = originalIslemlerDataTable.NewRow();
                            row["ID"] = reader.GetInt64(0);
                            row["AdSoyad"] = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            row["TelefonNo"] = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            row["YapilanIslem"] = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            row["IslemUcreti"] = reader.IsDBNull(4) ? 0.0 : reader.GetDouble(4);
                            row["OdemeSekli"] = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                            row["KayitTarihi"] = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                            row["Borç"] = reader.IsDBNull(7) ? 0.0 : reader.GetDouble(7);
                            originalIslemlerDataTable.Rows.Add(row);
                        }

                        // Verileri DataGridView1'e yükle
                        dataGridView1.DataSource = originalIslemlerDataTable;

                        // DataGridView1 özelleştirmeleri
                        CustomizeDataGridView1();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Veri yüklenirken hata oluştu.");
            }
        }

        // DataGridView1 düzenlemeleri (Islemler tablosu için)
        private void CustomizeDataGridView1()
        {
            if (dataGridView1.Columns.Count == 0) return;

            dataGridView1.Columns["ID"].Width = 80;
            dataGridView1.Columns["AdSoyad"].Width = 150;
            dataGridView1.Columns["TelefonNo"].Width = 120;
            dataGridView1.Columns["YapilanIslem"].Width = 150;
            dataGridView1.Columns["IslemUcreti"].Width = 100;
            dataGridView1.Columns["OdemeSekli"].Width = 100;
            dataGridView1.Columns["KayitTarihi"].Width = 120;
            dataGridView1.Columns["Borç"].Width = 100;

            dataGridView1.Columns["ID"].HeaderText = "ID";
            dataGridView1.Columns["AdSoyad"].HeaderText = "Ad Soyad";
            dataGridView1.Columns["TelefonNo"].HeaderText = "Telefon No";
            dataGridView1.Columns["YapilanIslem"].HeaderText = "Yapılan İşlem";
            dataGridView1.Columns["IslemUcreti"].HeaderText = "İşlem Ücreti";
            dataGridView1.Columns["OdemeSekli"].HeaderText = "Ödeme Şekli";
            dataGridView1.Columns["KayitTarihi"].HeaderText = "Kayıt Tarihi";
            dataGridView1.Columns["Borç"].HeaderText = "Borç";

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        // Verileri DataGridView3'e yüklemek için method (AsiBilgisi tablosu)
        private void LoadAsiBilgisiData()
        {
            try
            {
                using (var con = new SqliteConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT AşıAdı, Periyot, HayvanID, HayvanSahibi, Telefon, KupeNo, Cinsi, Adres, Durum, DigerDoz, KayitTarihi FROM AsiBilgisi LIMIT 1000";
                    using (var cmd = new SqliteCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        // DataTable oluştur ve sütunları tanımla
                        originalAsiBilgisiDataTable = new DataTable();
                        originalAsiBilgisiDataTable.Columns.Add("AşıAdı", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("Periyot", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("HayvanID", typeof(long));
                        originalAsiBilgisiDataTable.Columns.Add("HayvanSahibi", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("Telefon", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("KupeNo", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("Cinsi", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("Adres", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("Durum", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("DigerDoz", typeof(string));
                        originalAsiBilgisiDataTable.Columns.Add("KayitTarihi", typeof(string));

                        // Verileri oku ve DataTable'a ekle
                        while (reader.Read())
                        {
                            var row = originalAsiBilgisiDataTable.NewRow();
                            row["AşıAdı"] = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            row["Periyot"] = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            row["HayvanID"] = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);
                            row["HayvanSahibi"] = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            row["Telefon"] = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            row["KupeNo"] = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                            row["Cinsi"] = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                            row["Adres"] = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                            row["Durum"] = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);
                            row["DigerDoz"] = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
                            row["KayitTarihi"] = reader.IsDBNull(10) ? string.Empty : reader.GetString(10);
                            originalAsiBilgisiDataTable.Rows.Add(row);
                        }

                        // Verileri DataGridView3'e yükle
                        dataGridView3.DataSource = originalAsiBilgisiDataTable;

                        // DataGridView3 özelleştirmeleri
                        CustomizeDataGridView3();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Veri yüklenirken hata oluştu.");
            }
        }

        // DataGridView3 düzenlemeleri (AsiBilgisi tablosu için)
        private void CustomizeDataGridView3()
        {
            if (dataGridView3.Columns.Count == 0) return;

            dataGridView3.Columns["AşıAdı"].Width = 120;
            dataGridView3.Columns["Periyot"].Width = 100;
            dataGridView3.Columns["HayvanID"].Width = 80;
            dataGridView3.Columns["HayvanSahibi"].Width = 150;
            dataGridView3.Columns["Telefon"].Width = 120;
            dataGridView3.Columns["KupeNo"].Width = 100;
            dataGridView3.Columns["Cinsi"].Width = 100;
            dataGridView3.Columns["Adres"].Width = 150;
            dataGridView3.Columns["Durum"].Width = 100;
            dataGridView3.Columns["DigerDoz"].Width = 100;
            dataGridView3.Columns["KayitTarihi"].Width = 120;

            dataGridView3.Columns["AşıAdı"].HeaderText = "Aşı Adı";
            dataGridView3.Columns["Periyot"].HeaderText = "Periyot";
            dataGridView3.Columns["HayvanID"].HeaderText = "Hayvan ID";
            dataGridView3.Columns["HayvanSahibi"].HeaderText = "Hayvan Sahibi";
            dataGridView3.Columns["Telefon"].HeaderText = "Telefon";
            dataGridView3.Columns["KupeNo"].HeaderText = "Kupe No";
            dataGridView3.Columns["Cinsi"].HeaderText = "Cinsi";
            dataGridView3.Columns["Adres"].HeaderText = "Adres";
            dataGridView3.Columns["Durum"].HeaderText = "Durum";
            dataGridView3.Columns["DigerDoz"].HeaderText = "Diğer Doz";
            dataGridView3.Columns["KayitTarihi"].HeaderText = "Kayıt Tarihi";

            dataGridView3.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        // TextBox2 ile filtreleme işlemi (DataGridView1 - Islemler) - Sadece AdSoyad sütununa göre
        private void TextBox2_TextChanged(object? sender, EventArgs e)
        {
            if (originalIslemlerDataTable == null) return;

            string filterText = textBox2.Text.Trim();
            var filteredTable = originalIslemlerDataTable.Clone();

            foreach (DataRow row in originalIslemlerDataTable.Rows)
            {
                if (row["AdSoyad"]?.ToString()?.Contains(filterText, StringComparison.OrdinalIgnoreCase) == true)
                {
                    filteredTable.ImportRow(row);
                }
            }
            dataGridView1.DataSource = filteredTable;
        }

        // TextBox1 ile filtreleme işlemi (DataGridView3 - AsiBilgisi) - Sadece KupeNo sütununa göre
        private void TextBox1_TextChanged(object? sender, EventArgs e)
        {
            if (originalAsiBilgisiDataTable == null) return;

            string filterText = textBox1.Text.Trim();
            var filteredTable = originalAsiBilgisiDataTable.Clone();

            foreach (DataRow row in originalAsiBilgisiDataTable.Rows)
            {
                if (row["KupeNo"]?.ToString()?.Contains(filterText, StringComparison.OrdinalIgnoreCase) == true)
                {
                    filteredTable.ImportRow(row);
                }
            }
            dataGridView3.DataSource = filteredTable;
        }

        // Form6'ya geri dönme butonu
        private void pictureBox2_Click(object? sender, EventArgs e)
        {
            refreshTimer.Stop(); // Timer'ı durdur
            this.Hide();
            if (Application.OpenForms["Form6"] is Form6 form6)
            {
                form6.Show();
            }
            else
            {
                form6 = new Form6();
                form6.Show();
            }
        }

        // Form yüklenirken DataGridView'lerin stil ayarlarını yap
        private void Form11_Load(object? sender, EventArgs e)
        {
            // DataGridView1 için stil ayarları (Islemler tablosu)
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

            // DataGridView3 için stil ayarları (AsiBilgisi tablosu)
            dataGridView3.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView3.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dataGridView3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.EnableHeadersVisualStyles = false;

            dataGridView3.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Regular);
            dataGridView3.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView3.DefaultCellStyle.BackColor = Color.White;
            dataGridView3.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridView3.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        // Form kapanırken Timer'ı durdur
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimer.Stop();
            refreshTimer.Dispose();
            base.OnFormClosing(e);
        }
    }
}