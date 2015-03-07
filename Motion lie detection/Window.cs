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
		}


		public void panel1_Paint(Object source, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (frame.Joints == null)
				return;

			// Loop over the joints.
			foreach (Joint joint in frame.Joints) {
				Console.WriteLine (joint.Position.X);

				int x = (int)(joint.Position.X * 100) + panel1.Width / 2;
				int y = (int)(-joint.Position.Z * 100) + panel1.Height / 2;
				g.DrawEllipse (Pens.Green, x, y, 5, 5);

				// Find the corresponding body part.
				//BodyConfiguration bodyConfiguration = recording.BodyConfiguration;
			}
		}

		public void panel1_Click(Object source, EventArgs e) 
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
