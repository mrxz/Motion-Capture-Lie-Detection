using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Motion_lie_detection
{
    public class MotionLieDetection
    {
		public static readonly Logger LOG = Logger.getInstance("Main");
        public static Visualizer visualizer;

		[STAThread]
        public static void Main()
        {
			LOG.info ("Motion lie detection starting up");

			/**
			 * Suit demo code
			 */
			/*
			SuitController controller = new XSensController ();
			controller.Calibrate ();
			controller.Connect();
			RecordingProvider provider = new SuitRecordingProvider (controller);
			provider.Init ();
			Recording recording = new Recording (provider, new FixedBodyConfiguration());

			DummyStream stream = new DummyStream ();
			stream.Start (); 
			*/

			/**
			 * File recording demo
			 */
			/*
			OpenFileDialog dialog = new OpenFileDialog ();
			dialog.DefaultExt = "mvnx";
			dialog.Multiselect = false;
			dialog.CheckFileExists = true;
			DialogResult result = dialog.ShowDialog ();
			if (result == DialogResult.Cancel)
				Environment.Exit (0);

			RecordingProvider provider = new FileRecordingProvider (dialog.FileName);
			provider.Init ();
		    recording = new Recording (provider);
			recording.Update ();
			*
			/**
			 * Saving the recording.
			 */
			//RecordingSaver saver = new MVNXSaver ("test.mvnx");
			//saver.saveToFile (recording);

            /**
             * DEBUG: Algorithm,
             */
			/*
            Algorithm algo = new DownsamplePass(new NormalizeOrientation(new NormalizePosition(new LieDetectionAlgorithm())), 5);
            AlgorithmContext context = new AlgorithmContext();
            LieResult res = LieResult.Empty;
            while (res.NextFrameId < recording.FrameCount)
            {
                res = algo.Compute(ref recording, ref context, res);
            }*/

			/**
			 * DEBUG: Visualization
			 */
            
            //new Thread(updateVisualizer).Start();
            Algorithm algo= new DownsamplePass(new NormalizeOrientation(new NormalizePosition(new NormalizeLength(new LieDetectionAlgorithm()))), 5);
            
            LOG.info("Opening window for visualization");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window(algo));
            //new Thread(openWindow).Start();
        }

        //public static void openWindow() {
        //    LOG.info ("Opening window for visualization");
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Window(recording));
        //}

        public static void updateVisualizer()
        {
            LOG.info("Opening window for visualization");

            LOG.info("Closing window");
        }
    }
}
