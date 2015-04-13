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
		
		public Window(Algorithm algorithm)
		{			
			InitializeComponent();
            
            // Set algorithm
            this.algo = algorithm;

            // Used for drawing and calculation speed, those should be done as often as possible only needed for play speed
            // Note: I decreased the interval to ensure we can handle 120 fps, but this isn't the nicest solution.
			var timer = new System.Windows.Forms.Timer();
			timer.Interval = 1000 / 150;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();            

            //Set Lieresult
            this.LieResult = new LieResult(0);

            //Set Timeline
            timeline.LieResult = LieResult;
		}

        public Recording Recording
        {
            get
            {
                return this.recording;
            }
            set
            {
                this.recording = value;
                this.timeline.Recording = value;
                //this.frame = Frame.Empty;
                ////this.visualizer.Frame = frame;
                if (value != null)
                {
                    this.visualizer.BodyConfiguration = recording.BodyConfiguration;
                    this.context = new AlgorithmContext(recording.ClassificationConfiguration);
                }
                else
                    this.context = null;
            }
        }

		public void timer_Tick(Object source, EventArgs e)
		{
			if (recording == null)
				return;

			recording.Update();
            timeline.Update();
            leftSidePanel.Update();

            // Note: it might be worthwhile to ensure the frame is valid and contains joint data.
            // It shouldn't be a problem since the timeline should ensure that CurrentPos is within the recordings bound.
            visualizer.Frame = timeline.CurrentFrame;
            // TODO: Implement a better method for buffering ahead in the algoritm computation.
            algo.Compute(ref recording, ref context, ref LieResult, Math.Max(timeline.CurrentPos + 20, LieResult.End + 100));

		}

		public void keyDown(object source, KeyEventArgs e)
		{
			//if (e.KeyCode == Keys.Space)
            //    timeline.Playing = !timeline.Playing;
		}

		public void form_Closed(Object source, EventArgs e)
		{
			Environment.Exit (0);
		}
	}

}

