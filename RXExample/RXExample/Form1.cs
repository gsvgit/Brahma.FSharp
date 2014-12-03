using System;
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
using System.Windows.Forms;

//Install-Package Rx-Main
//Install-Package Rx-WinForms

namespace RXExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           
            InitializeComponent();
            var o = Observable.FromEventPattern(h => button1.Click += h, h => button1.Click -= h);
            o.ObserveOn(SynchronizationContext.Current).Subscribe(x => button1.Text =  button1.Text + "!");

            var kb = Observable.FromEventPattern<KeyPressEventHandler, KeyPressEventArgs>(h => button1.KeyPress += h, h => button1.KeyPress -= h);
            
            var filtered = kb.ObserveOn(SynchronizationContext.Current).Where(x => x.EventArgs.KeyChar == 'g');
            var filtered2 = kb.ObserveOn(SynchronizationContext.Current).Where(x => x.EventArgs.KeyChar == 'h');
            filtered2.Zip(filtered, (left, obsOfRight) => left).ObserveOn(SynchronizationContext.Current).Subscribe(x => button1.Text = button1.Text + "?");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
