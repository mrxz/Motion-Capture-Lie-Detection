﻿using System;
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
        public static Recording recording;

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
			stream.Start (); */


			/**
			 * File recording demo
			 */
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

			/**
			 * Saving the recording.
			 */
			//RecordingSaver saver = new MVNXSaver ("test.mvnx");
			//saver.saveToFile (recording);

			/**
			 * DEBUG: Visualization
			 */
            
            new Thread(updateVisualizer).Start();
			//new Thread(openWindow).Start(recording);
        }

		public static void openWindow(Object data) {
			LOG.info ("Opening window for visualization");
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Window((Recording)data));
		}

        public static void updateVisualizer()
        {
            LOG.info("Opening window for visualization");
            visualizer = new Visualizer(recording);
			visualizer.Run ();
            LOG.info("Closing window");
        }
    }
}
