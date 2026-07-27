namespace WinFormsApp8
{
    partial class Form20
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form20));
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            groupBox1 = new GroupBox();
            button1 = new Button();
            button2 = new Button();
            richTextBox1 = new RichTextBox();
            textBox1 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            richTextBox2 = new RichTextBox();
            label3 = new Label();
            dataGridView1 = new DataGridView();
            label5 = new Label();
            pictureBox1 = new PictureBox();
            label4 = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(6, 35);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(50, 19);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "KEDİ";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(6, 74);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(59, 19);
            checkBox2.TabIndex = 2;
            checkBox2.Text = "Köpek";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ActiveCaptionText;
            groupBox1.Controls.Add(checkBox2);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.ForeColor = Color.Gold;
            groupBox1.Location = new Point(155, 27);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(244, 103);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "hayvan seçimi";
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveCaptionText;
            button1.ForeColor = Color.Gold;
            button1.Location = new Point(144, 651);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "KAYIT ET";
            button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ActiveCaptionText;
            button2.ForeColor = Color.Gold;
            button2.Location = new Point(309, 651);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 6;
            button2.Text = "BUL";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.Gold;
            richTextBox1.ForeColor = SystemColors.ActiveCaptionText;
            richTextBox1.Location = new Point(155, 252);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(244, 175);
            richTextBox1.TabIndex = 7;
            richTextBox1.Text = "";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.Gold;
            textBox1.ForeColor = SystemColors.ActiveCaptionText;
            textBox1.Location = new Point(155, 198);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(244, 23);
            textBox1.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ActiveCaptionText;
            label1.ForeColor = Color.Gold;
            label1.Location = new Point(48, 206);
            label1.Name = "label1";
            label1.Size = new Size(73, 15);
            label1.TabIndex = 9;
            label1.Text = "Hastalık Adı:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.ActiveCaptionText;
            label2.ForeColor = Color.Gold;
            label2.Location = new Point(48, 252);
            label2.Name = "label2";
            label2.Size = new Size(101, 15);
            label2.TabIndex = 10;
            label2.Text = "Hastalık Belirtileri:";
            // 
            // richTextBox2
            // 
            richTextBox2.BackColor = Color.Gold;
            richTextBox2.ForeColor = SystemColors.ActiveCaptionText;
            richTextBox2.Location = new Point(161, 458);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(244, 175);
            richTextBox2.TabIndex = 11;
            richTextBox2.Text = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = SystemColors.ActiveCaptionText;
            label3.ForeColor = Color.Gold;
            label3.Location = new Point(48, 458);
            label3.Name = "label3";
            label3.Size = new Size(100, 15);
            label3.TabIndex = 12;
            label3.Text = "Hastalık Tedavisi :";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(809, -1);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(830, 242);
            dataGridView1.TabIndex = 13;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(854, 242);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 15;
            label5.Text = "label5";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.btnHome1;
            pictureBox1.Location = new Point(1705, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(59, 33);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 16;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1699, 48);
            label4.Name = "label4";
            label4.Size = new Size(65, 15);
            label4.TabIndex = 17;
            label4.Text = "ANASAYFA";
            // 
            // Form20
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            BackgroundImage = Properties.Resources.kedi;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1869, 830);
            ControlBox = false;
            Controls.Add(label4);
            Controls.Add(pictureBox1);
            Controls.Add(label5);
            Controls.Add(dataGridView1);
            Controls.Add(label3);
            Controls.Add(richTextBox2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(richTextBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(groupBox1);
            ForeColor = Color.Gold;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form20";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HAYVAN HASTALIK TAHMİNİ";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private GroupBox groupBox1;
        private Button button1;
        private Button button2;
        private RichTextBox richTextBox1;
        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private RichTextBox richTextBox2;
        private Label label3;
        private DataGridView dataGridView1;
        private Label label5;
        private PictureBox pictureBox1;
        private Label label4;
    }
}