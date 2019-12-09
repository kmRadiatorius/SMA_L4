using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SMA4
{
    public partial class Form1 : Form
    {
        private const double M = 10;
        private const double V0 = 100;
        private const double H0 = 30;
        private const double K1 = 0.05;
        private const double K2 = 0.01;
        private const double G = 9.8;
        
        private const double DELTA_T = 0.01;

        Series Fx1, Fx2, Fx3, Fx4;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(0, 14, -100, 220);
            Fx3 = FormatSeries("v1(t)", Color.Green);
            Fx4 = FormatSeries("v2(t)", Color.LightGreen);
            Fx1 = FormatSeries("h1(t)", Color.Blue);
            Fx2 = FormatSeries("h2(t)", Color.LightBlue);

            double h1 = DrawEuler(DELTA_T, Fx3, Fx1);
            double h2 = DrawEuler(DELTA_T / 2, Fx4, Fx2);
            double a = Math.Abs(h2 - h1);
            double b = a / h1;
            richTextBox1.AppendText("|h1 - h2| = " + FormatNumber(a) + "\n");
            richTextBox1.AppendText("|h1 - h2| / h1 = " + FormatNumber(b) + "\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(0, 14, -100, 220);
            Fx3 = FormatSeries("v1(t)", Color.Green);
            Fx4 = FormatSeries("v2(t)", Color.LightGreen);
            Fx1 = FormatSeries("h1(t)", Color.Blue);
            Fx2 = FormatSeries("h2(t)", Color.LightBlue);

            double h1 = DrawRk(DELTA_T, Fx3, Fx1);
            double h2 = DrawRk(DELTA_T / 2, Fx4, Fx2);
            double a = Math.Abs(h2 - h1);
            double b = a / h1;
            richTextBox1.AppendText("|h1 - h2| = " + FormatNumber(a) + "\n");
            richTextBox1.AppendText("|h1 - h2| / h1 = " + FormatNumber(b) + "\n");
        }

        private double DrawEuler(double deltaT, Series Fx1, Series Fx2)
        {
            double v = V0;
            double v0 = V0;
            double h = H0;
            double maxH = 0;
            double maxHT = 0;
            double groundT = 0;

            for (double t = 0; ; t += deltaT)
            {
                Fx1.Points.AddXY(t, v);
                Fx2.Points.AddXY(t, h);
                if (v0 >= 0 && v <= 0)
                {
                    maxH = h;
                    maxHT = t;
                }
                if (h <= 0)
                {
                    groundT = t;
                    break;
                }
                v0 = v;
                var d = deltaT * A(v, t);
                h += v * deltaT;
                v += d;
            }

            richTextBox1.AppendText("\nEulerio, kai žingsnis t = " + deltaT + ":\n");
            richTextBox1.AppendText("Maksimalus aukštis = " + FormatNumber(maxH) + "m, kai t = " + FormatNumber(maxHT) + "s\n");
            richTextBox1.AppendText("Aukštis = 0m, kai t = " + FormatNumber(groundT) + "s\n");

            return maxH;
        }

        private double DrawRk(double deltaT, Series Fx1, Series Fx2)
        {
            double v = V0;
            double h = H0;
            double v0 = V0;
            double maxH = 0;
            double maxHT = 0;
            double groundT = 0;

            for (double t = 0; ; t += deltaT)
            {
                Fx1.Points.AddXY(t, v);
                Fx2.Points.AddXY(t, h);
                if (v0 >= 0 && v <= 0)
                {
                    maxH = h;
                    maxHT = t;
                }
                if (h <= 0)
                {
                    groundT = t;
                    break;
                }
                v0 = v;

                var deltaT2 = deltaT / 2;
                var v1 = v + deltaT2 * A(v, t);
                var v2 = v + deltaT2 * A(v1, t + deltaT2);
                var v3 = v + deltaT * A(v2, t + deltaT2);
                h += v * deltaT;
                v += deltaT / 6 * (A(v, t) + 2 * A(v1, t + deltaT2) + 2 * A(v2, t + deltaT2) + A(v3, t + deltaT));

            }

            richTextBox1.AppendText("\nIV Eilės RK, kai žingsnis = " + deltaT + "\n:");
            richTextBox1.AppendText("Maksimalus aukštis = " + FormatNumber(maxH) + "m, kai t = " + FormatNumber(maxHT) + "s\n");
            richTextBox1.AppendText("Aukštis = 0m, kai t = " + FormatNumber(groundT) + "s\n");

            return maxH;
        }

        private double A(double v, double t)
        {
            if (v > 0)
            {
                return -(G + K1 * v * v / M);
            }
            return -(G - K2 * v * v / M);
        }

        private double Y(double y, double x)
        {
            return -y + 1;
        }

        private string FormatNumber(double t)
        {
            return string.Format("{0:0.000000}", t);
        }

        private Series FormatSeries(string title, Color color)
        {
            Series f;
            f = chart1.Series.Add(title);
            f.ChartType = SeriesChartType.Line;
            f.BorderWidth = 3;
            f.Color = color;

            return f;
        }
    }
}
