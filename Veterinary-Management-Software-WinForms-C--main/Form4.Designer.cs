using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WinFormsApp8
{
    partial class Form4
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form4));
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            dataGridView1 = new DataGridView();
            textBox5 = new TextBox();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            groupBox1 = new GroupBox();
            pictureBox7 = new PictureBox();
            label4 = new Label();
            textBox6 = new TextBox();
            groupBox2 = new GroupBox();
            pictureBox6 = new PictureBox();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            groupBox3 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(33, 113);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "ürün adı:";
            textBox1.Size = new Size(200, 23);
            textBox1.TabIndex = 9;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(33, 143);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "varsa ürün no:";
            textBox2.Size = new Size(200, 23);
            textBox2.TabIndex = 10;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(33, 173);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "adet:";
            textBox3.Size = new Size(200, 23);
            textBox3.TabIndex = 11;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(33, 204);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "alış fiyatı:";
            textBox4.Size = new Size(200, 23);
            textBox4.TabIndex = 12;
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(13, 31);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(749, 326);
            dataGridView1.TabIndex = 18;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(33, 236);
            textBox5.Name = "textBox5";
            textBox5.PlaceholderText = "satış fiyatı:";
            textBox5.Size = new Size(200, 23);
            textBox5.TabIndex = 17;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.btnHome;
            pictureBox1.Location = new Point(980, 17);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(57, 45);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 20;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (System.Drawing.Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(110, 288);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(54, 42);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 21;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (System.Drawing.Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(32, 288);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(52, 42);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 22;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (System.Drawing.Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(-8, -10);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(1098, 547);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 23;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = (System.Drawing.Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.Location = new Point(188, 288);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(53, 42);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 24;
            pictureBox5.TabStop = false;
            pictureBox5.Click += pictureBox5_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(pictureBox7);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(textBox6);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1059, 68);
            groupBox1.TabIndex = 25;
            groupBox1.TabStop = false;
            // 
            // pictureBox7
            // 
            pictureBox7.BackgroundImage = (System.Drawing.Image)resources.GetObject("pictureBox7.BackgroundImage");
            pictureBox7.Image = (System.Drawing.Image)resources.GetObject("pictureBox7.Image");
            pictureBox7.Location = new Point(603, 26);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(39, 31);
            pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 58;
            pictureBox7.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Yu Gothic UI", 15.75F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(64, 64, 64);
            label4.Location = new Point(22, 19);
            label4.Name = "label4";
            label4.Size = new Size(228, 30);
            label4.TabIndex = 19;
            label4.Text = "Stok Malzeme Bölümü";
            // 
            // textBox6
            // 
            textBox6.Location = new Point(666, 28);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(248, 23);
            textBox6.TabIndex = 57;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(pictureBox6);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(pictureBox5);
            groupBox2.Controls.Add(pictureBox3);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Controls.Add(pictureBox2);
            groupBox2.Controls.Add(textBox4);
            groupBox2.Controls.Add(textBox3);
            groupBox2.Controls.Add(textBox2);
            groupBox2.Controls.Add(textBox5);
            groupBox2.Location = new Point(12, 89);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(269, 377);
            groupBox2.TabIndex = 26;
            groupBox2.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.Image = (System.Drawing.Image)resources.GetObject("pictureBox6.Image");
            pictureBox6.InitialImage = (System.Drawing.Image)resources.GetObject("pictureBox6.InitialImage");
            pictureBox6.Location = new Point(78, 22);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(99, 76);
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 28;
            pictureBox6.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label3.ForeColor = Color.Yellow;
            label3.Location = new Point(188, 340);
            label3.Name = "label3";
            label3.Size = new Size(59, 17);
            label3.TabIndex = 27;
            label3.Text = "Güncelle";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(125, 340);
            label2.Name = "label2";
            label2.Size = new Size(38, 17);
            label2.TabIndex = 26;
            label2.Text = "Satış";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
            label1.ForeColor = Color.Green;
            label1.Location = new Point(44, 340);
            label1.Name = "label1";
            label1.Size = new Size(31, 17);
            label1.TabIndex = 25;
            label1.Text = "Alış";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(dataGridView1);
            groupBox3.Location = new Point(287, 89);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(784, 377);
            groupBox3.TabIndex = 26;
            groupBox3.TabStop = false;
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(1083, 507);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox3);
            Controls.Add(pictureBox4);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form4";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "stok bilgileri";
            Load += Form4_Load;
            KeyDown += dataGridView1_KeyDown;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private DataGridView dataGridView1;
        private TextBox textBox5;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label4;
        private PictureBox pictureBox6;
        private PictureBox pictureBox7;
        private TextBox textBox6;
    }
}
