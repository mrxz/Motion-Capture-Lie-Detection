using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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


		public void panel1_Paint(Object source, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (frame.Joints == null)
				return;

			// Loop over the joints.
			Dictionary<int, Tuple<Joint, int, int>> joints = new Dictionary<int, Tuple<Joint, int, int>>();
			foreach (Joint joint in frame.Joints) {
				// DEBUG: Use different x positioning code for drawing the running jump
				//int x = (int)(joint.Position.X * 100) + panel1.Width / 2;
				int x = (int)(joint.Position.X * 100) + 100;
				int y = (int)(-joint.Position.Z * 100) + panel1.Height / 2;
				joints.Add(joint.Id, Tuple.Create(joint, x, y));
				g.DrawEllipse (Pens.Green, x, y, 2, 2);
			}

			BodyConfiguration bodyConfiguration = recording.BodyConfiguration;

			// Draw lines.
			drawLine (joints, bodyConfiguration, g, BodyPart.PELVIS, BodyPart.L5);
			drawLine (joints, bodyConfiguration, g, BodyPart.L5, BodyPart.L3);
			drawLine (joints, bodyConfiguration, g, BodyPart.L3, BodyPart.T12);
			drawLine (joints, bodyConfiguration, g, BodyPart.T12, BodyPart.T8);
			drawLine (joints, bodyConfiguration, g, BodyPart.T8, BodyPart.NECK);
			drawLine (joints, bodyConfiguration, g, BodyPart.NECK, BodyPart.HEAD);
			//drawLine (joints, bodyConfiguration, g, BodyPart.LEFT_KNEE, BodyPart.LEFT_FOOT);

		}

		private void drawLine(Dictionary<int, Tuple<Joint, int, int>> joints, BodyConfiguration configuration, Graphics g, BodyPart first, BodyPart second)
		{
			int one = configuration.GetJointFor (first);
			int two = configuration.GetJointFor (second);
			if (one == -1 || two == -1)
				return;

			Tuple<Joint, int, int> firstJoint = joints [one];
			Tuple<Joint, int, int> secondJoint = joints [two];

			g.DrawLine (Pens.Blue, firstJoint.Item2, firstJoint.Item3, secondJoint.Item2, secondJoint.Item3);
		}

		public void timer_Tick(Object source, EventArgs e)
		{
			frame = recording.GetFrame (recording.LastFrame());
			panel1.Refresh ();
		}

		public void form_Closed(Object source, EventArgs e)
		{
			Environment.Exit (0);
		}
	}
}

