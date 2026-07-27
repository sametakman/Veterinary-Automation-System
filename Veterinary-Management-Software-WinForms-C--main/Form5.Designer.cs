namespace WinFormsApp8
{
    partial class Form5
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form5));
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            checkedListBox1 = new CheckedListBox();
            dataGridView1 = new DataGridView();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            groupBox3 = new GroupBox();
            pictureBox5 = new PictureBox();
            pictureBox1 = new PictureBox();
            pictureBox4 = new PictureBox();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            pictureBox7 = new PictureBox();
            pictureBox6 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            SuspendLayout();
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "NAKİT ÖDEME", "KREDİ KARTI" });
            checkedListBox1.Location = new Point(126, 286);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(200, 40);
            checkedListBox1.TabIndex = 50;
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(369, 22);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(811, 435);
            dataGridView1.TabIndex = 49;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(126, 143);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "adı soyadı:";
            textBox1.Size = new Size(200, 23);
            textBox1.TabIndex = 45;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(126, 173);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "telefon no:";
            textBox2.Size = new Size(200, 23);
            textBox2.TabIndex = 46;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(126, 214);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "yapılan işlem";
            textBox3.Size = new Size(200, 23);
            textBox3.TabIndex = 47;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(126, 257);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "işlem ücreti:";
            textBox4.Size = new Size(200, 23);
            textBox4.TabIndex = 48;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(810, 35);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(248, 23);
            textBox5.TabIndex = 55;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.ForeColor = Color.Red;
            label1.Image = (Image)resources.GetObject("label1.Image");
            label1.Location = new Point(45, 143);
            label1.Name = "label1";
            label1.Size = new Size(58, 15);
            label1.TabIndex = 56;
            label1.Text = "Ad Soyad";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.ForeColor = Color.Red;
            label2.Image = (Image)resources.GetObject("label2.Image");
            label2.Location = new Point(53, 178);
            label2.Name = "label2";
            label2.Size = new Size(52, 15);
            label2.TabIndex = 57;
            label2.Text = "Telefon:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.ForeColor = Color.Red;
            label3.Image = (Image)resources.GetObject("label3.Image");
            label3.Location = new Point(64, 219);
            label3.Name = "label3";
            label3.Size = new Size(40, 15);
            label3.TabIndex = 58;
            label3.Text = "İşlem:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.ForeColor = Color.Red;
            label4.Image = (Image)resources.GetObject("label4.Image");
            label4.Location = new Point(30, 262);
            label4.Name = "label4";
            label4.Size = new Size(78, 15);
            label4.TabIndex = 59;
            label4.Text = "İşlem Ücreti:";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.temizle;
            pictureBox2.Location = new Point(264, 368);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(46, 38);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 62;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(136, 368);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(42, 38);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 63;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // groupBox3
            // 
            groupBox3.BackgroundImage = (Image)resources.GetObject("groupBox3.BackgroundImage");
            groupBox3.Controls.Add(pictureBox5);
            groupBox3.Controls.Add(pictureBox1);
            groupBox3.Controls.Add(pictureBox4);
            groupBox3.Controls.Add(textBox5);
            groupBox3.Location = new Point(12, 14);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(1198, 88);
            groupBox3.TabIndex = 64;
            groupBox3.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.BackgroundImage = (Image)resources.GetObject("pictureBox5.BackgroundImage");
            pictureBox5.Image = (Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.Location = new Point(747, 33);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(39, 31);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 56;
            pictureBox5.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(6, 11);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(154, 73);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.BackgroundImage = (Image)resources.GetObject("pictureBox4.BackgroundImage");
            pictureBox4.Image = Properties.Resources.btnHome;
            pictureBox4.Location = new Point(1111, 19);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(69, 55);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 7;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // groupBox1
            // 
            groupBox1.BackgroundImage = (Image)resources.GetObject("groupBox1.BackgroundImage");
            groupBox1.Controls.Add(dataGridView1);
            groupBox1.Location = new Point(12, 115);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1198, 476);
            groupBox1.TabIndex = 65;
            groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.BackgroundImage = (Image)resources.GetObject("groupBox2.BackgroundImage");
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(pictureBox7);
            groupBox2.Controls.Add(pictureBox6);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Controls.Add(textBox4);
            groupBox2.Controls.Add(textBox3);
            groupBox2.Controls.Add(pictureBox2);
            groupBox2.Controls.Add(pictureBox3);
            groupBox2.Controls.Add(textBox2);
            groupBox2.Controls.Add(checkedListBox1);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label2);
            groupBox2.Location = new Point(12, 115);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(347, 476);
            groupBox2.TabIndex = 66;
            groupBox2.TabStop = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label7.ForeColor = Color.Red;
            label7.Image = (Image)resources.GetObject("label7.Image");
            label7.Location = new Point(264, 423);
            label7.Name = "label7";
            label7.Size = new Size(50, 15);
            label7.TabIndex = 66;
            label7.Text = "Temizle";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label6.ForeColor = Color.RoyalBlue;
            label6.Image = (Image)resources.GetObject("label6.Image");
            label6.Location = new Point(197, 423);
            label6.Name = "label6";
            label6.Size = new Size(48, 15);
            label6.TabIndex = 65;
            label6.Text = "Ödeme";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.ForeColor = Color.Green;
            label5.Image = (Image)resources.GetObject("label5.Image");
            label5.Location = new Point(136, 423);
            label5.Name = "label5";
            label5.Size = new Size(35, 15);
            label5.TabIndex = 64;
            label5.Text = "Kayıt";
            // 
            // pictureBox7
            // 
            pictureBox7.BackgroundImage = (Image)resources.GetObject("pictureBox7.BackgroundImage");
            pictureBox7.Image = (Image)resources.GetObject("pictureBox7.Image");
            pictureBox7.Location = new Point(197, 362);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(56, 44);
            pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 57;
            pictureBox7.TabStop = false;
            pictureBox7.Click += pictureBox7_Click;
            // 
            // pictureBox6
            // 
            pictureBox6.BackgroundImage = (Image)resources.GetObject("pictureBox6.BackgroundImage");
            pictureBox6.Image = (Image)resources.GetObject("pictureBox6.Image");
            pictureBox6.Location = new Point(126, 22);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(138, 115);
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 57;
            pictureBox6.TabStop = false;
            // 
            // Form5
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(1220, 603);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(groupBox3);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form5";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Veresiye Defteri";
            Load += Form5_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private CheckedListBox checkedListBox1;
        private DataGridView dataGridView1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox5;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private GroupBox groupBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;
        private PictureBox pictureBox7;
        private Label label7;
        private Label label6;
        private Label label5;
    }
}