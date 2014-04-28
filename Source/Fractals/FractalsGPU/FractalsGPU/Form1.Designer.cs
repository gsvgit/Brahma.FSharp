namespace FractalsGPU
{
    partial class FractalsForm
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
            this.up = new System.Windows.Forms.Button();
            this.down = new System.Windows.Forms.Button();
            this.left = new System.Windows.Forms.Button();
            this.right = new System.Windows.Forms.Button();
            this.zoomin = new System.Windows.Forms.Button();
            this.zoomout = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fDraw = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // up
            // 
            this.up.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.up.Location = new System.Drawing.Point(494, 37);
            this.up.Name = "up";
            this.up.Size = new System.Drawing.Size(40, 23);
            this.up.TabIndex = 0;
            this.up.Text = "UP";
            this.up.UseVisualStyleBackColor = true;
            this.up.Click += new System.EventHandler(this.up_Click);
            // 
            // down
            // 
            this.down.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.down.Location = new System.Drawing.Point(494, 95);
            this.down.Name = "down";
            this.down.Size = new System.Drawing.Size(40, 23);
            this.down.TabIndex = 1;
            this.down.Text = "Down";
            this.down.UseVisualStyleBackColor = true;
            this.down.Click += new System.EventHandler(this.down_Click);
            // 
            // left
            // 
            this.left.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.left.Location = new System.Drawing.Point(457, 66);
            this.left.Name = "left";
            this.left.Size = new System.Drawing.Size(40, 23);
            this.left.TabIndex = 2;
            this.left.Text = "Left";
            this.left.UseVisualStyleBackColor = true;
            this.left.Click += new System.EventHandler(this.left_Click);
            // 
            // right
            // 
            this.right.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.right.Location = new System.Drawing.Point(531, 66);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(40, 23);
            this.right.TabIndex = 3;
            this.right.Text = "Right";
            this.right.UseVisualStyleBackColor = true;
            this.right.Click += new System.EventHandler(this.right_Click);
            // 
            // zoomin
            // 
            this.zoomin.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.zoomin.Location = new System.Drawing.Point(474, 310);
            this.zoomin.Name = "zoomin";
            this.zoomin.Size = new System.Drawing.Size(75, 23);
            this.zoomin.TabIndex = 4;
            this.zoomin.Text = "Zoom In";
            this.zoomin.UseVisualStyleBackColor = true;
            this.zoomin.Click += new System.EventHandler(this.zoomin_Click);
            // 
            // zoomout
            // 
            this.zoomout.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.zoomout.Location = new System.Drawing.Point(474, 339);
            this.zoomout.Name = "zoomout";
            this.zoomout.Size = new System.Drawing.Size(75, 23);
            this.zoomout.TabIndex = 5;
            this.zoomout.Text = "Zoom Out";
            this.zoomout.UseVisualStyleBackColor = true;
            this.zoomout.Click += new System.EventHandler(this.zoomout_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(404, 399);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // fDraw
            // 
            this.fDraw.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.fDraw.Location = new System.Drawing.Point(474, 368);
            this.fDraw.Name = "fDraw";
            this.fDraw.Size = new System.Drawing.Size(75, 23);
            this.fDraw.TabIndex = 7;
            this.fDraw.Text = "fDraw";
            this.fDraw.UseVisualStyleBackColor = true;
            this.fDraw.Click += new System.EventHandler(this.fDraw_Click_1);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(470, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Scaling";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(478, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Size";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(473, 211);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "XOffset";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(473, 237);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "YOffset";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox1.Location = new System.Drawing.Point(524, 155);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(38, 20);
            this.textBox1.TabIndex = 12;
            this.textBox1.Text = "0,5";
            // 
            // textBox2
            // 
            this.textBox2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox2.Location = new System.Drawing.Point(524, 181);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(38, 20);
            this.textBox2.TabIndex = 13;
            this.textBox2.Text = "100";
            // 
            // textBox3
            // 
            this.textBox3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox3.Location = new System.Drawing.Point(524, 208);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(38, 20);
            this.textBox3.TabIndex = 14;
            this.textBox3.Text = "-1,5";
            // 
            // textBox4
            // 
            this.textBox4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox4.Location = new System.Drawing.Point(524, 234);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(38, 20);
            this.textBox4.TabIndex = 15;
            this.textBox4.Text = "-1,0";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Mandelbrot",
            "Julia"});
            this.comboBox1.Location = new System.Drawing.Point(454, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(116, 21);
            this.comboBox1.TabIndex = 16;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_Change);
            // 
            // textBox5
            // 
            this.textBox5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox5.Location = new System.Drawing.Point(524, 260);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(38, 20);
            this.textBox5.TabIndex = 17;
            this.textBox5.Text = "0,4";
            this.textBox5.Visible = false;
            // 
            // textBox6
            // 
            this.textBox6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox6.Location = new System.Drawing.Point(524, 286);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(38, 20);
            this.textBox6.TabIndex = 18;
            this.textBox6.Text = "0,24";
            this.textBox6.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(473, 263);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "C Real";
            this.label5.Visible = false;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(470, 289);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "C Imagine";
            this.label6.Visible = false;
            // 
            // textBox7
            // 
            this.textBox7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox7.Location = new System.Drawing.Point(524, 125);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(38, 20);
            this.textBox7.TabIndex = 22;
            this.textBox7.Text = "0,5";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(470, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Step";
            // 
            // FractalsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 435);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fDraw);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.zoomout);
            this.Controls.Add(this.zoomin);
            this.Controls.Add(this.right);
            this.Controls.Add(this.left);
            this.Controls.Add(this.down);
            this.Controls.Add(this.up);
            this.Name = "FractalsForm";
            this.Text = "FractalsGPU";
            this.ResizeEnd += new System.EventHandler(this.form_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button up;
        private System.Windows.Forms.Button down;
        private System.Windows.Forms.Button left;
        private System.Windows.Forms.Button right;
        private System.Windows.Forms.Button zoomin;
        private System.Windows.Forms.Button zoomout;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button fDraw;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label7;
        
    }
}

