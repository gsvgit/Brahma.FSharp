﻿using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.FSharp.Core;
using System.Windows.Forms;
//using Processor;
using System.Reactive;

namespace IDE
{
    public partial class MainScreen : Form
    {

        public MainScreen()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        private void button2_Click(object sender, EventArgs e)
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
            catch(Exception){};
        }

        private void button1_Click_1(object sender, EventArgs e)
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
