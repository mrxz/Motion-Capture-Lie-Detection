using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Motion_lie_detection
{
    public class LeftSidePanel : Panel
    {
        private Timeline timeline;

        private Panel light;
        private Label titleLabel;
        private Label absoluteMovement;

        private List<Tuple<double, double>> results = null;
        public Chart Chart;

        public LeftSidePanel(Timeline timeline)
        {
            this.timeline = timeline;

            light = new Light();
            light.Paint += (obj, e) =>
            {
                Graphics g = e.Graphics;

                // Determine the color
                Brush meanbrush = Brushes.Orange;
                if (results != null)
                {
                    var trueTot = 1.0;
                    var falseTot = 1.0;
                    for (int i = 0; i < 5; i++)
                    {
                        double ptruth = results[24 + i].Item1;
                        double plie = results[24 + i].Item2;

                        trueTot *= ptruth;
                        falseTot *= plie;
                    }

                    double result = trueTot / (trueTot + falseTot);
                    if (result < 0.5f)
                        meanbrush = Brushes.Red;
                    else if (result > 0.5f)
                        meanbrush = Brushes.Green;
                }

                // Draw the ellipse
                Rectangle draw = ((Panel)obj).ClientRectangle;
                draw.Inflate(-20, -20);
                if (draw.Height > draw.Width)
                {
                    int diff = draw.Height - draw.Width;
                    draw.Height = draw.Width;
                    draw.Y += diff / 2;
                }
                else
                {
                    int diff = draw.Width - draw.Height;
                    draw.Width = draw.Height;
                    draw.X += diff / 2;
                }
                g.FillEllipse(meanbrush, draw);
            };
            this.Controls.Add(light);

            titleLabel = new Label();
            titleLabel.Text = "Frame dgiference";
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font(titleLabel.Font.FontFamily, titleLabel.Font.Size, FontStyle.Bold);
            //this.Controls.Add(titleLabel);

            absoluteMovement = new Label();
            absoluteMovement.Text = "";
            absoluteMovement.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(absoluteMovement);

            Chart = new Chart();

            //Chart Settings 
            // Populating the data arrays.
            this.Chart.Series.Clear();

            this.Chart.Palette = ChartColorPalette.Pastel;

            // Set chart title.
            this.Chart.Titles.Add("Frame difference");

            // add a chart legend. neat right.
            this.Chart.Legends.Add("chart legend");
            Chart.ResetAutoValues();
            Chart.ChartAreas.Add(new ChartArea());
            Chart.ChartAreas[0].AxisY.Maximum = 0.02;
            Chart.ChartAreas[0].AxisY.Minimum = 0;
            Chart.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
            Chart.ChartAreas[0].AxisY.Title = "difference";
            Chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            this.Controls.Add(Chart);

            this.Resize += resize;
        }

        public new void Update()
        {





            String text = "";
            if (timeline.Recording != null)
            {

            }
            // update the traffic light
            List<double> movement = timeline.LieResult.ComputeAbsoluteMovements(timeline.SelectionStart, timeline.SelectionEnd);



            if (movement != null)
            {
                BodyConfiguration bodyConfiguration = timeline.Recording.BodyConfiguration;
                int index = bodyConfiguration.Size;


                var abs = movement[index++];

                text += String.Format("Abs. movement: {0:0.000}\n", abs);


                foreach (BodyNode node in timeline.Recording.ClassificationConfiguration.Rootnodes)
                {
                    var value = movement[index++];

                    text += String.Format("Limb {0}: {1:0.000}\n", node.getName(), value);

                }

                this.results = Classification.ClassifyBoth(timeline.Recording.ClassificationConfiguration, movement);
                light.Invalidate();
            }


            absoluteMovement.Text = text;
            absoluteMovement.Invalidate();
            base.Update();
        }

        private void resize(Object sender, EventArgs e)
        {
            Size newSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

            light.Left = 0;
            light.Top = 0;
            light.Width = newSize.Width / 2;
            light.Height = newSize.Height / 2;
            light.Invalidate();

            absoluteMovement.Left = newSize.Width / 2;
            absoluteMovement.Top = 0;
            absoluteMovement.Width = newSize.Width / 2;
            absoluteMovement.Height = newSize.Height / 2;

            Chart.Left = 0;
            Chart.Top = newSize.Height / 2 + 20;
            Chart.Width = newSize.Width;
            Chart.Height = newSize.Height / 2 - 20;
        }

        public class Light : Panel
        {
            public Light()
            {
                // Note: set the style to prevent flickering of the contorl.
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint, true);
            }
        }
    }
}
