using System;
using System.IO;
using System.Windows.Forms;

namespace WinFormsApp8
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                string isActivatedFilePath = Path.Combine(Application.StartupPath, "isActivated.txt");
                bool isActivated = false;

                try
                {
                    isActivated = File.Exists(isActivatedFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dosya kontrolü sýrasýnda hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Form startForm;
                try
                {
                    startForm = isActivated ? new Form1(true) : new Form10();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Form baþlatýlýrken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Application.Run(startForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama baþlatýlýrken hata: {ex.Message}", "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}