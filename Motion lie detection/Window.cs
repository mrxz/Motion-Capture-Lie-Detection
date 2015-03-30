using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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

		/**
		 * Simple playback control variables.
		 */
		private int currentFrameID = 0;
		private bool forward = true;
		private bool stepMode = false;

		Visualizer visualizer;

		public Window(Recording recording)
		{
			this.recording = recording;
			InitializeComponent();

			// DEBUG: Render update timer thingy
			var timer = new System.Windows.Forms.Timer();
			timer.Interval = 1000 / 60;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			this.visualizer = new Visualizer (recording);
			new Thread (visualizer.Run).Start ();
		}

		public void panel1_Click(Object source, EventArgs e)
		{
			forward = !forward;
		}
			

		public void timer_Tick(Object source, EventArgs e)
		{
			/*recording.Update ();
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
			panel1.Refresh ();*/
			visualizer.Update ();
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

