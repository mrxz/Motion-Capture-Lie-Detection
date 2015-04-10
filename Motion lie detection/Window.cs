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
	 * DEBUG CLASS, not anymore :P
	 */
	public partial class Window : Form
	{
		/**
		 * The recording to visualize.
		 */
		private Recording recording;

        /**
		 * The Algorithm object,
		 */
        private Algorithm algo;

        /**
		 * The Algorithm context.
		 */
        private AlgorithmContext context;

        /**
		 * The Lieresult
		 */
        private LieResult LieResult;
		/**
		 * The frame that is being drawn.
		 */
        private Frame frame;
		
		private VisualizerPass visPass = null;
		private NormalizeOrientation ortPass = null;


		public Window()
		{
			
			InitializeComponent();

            // Used for drawing and calculation speed, those should be done as often as possible only needed for play speed
            // Note: I decreased the interval to ensure we can handle 120 fps, but this isn't the nicest solution.
			var timer = new System.Windows.Forms.Timer();
			timer.Interval = 1000 / 150;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

            //For camera?
            ortPass = new NormalizeOrientation(visPass);

            // Construct the algorithm
            this.algo = new DownsamplePass(new NormalizeOrientation(new NormalizePosition(new NormalizeLength(new LieDetectionAlgorithm()))), 5);

            //Set Algorithmcontext
            this.context = new AlgorithmContext();

            //Set recording
            this.Recording = null;

            //Set Lieresult
            this.LieResult = new LieResult(0);

            //Set Timeline
            timeline.LieResult = LieResult;

		}
              
		public Recording Recording {
			get {
				return this.recording;
			}
			set {
				this.recording = value;
				this.timeline.Recording = value;
				this.frame = Frame.Empty;
                this.visualizer.Frame = frame;
                if (value != null)
                {
                    this.context.Normalizeconfiguration = recording.BodyConfiguration;
                    this.visualizer.BodyConfiguration = recording.BodyConfiguration;
                }
			}
		}

		public void timer_Tick(Object source, EventArgs e)
		{
			if (recording == null)
				return;

			recording.Update();
            timeline.Update();

            // Note: it might be worthwhile to ensure the frame is valid and contains joint data.
            // It shouldn't be a problem since the timeline should ensure that CurrentPos is within the recordings bound.
            visualizer.Frame = timeline.CurrentFrame;
            // TODO: Implement a better method for buffering ahead in the algoritm computation.
            algo.Compute(ref recording, ref context, ref LieResult, timeline.CurrentPos + 20);
		}

		public void keyDown(object source, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
                timeline.Playing = !timeline.Playing;
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

