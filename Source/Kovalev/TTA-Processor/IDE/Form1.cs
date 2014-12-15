using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.IO;
using Controller;


namespace IDE
{
    public partial class Form1 : Form
    {
        private readonly ProcessorController controller = new ProcessorController();

        public Form1()
        {
            InitializeComponent();
            var newClick = Observable.FromEventPattern(h => newToolStripMenuItem.Click += h,
                h => newToolStripMenuItem.Click -= h);
            newClick.Subscribe(x => { editor.Text = ""; clearDataGrid(); });

            var openClick = Observable.FromEventPattern(h => openToolStripMenuItem.Click += h,
                h => openToolStripMenuItem.Click -= h);
            openClick.Subscribe(x => openButtonPressed());

            var saveClick = Observable.FromEventPattern(h => saveToolStripMenuItem.Click += h,
                h => saveToolStripMenuItem.Click -= h);
            saveClick.Subscribe(x => saveButtonPressed());

            var buildClick = Observable.FromEventPattern(h => buildToolStripMenuItem.Click += h,
                h => buildToolStripMenuItem.Click -= h);
            buildClick.Subscribe(x =>
            {
                controller.Build(editor.Text);
                writeError();
            });

            var startClick = Observable.FromEventPattern(h => startToolStripMenuItem.Click += h,
                h => startToolStripMenuItem.Click -= h);
            startClick.Subscribe(x =>
            {
                controller.Run(editor.Text);
                updateDataGrid();
                writeError();
            });

            var debugClick = Observable.FromEventPattern(h => startDebuggingToolStripMenuItem.Click += h,
                h => startDebuggingToolStripMenuItem.Click -= h);
            debugClick.Subscribe(x =>
            {
                controller.StartDebugging(editor.Text);
                disableVisualElements();
                clearDataGrid();
                writeError();
            });

            var stepClick = Observable.FromEventPattern(h => nextStepToolStripMenuItem.Click += h,
                h => nextStepToolStripMenuItem.Click -= h);
            stepClick.Subscribe(x =>
            {
                nextStep();
                updateDataGrid();
                writeError();
            });

            var stopClick = Observable.FromEventPattern(h => stopToolStripMenuItem.Click += h,
                h => stopToolStripMenuItem.Click -= h);
            stopClick.Subscribe(x =>
            {
                enableVisualElements();
                controller.StopDebugging();
            });

            var aboutClick = Observable.FromEventPattern(h => aboutToolStripMenuItem.Click += h,
                h => aboutToolStripMenuItem.Click -= h);
            aboutClick.Subscribe(x => MessageBox.Show("       My Little IDE v1.0    \n          TTA is magic!", "About"));
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
                    editor.Text += sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't open file: " + ex.Message);
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
                    sr.Write(editor.Text);
                    sr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't save file: " + ex.Message);
                }
            }
        }

        private void clearDataGrid()
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
        }

        private void updateDataGrid()
        {
            clearDataGrid();

            var width = controller.NumOfCols;
            var height = controller.NumOfRows;

            for (int i = 0; i < width; i++)
                dataGridView.Columns.Add(i.ToString(), i.ToString());
            for (int i = 0; i < height; i++)
            {
                var row = new DataGridViewRow {HeaderCell = {Value = i.ToString()}};
                for (int j = 0; j < width; j++)
                    row.Cells.Add(new DataGridViewTextBoxCell());
                dataGridView.Rows.Add(row);
            }

            var allCells = controller.AllValues;
            foreach (var c in allCells)
                dataGridView[c.Item2, c.Item1].Value = c.Item3;
        }

        private void disableVisualElements()
        {
            if (controller.DebugState)            
            {   
                foreach (ToolStripMenuItem item in menuStrip1.Items)
                   foreach (ToolStripMenuItem button in item.DropDownItems)
                       if (button.Name != "nextStepToolStripMenuItem" && button.Name != "stopToolStripMenuItem")
                           button.Enabled = false;
                editor.ReadOnly = true;
            }            
        }

        private void enableVisualElements()
        {
            foreach (ToolStripMenuItem item in menuStrip1.Items)
                foreach (ToolStripMenuItem button in item.DropDownItems)
                    button.Enabled = true;
            editor.ReadOnly = false;
        }

        private void nextStep()
        {            
            controller.NextStep();
            if (!controller.DebugState)
                enableVisualElements();
        }

        private void writeError()
        {            
            output.Text = controller.GetError;
        }
    }
}