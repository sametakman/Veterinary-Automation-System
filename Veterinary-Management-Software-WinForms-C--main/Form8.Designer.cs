namespace WinFormsApp8
{
    partial class Form8
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form8));
            textBox1 = new TextBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            dataGridView1 = new DataGridView();
            textBox2 = new TextBox();
            checkedListBox1 = new CheckedListBox();
            timer1 = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            label3 = new Label();
            notifyIcon1 = new NotifyIcon(components);
            pictureBox1 = new PictureBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            pictureBox2 = new PictureBox();
            groupBox2 = new GroupBox();
            pictureBox5 = new PictureBox();
            label8 = new Label();
            label7 = new Label();
            pictureBox3 = new PictureBox();
            groupBox3 = new GroupBox();
            pictureBox4 = new PictureBox();
            groupBox4 = new GroupBox();
            groupBox5 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(93, 133);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "küpe no:";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 26;
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(20, 22);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(690, 316);
            dataGridView1.TabIndex = 28;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(93, 162);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "cinsi:";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 27;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "gebe ", "boş" });
            checkedListBox1.Location = new Point(93, 191);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(100, 40);
            checkedListBox1.TabIndex = 32;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label2.Location = new Point(19, 136);
            label2.Name = "label2";
            label2.Size = new Size(64, 17);
            label2.TabIndex = 39;
            label2.Text = "Küpe No:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label3.Location = new Point(44, 170);
            label3.Name = "label3";
            label3.Size = new Size(39, 17);
            label3.TabIndex = 40;
            label3.Text = "Cinsi:";
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.btnUpdate_1;
            pictureBox1.Location = new Point(147, 260);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(46, 38);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 43;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label4.ForeColor = Color.Yellow;
            label4.Location = new Point(141, 302);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 44;
            label4.Text = "Güncelle";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label5.ForeColor = Color.Green;
            label5.Location = new Point(78, 302);
            label5.Name = "label5";
            label5.Size = new Size(50, 15);
            label5.TabIndex = 45;
            label5.Text = "Kayıt et";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label6.Location = new Point(932, 42);
            label6.Name = "label6";
            label6.Size = new Size(53, 13);
            label6.TabIndex = 46;
            label6.Text = "Anasayfa";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(81, 260);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(46, 38);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 45;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.ActiveBorder;
            groupBox2.Controls.Add(pictureBox5);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(pictureBox3);
            groupBox2.Controls.Add(label6);
            groupBox2.Location = new Point(2, -2);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(991, 58);
            groupBox2.TabIndex = 48;
            groupBox2.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = Properties.Resources.images;
            pictureBox5.Location = new Point(823, 14);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(46, 38);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 53;
            pictureBox5.TabStop = false;
            pictureBox5.Click += pictureBox5_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label8.Location = new Point(409, 20);
            label8.Name = "label8";
            label8.Size = new Size(176, 25);
            label8.TabIndex = 52;
            label8.Text = "00/00/0000/00/00";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Yu Gothic UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label7.Location = new Point(27, 18);
            label7.Name = "label7";
            label7.Size = new Size(187, 30);
            label7.TabIndex = 51;
            label7.Text = "İnek Gebelik Takip";
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.btnHome;
            pictureBox3.Location = new Point(945, 4);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(38, 36);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 50;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.ActiveBorder;
            groupBox3.Controls.Add(pictureBox4);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(pictureBox1);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(pictureBox2);
            groupBox3.Controls.Add(textBox1);
            groupBox3.Controls.Add(checkedListBox1);
            groupBox3.Controls.Add(textBox2);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(label3);
            groupBox3.Location = new Point(10, 22);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(242, 344);
            groupBox3.TabIndex = 49;
            groupBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(62, 25);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(96, 83);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 52;
            pictureBox4.TabStop = false;
            // 
            // groupBox4
            // 
            groupBox4.BackColor = SystemColors.ActiveBorder;
            groupBox4.Controls.Add(groupBox3);
            groupBox4.Controls.Add(groupBox5);
            groupBox4.Location = new Point(2, 70);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(991, 394);
            groupBox4.TabIndex = 50;
            groupBox4.TabStop = false;
            groupBox4.Enter += groupBox4_Enter;
            // 
            // groupBox5
            // 
            groupBox5.BackColor = SystemColors.ActiveBorder;
            groupBox5.Controls.Add(dataGridView1);
            groupBox5.Location = new Point(258, 22);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(716, 344);
            groupBox5.TabIndex = 51;
            groupBox5.TabStop = false;
            // 
            // Form8
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(224, 224, 224);
            ClientSize = new Size(989, 463);
            Controls.Add(groupBox4);
            Controls.Add(groupBox2);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form8";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gebelik";
           
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DataGridView dataGridView1;
        private TextBox textBox2;
        private CheckedListBox checkedListBox1;
        private System.Windows.Forms.Timer timer1;
        private Label label2;
        private Label label3;
        private NotifyIcon notifyIcon1;
        private PictureBox pictureBox1;
        private Label label4;
        private Label label5;
        private Label label6;
        private PictureBox pictureBox2;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private PictureBox pictureBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private Label label7;
        private PictureBox pictureBox4;
        private Label label8;
        private PictureBox pictureBox5;
    }
}