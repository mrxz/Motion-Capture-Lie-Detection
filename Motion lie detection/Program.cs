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

		[STAThread]
        public static void Main()
        {
			LOG.info ("Motion lie detection starting up");

			// Calibrate and connect the suit.
			SuitController controller = new XSensController ();
			controller.Calibrate ();
			controller.Connect();

			// Wrap the controller in a recording.
			//RecordingProvider provider = new SuitRecordingProvider (controller);
			OpenFileDialog dialog = new OpenFileDialog ();
			dialog.DefaultExt = "mvnx";
			dialog.Multiselect = false;
			dialog.CheckFileExists = true;
			DialogResult result = dialog.ShowDialog ();
			if (result == DialogResult.Cancel)
				Environment.Exit (0);

			RecordingProvider provider = new FileRecordingProvider (dialog.FileName);
			provider.Init ();
			Recording recording = new Recording (provider, new FixedBodyConfiguration());

			// DEBUG: Open a window.
			new Thread(openWindow).Start(recording);

			// DEBUG: Send dummy stream.
			//DummyStream stream = new DummyStream ();
			//stream.Start ();
        }

		public static void openWindow(Object data) {
			LOG.info ("Opening window for visualization");
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Window((Recording)data));
		}
    }
}
