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

namespace IDE
{
    public partial class MainScreen : Form
    {

        public MainScreen()
        {

            InitializeComponent();
            var open = Observable.FromEventPattern(h => openButton.Click += h, h =>  openButton.Click -= h);
            open.ObserveOn(SynchronizationContext.Current).Subscribe(x => OpenFile(openButton));
            var load = Observable.FromEventPattern(h => saveButton.Click += h, h => saveButton.Click -= h);
            load.ObserveOn(SynchronizationContext.Current).Subscribe(x => LoadFile(saveButton));
            var start = Observable.FromEventPattern(h => startButton.Click += h, h => startButton.Click -= h);
            start.ObserveOn(SynchronizationContext.Current).Subscribe(x => Start(startButton));
            var debug = Observable.FromEventPattern(h => debugButton.Click += h, h => debugButton.Click -= h);
            debug.ObserveOn(SynchronizationContext.Current).Subscribe(x => Debug(debugButton));
            var step = Observable.FromEventPattern(h => stepButton.Click += h, h => stepButton.Click -= h);
            step.ObserveOn(SynchronizationContext.Current).Subscribe(x => NextStep(stopButton));
            var stop = Observable.FromEventPattern(h => stopButton.Click += h, h => stopButton.Click -= h);
            stop.ObserveOn(SynchronizationContext.Current).Subscribe(x => Stop(stopButton));

        }
        
        private Compiler.Compiler comp = new Compiler.Compiler();
        public string CreateCode
        {
            get { return richTextBox1.Text; }
        }
        int count = 0;

        private void Debug(object sender)
        {
            try
            {
                comp.Compile(richTextBox1.Text);
                if (createErrorBox().Count == 0)
                {
                    startButton.Visible = false;
                    debugButton.Visible = false;
                    stepButton.Visible = true;
                    stopButton.Visible = true;
                }
            }
            catch(Compiler.CompileException e)
            {
                List<String> list = new List<String>();
                list.Add(e.Message);
                errorsListBox.DataSource = list;
            }
        }

        private void NextStep(object sender)
        {
            if (count < richTextBox1.Lines.Length)
            {
                Highlight();
                comp.Step(count);
                count++;
                this.CreateDataGrid(comp, data);
            }
            else
            {
                Stop(sender);
            }
        }
        private void Highlight()
        {
            //Select Color
            richTextBox1.SelectionColor = System.Drawing.Color.White;
            richTextBox1.SelectionBackColor = System.Drawing.Color.Blue;
   
            //Char position
            int firstCharPosition = richTextBox1.GetFirstCharIndexFromLine(count);
            int ln = richTextBox1.Lines[count].Length;
            //Select
            richTextBox1.Select(firstCharPosition, ln);
            richTextBox1.Select();
        }
        private void Stop(object sender)
        {
            startButton.Visible = true;
            debugButton.Visible = true;
            stepButton.Visible = false;
            stopButton.Visible = false;
            comp.Stop();
            data.Rows.Clear();
            count = 0;
        }
        private void Start(object sender)
        {
            try
            {
                data.Rows.Clear();
                comp.Compile(richTextBox1.Text);
                comp.Run();
                this.CreateDataGrid(comp, data);
                comp.Stop();
                errorsListBox.DataSource = new List<string>();
            }
            catch (Compiler.CompileException e)
            {
                List<String> list = new List<String>();
                list.Add(e.Message);
                errorsListBox.DataSource = list;
            }
            catch (Exception e)
            {
                createErrorBox();
            }
        }
        public List<string> createErrorBox()
        {
            HashSet<Tuple<String, int, int>> errs = comp.getErrorsList;
            List<string> _items = new List<string>();
            foreach (Tuple<String, int, int> i in errs)
            {
                _items.Add(i.Item1 + " in code line " + (i.Item2 + 1) + " operation " + (i.Item3 + 1));
            }
            data.Rows.Clear();
            this.errorsListBox.DataSource = _items;
            return _items;
        }
        private void CreateDataGrid(Compiler.Compiler compiler, DataGridView data)
        {
            data.RowCount = compiler.CountRows();
            for (int i = 0; i < compiler.CountCols(); i++)
            {
                //data[i]
                Dictionary<int, string> cells = compiler.getStringGrid(i);
                foreach (KeyValuePair<int, string> kvp in cells){
                    data[i, kvp.Key].Value = kvp.Value;
                }
            }
        }
        private void OpenFile(object sender)
        {
            try
            {
                string filePath = "";
                OpenFileDialog Fd = new OpenFileDialog();
                if (Fd.ShowDialog() == DialogResult.OK)
                {
                    filePath = Fd.FileName;
                }
                string str = System.IO.File.ReadAllText(@filePath);
                richTextBox1.Text = str;
            }
            catch (Exception) { };
        }
        private void LoadFile(object sender)
        {
            string filePath = "";
            string str = richTextBox1.Text;

            SaveFileDialog Sd = new SaveFileDialog();
            Sd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (Sd.ShowDialog() == DialogResult.OK)
            {
                filePath = Sd.FileName;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath);
                sw.Write(richTextBox1.Text);
                sw.Close();
            }
        }
    }
}
