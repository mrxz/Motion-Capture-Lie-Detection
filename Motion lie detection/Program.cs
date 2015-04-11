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
                        
            //new Thread(updateVisualizer).Start();
            //Algorithm algo= new DownsamplePass(new NormalizeOrientation(new NormalizePosition(new NormalizeLength(new LieDetectionAlgorithm()))), 5);
            Algorithm algo = new DownsamplePass(new NormalizeOrientation(new NormalizePosition(new LieDetectionAlgorithm())), 12);
            
            LOG.info("Opening window for visualization");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window(algo));
        }

    }
}
