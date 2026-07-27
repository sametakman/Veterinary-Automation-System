namespace WinFormsApp8
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.RichTextBox richTextBox1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            richTextBox1 = new RichTextBox();
            pictureBox4 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            groupBox2 = new GroupBox();
            label4 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            groupBox1 = new GroupBox();
            label1 = new Label();
            groupBox3 = new GroupBox();
            label5 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 100);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Hayvan sahibi:";
            textBox1.Size = new Size(251, 23);
            textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(12, 158);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Telefon no:";
            textBox2.Size = new Size(251, 23);
            textBox2.TabIndex = 1;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(12, 129);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Küpe no:";
            textBox3.Size = new Size(251, 23);
            textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(12, 187);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "Hayvan cinsi:";
            textBox4.Size = new Size(251, 23);
            textBox4.TabIndex = 3;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(6, 26);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(566, 316);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.btnHome;
            pictureBox4.Location = new Point(799, 5);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(50, 42);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 64;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.temizle;
            pictureBox2.Location = new Point(174, 231);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(59, 49);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 65;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(76, 231);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(55, 49);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 66;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.ControlLight;
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(pictureBox1);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Controls.Add(textBox2);
            groupBox2.Controls.Add(pictureBox2);
            groupBox2.Controls.Add(textBox3);
            groupBox2.Controls.Add(pictureBox3);
            groupBox2.Controls.Add(textBox4);
            groupBox2.Location = new Point(13, 82);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(269, 362);
            groupBox2.TabIndex = 67;
            groupBox2.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.FlatStyle = FlatStyle.Flat;
            label4.ForeColor = Color.Red;
            label4.Location = new Point(176, 293);
            label4.Name = "label4";
            label4.Size = new Size(47, 15);
            label4.TabIndex = 67;
            label4.Text = "Temizle";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.FlatStyle = FlatStyle.Flat;
            label2.ForeColor = Color.Lime;
            label2.Location = new Point(85, 293);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 6;
            label2.Text = "Kayıt et";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(76, 16);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(94, 78);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 66;
            pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ControlLight;
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(richTextBox1);
            groupBox1.Location = new Point(288, 82);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(591, 362);
            groupBox1.TabIndex = 68;
            groupBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 8);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 5;
            label1.Text = "Adres :";
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.ControlLight;
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(pictureBox4);
            groupBox3.Location = new Point(12, 6);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(865, 70);
            groupBox3.TabIndex = 69;
            groupBox3.TabStop = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.FlatStyle = FlatStyle.Flat;
            label5.ForeColor = Color.Blue;
            label5.Location = new Point(802, 50);
            label5.Name = "label5";
            label5.Size = new Size(56, 15);
            label5.TabIndex = 68;
            label5.Text = "AnaSayfa";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label3.ForeColor = SystemColors.ActiveCaption;
            label3.Location = new Point(23, 22);
            label3.Name = "label3";
            label3.Size = new Size(200, 25);
            label3.TabIndex = 65;
            label3.Text = "Müşteri Kayıt Bölümü";
            // 
            // Form2
            // 
            BackColor = SystemColors.ActiveBorder;
            ClientSize = new Size(889, 456);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox3);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form2";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hayvan Kayıt";
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }
        private PictureBox pictureBox4;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private PictureBox pictureBox1;
        private Label label3;
        private Label label4;
        private Label label2;
        private Label label1;
        private Label label5;
    }
}
