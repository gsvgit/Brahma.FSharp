using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.FSharp.Core;
using System.Windows.Forms;
using Processor;
using Controller;
using TTA;
using System.Reactive;

namespace IDE
{
	public partial class MainForm : Form
	{
		private IController<int> ctrl = new Controller<int>();

		private IObservable<object> openClick;
		private IObservable<object> exitClick;
		private IObservable<object> saveClick;
		private IObservable<object> saveAsClick;
		private IObservable<object> initClick;
		private IObservable<object> clearClick;
		private IObservable<object> clearOnRunClick;
		private IObservable<object> showGridClick;
		private IObservable<object> compileClick;
		private IObservable<object> checkClick;
		private IObservable<object> runClick;
		private IObservable<object> debugClick;
		private IObservable<object> stepClick;
		private IObservable<object> stopDebugClick;
		private IObservable<object> threadsClick;
		private IObservable<object> multiThreadClick;
		private IObservable<object> aboutClick;
		private IObservable<object> formResized;
		private IObservable<object> panel1Resized;
		private IObservable<EventPattern<ControlEventArgs>> controlAdded;
		private IObservable<EventPattern<ControlEventArgs>> controlRemoved;

		public MainForm()
		{
			InitializeComponent();

			InitEvents();
			SubscribeEvents();

			InitEditor();
		}

		private void InitEvents()
		{
			openClick = Observable.FromEventPattern(h => loadToolStripMenuItem.Click += h, h => loadToolStripMenuItem.Click -= h);
			exitClick = Observable.FromEventPattern(h => exitToolStripMenuItem.Click += h, h => exitToolStripMenuItem.Click -= h);
			saveClick = Observable.FromEventPattern(h => saveToolStripMenuItem.Click += h, h => saveToolStripMenuItem.Click -= h);
			saveAsClick = Observable.FromEventPattern(h => saveAsToolStripMenuItem.Click += h, h => saveAsToolStripMenuItem.Click -= h);
			initClick = Observable.FromEventPattern(h => configureToolStripMenuItem.Click += h, h => configureToolStripMenuItem.Click -= h);
			clearClick = Observable.FromEventPattern(h => clearToolStripMenuItem.Click += h, h => clearToolStripMenuItem.Click -= h);
			clearOnRunClick = Observable.FromEventPattern(h => clearOnRunToolStripMenuItem.Click += h, h => clearOnRunToolStripMenuItem.Click -= h);
			showGridClick = Observable.FromEventPattern(h => showGridToolStripMenuItem.Click += h, h => showGridToolStripMenuItem.Click -= h);
			compileClick = Observable.FromEventPattern(h => compileToolStripMenuItem.Click += h, h => compileToolStripMenuItem.Click -= h);
			checkClick = Observable.FromEventPattern(h => checkToolStripMenuItem.Click += h, h => checkToolStripMenuItem.Click -= h);
			runClick = Observable.FromEventPattern(h => runWoDebugToolStripMenuItem.Click += h, h => runWoDebugToolStripMenuItem.Click -= h);
			debugClick = Observable.FromEventPattern(h => debugToolStripMenuItem1.Click += h, h => debugToolStripMenuItem1.Click -= h);
			stepClick = Observable.FromEventPattern(h => stepToolStripMenuItem.Click += h, h => stepToolStripMenuItem.Click -= h);
			stopDebugClick = Observable.FromEventPattern(h => stopDebuggingToolStripMenuItem.Click += h, h => stopDebuggingToolStripMenuItem.Click -= h);
			threadsClick = Observable.FromEventPattern(h => threadsToolStripMenuItem.Click += h, h => threadsToolStripMenuItem.Click -= h);
			multiThreadClick = Observable.FromEventPattern(h => multiThreadToolStripMenuItem.Click += h, h => multiThreadToolStripMenuItem.Click -= h);
			aboutClick = Observable.FromEventPattern(h => aboutToolStripMenuItem.Click += h, h => aboutToolStripMenuItem.Click -= h);
			formResized = Observable.FromEventPattern(h => this.ResizeEnd += h, h => this.ResizeEnd -= h);
			panel1Resized = Observable.FromEventPattern(h => splitContainer2.Panel1.Resize += h, h => splitContainer2.Panel1.Resize -= h);
			controlAdded = Observable.FromEventPattern<ControlEventHandler, ControlEventArgs>(h => splitContainer2.Panel1.ControlAdded += h, h => splitContainer2.Panel1.ControlAdded -= h).Where(p => p.EventArgs.Control is TextBox);
			controlRemoved = Observable.FromEventPattern<ControlEventHandler, ControlEventArgs>(h => splitContainer2.Panel1.ControlRemoved += h, h => splitContainer2.Panel1.ControlRemoved -= h).Where(p => p.EventArgs.Control is TextBox);
		}

		private void SubscribeEvents()
		{
			openClick.Subscribe(openHandler);
			exitClick.Subscribe(s => Close());
			saveClick.Subscribe(s => ctrl.Save());
			saveAsClick.Subscribe(saveAsHandler);
			initClick.Subscribe(initProcessorHandler);
			clearClick.Subscribe();
			clearOnRunClick.Subscribe();
			showGridClick.Subscribe();
			compileClick.Subscribe();
			checkClick.Subscribe();
			runClick.Subscribe();
			debugClick.Subscribe();
			stepClick.Subscribe();
			stopDebugClick.Subscribe();
			threadsClick.Subscribe(threadsHandler);
			multiThreadClick.Subscribe();
			aboutClick.Subscribe();
			formResized.Subscribe(s => InitEditor());
			panel1Resized.Subscribe(s => InitEditor());
			controlAdded.Subscribe(s => SubscribeOnTextBox(s.EventArgs.Control as TextBox));
			controlRemoved.Subscribe(s => UnsubscribeOnTextBox(s.EventArgs.Control as TextBox));

			ctrl.Alert += AlertHandler;
		}

		private void UnsubscribeOnTextBox(TextBox tb)
		{
			tb.TextChanged -= UpdateCode;
		}

		private void SubscribeOnTextBox(TextBox tb)
		{
			tb.TextChanged += UpdateCode;
		}

		private void InitEditor()
		{
			splitContainer2.Panel1.Controls.Clear();
			var n = ctrl.ThreadNumber;
			var w = splitContainer2.Panel1.Width;
			var h = splitContainer2.Panel1.Height;
			for (int i = 0; i < n; i++)
			{
				splitContainer2.Panel1.Controls.Add(new TextBox() { Multiline = true, Location = new Point(i * w / n, 0), Size = new Size(w / n, h), Lines = ctrl.Source[i] });
			}
		}

		private void UpdateCode(object s = null, EventArgs e = null)
		{
			var n = ctrl.ThreadNumber;
			var arr = new string[n][];
			for (int i = 0; i < n; i++)
			{
				var l = splitContainer2.Panel1.Controls[i] as TextBox;
				arr[i] = l.Lines;
			}
			ctrl.Update(arr);
		}

		private void AlertHandler(object s, AlertEventArgs e)
		{
			MessageBox.Show(e.Message);
		}

		private void openHandler(object s)
		{
			OpenFileDialog d = new OpenFileDialog();
			var dr = d.ShowDialog();
			if (dr == DialogResult.OK)
			{
				ctrl.Open(d.FileName);
				InitEditor();
			}
		}

		private void saveAsHandler(object s)
		{
			SaveFileDialog d = new SaveFileDialog();
			var dr = d.ShowDialog();
			if (dr == DialogResult.OK)
			{
				ctrl.Save(d.FileName);
			}
		}

		private void initProcessorHandler(object s)
		{
			InitProcessorForm f = new InitProcessorForm();
			f.InitCode = ctrl.InitCode;
			var d = f.ShowDialog();
			if (d == DialogResult.OK)
			{
				var t = f.InitCode;
				var err = ctrl.Init(t);
				if (err == null)
					return;
				MessageBox.Show("Errors occured");
				ctrl.ThreadNumber = 0;
			}
		}

		private void threadsHandler (object s)
		{
			var f = new ThreadNumberForm() {Number = ctrl.ThreadNumber};
			var d = f.ShowDialog();
			if (d == DialogResult.OK)
				ctrl.ThreadNumber = f.Number;
			InitEditor();
		}
	}
}
