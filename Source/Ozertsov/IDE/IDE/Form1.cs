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
//using Compilator;

namespace IDE
{
    public partial class MainScreen : Form
    {
        Compilator.Compiler comp = new Compilator.Compiler();
        static int count = 0;
        public MainScreen()
        {

            InitializeComponent();
            data.RowCount = 100;
            NumerateDataGrid(data);
            var open = Observable.FromEventPattern(h => Open.Click += h, h =>  Open.Click -= h);
            open.ObserveOn(SynchronizationContext.Current).Subscribe(x => OpenFile(Open));
            var load = Observable.FromEventPattern(h => Save.Click += h, h => Save.Click -= h);
            load.ObserveOn(SynchronizationContext.Current).Subscribe(x => LoadFile(Save));
            var start = Observable.FromEventPattern(h => Starting.Click += h, h => Starting.Click -= h);
            start.ObserveOn(SynchronizationContext.Current).Subscribe(x => Start(Starting));
            var debug = Observable.FromEventPattern(h => Debagging.Click += h, h => Debagging.Click -= h);
            debug.ObserveOn(SynchronizationContext.Current).Subscribe(x => Debug(Debagging));
            var stop = Observable.FromEventPattern(h => StopDebagging.Click += h, h => StopDebagging.Click -= h);
            stop.ObserveOn(SynchronizationContext.Current).Subscribe(x => Stop(StopDebagging));
            var close = Observable.FromEventPattern<FormClosingEventHandler, FormClosingEventArgs>(h => FormClosing += h, h => FormClosing -= h);
            close.Subscribe(x => CloseTable());
        }
        public string CreateCode
        {
            get { return CodeText.Text; }
        }
        private void CloseTable()
        {
            if (CodeText.Text != "")
            {
                if (MessageBox.Show("Do you want to save changes to your code?", "SavingChanges",
                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.LoadFile(Open);
                }
            }
        }
        private void Stop(object sender)
        {
            StopDebagging.Visible = false;
            Starting.Visible = true;
            CodeText.Enabled = true;
            DisposeDataGrid(data);
            count = 0;
            NumerateDataGrid(data);
            CodeText.Select(0, CodeText.Text.Length);
            CodeText.SelectionColor = System.Drawing.Color.Black;
            CodeText.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
        }
        private void Debug(object sender)
        {
            ErrorBox.Text = "";
            try
            {
                if (count < CodeText.Lines.Length - 1)
                {
                    if (count == 0)
                        DisposeDataGrid(data);
                    Highlight();
                    StopDebagging.Visible = true;
                    Starting.Visible = false;
                    CodeText.Enabled = false;
                    try
                    {
                        comp.Compile(CodeText.Text);
                    }
                    catch (Exception e)
                    {
                        //Compilator.CompileException ex = new Compilator.CompileException();
                        throw e;
                    }
                    comp.Step(count);
                    count++;
                    this.CreateDataGrid(comp, data);
                }
                else
                {

                    StopDebagging.Visible = false;
                    Highlight();
                    try
                    {
                        comp.Compile(CodeText.Text);
                    }
                    catch (Exception e)
                    {
                        //Compilator.CompileException ex = new Compilator.CompileException();
                        throw e;
                    } 
                    comp.Step(count);
                    this.CreateDataGrid(comp, data);
                    count = 0;
                    comp.Stop();
                    Starting.Visible = true;

                    CodeText.Enabled = true;
                }
            }
            catch (Exception e)
            {

                CodeText.Select(0, CodeText.Text.Length);
                CodeText.SelectionColor = System.Drawing.Color.Black;
                CodeText.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
                ErrorBox.Text = e.Message;
                DisposeDataGrid(data);
                StopDebagging.Visible = false;
                Starting.Visible = true;
                CodeText.Enabled = true;
                count = 0;
                NumerateDataGrid(data);
            }
        }
        private void Start(object sender)
        {
            ErrorBox.Text = "";
            DisposeDataGrid(data);
            try
            {
                try
                {
                    comp.Compile(CodeText.Text);
                }
                catch (Exception e)
                {
                    throw e;
                }    
                comp.Run();
                this.CreateDataGrid(comp, data);
                comp.Stop();
            }
            catch (Exception e)
            {
                ErrorBox.Text = e.Message;
                NumerateDataGrid(data);
            }
        }
        private void CreateDataGrid(Compilator.Compiler compiler, DataGridView data)
        {
            if (data.RowCount < compiler.NumRows()) { data.RowCount = compiler.NumRows(); }
            for (int i = 0; i < compiler.NumCols(); i++)
            {
                Dictionary<int, string> cells = compiler.getGrid(i);
                foreach (KeyValuePair<int, string> kvp in cells){
                    data[i, kvp.Key].Value = kvp.Value;
                }
            }
            NumerateDataGrid(data);
        }
        private void NumerateDataGrid(DataGridView data)
        {
            for (int i = 0; i < data.Rows.Count; i++)
            {
                data.Rows[i].HeaderCell.Value
                    = i.ToString();
            }
        }
        private void DisposeDataGrid(DataGridView data)
        {
            data.Rows.Clear();
            data.RowCount = 420;
        }
        private void OpenFile(object sender)
        {
            if (CodeText.Text != "")
            {
                if (MessageBox.Show("Do you want to save changes to your code?", "SavingChanges",
                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.LoadFile(Open);
                }
            }
            try
            {
                string filePath = "";
                OpenFileDialog Fd = new OpenFileDialog(); 
                Fd.Filter = "txt files (*.txt)|*.txt";
                if (Fd.ShowDialog() == DialogResult.OK)
                {
                    filePath = Fd.FileName;
                }
                string str = System.IO.File.ReadAllText(@filePath);
                CodeText.Text = str;
            }
            catch (Exception) { };
        }
        private void LoadFile(object sender)
        {
            string filePath = "";
            string str = CodeText.Text;

            SaveFileDialog Sd = new SaveFileDialog();
            Sd.Filter = "txt files (*.txt)|*.txt";
            if (Sd.ShowDialog() == DialogResult.OK)
            {
                filePath = Sd.FileName;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath);
                sw.Write(CodeText.Text);
                sw.Close();
            }
        }
        private void Highlight()
        {
            if (count < CodeText.Lines.Length - 1)
            {
                CodeText.Select(0, CodeText.GetFirstCharIndexFromLine(count));
                CodeText.SelectionColor = System.Drawing.Color.Black;
                CodeText.SelectionBackColor = System.Drawing.Color.WhiteSmoke;

                int firstCharPosition = CodeText.GetFirstCharIndexFromLine(count);
                int ln = CodeText.Lines[count].Length;

                CodeText.Select(firstCharPosition, ln);
                CodeText.Select();

                CodeText.SelectionColor = System.Drawing.Color.White;
                CodeText.SelectionBackColor = System.Drawing.Color.YellowGreen;
            }
            else
            {
                CodeText.Select(0, CodeText.Text.Length);
                CodeText.SelectionColor = System.Drawing.Color.Black;
                CodeText.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            }
        }

    }
}
