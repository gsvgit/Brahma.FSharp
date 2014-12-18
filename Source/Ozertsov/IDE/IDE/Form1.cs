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
            NumerateDataGrid(data);
            var open = Observable.FromEventPattern(h => Open.Click += h, h =>  Open.Click -= h);
            open.ObserveOn(SynchronizationContext.Current).Subscribe(x => OpenFile(Open));
            var load = Observable.FromEventPattern(h => Save.Click += h, h => Save.Click -= h);
            load.ObserveOn(SynchronizationContext.Current).Subscribe(x => LoadFile(Save));
            var start = Observable.FromEventPattern(h => button3.Click += h, h => button3.Click -= h);
            start.ObserveOn(SynchronizationContext.Current).Subscribe(x => Start(button3));
            var debug = Observable.FromEventPattern(h => button4.Click += h, h => button4.Click -= h);
            debug.ObserveOn(SynchronizationContext.Current).Subscribe(x => Debug(button4));
            var stop = Observable.FromEventPattern(h => button5.Click += h, h => button5.Click -= h);
            stop.ObserveOn(SynchronizationContext.Current).Subscribe(x => Stop(button5));
            var close = Observable.FromEventPattern<FormClosingEventHandler, FormClosingEventArgs>(h => FormClosing += h, h => FormClosing -= h);
            close.Subscribe(x => CloseTable());
        }
        public string CreateCode
        {
            get { return richTextBox1.Text; }
        }
        private void CloseTable()
        {
            if (richTextBox1.Text != "")
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
            button5.Visible = false;
            button3.Visible = true;
            richTextBox1.Enabled = true;
            DisposeDataGrid(data);
            count = 0;
        }
        private void Debug(object sender)
        {
            errorBox.Text = "";
            try
            {
                if (count < richTextBox1.Lines.Length - 1)
                {
                    if (count == 0)
                        DisposeDataGrid(data);
                    button5.Visible = true;
                    button3.Visible = false;
                    richTextBox1.Enabled = false;
                    try
                    {
                        comp.Compile(richTextBox1.Text);
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

                    button5.Visible = false; 
                    try
                    {
                        comp.Compile(richTextBox1.Text);
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
                    button3.Visible = true;

                    richTextBox1.Enabled = true;
                }
            }
            catch (Exception e)
            {
                errorBox.Text = e.Message;
                DisposeDataGrid(data);
                button5.Visible = false;
                button3.Visible = true;
                richTextBox1.Enabled = true;
                count = 0;
                NumerateDataGrid(data);
            }
        }
        private void Start(object sender)
        {
            errorBox.Text = "";
            DisposeDataGrid(data);
            try
            {
                try
                {
                    comp.Compile(richTextBox1.Text);
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
                errorBox.Text = e.Message;
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
            if (richTextBox1.Text != "")
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
                richTextBox1.Text = str;
            }
            catch (Exception) { };
        }
        private void LoadFile(object sender)
        {
            string filePath = "";
            string str = richTextBox1.Text;

            SaveFileDialog Sd = new SaveFileDialog();
            Sd.Filter = "txt files (*.txt)|*.txt";
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
