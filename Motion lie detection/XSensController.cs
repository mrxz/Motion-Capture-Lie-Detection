using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
	public class XSensController : SuitController
	{
		/**
		 * The XSensListener used to receive packages.
		 */
		private readonly XSensListener listener;

		public XSensController ()
		{
			this.listener = new XSensListener ("127.0.0.1", 9763);
		}

		public override bool Connect ()
		{
			return listener.Start ();
		}

		public override bool Disconnect ()
		{
			return listener.Stop ();
		}

		public override bool Calibrate ()
		{
			// Note: in the case of the network stream the calibrating is done by the MVN Studio.
			// TODO: Perhaps use this to determine the framerate?
			// Ideally we want to receive an MXTP13 packages containing the body configuration (but how???).
			return true;
		}
	}
}
