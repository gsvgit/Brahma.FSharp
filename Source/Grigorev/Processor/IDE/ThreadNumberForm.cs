using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDE
{
	public partial class ThreadNumberForm : Form
	{
		public int Number { get; set; }

		public ThreadNumberForm()
		{
			InitializeComponent();
			numeric.Value = Number;
		}

		private void ok_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Number = (int) numeric.Value;
			Close();
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}


	}
}
