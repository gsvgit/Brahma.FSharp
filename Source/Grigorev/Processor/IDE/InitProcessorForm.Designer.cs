namespace IDE
{
	partial class InitProcessorForm
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
			this.editor = new System.Windows.Forms.TextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.init = new System.Windows.Forms.ToolStripMenuItem();
			this.exit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// editor
			// 
			this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editor.Location = new System.Drawing.Point(0, 24);
			this.editor.Multiline = true;
			this.editor.Name = "editor";
			this.editor.Size = new System.Drawing.Size(786, 582);
			this.editor.TabIndex = 0;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.init,
            this.exit});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(786, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// init
			// 
			this.init.Name = "init";
			this.init.Size = new System.Drawing.Size(36, 20);
			this.init.Text = "Init";
			// 
			// exit
			// 
			this.exit.Name = "exit";
			this.exit.Size = new System.Drawing.Size(37, 20);
			this.exit.Text = "Exit";
			// 
			// InitProcessorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(786, 606);
			this.Controls.Add(this.editor);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "InitProcessorForm";
			this.Text = "InitProcessorForm";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox editor;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem init;
		private System.Windows.Forms.ToolStripMenuItem exit;
	}
}