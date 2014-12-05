namespace IDE
{
	partial class ThreadNumberForm
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
			this.numeric = new System.Windows.Forms.NumericUpDown();
			this.ok = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numeric)).BeginInit();
			this.SuspendLayout();
			// 
			// numeric
			// 
			this.numeric.Location = new System.Drawing.Point(12, 12);
			this.numeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numeric.Name = "numeric";
			this.numeric.Size = new System.Drawing.Size(166, 20);
			this.numeric.TabIndex = 0;
			this.numeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// ok
			// 
			this.ok.Location = new System.Drawing.Point(12, 52);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 23);
			this.ok.TabIndex = 1;
			this.ok.Text = "OK";
			this.ok.UseVisualStyleBackColor = true;
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// Cancel
			// 
			this.Cancel.Location = new System.Drawing.Point(103, 52);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 1;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// ThreadNumberForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(193, 86);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.numeric);
			this.Name = "ThreadNumberForm";
			this.Text = "ThreadNumberForm";
			((System.ComponentModel.ISupportInitialize)(this.numeric)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numeric;
		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.Button Cancel;
	}
}