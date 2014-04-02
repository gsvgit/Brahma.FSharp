using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FractalsGPU
{
    public partial class FractalsForm : Form
    {
        public FractalsForm()
        {
            InitializeComponent();

        }

        private void fDraw_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Image = Mandelbrot.drawIm(0.5, 100.0, -1.5, -1.0);
        }
    }
}
