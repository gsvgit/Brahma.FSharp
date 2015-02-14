using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace IDE
{
    public partial class Form1 : Form
    {
        Compiler.Compiler comp = new Compiler.Compiler();
        int count = 0;
        public Form1()
        {
            InitializeComponent();
            var open = Observable.FromEventPattern(h => openToolStripMenuItem.Click += h, h => openToolStripMenuItem.Click -= h);
            open.ObserveOn(SynchronizationContext.Current).Subscribe(x => OpenFile(openToolStripMenuItem));
            var save = Observable.FromEventPattern(h => saveToolStripMenuItem.Click += h, h => saveToolStripMenuItem.Click -= h);
            save.ObserveOn(SynchronizationContext.Current).Subscribe(x => SaveFile(saveToolStripMenuItem));
            var formClosing = Observable.FromEventPattern<FormClosingEventHandler, FormClosingEventArgs>(h => FormClosing += h, h => FormClosing -= h);
            formClosing.Subscribe(x => Closing(x.EventArgs));
            var start = Observable.FromEventPattern(h => startButton.Click += h, h => startButton.Click -= h);
            start.ObserveOn(SynchronizationContext.Current).Subscribe(x => Start(startButton));
            var debug = Observable.FromEventPattern(h => debugButton.Click += h, h => debugButton.Click -= h);
            debug.ObserveOn(SynchronizationContext.Current).Subscribe(x => Debug(debugButton));
            var step = Observable.FromEventPattern(h => stepButton.Click += h, h => stepButton.Click -= h);
            step.ObserveOn(SynchronizationContext.Current).Subscribe(x => NextStep(stepButton));
            var stop = Observable.FromEventPattern(h => stopButton.Click += h, h => stopButton.Click -= h);
            stop.ObserveOn(SynchronizationContext.Current).Subscribe(x => Stop(stopButton));
            var num = Observable.FromEventPattern<DataGridViewRowsAddedEventHandler, DataGridViewRowsAddedEventArgs>(h => dataGridView.RowsAdded += h, h => dataGridView.RowsAdded -= h);
            num.ObserveOn(SynchronizationContext.Current).Subscribe(x => NumerateDataGrid(x.EventArgs));
        }
        private void Open()
        {
            string filePath = "";
            OpenFileDialog Fd = new OpenFileDialog();
            Fd.Filter = "txt files (*.txt)|*.txt";
            if (Fd.ShowDialog() == DialogResult.OK)
            {
                filePath = Fd.FileName;
                string str = System.IO.File.ReadAllText(@filePath);
                textBox.Text = str;
            }
        }
        private bool SaveFile(object sender)
        {
            string filePath = "";
            string str = textBox.Text;

            SaveFileDialog Sd = new SaveFileDialog();
            Sd.Filter = "txt files (*.txt)|*.txt";
            if (Sd.ShowDialog() == DialogResult.OK)
            {
                filePath = Sd.FileName;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath);
                sw.Write(textBox.Text);
                sw.Close();
                return true;
            }
            return false;
        }
        
        private bool SaveOnClose()
        {
            var dr = MessageBox.Show("Do you want save program before exit?", "Alert", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Yes)
            {
                SaveFile(saveToolStripMenuItem);
                return false;
            }
            if (dr == DialogResult.No)
            {
                return false;
            }
            return true;
        }
        private void Closing(FormClosingEventArgs e)
        {
            e.Cancel = SaveOnClose(); ;
        }
        private void CreateDataGrid(Compiler.Compiler compiler, DataGridView dataGridView)
        {
            dataGridView.RowCount = compiler.CountRows();
            for (int i = 0; i < compiler.CountCols(); i++)
            {
                Dictionary<int, string> cells = compiler.GetStringGrid(i);
                foreach (KeyValuePair<int, string> kvp in cells)
                    {
                        dataGridView[i, kvp.Key].Value = kvp.Value;
                    }
            }
        }
        private void NumerateDataGrid(DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = String.Format("{0}", dataGridView.Rows[i].Index);
            }
            dataGridView.AutoResizeRowHeadersWidth(0, DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }
        public List<string> createErrorBox()
        {
            HashSet<Tuple<String, int, int>> errs = comp.GetErrorsList();
            List<string> _items = new List<string>();
            foreach (Tuple<String, int, int> i in errs)
            {
                _items.Add(i.Item1 + " in code line " + (i.Item2 + 1) + " operation " + (i.Item3 + 1));
            }
            dataGridView.Rows.Clear();
            this.errorBox.DataSource = _items;
            return _items;
        }
        private void Start(object sender)
        {
            try
            {
                errorBox.DataSource = new List<string>();
                dataGridView.Rows.Clear();
                comp.Stop();
                comp.Compile(textBox.Text);
                comp.Run();
                this.CreateDataGrid(comp, dataGridView);
            }
            catch (Compiler.CompileException e)
            {
                List<String> list = new List<String>();
                list.Add(e.Message);
                errorBox.DataSource = list;
                comp.Stop();
            }
            catch (Compiler.RunTimeException e)
            {
                List<String> list = new List<String>();
                list.Add(e.Message);
                errorBox.DataSource = list;
                comp.Stop();
            }
            catch (Exception e)
            {
                createErrorBox();
            }
        }
        private void Debug(object sender)
        {
            dataGridView.Rows.Clear();
            comp.Stop();
            try
            {
                comp.Compile(textBox.Text);
                comp.Debug();
                startButton.Visible = false;
                debugButton.Visible = false;
                stepButton.Visible = true;
                stopButton.Visible = true;
                textBox.Enabled = false;
                errorBox.DataSource = new List<string>();
            }
            catch (Compiler.CompileException e)
            {
                List<String> list = new List<String>();
                list.Add(e.Message);
                errorBox.DataSource = list;
            }
            catch (Exception e)
            {
                createErrorBox();
            }
        }
        private void NextStep(object sender)
        {
            if (count < textBox.Lines.Length)
            {                
                Highlight();
                textBox.Show();
                comp.Step(count);
                count++;
                dataGridView.Rows.Clear();
                this.CreateDataGrid(comp, dataGridView);
                
            }
            else
            {
                Stop(sender);
            }
        }
        private void Stop(object sender)
        {
            textBox.SelectAll();
            textBox.SelectionBackColor = System.Drawing.Color.White;
            textBox.SelectionColor = Color.Black;
            startButton.Visible = true;
            debugButton.Visible = true;
            stepButton.Visible = false;
            stopButton.Visible = false;
            textBox.Enabled = true;
            comp.Stop();
            dataGridView.Rows.Clear();
            count = 0;
        }       
        private void Highlight()
        {
            //Clear previous
            textBox.Select(0, textBox.GetFirstCharIndexFromLine(count));
            textBox.SelectionColor = System.Drawing.Color.Black;
            textBox.SelectionBackColor = System.Drawing.Color.WhiteSmoke;

            //Char position
            int firstCharPosition = textBox.GetFirstCharIndexFromLine(count);
            int ln = textBox.Lines[count].Length;

            //Select
            textBox.Select(firstCharPosition, ln);
            textBox.Select();

            //Select Color
            textBox.SelectionColor = System.Drawing.Color.White;
            textBox.SelectionBackColor = System.Drawing.Color.Blue;

        }
        private void OpenFile(object sender)
        {
            try
            {
                var dr = MessageBox.Show("Do you want save program before open?", "Alert", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    if (SaveFile(saveToolStripMenuItem))
                    {
                        Open();
                    }
                }
                if (dr == DialogResult.No)
                {
                    Open();
                }
            }
            catch (Exception) { };
        }      
    }
}

