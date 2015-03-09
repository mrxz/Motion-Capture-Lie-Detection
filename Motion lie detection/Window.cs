using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Motion_lie_detection
{
	/**
	 * DEBUG CLASS
	 */
	public partial class Window : Form
	{
		/**
		 * The recording to visualize.
		 */
		private readonly Recording recording;
		/**
		 * The frame that is being drawn.
		 */
		private Frame frame = new Frame();

		private int allTimeMinY = 0;
		private int maxLength = 0;
		private int airBorn = 0;
		private int airBornTime = 0;

		private int currentFrameID = 0;
		private bool forward = true;

		private bool stepMode = false;

		public Window(Recording recording)
		{
			this.recording = recording;
			InitializeComponent();

			// DEBUG: Render update timer thingy
			var timer = new Timer();
			timer.Interval = 1000 / 60;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		public void panel1_Click(Object source, EventArgs e)
		{
			forward = !forward;
		}

		public void panel1_Paint(Object source, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (frame.Joints == null)
				return;

			// Loop over the joints.
			Dictionary<int, Tuple<Joint, int, int>> joints = new Dictionary<int, Tuple<Joint, int, int>>();

			int maxX = 0;
			int minX = 10000;
			int maxY = 0;
			int minY = 10000;

			foreach (Joint joint in frame.Joints) {
				// DEBUG: Use different x positioning code for drawing the running jump
				//int x = (int)(joint.Position.X * 100) + panel1.Width / 2;
				int x = (int)(joint.Position.X * 100) + 100;
				int y = (int)(-joint.Position.Z * 100) + panel1.Height / 2;

				if (x < minX) minX = x;
				if (x > maxX) maxX = x;
				if (y < minY) minY = y;
				if (y > maxY) maxY = y;

				joints.Add(joint.Id, Tuple.Create(joint, x, y));
				g.DrawEllipse (Pens.Green, x - 2, y - 2, 4, 4);
			}
			g.DrawRectangle (Pens.Gray, minX, minY, maxX - minX, maxY - minY);
			g.DrawLine (Pens.IndianRed, 0, minY, 100000, minY);
			g.DrawString (minY.ToString(), new Font ("Arial", 10.0f), Brushes.LightGray, 10, minY - 20);
			g.DrawLine (Pens.IndianRed, 0, maxY, 100000, maxY);
			g.DrawString (maxY.ToString(), new Font ("Arial", 10.0f), Brushes.LightGray, 10, maxY - 20);

			g.DrawLine (Pens.GreenYellow, minX, 0, minX, 100000);
			g.DrawString ((maxX - minX).ToString (), new Font ("Arial", 10.0f), Brushes.Red, (float)(minX + maxX) / 2, (float)minY - 20);   
			g.DrawLine (Pens.GreenYellow, maxX, 0, maxX, 100000);
			if (maxY > allTimeMinY) {
				allTimeMinY = maxY;
				airBornTime = 0;
			} else {
				if (allTimeMinY - maxY > 2)
					airBornTime++;
				else
					airBornTime = 0;
			}
			g.DrawLine (Pens.White, 0, allTimeMinY, 100000, allTimeMinY);

			if (maxY - minY > maxLength)
				maxLength = maxY - minY;

			if (allTimeMinY - maxY > airBorn)
				airBorn = allTimeMinY - maxY;

			g.DrawString("Current frame: " + currentFrameID, new Font ("Arial", 10.0f), Brushes.Cyan, 5, 500);
			g.DrawString("Length: " + maxLength + "cm", new Font ("Arial", 10.0f), Brushes.Cyan, 5, 520);
			g.DrawString("Airborn: " + airBorn + "cm", new Font ("Arial", 10.0f), Brushes.Cyan, 5, 540);
			g.DrawString("Airborn time: " + (float)airBornTime/120.0f + "s", new Font ("Arial", 10.0f), Brushes.Cyan, 5, 560);

			BodyConfiguration bodyConfiguration = recording.BodyConfiguration;

			// Draw lines.
			foreach(Tuple<BodyPart, BodyPart> connection in bodyConfiguration.GetConnections()) 
				drawLine (joints, bodyConfiguration, g, connection.Item1 , connection.Item2, 255);

		}

		private void drawLine(Dictionary<int, Tuple<Joint, int, int>> joints, BodyConfiguration configuration, Graphics g, BodyPart first, BodyPart second, int intensity)
		{
			int one = configuration.GetJointFor (first);
			int two = configuration.GetJointFor (second);
			if (one == -1 || two == -1)
				return;

			Tuple<Joint, int, int> firstJoint = joints [one];
			Tuple<Joint, int, int> secondJoint = joints [two];

			Pen pen = new Pen (Color.FromArgb (0, 0, intensity));
			g.DrawLine (pen, firstJoint.Item2, firstJoint.Item3, secondJoint.Item2, secondJoint.Item3);
		}

		public void timer_Tick(Object source, EventArgs e)
		{
			if (!stepMode) {
				if (forward) {
					currentFrameID++;
					currentFrameID = Math.Min (currentFrameID, recording.LastFrame ());
				} else {
					currentFrameID--;
					currentFrameID = Math.Max (currentFrameID, 0);
				}
			}

			frame = recording.GetFrame (currentFrameID);
			panel1.Refresh ();
		}

		public void keyDown(object source, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
				stepMode = !stepMode;
			if(e.KeyCode == Keys.Right) {
				currentFrameID++;
				currentFrameID = Math.Min (currentFrameID, recording.LastFrame ());
			} else if(e.KeyCode == Keys.Left) {
				currentFrameID--;
				currentFrameID = Math.Max (currentFrameID, 0);
			}
		}

		public void form_Closed(Object source, EventArgs e)
		{
			Environment.Exit (0);
		}
	}
}

