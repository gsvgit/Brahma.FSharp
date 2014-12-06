using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.IO;

namespace IDE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var newButton = Observable.FromEventPattern(h => newToolStripMenuItem.Click += h, h => newToolStripMenuItem.Click -= h);
            newButton.Subscribe(x => newToolStripMenuItem.Text = newToolStripMenuItem.Text + "!");

            var openButton = Observable.FromEventPattern(h => openToolStripMenuItem.Click += h, h => openToolStripMenuItem.Click -= h);
            openButton.Subscribe(x => openButtonPressed());

            var saveButton = Observable.FromEventPattern(h => saveToolStripMenuItem.Click += h, h => saveToolStripMenuItem.Click -= h);
            saveButton.Subscribe(x => saveButtonPressed());
        }

        private void openButtonPressed()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "txt files (*.txt)|*.txt",
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog.FileName);
                    textBox1.Text += sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void saveButtonPressed()
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "txt files (*.txt)|*.txt",
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamWriter(saveFileDialog.FileName);
                    sr.Write(textBox1.Text);
                    sr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}