namespace WinFormsApp8
{
    partial class Form21
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form21));
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            timer1 = new System.Windows.Forms.Timer(components);
            notifyIcon1 = new NotifyIcon(components);
            dataGridView1 = new DataGridView();
            groupBox5 = new GroupBox();
            timer2 = new System.Windows.Forms.Timer(components);
            button2 = new Button();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            button1 = new Button();
            textBox3 = new TextBox();
            groupBox2 = new GroupBox();
            pictureBox6 = new PictureBox();
            label8 = new Label();
            pictureBox2 = new PictureBox();
            label7 = new Label();
            pictureBox3 = new PictureBox();
            label1 = new Label();
            pictureBox4 = new PictureBox();
            label4 = new Label();
            label5 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            groupBox3 = new GroupBox();
            textBox2 = new TextBox();
            groupBox4 = new GroupBox();
            textBox4 = new TextBox();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox5.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(20, 22);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(685, 316);
            dataGridView1.TabIndex = 28;
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
            // button2
            // 
            button2.Location = new Point(143, 276);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 58;
            button2.Text = "güncelle";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(93, 239);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(48, 19);
            checkBox2.TabIndex = 57;
            checkBox2.Text = "BOŞ";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(93, 214);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(53, 19);
            checkBox1.TabIndex = 56;
            checkBox1.Text = "GEBE";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(62, 276);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 55;
            button1.Text = "Kayıt et";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(93, 127);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "ad soyad:";
            textBox3.Size = new Size(100, 23);
            textBox3.TabIndex = 53;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.ActiveBorder;
            groupBox2.Controls.Add(pictureBox6);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(pictureBox2);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(pictureBox3);
            groupBox2.Location = new Point(3, -14);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(991, 58);
            groupBox2.TabIndex = 55;
            groupBox2.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.Image = Properties.Resources.sol_ok3;
            pictureBox6.Location = new Point(868, 14);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(46, 38);
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 90;
            pictureBox6.TabStop = false;
            pictureBox6.Click += pictureBox6_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 15F);
            label8.Location = new Point(458, 20);
            label8.Name = "label8";
            label8.Size = new Size(176, 28);
            label8.TabIndex = 87;
            label8.Text = "00/00/0000/00/00";
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(10, 10);
            pictureBox2.TabIndex = 88;
            pictureBox2.TabStop = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Yu Gothic UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label7.Location = new Point(117, 18);
            label7.Name = "label7";
            label7.Size = new Size(145, 30);
            label7.TabIndex = 51;
            label7.Text = "KEDİ GEBELİK";
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(0, 0);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(20, 10);
            pictureBox3.TabIndex = 53;
            pictureBox3.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label1.Location = new Point(0, 133);
            label1.Name = "label1";
            label1.Size = new Size(66, 17);
            label1.TabIndex = 54;
            label1.Text = "ad soyad:";
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
            // textBox1
            // 
            textBox1.Location = new Point(97, 156);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Telefon no:";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 26;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label2.Location = new Point(0, 157);
            label2.Name = "label2";
            label2.Size = new Size(74, 17);
            label2.TabIndex = 39;
            label2.Text = "Telefon no:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label3.Location = new Point(0, 185);
            label3.Name = "label3";
            label3.Size = new Size(39, 17);
            label3.TabIndex = 40;
            label3.Text = "Cinsi:";
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.ActiveBorder;
            groupBox3.Controls.Add(button2);
            groupBox3.Controls.Add(checkBox2);
            groupBox3.Controls.Add(checkBox1);
            groupBox3.Controls.Add(button1);
            groupBox3.Controls.Add(textBox3);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(pictureBox4);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(textBox1);
            groupBox3.Controls.Add(textBox2);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(label3);
            groupBox3.Location = new Point(10, 22);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(242, 344);
            groupBox3.TabIndex = 49;
            groupBox3.TabStop = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(97, 185);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "cinsi:";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 27;
            // 
            // groupBox4
            // 
            groupBox4.BackColor = SystemColors.ActiveBorder;
            groupBox4.Controls.Add(textBox4);
            groupBox4.Controls.Add(pictureBox1);
            groupBox4.Controls.Add(groupBox3);
            groupBox4.Controls.Add(groupBox5);
            groupBox4.Location = new Point(3, 58);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(991, 394);
            groupBox4.TabIndex = 56;
            groupBox4.TabStop = false;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(720, 0);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "Telefon no göre filtreleme";
            textBox4.Size = new Size(221, 23);
            textBox4.TabIndex = 52;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.Image = Properties.Resources.pngtree_search_flat_red_color_icon_test_search_glass_vector_png_image_19940566;
            pictureBox1.Location = new Point(672, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(33, 23);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 53;
            pictureBox1.TabStop = false;
            // 
            // Form21
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(995, 448);
            ControlBox = false;
            Controls.Add(groupBox2);
            Controls.Add(groupBox4);
            Name = "Form21";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KEDİ GEBELİK";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer1;
        private NotifyIcon notifyIcon1;
        private DataGridView dataGridView1;
        private GroupBox groupBox5;
        private System.Windows.Forms.Timer timer2;
        private Button button2;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Button button1;
        private TextBox textBox3;
        private GroupBox groupBox2;
        private Label label8;
        private PictureBox pictureBox2;
        private Label label7;
        private PictureBox pictureBox3;
        private Label label1;
        private PictureBox pictureBox4;
        private Label label4;
        private Label label5;
        private TextBox textBox1;
        private Label label2;
        private Label label3;
        private GroupBox groupBox3;
        private TextBox textBox2;
        private GroupBox groupBox4;
        private TextBox textBox4;
        private PictureBox pictureBox1;
        private PictureBox pictureBox6;
    }
}