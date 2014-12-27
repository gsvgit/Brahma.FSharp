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
        private Controller.Controller controller = new Controller.Controller();
        private string savedCode = "";

        public Form1()
        {
            InitializeComponent();
            var newClick = Observable.FromEventPattern(h => newToolStripMenuItem.Click += h,
                h => newToolStripMenuItem.Click -= h);
            newClick.Subscribe(x =>
            {
                checkForChanges();
                editor.Text = "";
                errors.Text = "";
                clearDataGrid();
            });

            var openClick = Observable.FromEventPattern(h => openToolStripMenuItem.Click += h,
                h => openToolStripMenuItem.Click -= h);
            openClick.Subscribe(x =>
            {
                checkForChanges();
                openButtonPressed();
            });

            var saveClick = Observable.FromEventPattern(h => saveToolStripMenuItem.Click += h,
                h => saveToolStripMenuItem.Click -= h);
            saveClick.Subscribe(x => saveButtonPressed());

            var startClick = Observable.FromEventPattern(h => startToolStripMenuItem.Click += h,
                h => startToolStripMenuItem.Click -= h);
            startClick.Subscribe(x => startWithoutDebug());

            var debugClick = Observable.FromEventPattern(h => startDebugToolStripMenuItem.Click += h,
                h => startDebugToolStripMenuItem.Click -= h);
            debugClick.Subscribe(x => startDebug());

            var stepClick = Observable.FromEventPattern(h => nextStepToolStripMenuItem.Click += h,
                h => nextStepToolStripMenuItem.Click -= h);
            stepClick.Subscribe(x => nextStep());

            var stopClick = Observable.FromEventPattern(h => stopToolStripMenuItem.Click += h,
                h => stopToolStripMenuItem.Click -= h);
            stopClick.Subscribe(x => stopDebug());           

            var formClosing = Observable.FromEventPattern<FormClosingEventHandler, FormClosingEventArgs>(h => FormClosing += h,
                                                                                                         h => FormClosing -= h);
            formClosing.Subscribe((x => checkForChanges()));

            var keyPressed = Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => this.KeyDown += h, h => this.KeyDown -= h);

            keyPressed.Where(p => p.EventArgs.KeyCode == Keys.F9 && p.EventArgs.Modifiers == Keys.Control).Subscribe(e => startDebug());
            keyPressed.Where(p => p.EventArgs.KeyCode == Keys.F9).Subscribe(e => startWithoutDebug());
            keyPressed.Where(p => p.EventArgs.KeyCode == Keys.F10).Subscribe(e => nextStep());
            keyPressed.Where(p => p.EventArgs.KeyCode == Keys.F10 && p.EventArgs.Modifiers == Keys.Control).Subscribe(e => stopDebug());
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
                    savedCode = editor.Text;
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
                    savedCode = editor.Text;
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

            if (height != 0)
            {
                for (int i = 0; i < width; i++)
                    dataGridView.Columns.Add(i.ToString(), i.ToString());
                for (int i = 0; i < height; i++)
                {
                    var row = new DataGridViewRow { HeaderCell = { Value = i.ToString() } };
                    for (int j = 0; j < width; j++)
                        row.Cells.Add(new DataGridViewTextBoxCell());
                    dataGridView.Rows.Add(row);
                }

                var allCells = controller.AllValues;
                foreach (var c in allCells)
                    dataGridView[c.Item2, c.Item1].Value = c.Item3;
            }
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

        private void lineHighlight(int lineNumber)
        {
            editor.Select(0, editor.GetFirstCharIndexFromLine(lineNumber));
            editor.SelectionColor = Color.Black;
            editor.SelectionBackColor = Color.WhiteSmoke;

            if (lineNumber < editor.Lines.Length)
            {
                int firstCharPosition = editor.GetFirstCharIndexFromLine(lineNumber);
                int length = editor.Lines[lineNumber].Length;

                editor.Select(firstCharPosition, length);
                editor.Select();
                editor.SelectionColor = Color.White;
                editor.SelectionBackColor = Color.Blue;
            }
            else
            {
                editor.SelectAll();
                editor.SelectionColor = Color.Black;
                editor.SelectionBackColor = Color.White;
                editor.DeselectAll();
            }
        }

        private void startDebug()
        {
            clearDataGrid();
            controller.StartDebugging(editor.Text);

            if (controller.GetError != "") writeError();
            else
            {
                disableVisualElements();
                nextStepToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = true;
            }
        }

        private void stopDebug()
        {
            enableVisualElements();
            nextStepToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            controller.StopDebugging();
        }

        private void startWithoutDebug()
        {
            controller.Run(editor.Text);
            updateDataGrid();
            writeError();
        }

        private void nextStep()
        {
            lineHighlight(controller.DebugLineIndex);
            controller.NextStep();
            updateDataGrid();
            writeError();
            if (!controller.DebugState)
            {
                enableVisualElements();
                nextStepToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = false;
            }
        }

        private void writeError()
        {
            errors.Text = controller.GetError;
        }

        private void checkForChanges()
        {
            if (editor.Text != savedCode)
            {
                var result = MessageBox.Show("Do you want to save changes?", "IDE", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) saveButtonPressed();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Дополнительная информация в файле Документация.pdf");
        }
    }
}
