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

            var open = Observable.FromEventPattern(h => button1.Click += h, h =>  button1.Click -= h);
            open.ObserveOn(SynchronizationContext.Current).Subscribe(x => OpenFile(button1));
            var load = Observable.FromEventPattern(h => button2.Click += h, h => button2.Click -= h);
            load.ObserveOn(SynchronizationContext.Current).Subscribe(x => LoadFile(button2));
            var start = Observable.FromEventPattern(h => button3.Click += h, h => button3.Click -= h);
            start.ObserveOn(SynchronizationContext.Current).Subscribe(x => Start(button3));
            var debug = Observable.FromEventPattern(h => button4.Click += h, h => button4.Click -= h);
            debug.ObserveOn(SynchronizationContext.Current).Subscribe(x => Debug(button4));
            var stop = Observable.FromEventPattern(h => button5.Click += h, h => button5.Click -= h);
            stop.ObserveOn(SynchronizationContext.Current).Subscribe(x => Stop(button5));
        }

        public string CreateCode
        {
            get { return richTextBox1.Text; }
        }
        private void Stop(object sender)
        {
            //MessageBox.Show("закончить debug");
            button5.Visible = false;
            DisposeDataGrid(data);
            count = 0;
        }
        private void Debug(object sender)
        {
            errorBox.Text = "";
            try
            {
                //MessageBox.Show("Передать строку в метод compile для debug");
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
                        Compilator.CompileException ex = new Compilator.CompileException();
                        throw ex;
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
                        Compilator.CompileException ex = new Compilator.CompileException();
                        throw ex;
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
                //MessageBox.Show(e.Message);
            }
        }

        private void Start(object sender)
        {

            //Compilator.Compiler comp = new Compilator.Compiler();
            errorBox.Text = "";
            DisposeDataGrid(data);
            try
            {
                //MessageBox.Show("Передать строку в compile");
                try
                {
                    comp.Compile(richTextBox1.Text);
                }
                catch (Exception e)
                {
                    Compilator.CompileException ex = new Compilator.CompileException();
                    throw ex;
                }
                    
                comp.Run();
                this.CreateDataGrid(comp, data);
                comp.Stop();
            }
            catch (Exception e)
            {
                errorBox.Text = e.Message;
                //MessageBox.Show(e.Message);    
            }
        }

        private void CreateDataGrid(Compilator.Compiler compiler, DataGridView data)
        {
            if (data.RowCount < compiler.NumRows()) { data.RowCount = compiler.NumRows(); }
            for (int i = 0; i < compiler.NumCols(); i++)
            {
                //data[i]
                Dictionary<int, string> cells = compiler.getGrid(i);
                foreach (KeyValuePair<int, string> kvp in cells){
                    data[i, kvp.Key].Value = kvp.Value;
                }
            }
        }
        private void DisposeDataGrid(DataGridView data)
        {
            for (int col = 0; col< data.ColumnCount; col++)
            {
                for (int row = 0; row < data.ColumnCount; row++)
                {
                    data[col, row].Value = null;
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
