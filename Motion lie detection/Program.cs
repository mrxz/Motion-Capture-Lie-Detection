﻿using System;
using System.Windows.Forms;

namespace Motion_lie_detection
{
    /**
     * Entry point of the motion lie detection program.
     */
    public class MotionLieDetection
    {
        /**
         * The main logger
         */
		public static readonly Logger LOG = Logger.getInstance("Main");

		[STAThread]
        public static void Main()
        {
			LOG.info ("Motion lie detection starting up");
                        
            // Construct the algorithm with the needed filter passes.
            Algorithm algo = new DownsamplePass(new NormalizeOrientation(new NormalizePosition(new LieDetectionAlgorithm())), 12);
            
            LOG.info("Opening window for visualization");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window(algo));
            LOG.info("Window closed, exiting progam");
        }

    }
}
