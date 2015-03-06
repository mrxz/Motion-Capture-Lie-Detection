using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	/**
	 * Recording provider that uses a motion capture suit as source of joint position data.
	 */
	public class SuitRecordingProvider : RecordingProvider
	{

		private readonly SuitController controller;

		public SuitRecordingProvider (SuitController controller)
		{
			this.controller = controller;
		}

		public override int GetFrameRate ()
		{
			return 60; // FIXME: Hard-coded frame-rate.
		}

		public override BodyConfiguration getBodyConfiguration ()
		{
			return null;
		}
	}
}
