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

			// DEBUG: Render update timer thingy, ;)
            //Used for drawing and calculation speed, those should be done as often as possible only needed for play speed
			var timer = new System.Windows.Forms.Timer();
			timer.Interval = 1000 / 60;
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
            //FIXME update should only have to be done if recording update has new frames maybe make some kind of event on new frames.
            // WONT FIX => The timeline requires to know a correct delta time during updates, besides the timeline now also handles the playback, so...
            timeline.Update(); 

            visualizer.Frame = timeline.CurrentFrame;
            // TODO: Implement a better method for buffering ahead in the algoritm computation.
            algo.Compute(ref recording, ref context, ref LieResult, timeline.CurrentPos + 20);
		}

		public void keyDown(object source, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
                timeline.Playing = !timeline.Playing;
			else if (e.KeyCode == Keys.A) {
			//	recording.AddMarkPoint(new MarkPoint(recording.MarkPoints.Count, "This is a description", timeline.CurrentPos));
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

