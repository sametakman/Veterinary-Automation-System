using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WinFormsApp8
{
    public partial class Form16 : Form
    {
        private readonly string connectionString = @"Data Source=C:\Users\ridva\Desktop\SQLİTE3_VETERİNEROTOMASYON\veteriner_otomasyon.db;Version=3;";
        private List<Yem> mevcutYemler = new List<Yem>();
        private string? sonHesaplamaMesaji;
        private string? sonOneriPrefix;
        private double eksikHP = 0, eksikP = 0, eksikKM = 0, eksikME = 0, eksikCa = 0;
        private double gunlukYemMiktari = 0;
        private Panel label6Panel;
        private bool isSuggestionFormOpen = false;

        public Form16()
        {
            InitializeComponent();
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
            button3.Click += new EventHandler(button3_Click);
            button4.Click += new EventHandler(button4_Click);
            textBox3.TextChanged += new EventHandler(textBox3_TextChanged);
            ConfigureAutoCompleteForTextBox3();
            SetupLabel6Panel();
            this.AutoScroll = true;

            label6.Text = "Yem stoğu boş. Lütfen yem ekleyin.";
        }

        private void SetupLabel6Panel()
        {
            Point label6Location = label6.Location;
            Size label6Size = new Size(400, 300);

            label6Panel = new Panel
            {
                Location = label6Location,
                Size = label6Size,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            label6.Location = new Point(0, 0);
            label6.AutoSize = true;
            label6.MaximumSize = new Size(label6Panel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth, 0);
            label6.BackColor = Color.LightGray;
            label6.ForeColor = Color.DarkBlue;
            label6.Font = new Font("Arial", 10, FontStyle.Bold);
            label6.Padding = new Padding(5);
            label6.TextAlign = ContentAlignment.TopLeft;

            label6Panel.Controls.Add(label6);
            this.Controls.Remove(label6);
            this.Controls.Add(label6Panel);
        }

        public class Yem
        {
            public required string Ad { get; set; }
            public double KM { get; set; }
            public double ME { get; set; }
            public double HP { get; set; }
            public double Ca { get; set; }
            public double P { get; set; }
            public double Miktar { get; set; }
        }

        public class Ihtiyac
        {
            public double KM { get; set; }
            public double ME { get; set; }
            public double HP { get; set; }
            public double Ca { get; set; }
            public double P { get; set; }
        }

        private string? GetHayvanTuru()
        {
            if (radioButton1.Checked) return "YERLİ IRK ERKEK DANA".Trim().ToUpper();
            if (radioButton2.Checked) return "KÜLTÜR IRKI ERKEK DANA".Trim().ToUpper();
            if (radioButton3.Checked) return "YERLİ IRK DİŞİ DANA".Trim().ToUpper();
            if (radioButton4.Checked) return "KÜLTÜR IRKI DİŞİ DANA".Trim().ToUpper();
            return null;
        }

        // Define MessageBoxHelper within the same namespace
        public static class MessageBoxHelper
        {
            public enum MessageType
            {
                Error,
                Warning,
                Information,
                Question
            }

            public static DialogResult Show(string message, string title, MessageType messageType, string errorCode = null)
            {
                MessageBoxIcon icon = messageType switch
                {
                    MessageType.Error => MessageBoxIcon.Error,
                    MessageType.Warning => MessageBoxIcon.Warning,
                    MessageType.Information => MessageBoxIcon.Information,
                    MessageType.Question => MessageBoxIcon.Question,
                    _ => MessageBoxIcon.None
                };

                MessageBoxButtons buttons = messageType == MessageType.Question
                    ? MessageBoxButtons.YesNo
                    : MessageBoxButtons.OK;

                string formattedMessage = string.IsNullOrEmpty(errorCode)
                    ? message
                    : $"{message}\n\nHata Kodu: {errorCode}";

                return MessageBox.Show(formattedMessage, title, buttons, icon);
            }

            public static void ShowError(string userMessage, Exception ex = null, string errorCode = null)
            {
                string message = userMessage;
                if (ex != null)
                {
                    message += "\n\nEk Bilgi: " + ex.Message;
                }
                Show(message, "Hata", MessageType.Error, errorCode);
            }

            public static void ShowInfo(string message, string title = "Bilgi")
            {
                Show(message, title, MessageType.Information);
            }

            public static void ShowWarning(string message, string title = "Uyarı")
            {
                Show(message, title, MessageType.Warning);
            }

            public static DialogResult ShowQuestion(string message, string title = "Soru")
            {
                return Show(message, title, MessageType.Question);
            }
        }

        private async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Veritabanına bağlanılamadı.", ex, "ERR-001");
                return false;
            }
        }

        private async Task<List<Yem>> YemiYukleAsync()
        {
            var yemler = new List<Yem>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "SELECT Yemler, KM_Yuzde, ME_Mcal_kg, HP_Yuzde, Ca_Yuzde, P_Yuzde FROM Yem";
                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string? yemAdi = reader["Yemler"] as string;
                            if (string.IsNullOrEmpty(yemAdi)) continue;

                            yemler.Add(new Yem
                            {
                                Ad = yemAdi,
                                KM = reader["KM_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["KM_Yuzde"]),
                                ME = reader["ME_Mcal_kg"] == DBNull.Value ? 0 : Convert.ToDouble(reader["ME_Mcal_kg"]),
                                HP = reader["HP_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["HP_Yuzde"]),
                                Ca = reader["Ca_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Ca_Yuzde"]),
                                P = reader["P_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["P_Yuzde"]),
                                Miktar = 0
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError("Yem verileri yüklenirken bir hata oluştu.", ex, "ERR-002");
                }
            }
            return yemler;
        }

        private async Task<List<Yem>> GetYemSuggestionsAsync(string prefix)
        {
            var suggestions = new List<Yem>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "SELECT Yemler, KM_Yuzde, ME_Mcal_kg, HP_Yuzde, Ca_Yuzde, P_Yuzde FROM Yem WHERE Yemler LIKE @Prefix || '%'";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Prefix", prefix);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string? yemAdi = reader["Yemler"] as string;
                                if (string.IsNullOrEmpty(yemAdi)) continue;

                                suggestions.Add(new Yem
                                {
                                    Ad = yemAdi,
                                    KM = reader["KM_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["KM_Yuzde"]),
                                    ME = reader["ME_Mcal_kg"] == DBNull.Value ? 0 : Convert.ToDouble(reader["ME_Mcal_kg"]),
                                    HP = reader["HP_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["HP_Yuzde"]),
                                    Ca = reader["Ca_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Ca_Yuzde"]),
                                    P = reader["P_Yuzde"] == DBNull.Value ? 0 : Convert.ToDouble(reader["P_Yuzde"]),
                                    Miktar = 0
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError("Yem önerileri yüklenirken bir hata oluştu.", ex, "ERR-003");
                }
            }
            return suggestions;
        }

        private async void ConfigureAutoCompleteForTextBox3()
        {
            try
            {
                textBox3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;

                var suggestions = await GetYemSuggestionsAsync("");
                AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();
                autoCompleteCollection.AddRange(suggestions.Select(y => y.Ad).ToArray());
                textBox3.AutoCompleteCustomSource = autoCompleteCollection;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Otomatik tamamlama yüklenirken bir hata oluştu.", ex, "ERR-004");
            }
        }

        private async void textBox3_TextChanged(object? sender, EventArgs e)
        {
            try
            {
                string input = textBox3.Text?.Trim() ?? string.Empty;
                if (input.Length < 2) return;

                string prefix = input.Substring(0, Math.Min(2, input.Length));
                if (prefix == sonOneriPrefix || isSuggestionFormOpen) return;

                sonOneriPrefix = prefix;
                isSuggestionFormOpen = true;

                var suggestions = await GetYemSuggestionsAsync(prefix);
                if (!suggestions.Any())
                {
                    MessageBoxHelper.ShowInfo($"İlk 2 harfi '{prefix}' olan yem bulunamadı.", "Yem Önerileri");
                    sonOneriPrefix = null;
                    isSuggestionFormOpen = false;
                    return;
                }

                using (var suggestionForm = new Form())
                {
                    suggestionForm.Text = "Yem Önerileri";
                    suggestionForm.Size = new Size(600, 300);
                    suggestionForm.StartPosition = FormStartPosition.CenterParent;
                    suggestionForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    suggestionForm.MaximizeBox = false;
                    suggestionForm.MinimizeBox = false;

                    DataGridView dataGridView = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        ReadOnly = true,
                        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                        AllowUserToAddRows = false,
                        RowHeadersVisible = false
                    };

                    dataGridView.Columns.Add("Ad", "Yem Adı");
                    dataGridView.Columns.Add("KM", "KM (%)");
                    dataGridView.Columns.Add("ME", "ME (Mcal/kg)");
                    dataGridView.Columns.Add("HP", "HP (%)");
                    dataGridView.Columns.Add("Ca", "Ca (%)");
                    dataGridView.Columns.Add("P", "P (%)");

                    foreach (var suggestion in suggestions)
                    {
                        dataGridView.Rows.Add(suggestion.Ad, suggestion.KM, suggestion.ME, suggestion.HP, suggestion.Ca, suggestion.P);
                    }

                    dataGridView.CellClick += (s, args) =>
                    {
                        if (args.RowIndex >= 0)
                        {
                            textBox3.Text = dataGridView.Rows[args.RowIndex].Cells["Ad"].Value?.ToString() ?? string.Empty;
                            suggestionForm.Close();
                        }
                    };

                    suggestionForm.Controls.Add(dataGridView);
                    suggestionForm.FormClosed += (s, args) =>
                    {
                        isSuggestionFormOpen = false;
                        sonOneriPrefix = null;
                    };

                    suggestionForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Yem önerisi sırasında bir hata oluştu.", ex, "ERR-005");
                sonOneriPrefix = null;
                isSuggestionFormOpen = false;
            }
        }

        private async Task<Ihtiyac?> IhtiyaciYukleAsync(string hayvanTuru, double agirlik, double gunlukArtis)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT CA_kg, GCAA_g FROM Hayvan_Turleri WHERE UPPER(Hayvan_Turu) = UPPER(@HayvanTuru)";
                    List<double> agirliklar = new List<double>();
                    List<double> gunlukArtislar = new List<double>();

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@HayvanTuru", hayvanTuru);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader["CA_kg"] != DBNull.Value)
                                    agirliklar.Add(Convert.ToDouble(reader["CA_kg"]));
                                if (reader["GCAA_g"] != DBNull.Value)
                                    gunlukArtislar.Add(Convert.ToDouble(reader["GCAA_g"]));
                            }
                        }
                    }

                    if (!agirliklar.Any() || !gunlukArtislar.Any())
                    {
                        DialogResult result = MessageBoxHelper.ShowQuestion(
                            $"Hayvan türü: {hayvanTuru} için veritabanında kayıt bulunamadı! Varsayılan değerler kullanılacak.\n" +
                            "Devam etmek istiyor musunuz?");

                        if (result == DialogResult.No)
                        {
                            return null;
                        }

                        return new Ihtiyac
                        {
                            KM = 10,
                            ME = 25,
                            HP = 500,
                            Ca = 40,
                            P = 20
                        };
                    }

                    double enYakinAgirlik = agirliklar.OrderBy(x => Math.Abs(x - agirlik)).First();
                    double enYakinGunlukArtis = gunlukArtislar.OrderBy(x => Math.Abs(x - gunlukArtis)).First();

                    if (enYakinAgirlik != agirlik || enYakinGunlukArtis != gunlukArtis)
                    {
                        MessageBoxHelper.ShowInfo(
                            $"Girilen ağırlık ({agirlik} kg) ve günlük artış ({gunlukArtis} kg) değerleri yuvarlandı.\n" +
                            $"Kullanılan değerler: Ağırlık = {enYakinAgirlik} kg, Günlük Artış = {enYakinGunlukArtis} kg.");
                    }

                    string ihtiyacQuery = "SELECT KM_kg, ME_Mcal, HP_g, Ca_g, P_g FROM Hayvan_Turleri " +
                                         "WHERE UPPER(Hayvan_Turu) = UPPER(@HayvanTuru) AND CA_kg = @Agirlik AND GCAA_g = @GunlukArtis";

                    using (var cmd = new SQLiteCommand(ihtiyacQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@HayvanTuru", hayvanTuru);
                        cmd.Parameters.AddWithValue("@Agirlik", enYakinAgirlik);
                        cmd.Parameters.AddWithValue("@GunlukArtis", enYakinGunlukArtis);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Ihtiyac
                                {
                                    KM = reader["KM_kg"] == DBNull.Value ? 0 : Convert.ToDouble(reader["KM_kg"]),
                                    ME = reader["ME_Mcal"] == DBNull.Value ? 0 : Convert.ToDouble(reader["ME_Mcal"]),
                                    HP = reader["HP_g"] == DBNull.Value ? 0 : Convert.ToDouble(reader["HP_g"]),
                                    Ca = reader["Ca_g"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Ca_g"]),
                                    P = reader["P_g"] == DBNull.Value ? 0 : Convert.ToDouble(reader["P_g"])
                                };
                            }
                            else
                            {
                                MessageBoxHelper.ShowWarning(
                                    $"Hayvan türü: {hayvanTuru} için yuvarlanmış değerlerle (Ağırlık: {enYakinAgirlik} kg, Günlük Artış: {enYakinGunlukArtis} kg) kayıt bulunamadı!\n" +
                                    "Varsayılan değerler kullanılacak.");
                                return new Ihtiyac
                                {
                                    KM = 10,
                                    ME = 25,
                                    HP = 500,
                                    Ca = 40,
                                    P = 20
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError("İhtiyaç verileri yüklenirken bir hata oluştu.", ex, "ERR-006");
                    return null;
                }
            }
        }

        private Dictionary<string, double> YemDagit(List<Yem> yemler, Ihtiyac ihtiyac, double gunlukYemMiktari)
        {
            var sonuc = new Dictionary<string, double>();
            double kalanKM = ihtiyac.KM;
            double kalanME = ihtiyac.ME;
            double kalanHP = ihtiyac.HP;
            double kalanCa = ihtiyac.Ca;
            double kalanP = ihtiyac.P;
            double toplamMiktar = 0;

            var uygunYemler = yemler.Where(y => y.Miktar > 0).ToList();
            if (!uygunYemler.Any())
            {
                return sonuc;
            }

            double toplamPuan = 0;
            var puanlar = new Dictionary<string, double>();

            foreach (var yem in uygunYemler)
            {
                double puan = 0;
                if (kalanHP > 0 && yem.HP > 0) puan += (yem.HP / 100) * (kalanHP / ihtiyac.HP);
                if (kalanP > 0 && yem.P > 0) puan += (yem.P / 100) * (kalanP / ihtiyac.P);
                if (kalanKM > 0 && yem.KM > 0) puan += (yem.KM / 100) * (kalanKM / ihtiyac.KM);
                if (kalanME > 0 && yem.ME > 0) puan += yem.ME * (kalanME / ihtiyac.ME);
                if (kalanCa > 0 && yem.Ca > 0) puan += (yem.Ca / 100) * (kalanCa / ihtiyac.Ca);

                puanlar[yem.Ad] = puan;
                toplamPuan += puan;
            }

            if (toplamPuan <= 0)
            {
                double toplamMevcutMiktar = uygunYemler.Sum(y => y.Miktar);
                if (toplamMevcutMiktar <= 0) return sonuc;

                foreach (var yem in uygunYemler)
                {
                    double oran = yem.Miktar / toplamMevcutMiktar;
                    double kullanilacakMiktar = Math.Min(yem.Miktar, gunlukYemMiktari * oran);
                    if (kullanilacakMiktar > 0)
                    {
                        sonuc[yem.Ad] = kullanilacakMiktar;
                        toplamMiktar += kullanilacakMiktar;

                        kalanKM -= kullanilacakMiktar * (yem.KM / 100);
                        kalanME -= kullanilacakMiktar * yem.ME;
                        kalanHP -= (kullanilacakMiktar * (yem.HP / 100)) * 1000;
                        kalanCa -= (kullanilacakMiktar * (yem.Ca / 100)) * 1000;
                        kalanP -= (kullanilacakMiktar * (yem.P / 100)) * 1000;
                    }
                }
            }
            else
            {
                foreach (var yem in uygunYemler)
                {
                    double puan = puanlar[yem.Ad];
                    if (puan <= 0) continue;

                    double oran = puan / toplamPuan;
                    double kullanilacakMiktar = Math.Min(yem.Miktar, gunlukYemMiktari * oran);
                    if (kullanilacakMiktar > 0)
                    {
                        sonuc[yem.Ad] = kullanilacakMiktar;
                        toplamMiktar += kullanilacakMiktar;

                        kalanKM -= kullanilacakMiktar * (yem.KM / 100);
                        kalanME -= kullanilacakMiktar * yem.ME;
                        kalanHP -= (kullanilacakMiktar * (yem.HP / 100)) * 1000;
                        kalanCa -= (kullanilacakMiktar * (yem.Ca / 100)) * 1000;
                        kalanP -= (kullanilacakMiktar * (yem.P / 100)) * 1000;
                    }
                }

                if (toplamMiktar < gunlukYemMiktari)
                {
                    double kalanMiktar = gunlukYemMiktari - toplamMiktar;
                    var kalanYemler = uygunYemler.Where(y => y.Miktar > sonuc.GetValueOrDefault(y.Ad, 0))
                                                .OrderByDescending(y => puanlar[y.Ad])
                                                .ToList();

                    foreach (var yem in kalanYemler)
                    {
                        if (kalanMiktar <= 0) break;

                        double mevcutKullanilmis = sonuc.GetValueOrDefault(yem.Ad, 0);
                        double ekMiktar = Math.Min(yem.Miktar - mevcutKullanilmis, kalanMiktar);
                        if (ekMiktar > 0)
                        {
                            sonuc[yem.Ad] = mevcutKullanilmis + ekMiktar;
                            toplamMiktar += ekMiktar;
                            kalanMiktar -= ekMiktar;

                            kalanKM -= ekMiktar * (yem.KM / 100);
                            kalanME -= ekMiktar * yem.ME;
                            kalanHP -= (ekMiktar * (yem.HP / 100)) * 1000;
                            kalanCa -= (ekMiktar * (yem.Ca / 100)) * 1000;
                            kalanP -= (ekMiktar * (yem.P / 100)) * 1000;
                        }
                    }
                }
            }

            eksikKM = Math.Max(0, kalanKM);
            eksikME = Math.Max(0, kalanME);
            eksikHP = Math.Max(0, kalanHP);
            eksikCa = Math.Max(0, kalanCa);
            eksikP = Math.Max(0, kalanP);

            return sonuc;
        }

        private string SayiyiYaziyaCevir(double sayi, bool isGram = false)
        {
            if (sayi == 0) return "sıfır";

            string[] birler = { "", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz" };
            string[] onlar = { "", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan" };
            string[] binler = { "", "bin", "milyon", "milyar" };

            string birim = isGram ? "gram" : "kilogram";
            long tamKisim = (long)sayi;
            double ondalikKisim = sayi - tamKisim;
            string sonuc = "";

            if (tamKisim == 0 && ondalikKisim == 0)
            {
                return $"sıfır {birim}";
            }

            int grupSayisi = 0;
            while (tamKisim > 0)
            {
                int grup = (int)(tamKisim % 1000);
                tamKisim /= 1000;

                string grupYazi = "";
                if (grup > 0)
                {
                    if (grup >= 100)
                    {
                        int yuzler = grup / 100;
                        if (yuzler > 1) grupYazi += birler[yuzler] + " yüz ";
                        else grupYazi += "yüz ";
                        grup %= 100;
                    }

                    if (grup >= 10)
                    {
                        grupYazi += onlar[grup / 10] + " ";
                        grup %= 10;
                    }

                    if (grup > 0)
                    {
                        grupYazi += birler[grup] + " ";
                    }

                    grupYazi = grupYazi.Trim();
                    if (grupSayisi > 0 && grup == 1 && binler[grupSayisi] == "bin")
                    {
                        grupYazi = "";
                    }

                    if (!string.IsNullOrEmpty(grupYazi))
                    {
                        grupYazi += " " + binler[grupSayisi];
                    }

                    sonuc = grupYazi.Trim() + " " + sonuc;
                }

                grupSayisi++;
            }

            if (ondalikKisim > 0)
            {
                int ondalikTam = (int)(ondalikKisim * 100);
                string ondalikYazi = "";
                if (ondalikTam >= 10)
                {
                    ondalikYazi += onlar[ondalikTam / 10] + " ";
                    ondalikTam %= 10;
                }

                if (ondalikTam > 0)
                {
                    ondalikYazi += birler[ondalikTam] + " ";
                }

                if (!string.IsNullOrEmpty(sonuc))
                {
                    sonuc += " virgül " + ondalikYazi.Trim();
                }
                else
                {
                    sonuc = "sıfır virgül " + ondalikYazi.Trim();
                }
            }

            return $"{sonuc.Trim()} {birim}";
        }

        private async void button1_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!await TestConnectionAsync())
                {
                    return;
                }

                string? hayvanTuru = GetHayvanTuru();
                if (string.IsNullOrEmpty(hayvanTuru))
                {
                    MessageBoxHelper.ShowError("Lütfen bir hayvan türü seçin.", null, "ERR-007");
                    return;
                }

                if (!double.TryParse(textBox1.Text, out double agirlik) || !double.TryParse(textBox2.Text, out double aylikArtis) || agirlik <= 0 || aylikArtis <= 0)
                {
                    MessageBoxHelper.ShowError("Ağırlık ve aylık kilo artışı pozitif sayısal değerler olmalıdır.", null, "ERR-008");
                    return;
                }

                double gunlukArtis = aylikArtis / 30;
                gunlukYemMiktari = gunlukArtis * 10;

                var ihtiyac = await IhtiyaciYukleAsync(hayvanTuru, agirlik, gunlukArtis);
                if (ihtiyac == null)
                {
                    return;
                }

                if (!mevcutYemler.Any())
                {
                    MessageBoxHelper.ShowError("Hesaplama yapmak için en az bir yem ekleyin.", null, "ERR-009");
                    return;
                }

                var hesaplamaYemler = mevcutYemler.Where(y => y.Miktar > 0).ToList();
                var yemDagilim = YemDagit(hesaplamaYemler, ihtiyac, gunlukYemMiktari);

                if (yemDagilim.Count == 0)
                {
                    MessageBoxHelper.ShowWarning("İhtiyacı karşılayacak yeterli yem bulunamadı! Daha fazla yem eklemeyi deneyin.", "Hesaplama Sonucu");
                    sonHesaplamaMesaji = null;
                    return;
                }

                double toplamKullanilacakYem = yemDagilim.Sum(x => x.Value);
                StringBuilder finalMesaj = new StringBuilder();
                finalMesaj.AppendLine("=====================================");
                finalMesaj.AppendLine("        BESLEME PROGRAMI RAPORU      ");
                finalMesaj.AppendLine("=====================================");
                finalMesaj.AppendLine($"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                finalMesaj.AppendLine($"Hayvan Türü: {hayvanTuru}");
                finalMesaj.AppendLine($"Ağırlık: {agirlik:F2} kg ({SayiyiYaziyaCevir(agirlik)})");
                finalMesaj.AppendLine($"Aylık Kilo Artışı: {aylikArtis:F2} kg ({SayiyiYaziyaCevir(aylikArtis)}) (Günlük: {gunlukArtis:F2} kg)");
                finalMesaj.AppendLine($"Günlük Toplam Yem Miktarı: {gunlukYemMiktari:F2} kg ({SayiyiYaziyaCevir(gunlukYemMiktari)})");
                finalMesaj.AppendLine();

                finalMesaj.AppendLine("-------------------------------------");
                finalMesaj.AppendLine("Günlük Yem Dağılımı");
                finalMesaj.AppendLine("-------------------------------------");
                finalMesaj.AppendLine("Yem Adı".PadRight(20) + "Miktar".PadRight(20) + "Okunuş");
                finalMesaj.AppendLine(new string('-', 60));
                foreach (var item in yemDagilim)
                {
                    double gunlukYem = item.Value;
                    string miktarStr = gunlukYem >= 1 ? $"{gunlukYem:F2} kg" : $"{gunlukYem * 1000:F2} g";
                    string okunus = gunlukYem >= 1 ? SayiyiYaziyaCevir(gunlukYem) : SayiyiYaziyaCevir(gunlukYem * 1000, true);
                    finalMesaj.AppendLine($"{item.Key.PadRight(20)}{miktarStr.PadRight(20)}{okunus}");
                }

                double toplamKM = 0, toplamME = 0, toplamHP = 0, toplamCa = 0, toplamP = 0;
                foreach (var item in yemDagilim)
                {
                    var yem = hesaplamaYemler.FirstOrDefault(y => y.Ad == item.Key);
                    if (yem == null) continue;

                    double miktar = item.Value;
                    toplamKM += miktar * (yem.KM / 100);
                    toplamME += miktar * yem.ME;
                    toplamHP += (miktar * (yem.HP / 100)) * 1000;
                    toplamCa += (miktar * (yem.Ca / 100)) * 1000;
                    toplamP += (miktar * (yem.P / 100)) * 1000;
                }

                double kmYuzde = toplamKullanilacakYem > 0 ? (toplamKM / toplamKullanilacakYem) * 100 : 0;
                double meMcalKg = toplamKullanilacakYem > 0 ? toplamME / toplamKullanilacakYem : 0;
                double hpYuzde = toplamKullanilacakYem > 0 ? (toplamHP / 1000 / toplamKullanilacakYem) * 100 : 0;
                double caYuzde = toplamKullanilacakYem > 0 ? (toplamCa / 1000 / toplamKullanilacakYem) * 100 : 0;
                double pYuzde = toplamKullanilacakYem > 0 ? (toplamP / 1000 / toplamKullanilacakYem) * 100 : 0;

                finalMesaj.AppendLine();
                finalMesaj.AppendLine("-------------------------------------");
                finalMesaj.AppendLine("Besin Yüzdeleri");
                finalMesaj.AppendLine("-------------------------------------");
                finalMesaj.AppendLine($"KM (%): {kmYuzde:F2}");
                finalMesaj.AppendLine($"ME (Mcal/kg): {meMcalKg:F2}");
                finalMesaj.AppendLine($"HP (%): {hpYuzde:F2}");
                finalMesaj.AppendLine($"Ca (%): {caYuzde:F2}");
                finalMesaj.AppendLine($"P (%): {pYuzde:F2}");

                finalMesaj.AppendLine();
                finalMesaj.AppendLine("-------------------------------------");
                finalMesaj.AppendLine("Besin İhtiyaçları ve Karşılanma Durumu");
                finalMesaj.AppendLine("-------------------------------------");
                finalMesaj.AppendLine("Besin".PadRight(10) + "İhtiyaç".PadRight(15) + "Kullanılan".PadRight(15) + "Kalan".PadRight(15) + "Durum");
                finalMesaj.AppendLine(new string('-', 60));
                finalMesaj.AppendLine($"KM (kg)".PadRight(10) + $"{ihtiyac.KM:F2}".PadRight(15) + $"{toplamKM:F2}".PadRight(15) + $"{eksikKM:F2}".PadRight(15) + (eksikKM > 0 ? "Eksik" : "Karşılandı"));
                finalMesaj.AppendLine($"ME (Mcal)".PadRight(10) + $"{ihtiyac.ME:F2}".PadRight(15) + $"{toplamME:F2}".PadRight(15) + $"{eksikME:F2}".PadRight(15) + (eksikME > 0 ? "Eksik" : "Karşılandı"));
                finalMesaj.AppendLine($"HP (g)".PadRight(10) + $"{ihtiyac.HP:F2}".PadRight(15) + $"{toplamHP:F2}".PadRight(15) + $"{eksikHP:F2}".PadRight(15) + (eksikHP > 0 ? "Eksik" : "Karşılandı"));
                finalMesaj.AppendLine($"Ca (g)".PadRight(10) + $"{ihtiyac.Ca:F2}".PadRight(15) + $"{toplamCa:F2}".PadRight(15) + $"{eksikCa:F2}".PadRight(15) + (eksikCa > 0 ? "Eksik" : "Karşılandı"));
                finalMesaj.AppendLine($"P (g)".PadRight(10) + $"{ihtiyac.P:F2}".PadRight(15) + $"{toplamP:F2}".PadRight(15) + $"{eksikP:F2}".PadRight(15) + (eksikP > 0 ? "Eksik" : "Karşılandı"));

                if (eksikKM > 0 || eksikME > 0 || eksikHP > 0 || eksikCa > 0 || eksikP > 0)
                {
                    finalMesaj.AppendLine();
                    finalMesaj.AppendLine("-------------------------------------");
                    finalMesaj.AppendLine("Eksik Besin Önerileri");
                    finalMesaj.AppendLine("-------------------------------------");
                    finalMesaj.AppendLine("Yem Adı".PadRight(20) + "Besin".PadRight(15) + "Miktar".PadRight(15) + "Okunuş");
                    finalMesaj.AppendLine(new string('-', 60));

                    if (eksikKM > 0)
                    {
                        var bestYemKM = hesaplamaYemler.Where(y => y.KM > 0).OrderByDescending(y => y.KM).FirstOrDefault();
                        if (bestYemKM != null)
                        {
                            double eksikMiktar = eksikKM / (bestYemKM.KM / 100);
                            string miktarStr = eksikMiktar >= 1 ? $"{eksikMiktar:F2} kg" : $"{(eksikMiktar * 1000):F2} g";
                            string okunus = eksikMiktar >= 1 ? SayiyiYaziyaCevir(eksikMiktar) : SayiyiYaziyaCevir(eksikMiktar * 1000, true);
                            finalMesaj.AppendLine($"{bestYemKM.Ad.PadRight(20)}KM".PadRight(15) + $"{miktarStr.PadRight(15)}{okunus}");
                        }
                    }
                    if (eksikME > 0)
                    {
                        var bestYemME = hesaplamaYemler.Where(y => y.ME > 0).OrderByDescending(y => y.ME).FirstOrDefault();
                        if (bestYemME != null)
                        {
                            double eksikMiktar = eksikME / bestYemME.ME;
                            string miktarStr = eksikMiktar >= 1 ? $"{eksikMiktar:F2} kg" : $"{(eksikMiktar * 1000):F2} g";
                            string okunus = eksikMiktar >= 1 ? SayiyiYaziyaCevir(eksikMiktar) : SayiyiYaziyaCevir(eksikMiktar * 1000, true);
                            finalMesaj.AppendLine($"{bestYemME.Ad.PadRight(20)}ME".PadRight(15) + $"{miktarStr.PadRight(15)}{okunus}");
                        }
                    }
                    if (eksikHP > 0)
                    {
                        var bestYemHP = hesaplamaYemler.Where(y => y.HP > 0).OrderByDescending(y => y.HP).FirstOrDefault();
                        if (bestYemHP != null)
                        {
                            double eksikMiktar = eksikHP / (bestYemHP.HP * 10);
                            string miktarStr = eksikMiktar >= 1 ? $"{eksikMiktar:F2} kg" : $"{(eksikMiktar * 1000):F2} g";
                            string okunus = eksikMiktar >= 1 ? SayiyiYaziyaCevir(eksikMiktar) : SayiyiYaziyaCevir(eksikMiktar * 1000, true);
                            finalMesaj.AppendLine($"{bestYemHP.Ad.PadRight(20)}HP".PadRight(15) + $"{miktarStr.PadRight(15)}{okunus}");
                        }
                    }
                    if (eksikCa > 0)
                    {
                        var bestYemCa = hesaplamaYemler.Where(y => y.Ca > 0).OrderByDescending(y => y.Ca).FirstOrDefault();
                        if (bestYemCa != null)
                        {
                            double eksikMiktar = eksikCa / (bestYemCa.Ca * 10);
                            string miktarStr = eksikMiktar >= 1 ? $"{eksikMiktar:F2} kg" : $"{(eksikMiktar * 1000):F2} g";
                            string okunus = eksikMiktar >= 1 ? SayiyiYaziyaCevir(eksikMiktar) : SayiyiYaziyaCevir(eksikMiktar * 1000, true);
                            finalMesaj.AppendLine($"{bestYemCa.Ad.PadRight(20)}Ca".PadRight(15) + $"{miktarStr.PadRight(15)}{okunus}");
                        }
                    }
                    if (eksikP > 0)
                    {
                        var bestYemP = hesaplamaYemler.Where(y => y.P > 0).OrderByDescending(y => y.P).FirstOrDefault();
                        if (bestYemP != null)
                        {
                            double eksikMiktar = eksikP / (bestYemP.P * 10);
                            string miktarStr = eksikMiktar >= 1 ? $"{eksikMiktar:F2} kg" : $"{(eksikMiktar * 1000):F2} g";
                            string okunus = eksikMiktar >= 1 ? SayiyiYaziyaCevir(eksikMiktar) : SayiyiYaziyaCevir(eksikMiktar * 1000, true);
                            finalMesaj.AppendLine($"{bestYemP.Ad.PadRight(20)}P".PadRight(15) + $"{miktarStr.PadRight(15)}{okunus}");
                        }
                    }
                }

                finalMesaj.AppendLine();
                finalMesaj.AppendLine("=====================================");
                finalMesaj.AppendLine("Veteriner Otomasyon Ekibi");
                finalMesaj.AppendLine("=====================================");

                MessageBoxHelper.ShowInfo(finalMesaj.ToString(), "Besleme Programı Raporu");
                sonHesaplamaMesaji = finalMesaj.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Hesaplama sırasında bir hata oluştu.", ex, "ERR-010");
                sonHesaplamaMesaji = null;
            }
        }

        private void UpdateLabel6()
        {
            if (!mevcutYemler.Any())
            {
                label6.Text = "Yem stoğu boş. Lütfen yem ekleyin.";
                return;
            }

            StringBuilder yemListesi = new StringBuilder();
            yemListesi.AppendLine("=================================");
            yemListesi.AppendLine("         YEM STOK RAPORU         ");
            yemListesi.AppendLine("=================================");
            yemListesi.AppendLine("Yem Adı".PadRight(20) + "Miktar".PadRight(15) + "Okunuş");
            yemListesi.AppendLine(new string('-', 50));

            double toplamKM = 0, toplamME = 0, toplamHP = 0, toplamCa = 0, toplamP = 0;
            foreach (var yem in mevcutYemler)
            {
                string miktarStr = yem.Miktar >= 1 ? $"{yem.Miktar:F2} kg" : $"{(yem.Miktar * 1000):F2} g";
                string okunus = yem.Miktar >= 1 ? SayiyiYaziyaCevir(yem.Miktar) : SayiyiYaziyaCevir(yem.Miktar * 1000, true);

                yemListesi.AppendLine($"{yem.Ad.PadRight(20)}{miktarStr.PadRight(15)}{okunus}");

                toplamKM += yem.Miktar * (yem.KM / 100);
                toplamME += yem.Miktar * yem.ME;
                toplamHP += (yem.Miktar * (yem.HP / 100)) * 1000;
                toplamCa += (yem.Miktar * (yem.Ca / 100)) * 1000;
                toplamP += (yem.Miktar * (yem.P / 100)) * 1000;
            }

            yemListesi.AppendLine();
            yemListesi.AppendLine("---------------------------------");
            yemListesi.AppendLine("Toplam Besin Değerleri");
            yemListesi.AppendLine("---------------------------------");
            yemListesi.AppendLine($"KM: {toplamKM:F2} kg".PadRight(20) + $"({SayiyiYaziyaCevir(toplamKM)})");
            yemListesi.AppendLine($"ME: {toplamME:F2} Mcal".PadRight(20) + $"({SayiyiYaziyaCevir(toplamME)} Mcal)");
            yemListesi.AppendLine($"HP: {toplamHP:F2} g".PadRight(20) + $"({SayiyiYaziyaCevir(toplamHP, true)})");
            yemListesi.AppendLine($"Ca: {toplamCa:F2} g".PadRight(20) + $"({SayiyiYaziyaCevir(toplamCa, true)})");
            yemListesi.AppendLine($"P: {toplamP:F2} g".PadRight(20) + $"({SayiyiYaziyaCevir(toplamP, true)})");

            yemListesi.AppendLine("=================================");
            yemListesi.AppendLine("Veteriner Otomasyon Sistemi");
            yemListesi.AppendLine("=================================");

            label6.Text = yemListesi.ToString();
        }

        private async void button2_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!await TestConnectionAsync())
                {
                    return;
                }

                string? yemAdi = textBox3.Text?.Trim();
                if (string.IsNullOrEmpty(yemAdi) || !double.TryParse(textBox4.Text, out double miktar) || miktar <= 0)
                {
                    MessageBoxHelper.ShowError("Yem türü ve miktarı geçerli ve pozitif olmalıdır.", null, "ERR-011");
                    return;
                }

                var yemler = await YemiYukleAsync();
                if (!yemler.Any())
                {
                    MessageBoxHelper.ShowError("Veritabanında yem verisi bulunamadı.", null, "ERR-012");
                    return;
                }

                var secilenYem = yemler.FirstOrDefault(y => y.Ad.Equals(yemAdi, StringComparison.OrdinalIgnoreCase));
                if (secilenYem == null)
                {
                    MessageBoxHelper.ShowError($"Belirtilen yem türü ({yemAdi}) veritabanında bulunamadı.", null, "ERR-013");
                    return;
                }

                var mevcutYem = mevcutYemler.FirstOrDefault(y => y.Ad == secilenYem.Ad);
                if (mevcutYem == null)
                {
                    mevcutYemler.Add(new Yem
                    {
                        Ad = secilenYem.Ad,
                        KM = secilenYem.KM,
                        ME = secilenYem.ME,
                        HP = secilenYem.HP,
                        Ca = secilenYem.Ca,
                        P = secilenYem.P,
                        Miktar = miktar
                    });
                }
                else
                {
                    mevcutYem.Miktar += miktar;
                }

                UpdateLabel6();

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();

                MessageBoxHelper.ShowInfo("Yem başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Yem ekleme sırasında bir hata oluştu.", ex, "ERR-014");
            }
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            try
            {
                Form1 anaSayfa = new Form1();
                anaSayfa.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("Ana sayfaya dönerken bir hata oluştu.", ex, "ERR-015");
            }
        }

        private async void button4_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(sonHesaplamaMesaji))
                {
                    MessageBoxHelper.ShowError("Önce bir hesaplama yapmalısınız! Lütfen hesaplama işlemini gerçekleştirin.", null, "ERR-016");
                    return;
                }

                string? aliciEmail = textBox5.Text?.Trim();
                if (string.IsNullOrEmpty(aliciEmail))
                {
                    MessageBoxHelper.ShowError("Lütfen alıcı e-posta adresini girin.", null, "ERR-017");
                    return;
                }

                string gondericiEmail = string.Empty;
                string uygulamaSifresi = string.Empty;

                using (var conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT email, password FROM mail LIMIT 1";
                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            gondericiEmail = reader["email"] as string ?? string.Empty;
                            uygulamaSifresi = reader["password"] as string ?? string.Empty;
                        }
                    }
                }

                if (string.IsNullOrEmpty(gondericiEmail) || string.IsNullOrEmpty(uygulamaSifresi))
                {
                    MessageBoxHelper.ShowError("Gönderici e-posta veya şifre veritabanından alınamadı.", null, "ERR-018");
                    return;
                }

                StringBuilder emailBody = new StringBuilder();
                emailBody.AppendLine("Merhaba,");
                emailBody.AppendLine();
                emailBody.AppendLine("Bu e-posta, veteriner otomasyon sisteminden hesaplanan 1 aylık besleme programını içermektedir.");
                emailBody.AppendLine($"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                emailBody.AppendLine();
                emailBody.AppendLine("Aşağıda, seçtiğiniz hayvan türü için hesaplanan 1 aylık besleme programı detaylı bir şekilde listelenmiştir. Bu program, sistemdeki güncel verilere dayanmaktadır ve raporun oluşturulduğu tarih ve saat yukarıda belirtilmiştir.");
                emailBody.AppendLine();
                emailBody.AppendLine("Besleme Programı:");
                emailBody.AppendLine(new string('-', 50));
                emailBody.AppendLine(sonHesaplamaMesaji);
                emailBody.AppendLine(new string('-', 50));
                emailBody.AppendLine();
                emailBody.AppendLine("İyi günler,");
                emailBody.AppendLine("Veteriner Otomasyon Ekibi");

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(gondericiEmail, uygulamaSifresi);
                    smtpClient.EnableSsl = true;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(gondericiEmail);
                        mailMessage.To.Add(aliciEmail);
                        mailMessage.Subject = $"Veteriner Otomasyon - 1 Aylık Besleme Programı - {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
                        mailMessage.Body = emailBody.ToString();
                        mailMessage.IsBodyHtml = false;

                        smtpClient.Send(mailMessage);
                    }
                }

                MessageBoxHelper.ShowInfo("E-posta başarıyla gönderildi!");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("E-posta gönderilirken bir hata oluştu.", ex, "ERR-019");
            }
        }
    }
}