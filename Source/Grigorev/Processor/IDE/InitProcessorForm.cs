using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reactive;

namespace IDE
{
	public partial class InitProcessorForm : Form
	{
		private IObservable<EventPattern<object>> initEvent;
		private IObservable<EventPattern<object>> exitEvent;

		public string Text { get; set; }

		public InitProcessorForm()
		{
			InitializeComponent();
			initEvent = Observable.FromEventPattern(h => init.Click += h, h => init.Click -= h);
			exitEvent = Observable.FromEventPattern(h => exit.Click += h, h => exit.Click -= h);
			exitEvent.Subscribe(ExitClickHandler);
			initEvent.Subscribe(InitClickHandler);
		}

		private void InitClickHandler (object s)
		{
			Text = editor.Text;
			DialogResult = DialogResult.OK;
			Close();
		}
		private void ExitClickHandler(object s)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
