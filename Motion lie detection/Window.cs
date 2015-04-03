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
		private Recording recording;

        /**
		 * The rlgorithm context.
		 */
        private AlgorithmContext context;
		/**
		 * The frame that is being drawn.
		 */
        private Frame frame;

		/**
		 * Simple playback control variables.
		 */
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
            this.context = new AlgorithmContext();
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

			timeline.Recording = recording;
			timeline.CurrentPos = 0;
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
				int x = (int)(joint.Position.X * 200) + canvas.Width / 2;
				int y = (int)(-joint.Position.Z * 200) + canvas.Height / 2;

				joints.Add(joint.Id, Tuple.Create(joint, x, y));
				g.DrawEllipse (Pens.Green, x - 2, y - 2, 4, 4);
			}

			g.DrawString("Current frame: " + timeline.CurrentPos + " (" + timeline.CurrentPos/recording.FrameRate + "s)", new Font ("Arial", 10.0f), Brushes.Red, 5, 560);
			g.DrawLine (Pens.LightGray, canvas.Width / 2, 0, canvas.Width / 2, canvas.Height);
			g.DrawLine (Pens.LightGray, 0, canvas.Height/2, canvas.Width, canvas.Height /2);

			// Draw lines.
			BodyConfiguration bodyConfiguration = recording.BodyConfiguration;
			Queue<BodyNode> q = new Queue<BodyNode> ();
			q.Enqueue (bodyConfiguration.getRoot ());
			while (q.Count > 0) {
				BodyNode node = q.Dequeue ();

				foreach (BodyNode neighbour in node.getNeighbours()) { 
					drawLine (joints, bodyConfiguration, g, node.JointId, neighbour.JointId, 255);

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

		public Recording Recording {
			get {
				return this.recording;
			}
			set {
				this.recording = value;
				this.timeline.Recording = value;
				this.timeline.CurrentPos = 0; // FIXME: should be in the setter of timeline.Recording
				this.frame = Frame.Empty;
				this.canvas.Invalidate ();
			}
		}

		public void timer_Tick(Object source, EventArgs e)
		{
			if (recording == null)
				return;

			recording.Update ();
			if (!stepMode) {
				if (forward)
					timeline.CurrentPos++;
				else
					timeline.CurrentPos--;
			}

			frame = recording.GetFrame (timeline.CurrentPos);
			if(timeline.CurrentPos > 1) {
				algo.Compute (ref recording, ref context, timeline.CurrentPos - 1, timeline.CurrentPos);
				frame = visPass.GetFrame ();
			}
			canvas.Refresh ();
		}

		public void keyDown(object source, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
				stepMode = !stepMode;
			else if (e.KeyCode == Keys.A) {
				recording.AddMarkPoint(new MarkPoint(recording.MarkPoints.Count, "This is a description", timeline.CurrentPos));
			} else if(e.KeyCode == Keys.Right) {
				timeline.CurrentPos++;
			} else if(e.KeyCode == Keys.Left) {
				timeline.CurrentPos--;
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
		private Frame frame;

		public VisualizerPass(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

		public override List<float> ComputeFrame (ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next)
		{
			frame = next;
			return BaseAlgorithm.ComputeFrame (ref context, bodyConfiguration, next);
		}

		public Frame GetFrame() 
		{
			return frame;
		}

	}
}

