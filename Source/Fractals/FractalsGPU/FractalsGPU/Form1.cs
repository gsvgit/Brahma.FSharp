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
        static double scaling = 0.5;
        static double size = 100.0;
        static double mx = -1.5;
        static double my = -1.0;
        static double cr = 0.4;
        static double ci = 0.24;
        static double step = 0.05;
        static int boxwidth = 400;
        static int boxheight = 400;
        static int[] array = new int[boxwidth * boxheight];
        public FractalsForm()
        {
            InitializeComponent();
            getCords();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.comboBox1.SelectedIndex = 0;
            this.Disposed += FractalsForm_Disposed;

        }

        void FractalsForm_Disposed(object sender, EventArgs e)
        {
            FracSharpGPU.closeAll();
        }


        private void fDraw_Click_1(object sender, EventArgs e)
        {
            getCords();
            drawFractal();
        }

        //private void form_ResizeEnd(object sender, EventArgs e)
        //{
        //    boxwidth = pictureBox1.Width;
        //    boxheight = pictureBox1.Height;
        //}

        public void getCords()
        {
            try
            {
                scaling = Convert.ToDouble(this.textBox1.Text);
                size = Convert.ToDouble(this.textBox2.Text);
                mx = Convert.ToDouble(this.textBox3.Text);
                my = Convert.ToDouble(this.textBox4.Text);
                cr = Convert.ToDouble(this.textBox5.Text);
                cr = Convert.ToDouble(this.textBox5.Text);
                step = Convert.ToDouble(this.textBox7.Text);
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
        public void drawFractal()
        {
            //pictureBox1.Image = (this.comboBox1.SelectedIndex == 0)?FracSharpGPU.drawIm(scaling, size, mx, my, array, boxwidth, boxheight):JuliaDraw.drawIm(scaling, size, mx, my, cr, ci);
            pictureBox1.Image = FracSharpGPU.drawIm(scaling, size, mx, my, array, boxwidth, boxheight, cr, ci, this.comboBox1.SelectedIndex);
        }
        public void setCords()
        {
            this.textBox1.Text = scaling.ToString();
            this.textBox2.Text = size.ToString();
            this.textBox3.Text = mx.ToString();
            this.textBox4.Text = my.ToString();
        }

        private void comboBox1_Change(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                this.label5.Visible = true;
                this.label6.Visible = true;
                this.textBox5.Visible = true;
                this.textBox6.Visible = true;
            }
            else
            {
                this.label5.Visible = false;
                this.label6.Visible = false;
                this.textBox5.Visible = false;
                this.textBox6.Visible = false;
            }
        }
        private void up_Click(object sender, EventArgs e)
        {
            getCords();
            my -= step;
            setCords();
            drawFractal();

        }

        private void down_Click(object sender, EventArgs e)
        {
            getCords();
            my += step;
            setCords();
            drawFractal();
        }

        private void left_Click(object sender, EventArgs e)
        {
            getCords();
            mx -= step;
            setCords();
            drawFractal();
        }

        private void right_Click(object sender, EventArgs e)
        {
            getCords();
            mx += step;
            setCords();
            drawFractal();
        }

        private void zoomin_Click(object sender, EventArgs e)
        {
            getCords();
            size += 100;
            mx += step;
            my += step;
            setCords();
            drawFractal();
        }

        private void zoomout_Click(object sender, EventArgs e)
        {
            getCords();
            size -= 100;
            mx -= step;
            my -= step;
            setCords();
            drawFractal();
        }

        private void about_Click(object sender, EventArgs e)
        {
            String message = "Thank you for using our product. \n Please buy the full version of this program, just 9.9$. \n © Bulgakov & Govoruha Labs";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Asterisk;
            DialogResult result = MessageBox.Show(message, "About", buttons,icon);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

    }
}
