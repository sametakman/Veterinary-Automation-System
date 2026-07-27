using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WinFormsApp8
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
        }

        // pictureBox13'e tıklandığında formun hangi kaynaktan geldiğini kontrol et
        private void pictureBox13_Click(object sender, EventArgs e)
        {
            // Form10'dan gelindi mi kontrol et
            if (Application.OpenForms["Form10"] != null)
            {
                // Form10'dan gelindiyse, "Yetkisiz Giriş" uyarısı ve Form10'a yönlendir
                MessageBox.Show("Yetkisiz Giriş!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Form10'u göster
                Form10 form10 = Application.OpenForms["Form10"] as Form10;
                if (form10 != null)
                {
                    form10.Show();
                }
                this.Hide(); // Form9'u gizle
            }
            else
            {
                // Diğer formlardan gelindiyse, Form1'e yönlendir
                Form1 form1 = Application.OpenForms["Form1"] as Form1;
                if (form1 != null)
                {
                    form1.Show();
                }
                else
                {
                    form1 = new Form1(); // Eğer Form1 kapalıysa, yeni bir Form1 oluştur
                    form1.Show();
                }
                this.Hide(); // Form9'u gizle
            }
        }

        // pictureBox3'e tıklandığında Instagram URL'sini aç
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string url = "https://www.instagram.com/ridvanyaglidere_?utm_source=ig_web_button_share_sheet&igsh=ZDNlZDc0MzIxNw=="; // Açılacak URL
            OpenUrl(url);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string url = "https://www.linkedin.com/in/r%C4%B1dvan-ya%C4%9Fl%C4%B1dere-041800265/"; // LinkedIn URL'si
            OpenUrl(url);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            string telefonNumarasi = "0535 300 2362";
            string isim = "RIDVAN YAĞLIDERE";
            // Telefon numarası ve ismi bir mesaj kutusunda göster
            MessageBox.Show($"İsim: {isim}\nTelefon Numarası: {telefonNumarasi}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string telefonNumarasi = "9005353002362";
            string whatsappLink = $"https://wa.me/{telefonNumarasi}"; // WhatsApp bağlantısı
            OpenUrl(whatsappLink);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            string telefonNumarasi = "+90 541 482 17 99";
            string isim = "Ata Kurnaz";
            // Telefon numarası ve ismi bir mesaj kutusunda göster
            MessageBox.Show($"İsim: {isim}\nTelefon Numarası: {telefonNumarasi}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            string telefonNumarasi = "9005414821799";
            string whatsappLink = $"https://wa.me/{telefonNumarasi}"; // WhatsApp bağlantısı
            OpenUrl(whatsappLink);
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            string url = "https://www.instagram.com/krnzata/?utm_source=ig_web_button_share_sheet"; // Açılacak URL
            OpenUrl(url);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            string telefonNumarasi = "+905353956302";
            string whatsappLink = $"https://wa.me/{telefonNumarasi}"; // WhatsApp bağlantısı
            OpenUrl(whatsappLink);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            string telefonNumarasi = "+90 535 395 63 02";
            string isim = "Samet Akman";
            // Telefon numarası ve ismi bir mesaj kutusunda göster
            MessageBox.Show($"İsim: {isim}\nTelefon Numarası: {telefonNumarasi}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            string url = "https://www.instagram.com/smtakmann/?utm_source=ig_web_button_share_sheet"; // Açılacak URL
            OpenUrl(url);
        }

        // URL açma işlemini genel bir metotla yapalım
        private void OpenUrl(string url)
        {
            try
            {
                // URL'nin geçerli olup olmadığını kontrol et
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    // ProcessStartInfo ile URL'yi varsayılan tarayıcıda aç
                    var psi = new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true // İşletim sistemi kabuğunu kullanarak aç
                    };
                    Process.Start(psi);
                }
                else
                {
                    MessageBox.Show("Geçersiz URL formatı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıyı bilgilendir
                MessageBox.Show("URL açılırken bir hata oluştu: " + ex.Message + "\nLütfen varsayılan bir tarayıcı ayarlayın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
