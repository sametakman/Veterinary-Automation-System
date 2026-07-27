namespace WinFormsApp8
{
    partial class Form11
    {
        private System.ComponentModel.IContainer components = null;

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
            pictureBox2 = new PictureBox();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            textBox1 = new TextBox();
            dataGridView3 = new DataGridView();
            label1 = new Label();
            pictureBox3 = new PictureBox();
            textBox2 = new TextBox();
            dataGridView1 = new DataGridView();
            groupBox4 = new GroupBox();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            label9 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox4.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.sol_ok4;
            pictureBox2.Location = new Point(847, 15);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(49, 47);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 52;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.ActiveCaptionText;
            label2.ForeColor = Color.Gold;
            label2.Location = new Point(172, 27);
            label2.Name = "label2";
            label2.Size = new Size(54, 15);
            label2.TabIndex = 51;
            label2.Text = "Aşı Takip";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.Ekran_görüntüsü_2024_12_24_153205;
            pictureBox1.Image = Properties.Resources.pngtree_search_flat_red_color_icon_test_search_glass_vector_png_image_19940566;
            pictureBox1.Location = new Point(83, 47);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(45, 23);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 50;
            pictureBox1.TabStop = false;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(134, 47);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Küpe no göre filtreleme:";
            textBox1.Size = new Size(229, 23);
            textBox1.TabIndex = 49;
            // 
            // dataGridView3
            // 
            dataGridView3.BackgroundColor = Color.White;
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView3.Location = new Point(53, 97);
            dataGridView3.Name = "dataGridView3";
            dataGridView3.Size = new Size(342, 388);
            dataGridView3.TabIndex = 48;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Black;
            label1.ForeColor = Color.Gold;
            label1.Location = new Point(121, 27);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 47;
            label1.Text = "Veresiye Defteri";
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = Properties.Resources.Ekran_görüntüsü_2024_12_24_153205;
            pictureBox3.Image = Properties.Resources.pngtree_search_flat_red_color_icon_test_search_glass_vector_png_image_19940566;
            pictureBox3.Location = new Point(66, 47);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(45, 23);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 45;
            pictureBox3.TabStop = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(117, 47);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "isim soyisime göre filtrelemme:";
            textBox2.Size = new Size(229, 23);
            textBox2.TabIndex = 44;
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(51, 97);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(342, 388);
            dataGridView1.TabIndex = 43;
            // 
            // groupBox4
            // 
            groupBox4.BackgroundImage = Properties.Resources.Ekran_görüntüsü_2024_12_24_153205;
            groupBox4.Controls.Add(dataGridView1);
            groupBox4.Controls.Add(textBox2);
            groupBox4.Controls.Add(pictureBox3);
            groupBox4.Controls.Add(label1);
            groupBox4.Location = new Point(8, 66);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(449, 501);
            groupBox4.TabIndex = 53;
            groupBox4.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.BackgroundImage = Properties.Resources.Ekran_görüntüsü_2024_12_24_153205;
            groupBox1.Controls.Add(dataGridView3);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(463, 66);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(449, 501);
            groupBox1.TabIndex = 54;
            groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.BackgroundImage = Properties.Resources.Ekran_görüntüsü_2024_12_24_153205;
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(pictureBox2);
            groupBox2.Location = new Point(9, 1);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(903, 68);
            groupBox2.TabIndex = 55;
            groupBox2.TabStop = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Yu Gothic UI", 15.75F, FontStyle.Bold);
            label9.ForeColor = SystemColors.MenuHighlight;
            label9.Image = Properties.Resources.Ekran_görüntüsü_2024_12_24_153205;
            label9.Location = new Point(15, 23);
            label9.Name = "label9";
            label9.Size = new Size(219, 30);
            label9.TabIndex = 53;
            label9.Text = "Raporlar Bölümü ( 2 )";
            // 
            // Form11
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(924, 579);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(groupBox4);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form11";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form11";
            Load += Form11_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        private PictureBox pictureBox2;
        private Label label2;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private DataGridView dataGridView3;
        private Label label1;
        private PictureBox pictureBox3;
        private TextBox textBox2;
        private DataGridView dataGridView1;
        private GroupBox groupBox4;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label9;
    }
}