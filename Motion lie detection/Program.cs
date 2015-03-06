using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Motion_lie_detection
{
    public class MotionLieDetection
    {
        public static void Main()
        {
			SuitController controller = new XSensController ();
			controller.Calibrate ();
			controller.Connect();

			Thread.Sleep (1000);
			controller.Disconnect ();
			Thread.Sleep (1000);
			controller.Connect ();
			//XSensListener.Listen();
        }
    }
}
