namespace WinFormsApp8
{
    partial class Form12
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form12));
            label3 = new Label();
            textBox1 = new TextBox();
            textBox3 = new TextBox();
            label1 = new Label();
            groupBox1 = new GroupBox();
            richTextBox1 = new RichTextBox();
            label4 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            label5 = new Label();
            pictureBox4 = new PictureBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label3.ForeColor = SystemColors.ActiveCaption;
            label3.Location = new Point(23, 22);
            label3.Name = "label3";
            label3.Size = new Size(183, 25);
            label3.TabIndex = 65;
            label3.Text = "AKTİVASYON KAYIT";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 143);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "isim soyisim:";
            textBox1.Size = new Size(251, 23);
            textBox1.TabIndex = 0;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(12, 189);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Telefon no:";
            textBox3.Size = new Size(251, 23);
            textBox3.TabIndex = 2;
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
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ControlLight;
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(richTextBox1);
            groupBox1.Location = new Point(278, 88);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(591, 362);
            groupBox1.TabIndex = 71;
            groupBox1.TabStop = false;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(6, 26);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(566, 316);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
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
            pictureBox1.Size = new Size(130, 105);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 66;
            pictureBox1.TabStop = false;
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
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.ControlLight;
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(pictureBox1);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Controls.Add(pictureBox2);
            groupBox2.Controls.Add(textBox3);
            groupBox2.Controls.Add(pictureBox3);
            groupBox2.Location = new Point(3, 88);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(269, 362);
            groupBox2.TabIndex = 70;
            groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.ControlLight;
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(pictureBox4);
            groupBox3.Location = new Point(2, 12);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(865, 70);
            groupBox3.TabIndex = 72;
            groupBox3.TabStop = false;
            // 
            // Form12
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(871, 453);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox3);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form12";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form12";
            Load += Form12_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label3;
        private TextBox textBox1;
        private TextBox textBox3;
        private Label label1;
        private GroupBox groupBox1;
        private RichTextBox richTextBox1;
        private Label label4;
        private Label label2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Label label5;
        private PictureBox pictureBox4;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
    }
}