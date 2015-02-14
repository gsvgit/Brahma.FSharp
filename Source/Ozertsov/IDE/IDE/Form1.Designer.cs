//using Processor;
//using Matrix;
using System.Windows.Forms;

namespace IDE
{
    partial class MainScreen
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        /// 

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.parametrExceptionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.CodeText = new System.Windows.Forms.RichTextBox();
            this.Open = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Starting = new System.Windows.Forms.Button();
            this.Debagging = new System.Windows.Forms.Button();
            this.StopDebagging = new System.Windows.Forms.Button();
            this.data = new System.Windows.Forms.DataGridView();
            this.plus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.minus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.division = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.multiplication = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErrorBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.parametrExceptionBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.data)).BeginInit();
            this.SuspendLayout();
            // 
            // CodeText
            // 
            this.CodeText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CodeText.Location = new System.Drawing.Point(12, 43);
            this.CodeText.Name = "CodeText";
            this.CodeText.Size = new System.Drawing.Size(887, 508);
            this.CodeText.TabIndex = 2;
            this.CodeText.Text = "";
            // 
            // Open
            // 
            this.Open.Location = new System.Drawing.Point(12, 12);
            this.Open.Name = "Open";
            this.Open.Size = new System.Drawing.Size(75, 23);
            this.Open.TabIndex = 3;
            this.Open.Text = "Open";
            this.Open.UseVisualStyleBackColor = true;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(93, 12);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 4;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            // 
            // Starting
            // 
            this.Starting.Location = new System.Drawing.Point(174, 12);
            this.Starting.Name = "Starting";
            this.Starting.Size = new System.Drawing.Size(75, 23);
            this.Starting.TabIndex = 5;
            this.Starting.Text = "Start";
            this.Starting.UseVisualStyleBackColor = true;
            // 
            // Debagging
            // 
            this.Debagging.Location = new System.Drawing.Point(255, 12);
            this.Debagging.Name = "Debagging";
            this.Debagging.Size = new System.Drawing.Size(75, 23);
            this.Debagging.TabIndex = 6;
            this.Debagging.Text = "Debug";
            this.Debagging.UseVisualStyleBackColor = true;
            // 
            // StopDebagging
            // 
            this.StopDebagging.Location = new System.Drawing.Point(336, 12);
            this.StopDebagging.Name = "StopDebagging";
            this.StopDebagging.Size = new System.Drawing.Size(75, 23);
            this.StopDebagging.TabIndex = 7;
            this.StopDebagging.Text = "Stop";
            this.StopDebagging.UseVisualStyleBackColor = true;
            this.StopDebagging.Visible = false;
            // 
            // data
            // 
            this.data.AllowUserToAddRows = false;
            this.data.AllowUserToDeleteRows = false;
            this.data.AllowUserToResizeColumns = false;
            this.data.AllowUserToResizeRows = false;
            this.data.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.data.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.data.BackgroundColor = System.Drawing.SystemColors.Info;
            this.data.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.plus,
            this.minus,
            this.division,
            this.multiplication});
            this.data.Location = new System.Drawing.Point(905, 43);
            this.data.Name = "data";
            this.data.ReadOnly = true;
            this.data.Size = new System.Drawing.Size(464, 679);
            this.data.TabIndex = 8;
            // 
            // plus
            // 
            this.plus.Name = "plus";
            this.plus.ReadOnly = true;
            // 
            // minus
            // 
            this.minus.Name = "minus";
            this.minus.ReadOnly = true;
            // 
            // division
            // 
            this.division.Name = "division";
            this.division.ReadOnly = true;
            // 
            // multiplication
            // 
            this.multiplication.Name = "multiplication";
            this.multiplication.ReadOnly = true;
            // 
            // ErrorBox
            // 
            this.ErrorBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorBox.Location = new System.Drawing.Point(12, 557);
            this.ErrorBox.Name = "ErrorBox";
            this.ErrorBox.Size = new System.Drawing.Size(887, 165);
            this.ErrorBox.TabIndex = 9;
            this.ErrorBox.Text = "";
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1381, 734);
            this.Controls.Add(this.ErrorBox);
            this.Controls.Add(this.data);
            this.Controls.Add(this.StopDebagging);
            this.Controls.Add(this.Debagging);
            this.Controls.Add(this.Starting);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Open);
            this.Controls.Add(this.CodeText);
            this.Name = "MainScreen";
            this.Text = "TTA IDE";
            ((System.ComponentModel.ISupportInitialize)(this.parametrExceptionBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.data)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource parametrExceptionBindingSource;
        private RichTextBox CodeText;
        private Button Open;
        private Button Save;
        private Button Starting;
        private Button Debagging;
        private Button StopDebagging;
        private DataGridView data;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn plus;
        private DataGridViewTextBoxColumn minus;
        private DataGridViewTextBoxColumn division;
        private DataGridViewTextBoxColumn multiplication;
        private RichTextBox ErrorBox;
    }
}

