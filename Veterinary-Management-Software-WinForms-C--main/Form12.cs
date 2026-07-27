using System;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;

namespace WinFormsApp8
{
    public partial class Form12 : Form
    {
        private string sistemEmail = "otomasyonveteriner@gmail.com";
        private string emailPassword = "ftkx mebp kdfn ufrj"; // Gmail uygulama şifresi
        private string currentTelefonNo; // Onay kontrolü için telefon numarasını sakla
        private string currentAktivasyonKodu; // Onay kontrolü için aktivasyon kodunu sakla

        public Form12()
        {
            InitializeComponent();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form10 form10 = Application.OpenForms["Form10"] as Form10;
            if (form10 != null)
                form10.Show();
            else
            {
                form10 = new Form10();
                form10.Show();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                string isimSoyisim = TruncateString(textBox1.Text, 255);
                string telefonNo = TruncateString(textBox3.Text, 11);
                string adres = TruncateString(richTextBox1.Text, 255);
                string macAddress = GetMacAddress(); // MAC adresini al
                string ipAddress = GetIpAddress();  // IP adresini al

                if (string.IsNullOrEmpty(telefonNo))
                {
                    MessageBox.Show("Lütfen telefon numaranızı giriniz!");
                    return;
                }

                // Telefon numarasının daha önce kayıtlı olup olmadığını kontrol et
                if (HasPhoneNumberReceivedCode(telefonNo))
                {
                    MessageBox.Show("Bu telefon numarası zaten kayıtlı!");
                    return;
                }

                string aktivasyonKodu = GenerateUniqueActivationCode(macAddress, ipAddress); // MAC ve IP’ye göre kod üret
                currentTelefonNo = telefonNo; // Onay kontrolü için sakla
                currentAktivasyonKodu = aktivasyonKodu; // Onay kontrolü için sakla

                SendActivationEmail(sistemEmail, aktivasyonKodu, isimSoyisim, adres, telefonNo, macAddress, ipAddress);

                MessageBox.Show("Kayıt başarıyla eklendi! Aktivasyon kodu e-posta adresinize gönderildi. Lütfen giriş yapınız.");

                // Kullanıcı aktivasyonunu kontrol et
                if (CheckEmailForActivation(telefonNo, aktivasyonKodu))
                {
                    SendStatusEmail(sistemEmail, "Giriş Başarılı", telefonNo, aktivasyonKodu);
                    textBox1.Clear();
                    textBox3.Clear();
                    richTextBox1.Clear();

                    this.Hide();
                    Form10 form10 = Application.OpenForms["Form10"] as Form10;
                    if (form10 != null)
                        form10.Show();
                    else
                    {
                        form10 = new Form10();
                        form10.Show();
                    }
                }
                else
                {
                    SendStatusEmail(sistemEmail, "Giriş Başarısız - Hatalı Kod", telefonNo, aktivasyonKodu);
                    MessageBox.Show("E-posta ile gönderilen telefon numarası veya aktivasyon kodu eşleşmedi! Aktivasyon kodu iptal edildi.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        // Telefon numarasının daha önce kod alıp almadığını kontrol et
        private bool HasPhoneNumberReceivedCode(string telefonNo)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(sistemEmail, emailPassword);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    var messages = inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
                    foreach (var message in messages)
                    {
                        var email = inbox.GetMessage(message.UniqueId);
                        string emailBody = email.TextBody ?? "";
                        if (emailBody.Contains(telefonNo) && emailBody.Contains("Aktivasyon Kodunuz"))
                        {
                            client.Disconnect(true);
                            return true; // Telefon numarası zaten kayıtlı
                        }
                    }

                    client.Disconnect(true);
                    return false; // Telefon numarası kayıtlı değil
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Telefon numarası kontrolü sırasında hata: " + ex.Message);
                return false;
            }
        }

        // E-posta ile aktivasyon kodunu kontrol et
        private bool CheckEmailForActivation(string telefonNo, string aktivasyonKodu)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(sistemEmail, emailPassword);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    var messages = inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
                    for (int i = Math.Max(0, messages.Count - 5); i < messages.Count; i++)
                    {
                        var message = inbox.GetMessage(messages[i].UniqueId);
                        string emailBody = message.TextBody ?? "";
                        if (emailBody.Contains(telefonNo) && emailBody.Contains(aktivasyonKodu))
                        {
                            client.Disconnect(true);
                            return true;
                        }
                    }

                    client.Disconnect(true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("E-posta kontrolü sırasında hata: " + ex.Message);
                return false;
            }
        }

        // MAC adresini alma
        private string GetMacAddress()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        return ni.GetPhysicalAddress().ToString();
                    }
                }
                return "UnknownMAC";
            }
            catch (Exception)
            {
                return "ErrorMAC";
            }
        }

        // IP adresini alma
        private string GetIpAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4
                    {
                        return address.ToString();
                    }
                }
                return "UnknownIP";
            }
            catch (Exception)
            {
                return "ErrorIP";
            }
        }

        // MAC ve IP’ye göre benzersiz aktivasyon kodu üret
        private string GenerateUniqueActivationCode(string macAddress, string ipAddress)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder code = new StringBuilder();

            // MAC ve IP’den türetilmiş bir başlangıç ekle
            string macPart = macAddress.Length >= 6 ? macAddress.Substring(0, 6) : macAddress.PadRight(6, '0');
            string ipPart = ipAddress.Replace(".", "").Length >= 4 ? ipAddress.Replace(".", "").Substring(0, 4) : ipAddress.Replace(".", "").PadRight(4, '0');
            code.Append(macPart.Substring(0, 3).ToUpper()); // MAC’in ilk 3 karakteri
            code.Append(ipPart.Substring(0, 2).ToUpper());  // IP’nin ilk 2 karakteri

            // Rastgele 6 karakter ekle
            bool hasLetter = false, hasDigit = false;
            while (code.Length < 11 || !hasLetter || !hasDigit)
            {
                char nextChar = chars[random.Next(chars.Length)];
                code.Append(nextChar);
                if (char.IsLetter(nextChar)) hasLetter = true;
                if (char.IsDigit(nextChar)) hasDigit = true;
            }

            return code.ToString().Substring(0, 11); // 11 karaktere sabitle
        }

        // Aktivasyon e-postası gönder (MAC ve IP eklendi)
        private void SendActivationEmail(string email, string aktivasyonKodu, string isimSoyisim, string adres, string telefonNo, string macAddress, string ipAddress)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
                {
                    mail.From = new MailAddress(sistemEmail);
                    mail.To.Add(email);
                    mail.Subject = "Aktivasyon Kodunuz";
                    mail.Body = $"Merhaba,\n\n" +
                                $"Veteriner otomasyon sistemine kaydınız başarıyla alınmıştır. Aşağıda kayıt bilgileriniz ve aktivasyon kodunuz yer almaktadır:\n\n" +
                                $"İsim Soyisim: {isimSoyisim}\n" +
                                $"Adres: {adres}\n" +
                                $"Telefon Numarası: {telefonNo}\n" +
                                $"MAC Adresi: {macAddress}\n" +
                                $"IP Adresi: {ipAddress}\n" +
                                $"Aktivasyon Kodunuz: {aktivasyonKodu}\n\n" +
                                $"Bu kodu kullanarak sistemdeki kaydınızı aktive edebilirsiniz.\n" +
                                $"Herhangi bir sorunuz olursa bizimle iletişime geçmekten çekinmeyin.\n\n" +
                                $"İyi günler dileriz!";

                    smtpServer.Port = 587;
                    smtpServer.Credentials = new NetworkCredential(sistemEmail, emailPassword);
                    smtpServer.EnableSsl = true;

                    smtpServer.Send(mail);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("E-posta gönderilirken hata oluştu: " + ex.Message);
            }
        }

        // Durum e-postası gönder
        private void SendStatusEmail(string adminEmail, string status, string telefonNo, string aktivasyonKodu)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
                {
                    mail.From = new MailAddress(sistemEmail);
                    mail.To.Add(adminEmail);
                    mail.Subject = "Kayıt Durum Bildirimi";
                    mail.Body = $"Sayın Admin,\n\n" +
                                $"Aşağıdaki kayıt için işlem durumu: {status}\n\n" +
                                $"Telefon Numarası: {telefonNo}\n" +
                                $"Aktivasyon Kodu: {aktivasyonKodu}\n\n" +
                                $"Bilgilendirme: \n" +
                                $"- Durum 'Giriş Başarılı' ise: Kullanıcı giriş yaptı.\n" +
                                $"- Durum 'Giriş Başarısız' ise: Kullanıcı giriş yapamadı, aktivasyon kodu iptal edildi.\n\n" +
                                $"İyi çalışmalar!";

                    smtpServer.Port = 587;
                    smtpServer.Credentials = new NetworkCredential(sistemEmail, emailPassword);
                    smtpServer.EnableSsl = true;

                    smtpServer.Send(mail);
                    MessageBox.Show($"Durum e-postası gönderildi: {status}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Durum e-postası gönderilirken hata oluştu: " + ex.Message);
            }
        }

        // String kesme fonksiyonu
        private string TruncateString(string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, maxLength));
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox3.Clear();
            richTextBox1.Clear();
        }

        private void Form12_Load(object sender, EventArgs e)
        {
        }
    }
}