﻿//using Processor;
//using Matrix;
using System.Windows.Forms;

namespace IDE
{
    public delegate int Deleg(int x, int y);
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.debugButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.data = new System.Windows.Forms.DataGridView();
            this.plus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subtraction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.multiplication = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.division = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorsListBox = new System.Windows.Forms.ListBox();
            this.stepButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.parametrExceptionBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.data)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(0, 35);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(578, 347);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(9, 10);
            this.openButton.Margin = new System.Windows.Forms.Padding(2);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(56, 21);
            this.openButton.TabIndex = 3;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(70, 10);
            this.saveButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(56, 21);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(130, 10);
            this.startButton.Margin = new System.Windows.Forms.Padding(2);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(56, 21);
            this.startButton.TabIndex = 5;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            // 
            // debugButton
            // 
            this.debugButton.Location = new System.Drawing.Point(191, 10);
            this.debugButton.Margin = new System.Windows.Forms.Padding(2);
            this.debugButton.Name = "debugButton";
            this.debugButton.Size = new System.Drawing.Size(56, 21);
            this.debugButton.TabIndex = 6;
            this.debugButton.Text = "Debug";
            this.debugButton.UseVisualStyleBackColor = true;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(317, 10);
            this.stopButton.Margin = new System.Windows.Forms.Padding(2);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(56, 21);
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Visible = false;
            // 
            // data
            // 
            this.data.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.plus,
            this.subtraction,
            this.multiplication,
            this.division});
            this.data.Location = new System.Drawing.Point(582, 35);
            this.data.Margin = new System.Windows.Forms.Padding(2);
            this.data.Name = "data";
            this.data.ReadOnly = true;
            this.data.Size = new System.Drawing.Size(445, 552);
            this.data.TabIndex = 8;
            // 
            // plus
            // 
            this.plus.Name = "plus";
            this.plus.ReadOnly = true;
            // 
            // subtraction
            // 
            this.subtraction.Name = "subtraction";
            this.subtraction.ReadOnly = true;
            // 
            // multiplication
            // 
            this.multiplication.Name = "multiplication";
            this.multiplication.ReadOnly = true;
            // 
            // division
            // 
            this.division.Name = "division";
            this.division.ReadOnly = true;
            // 
            // errorsListBox
            // 
            this.errorsListBox.FormattingEnabled = true;
            this.errorsListBox.Location = new System.Drawing.Point(9, 396);
            this.errorsListBox.Name = "errorsListBox";
            this.errorsListBox.Size = new System.Drawing.Size(568, 186);
            this.errorsListBox.TabIndex = 9;
            // 
            // stepButton
            // 
            this.stepButton.Location = new System.Drawing.Point(251, 10);
            this.stepButton.Margin = new System.Windows.Forms.Padding(2);
            this.stepButton.Name = "stepButton";
            this.stepButton.Size = new System.Drawing.Size(62, 21);
            this.stepButton.TabIndex = 10;
            this.stepButton.Text = "Next Step";
            this.stepButton.UseVisualStyleBackColor = true;
            this.stepButton.Visible = false;
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 596);
            this.Controls.Add(this.stepButton);
            this.Controls.Add(this.errorsListBox);
            this.Controls.Add(this.data);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.debugButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainScreen";
            this.Text = "TTA IDE";
            ((System.ComponentModel.ISupportInitialize)(this.parametrExceptionBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.data)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource parametrExceptionBindingSource;
        private RichTextBox richTextBox1;
        private Button openButton;
        private Button saveButton;
        private Button startButton;
        private Button debugButton;
        private Button stopButton;
        private DataGridView data;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn plus;
        private DataGridViewTextBoxColumn subtraction;
        private DataGridViewTextBoxColumn division;
        private DataGridViewTextBoxColumn multiplication;
        private ListBox errorsListBox;
        private Button stepButton;

    }
}