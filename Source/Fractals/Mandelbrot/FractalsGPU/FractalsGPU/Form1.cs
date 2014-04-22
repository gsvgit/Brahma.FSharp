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
        static double scaling = 0.5 ;
        static double size= 100.0;
        static double mx= -1.5;
        static double my= -1.0;
        public FractalsForm()
        {
            InitializeComponent();
            getCords();

        }

        private void fDraw_Click_1(object sender, EventArgs e)
        {
            getCords();
            drawFractal();
        }

        public void getCords()
        {
            try
            {
                scaling = Convert.ToDouble(this.textBox1.Text);
                size = Convert.ToDouble(this.textBox2.Text);
                mx = Convert.ToDouble(this.textBox3.Text);
                my = Convert.ToDouble(this.textBox4.Text);
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
        public void drawFractal()
        {
            pictureBox1.Image = Mandelbrot.drawIm(scaling, size, mx, my);
        }
        public void setCords()
        {
            this.textBox1.Text = scaling.ToString();
            this.textBox2.Text = size.ToString();
            this.textBox3.Text = mx.ToString();
            this.textBox4.Text = my.ToString();
        }

        private void up_Click(object sender, EventArgs e)
        {
            my -= 0.05D;
            setCords();
            drawFractal();

        }

        private void down_Click(object sender, EventArgs e)
        {
            my += 0.05D;
            setCords();
            drawFractal();
        }

        private void left_Click(object sender, EventArgs e)
        {
            mx -= 0.05D;
            setCords();
            drawFractal();
        }

        private void right_Click(object sender, EventArgs e)
        {
            mx += 0.05D;
            setCords();
            drawFractal();
        }

        private void zoomin_Click(object sender, EventArgs e)
        {
            size += 100;
            mx += 0.05;
            my += 0.05;
            setCords();
            drawFractal();
        }

        private void zoomout_Click(object sender, EventArgs e)
        {
            size -= 100;
            setCords();
            drawFractal();
        }

    }
}
