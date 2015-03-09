﻿using System;
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
		 * The frame-rate in which the suit-controller provides frames.
		 * Note: this shouldn't change once a recording has started.
		 */
		private int frameRate = 60; // FIXME: Default to 60 fps.

		/**
		 * The body configuration of the person in the suit-controller.
		 * Note: this shouldn't change once a recording has started.
		 */
		private BodyConfiguration bodyConfiguration = null;

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
			return frameRate;
		}

		public override BodyConfiguration GetBodyConfiguration ()
		{
			return bodyConfiguration;
		}

		public override List<Frame> GetNewFrames()
		{
			List<Frame> result = newFrames;
			newFrames = new List<Frame> ();
			return result;
		}

		public void notify(Object data)
		{
			// In case the data is a frame add it to the newFrames list.
			if (data is Frame) {
				// Add the frame.
				newFrames.Add ((Frame)data);
				return;
			}

			// In case it is a (new) body configuration store it.
			if (data is BodyConfiguration) {
				bodyConfiguration = (BodyConfiguration)data;
				return;
			}

			// FIXME: A bit rigid to assume an integer value == frameRate.
			if (data is int) {
				frameRate = (int)data;
				return;
			}
		}
	}
}
