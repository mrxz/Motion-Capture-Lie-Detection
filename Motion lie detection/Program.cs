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
		[STAThread]
        public static void Main()
        {
			// Calibrate and connect the suit.
			SuitController controller = new XSensController ();
			controller.Calibrate ();
			controller.Connect();

			// Wrap the controller in a recording.
			SuitRecordingProvider provider = new SuitRecordingProvider (controller);
			Recording recording = new Recording (provider, new FixedBodyConfiguration());

			// DEBUG:
			((XSensController)controller).Test ();

			// DEBUG: Open a window.
			new Thread(openWindow).Start(recording);
        }

		public static void openWindow(Object data) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Window((Recording)data));
		}
    }
}
