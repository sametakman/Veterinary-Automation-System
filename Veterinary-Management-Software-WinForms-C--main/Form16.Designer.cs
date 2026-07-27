namespace WinFormsApp8
{
    partial class Form16
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form16));
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            radioButton4 = new RadioButton();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            label6 = new Label();
            SuspendLayout();
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(40, 11);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(146, 19);
            radioButton1.TabIndex = 0;
            radioButton1.Text = "YERLİ IRK ERKEK DANA";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(40, 117);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(162, 19);
            radioButton2.TabIndex = 1;
            radioButton2.Text = "KÜLTÜR IRKI ERKEK DANA";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(40, 40);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(133, 19);
            radioButton3.TabIndex = 2;
            radioButton3.Text = "YERLİ IRK DİŞİ DANA";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(40, 92);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(149, 19);
            radioButton4.TabIndex = 3;
            radioButton4.Text = "KÜLTÜR IRKI DİŞİ DANA";
            radioButton4.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(40, 170);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Ağırlık (kg)";
            textBox1.Size = new Size(220, 23);
            textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(40, 210);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Aylık Kilo Artışı (kg)";
            textBox2.Size = new Size(220, 23);
            textBox2.TabIndex = 5;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(40, 250);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Yem Türü";
            textBox3.Size = new Size(220, 23);
            textBox3.TabIndex = 6;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(40, 290);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "Yem Miktarı (kg)";
            textBox4.Size = new Size(220, 23);
            textBox4.TabIndex = 7;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(40, 330);
            textBox5.Name = "textBox5";
            textBox5.PlaceholderText = "E-posta Adresi";
            textBox5.Size = new Size(220, 23);
            textBox5.TabIndex = 8;
            // 
            // button1
            // 
            button1.Location = new Point(40, 370);
            button1.Name = "button1";
            button1.Size = new Size(100, 30);
            button1.TabIndex = 9;
            button1.Text = "Hesapla";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(160, 370);
            button2.Name = "button2";
            button2.Size = new Size(100, 30);
            button2.TabIndex = 10;
            button2.Text = "Yem Ekle";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(628, 5);
            button3.Name = "button3";
            button3.Size = new Size(100, 30);
            button3.TabIndex = 11;
            button3.Text = "ANASAYFA";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(278, 370);
            button4.Name = "button4";
            button4.Size = new Size(100, 30);
            button4.TabIndex = 12;
            button4.Text = "E-posta Gönder";
            button4.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.Red;
            label6.Location = new Point(294, 61);
            label6.Name = "label6";
            label6.Size = new Size(194, 15);
            label6.TabIndex = 13;
            label6.Text = "Yem stoğu boş. Lütfen yem ekleyin.";
            // 
            // Form16
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(730, 470);
            ControlBox = false;
            Controls.Add(label6);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(radioButton4);
            Controls.Add(radioButton3);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form16";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BESİRASYON";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label6;
        private PictureBox pictureBox1;
        private Label label2;
    }
}