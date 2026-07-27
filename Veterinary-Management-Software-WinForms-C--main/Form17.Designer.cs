namespace WinFormsApp8
{
    partial class Form17
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form17));
            label1 = new Label();
            button1 = new Button();
            richTextBox1 = new RichTextBox();
            textBox1 = new TextBox();
            label2 = new Label();
            button2 = new Button();
            dataGridView1 = new DataGridView();
            dateTimePicker1 = new DateTimePicker();
            textBox2 = new TextBox();
            label3 = new Label();
            button3 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ActiveCaptionText;
            label1.ForeColor = Color.Gold;
            label1.Location = new Point(3, 46);
            label1.Name = "label1";
            label1.Size = new Size(104, 15);
            label1.TabIndex = 0;
            label1.Text = "HATIRLATMA ADI:";
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveCaptionText;
            button1.ForeColor = Color.Gold;
            button1.Location = new Point(129, 604);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "Kayıt et";
            button1.UseVisualStyleBackColor = false;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.Gold;
            richTextBox1.Location = new Point(113, 85);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(656, 124);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.Gold;
            textBox1.Location = new Point(113, 43);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(656, 23);
            textBox1.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.ActiveCaptionText;
            label2.ForeColor = Color.Gold;
            label2.Location = new Point(63, 88);
            label2.Name = "label2";
            label2.Size = new Size(44, 15);
            label2.TabIndex = 4;
            label2.Text = "İÇERİK:";
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ActiveCaptionText;
            button2.ForeColor = Color.Gold;
            button2.Location = new Point(694, 604);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 6;
            button2.Text = "TEMİZLE";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click_1;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(129, 388);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(640, 210);
            dataGridView1.TabIndex = 7;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.CalendarForeColor = Color.Gold;
            dateTimePicker1.CalendarMonthBackground = Color.Gold;
            dateTimePicker1.CalendarTrailingForeColor = SystemColors.Menu;
            dateTimePicker1.Location = new Point(113, 254);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(656, 23);
            dateTimePicker1.TabIndex = 8;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.Gold;
            textBox2.Location = new Point(129, 312);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(640, 23);
            textBox2.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = SystemColors.ActiveCaptionText;
            label3.ForeColor = Color.Gold;
            label3.Location = new Point(52, 312);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 9;
            label3.Text = "SAAT:";
            // 
            // button3
            // 
            button3.BackColor = SystemColors.ActiveCaptionText;
            button3.ForeColor = Color.Gold;
            button3.Location = new Point(754, -1);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 11;
            button3.Text = "ANASAYFA";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // Form17
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.SAAT;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(833, 629);
            ControlBox = false;
            Controls.Add(button3);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(dateTimePicker1);
            Controls.Add(dataGridView1);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(richTextBox1);
            Controls.Add(button1);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form17";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HATIRLATICI";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
        private RichTextBox richTextBox1;
        private TextBox textBox1;
        private Label label2;
        private Button button2;
        private DataGridView dataGridView1;
        private DateTimePicker dateTimePicker1;
        private TextBox textBox2;
        private Label label3;
        private Button button3;
    }
}