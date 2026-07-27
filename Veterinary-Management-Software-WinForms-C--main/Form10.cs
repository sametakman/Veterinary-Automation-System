using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace WinFormsApp8
{
    public partial class Form10 : Form
    {
        private const string Email = "otomasyonveteriner@gmail.com";
        private const string Sifre = "vdsu gmlh yfke mcbo"; // Gmail App Password
        private const string Anahtar = "VetOtomasyon123";
        private readonly string dosyaYolu = Path.Combine(Application.StartupPath, "activation.txt");
        private readonly string aktifDosya = Path.Combine(Application.StartupPath, "isActivated.txt");
        private int hataSayisi = 0;

        public Form10()
        {
            InitializeComponent();
            AktivasyonKontrol();
        }

        private void AktivasyonKontrol()
        {
            try
            {
                if (!File.Exists(dosyaYolu)) return;

                string[] veriler = File.ReadAllLines(dosyaYolu);
                if (veriler.Length <= 1) return;

                if (Coz(veriler[1], Anahtar) == MakineKimligi())
                {
                    File.WriteAllText(aktifDosya, "Activated");
                    FormDegistir(new Form1(true));
                }
                else
                {
                    MessageBox.Show("Program başka bir bilgisayarda aktif! Lütfen geliştiriciyle iletişime geçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aktivasyon kontrolü sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private string MakineKimligi() => MacAdresi();

        private string MacAdresi()
        {
            try
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
                return networkInterface?.GetPhysicalAddress().ToString() ?? "BilinmeyenMAC";
            }
            catch
            {
                return "HataMAC";
            }
        }

        private string IpAdresi()
        {
            try
            {
                var address = Dns.GetHostAddresses(Dns.GetHostName())
                    .FirstOrDefault(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                return address?.ToString() ?? "BilinmeyenIP";
            }
            catch
            {
                return "HataIP";
            }
        }

        private string Sifrele(string metin, string anahtar)
        {
            try
            {
                StringBuilder sonuc = new StringBuilder();
                for (int i = 0; i < metin.Length; i++)
                    sonuc.Append((char)(metin[i] ^ anahtar[i % anahtar.Length]));
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(sonuc.ToString()));
            }
            catch
            {
                return string.Empty;
            }
        }

        private string Coz(string sifreliMetin, string anahtar)
        {
            try
            {
                byte[] baytlar = Convert.FromBase64String(sifreliMetin);
                string metin = Encoding.UTF8.GetString(baytlar);
                StringBuilder sonuc = new StringBuilder();
                for (int i = 0; i < metin.Length; i++)
                    sonuc.Append((char)(metin[i] ^ anahtar[i % anahtar.Length]));
                return sonuc.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                string telefon = textBox3.Text.Trim();
                string kod = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(telefon))
                {
                    MessageBox.Show("Lütfen telefon numarası giriniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(kod))
                {
                    MessageBox.Show("Lütfen aktivasyon kodu giriniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!TelefonKoduAldiMi(telefon))
                {
                    MessageBox.Show("Bu telefon numarasına ait bir aktivasyon kodu bulunamadı! Lütfen doğru numarayı girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var (isValid, storedMac, isUsed, musteriBilgileri) = MailKontrol(telefon, kod);
                if (isValid && !isUsed)
                {
                    // Code is valid and not used, proceed with activation
                    string makineId = MakineKimligi();
                    File.WriteAllLines(dosyaYolu, new[] { "Activated", Sifrele(makineId, Anahtar) });
                    File.WriteAllText(aktifDosya, "Activated");
                    BasariMailiGonder(telefon, kod, makineId, musteriBilgileri);
                    MessageBox.Show("Giriş başarılı! Uygulama kapanacak. Tekrar açtığınızda ana ekran başlayacak.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormDegistir(new Form1(true));
                }
                else if (isUsed)
                {
                    // Code has already been used, notify admin and show error
                    KopyaIhbariGonder(telefon, kod, storedMac ?? "BilinmeyenMAC", musteriBilgileri);
                    MessageBox.Show("Bu aktivasyon kodu zaten kullanıldı! Yeni bir kod için geliştiriciyle iletişime geçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    // Invalid code
                    hataSayisi++;
                    if (hataSayisi >= 8)
                    {
                        MessageBox.Show("8 kez hatalı giriş yaptınız! Lütfen geliştiriciyle iletişime geçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Hide();
                        new Form9().ShowDialog();
                        hataSayisi = 0;
                    }
                    else
                    {
                        MessageBox.Show($"Hatalı giriş! Kalan deneme hakkı: {8 - hataSayisi}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TelefonKoduAldiMi(string telefon)
        {
            using var istemci = new ImapClient();
            try
            {
                istemci.Connect("imap.gmail.com", 993, true);
                istemci.Authenticate(Email, Sifre);
                var gelen = istemci.Inbox;
                gelen.Open(FolderAccess.ReadOnly);
                var mesajlar = gelen.Fetch(0, -1, MessageSummaryItems.UniqueId);
                foreach (var mesaj in mesajlar)
                {
                    var email = gelen.GetMessage(mesaj.UniqueId);
                    if (email.TextBody != null && email.TextBody.Contains(telefon) && email.TextBody.Contains("Aktivasyon Kodunuz"))
                    {
                        istemci.Disconnect(true);
                        return true;
                    }
                }
                istemci.Disconnect(true);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Telefon numarası kontrolü sırasında hata: {ex.Message}. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string KodUret()
        {
            try
            {
                Random rastgele = new Random();
                const string karakterler = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                while (true)
                {
                    string kod = new string(Enumerable.Repeat(karakterler, 11).Select(s => s[rastgele.Next(s.Length)]).ToArray());
                    if (kod.Any(char.IsLetter) && kod.Any(char.IsDigit))
                        return kod;
                }
            }
            catch
            {
                return "HataKod";
            }
        }

        private void AktivasyonMailiGonder(string kod, string telefon)
        {
            try
            {
                using var mail = new MailMessage();
                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Email, Sifre),
                    EnableSsl = true
                };
                mail.From = new MailAddress(Email);
                mail.To.Add(Email);
                mail.Subject = "Aktivasyon Kodunuz";
                mail.Body = $"Telefon: {telefon}\nMAC: {MacAdresi()}\nIP: {IpAdresi()}\nKod: {kod}";
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aktivasyon maili gönderilemedi: {ex.Message}. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BasariMailiGonder(string telefon, string kod, string makineId, (string isimSoyisim, string adres)? musteriBilgileri)
        {
            try
            {
                using var mail = new MailMessage();
                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Email, Sifre),
                    EnableSsl = true
                };
                mail.From = new MailAddress(Email);
                mail.To.Add(Email);
                mail.Subject = "Giriş Başarılı";
                mail.Body = $"Telefon: {telefon}\n" +
                            $"İsim Soyisim: {(musteriBilgileri.HasValue ? musteriBilgileri.Value.isimSoyisim : "Bilinmiyor")}\n" +
                            $"Adres: {(musteriBilgileri.HasValue ? musteriBilgileri.Value.adres : "Bilinmiyor")}\n" +
                            $"MAC: {makineId}\n" +
                            $"IP: {IpAdresi()}\n" +
                            $"Kod: {kod}\n" +
                            $"Kullanıldı: Evet\n" +
                            $"Kod artık sorulmayacak.";
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Başarı maili gönderilemedi: {ex.Message}. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void KopyaIhbariGonder(string telefon, string kod, string storedMac, (string isimSoyisim, string adres)? musteriBilgileri)
        {
            try
            {
                using var mail = new MailMessage();
                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Email, Sifre),
                    EnableSsl = true
                };
                mail.From = new MailAddress(Email);
                mail.To.Add(Email);
                mail.Subject = "Yazılım Kopyalama Girişimi";
                mail.Body = $"Telefon: {telefon}\n" +
                            $"İsim Soyisim: {(musteriBilgileri.HasValue ? musteriBilgileri.Value.isimSoyisim : "Bilinmiyor")}\n" +
                            $"Adres: {(musteriBilgileri.HasValue ? musteriBilgileri.Value.adres : "Bilinmiyor")}\n" +
                            $"Yeni MAC: {MacAdresi()}\n" +
                            $"Kayıtlı MAC: {storedMac}\n" +
                            $"IP: {IpAdresi()}\n" +
                            $"Kod: {kod}\n" +
                            $"Bu kod zaten kullanılmış ve başka bir bilgisayarda tekrar deneniyor!";
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kopya ihbarı maili gönderilemedi: {ex.Message}. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (bool isValid, string? storedMac, bool isUsed, (string isimSoyisim, string adres)? musteriBilgileri) MailKontrol(string telefon, string kod)
        {
            using var istemci = new ImapClient();
            try
            {
                istemci.Connect("imap.gmail.com", 993, true);
                istemci.Authenticate(Email, Sifre);
                var gelen = istemci.Inbox;
                gelen.Open(FolderAccess.ReadOnly);
                var mesajlar = gelen.Fetch(0, -1, MessageSummaryItems.UniqueId);
                string? storedMac = null;
                bool isUsed = false;
                (string isimSoyisim, string adres)? musteriBilgileri = null;

                foreach (var mesaj in mesajlar)
                {
                    var email = gelen.GetMessage(mesaj.UniqueId);
                    if (email.TextBody == null) continue;

                    if (email.TextBody.Contains(telefon) && email.TextBody.Contains(kod))
                    {
                        string[] lines = email.TextBody.Split('\n');
                        string isimSoyisim = "Bilinmiyor";
                        string adres = "Bilinmiyor";
                        foreach (var line in lines)
                        {
                            if (line.StartsWith("İsim Soyisim: ")) isimSoyisim = line.Substring("İsim Soyisim: ".Length).Trim();
                            if (line.StartsWith("Adres: ")) adres = line.Substring("Adres: ".Length).Trim();
                        }
                        musteriBilgileri = (isimSoyisim, adres);

                        // Check if the code has been used by looking for "Kullanıldı: Evet" in the email body
                        if (email.TextBody.Contains("Kullanıldı: Evet"))
                        {
                            var macLine = lines.FirstOrDefault(l => l.StartsWith("MAC: "));
                            storedMac = macLine != null ? macLine.Substring("MAC: ".Length).Trim() : "BilinmeyenMAC";
                            isUsed = true;
                            istemci.Disconnect(true);
                            return (false, storedMac, isUsed, musteriBilgileri); // Code is already used, return false
                        }
                        else if (email.Subject == "Aktivasyon Kodunuz")
                        {
                            // Code is valid and not used yet
                            istemci.Disconnect(true);
                            return (true, storedMac, isUsed, musteriBilgileri);
                        }
                    }
                }
                istemci.Disconnect(true);
                return (false, storedMac, isUsed, musteriBilgileri); // Code not found
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mail kontrolü sırasında hata: {ex.Message}. Lütfen internet bağlantınızı kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, null, false, null);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e) => FormDegistir(new Form9());

        private void button3_Click(object sender, EventArgs e) => Application.Exit();

        private void pictureBox1_Click(object sender, EventArgs e) => FormDegistir(new Form12());

        private void FormDegistir(Form form)
        {
            try
            {
                Hide();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form geçişi sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public partial class Form1 : Form
    {
        public Form1(bool aktif)
        {
            InitializeComponent();
            if (!aktif)
            {
                MessageBox.Show("Hata: Ana ekran direkt açılmamalı! Lütfen aktivasyon işlemini tamamlayın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}