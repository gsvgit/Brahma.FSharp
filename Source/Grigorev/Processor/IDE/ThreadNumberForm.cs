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
		public int Number
		{
			get { return (int)numeric.Value; }
			set { numeric.Value = value; }
		}

		public ThreadNumberForm()
		{
			InitializeComponent();
		}

		private void ok_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}


	}
}
