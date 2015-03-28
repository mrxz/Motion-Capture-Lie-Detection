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

		/**
		 * Simple playback control variables.
		 */
		private int currentFrameID = 0;
		private bool forward = true;
		private bool stepMode = false;

		/**
		 * Algorithm,
		 */
		private Algorithm algo = null;
		private VisualizerPass visPass = null;
		private NormalizeOrientation ortPass = null;

		private int prevMouseX = -1;

		public Window(Recording recording)
		{
			this.recording = recording;
			InitializeComponent();

			// DEBUG: Render update timer thingy
			var timer = new Timer();
			timer.Interval = 1000 / 60;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			// Construct the algo.
			visPass = new VisualizerPass(new LieDetectionAlgorithm ());
			ortPass = new NormalizeOrientation (visPass);
			algo = new NormalizePosition (ortPass);
		}

		public void panel1_Drag(object source, MouseEventArgs e) {
			if (prevMouseX != -1) {
				ortPass.AdditionalRotation += (float)(MousePosition.X - prevMouseX) * 0.05f;
				prevMouseX = MousePosition.X;
			}
		}
		public void panel1_StartDrag(object source, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				prevMouseX = MousePosition.X;
			} else {
				forward = !forward;
			}
		}
		public void panel1_StopDrag(object source, MouseEventArgs e) {
			prevMouseX = -1;
		}

		public void panel1_Paint(Object source, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (frame.Joints == null)
				return;

			// Loop over the joints.
			Dictionary<int, Tuple<Joint, int, int>> joints = new Dictionary<int, Tuple<Joint, int, int>>();
			foreach (Joint joint in frame.Joints) {
				int x = (int)(joint.Position.X * 200) + panel1.Width / 2;
				int y = (int)(-joint.Position.Z * 200) + panel1.Height / 2;

				joints.Add(joint.Id, Tuple.Create(joint, x, y));
				g.DrawEllipse (Pens.Green, x - 2, y - 2, 4, 4);
			}

			g.DrawString("Current frame: " + currentFrameID + " (" + currentFrameID/60 + "s)", new Font ("Arial", 10.0f), Brushes.Red, 5, 560);
			g.DrawLine (Pens.LightGray, panel1.Width / 2, 0, panel1.Width / 2, panel1.Height);
			g.DrawLine (Pens.LightGray, 0, panel1.Height/2, panel1.Width, panel1.Height /2);

			// Draw lines.
			BodyConfiguration bodyConfiguration = recording.BodyConfiguration;
			Queue<BodyNode> q = new Queue<BodyNode> ();
			q.Enqueue (bodyConfiguration.getRoot ());
			while (q.Count > 0) {
				BodyNode node = q.Dequeue ();

				foreach (BodyNode neighbour in node.getNeighbours()) { 
					drawLine (joints, bodyConfiguration, g, node.getJointId(), neighbour.getJointId(), 255);

					q.Enqueue (neighbour);
				}
			}
		}

		private void drawLine(Dictionary<int, Tuple<Joint, int, int>> joints, BodyConfiguration configuration, Graphics g, int first, int second, int intensity)
		{
			Tuple<Joint, int, int> firstJoint = joints [first];
			Tuple<Joint, int, int> secondJoint = joints [second];

			Pen pen = new Pen (Color.FromArgb (0, 0, intensity));
			g.DrawLine (pen, firstJoint.Item2, firstJoint.Item3, secondJoint.Item2, secondJoint.Item3);
		}

		public void timer_Tick(Object source, EventArgs e)
		{
			recording.Update ();
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
			if(currentFrameID > 1) {
				algo.Compute (recording, currentFrameID - 1, currentFrameID);
				frame = visPass.GetFrame ();
			}
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

	/**
	 * Filter pass that simply stores the frame for visualization.
	 */
	public class VisualizerPass : FilterPass
	{
		private Frame frame = Frame.Empty;

		public VisualizerPass(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

		public override List<float> ComputeFrame (LieResult result, BodyConfiguration bodyConfiguration, Frame next)
		{
			frame = next;
			return BaseAlgorithm.ComputeFrame (result, bodyConfiguration, next);
		}

		public Frame GetFrame() 
		{
			return frame;
		}

	}
}

