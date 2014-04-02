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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FractalsForm));
            this.up = new System.Windows.Forms.Button();
            this.down = new System.Windows.Forms.Button();
            this.left = new System.Windows.Forms.Button();
            this.right = new System.Windows.Forms.Button();
            this.zoomin = new System.Windows.Forms.Button();
            this.zoomout = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fDraw = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // up
            // 
            this.up.Location = new System.Drawing.Point(459, 23);
            this.up.Name = "up";
            this.up.Size = new System.Drawing.Size(40, 23);
            this.up.TabIndex = 0;
            this.up.Text = "UP";
            this.up.UseVisualStyleBackColor = true;
            // 
            // down
            // 
            this.down.Location = new System.Drawing.Point(459, 81);
            this.down.Name = "down";
            this.down.Size = new System.Drawing.Size(40, 23);
            this.down.TabIndex = 1;
            this.down.Text = "Down";
            this.down.UseVisualStyleBackColor = true;
            // 
            // left
            // 
            this.left.Location = new System.Drawing.Point(422, 52);
            this.left.Name = "left";
            this.left.Size = new System.Drawing.Size(40, 23);
            this.left.TabIndex = 2;
            this.left.Text = "Left";
            this.left.UseVisualStyleBackColor = true;
            // 
            // right
            // 
            this.right.Location = new System.Drawing.Point(496, 52);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(40, 23);
            this.right.TabIndex = 3;
            this.right.Text = "Right";
            this.right.UseVisualStyleBackColor = true;
            // 
            // zoomin
            // 
            this.zoomin.Location = new System.Drawing.Point(440, 110);
            this.zoomin.Name = "zoomin";
            this.zoomin.Size = new System.Drawing.Size(75, 23);
            this.zoomin.TabIndex = 4;
            this.zoomin.Text = "Zoom In";
            this.zoomin.UseVisualStyleBackColor = true;
            // 
            // zoomout
            // 
            this.zoomout.Location = new System.Drawing.Point(440, 139);
            this.zoomout.Name = "zoomout";
            this.zoomout.Size = new System.Drawing.Size(75, 23);
            this.zoomout.TabIndex = 5;
            this.zoomout.Text = "Zoom In";
            this.zoomout.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            //this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(404, 399);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // fDraw
            // 
            this.fDraw.Location = new System.Drawing.Point(440, 168);
            this.fDraw.Name = "fDraw";
            this.fDraw.Size = new System.Drawing.Size(75, 23);
            this.fDraw.TabIndex = 7;
            this.fDraw.Text = "fDraw";
            this.fDraw.UseVisualStyleBackColor = true;
            this.fDraw.Click += new System.EventHandler(this.fDraw_Click_1);
            // 
            // FractalsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 435);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

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
        
    }
}

