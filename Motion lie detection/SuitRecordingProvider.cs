using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	/**
	 * Recording provider that uses a motion capture suit as source of joint position data.
	 */
	public class SuitRecordingProvider : RecordingProvider, Observer
	{
		/**
		 * The suit controller that provides the frames and body configuration.
		 */
		private readonly SuitController controller;

		/**
		 * Buffer containing new frames (and unfinished/partial frames).
		 */
		private List<Frame> newFrames;

		public SuitRecordingProvider (SuitController controller)
		{
			this.controller = controller;
		}

		public override bool Init ()
		{
			// Register to the SuitController.
			this.controller.Register (this);
			newFrames = new List<Frame> ();

			return true;
		}

		public override bool Start ()
		{
			// FIXME: Clear the NewFrames list and capture the time stamp as start point.
			return true;
		}

		public override int GetFrameRate ()
		{
			return 60; // FIXME: Hard-coded frame-rate.
		}

		public override BodyConfiguration GetBodyConfiguration ()
		{
			return null;
		}

		public override List<Frame> GetNewFrames()
		{
			List<Frame> result = newFrames;
			newFrames = new List<Frame> ();
			return result;
		}

		public void notify(Object data)
		{
			// Only interested in frames.
			if (data is Frame) {
				// Add the frame.
				newFrames.Add ((Frame)data);
			}
		}
	}
}
