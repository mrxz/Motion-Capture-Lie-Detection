using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
	/**
	 * Class representing a recording.
	 */
	public class Recording
	{
		/**
		 * List of frames in the recording.
		 * The index in the list is the frame id, divided by the framerate will result in the time.
		 */
		private readonly List<Frame> frames;
		/**
		 * The last frame available in the recording.
		 */
		private int lastFrameID;
		/**
		 * List containing markpoints made in this recording.
		 */
		private readonly List<MarkPoint> markpoints;

		/**
		 * The recording provider that provides the frame data containing joint positions.
		 */
		private readonly RecordingProvider provider;
		/**
		 * The configuration of the body in this recording.
		 */
		private readonly BodyConfiguration bodyConfiguration;

		public Recording (RecordingProvider provider, BodyConfiguration bodyConfiguration)
		{
			this.provider = provider;
			this.bodyConfiguration = bodyConfiguration;

			frames = new List<Frame> ();
			lastFrameID = -1;
			markpoints = new List<MarkPoint> ();

			// Let the provider know the recording has started.
			// FIXME: Probably better to create a Start() method in Recording and
			//  a flag indicating whether it has started or not.
			this.provider.Start ();
		}

		public Recording (RecordingProvider provider) : this (provider, provider.GetBodyConfiguration ())
		{
		}

		/**
		 * Method for requesting a specific frame of the recording.
		 * @param frameID The id of the frame to return
		 * @return The requested frame if available, null otherwise.
		 */
		public Frame GetFrame(int frameID) {
			// TODO: Handle some rudimentary checks.
			return frames [frameID];
		}

		/**
		 * Method that returns the frameID of the last (or latest) frame of the recording.
		 * @return The last frame ID of the recording.
		 */
		public int LastFrame() {
			// DEBUG: Insert the new frames into the recording.
			List<Frame> frames = provider.GetNewFrames ();
			if (frames != null) {
				foreach (Frame frame in frames) {
					AddFrame (frame);
				}
			}

			// ACTUAL METHOD:
			return lastFrameID;
		}

		public int FrameRate {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public List<MarkPoint> MarkPoints { get { return markpoints; } }

		public List<Frame> Frames { get { return frames; } }

		public int FrameCount { get { return frames.Count; } }

		public BodyConfiguration BodyConfiguration { get { return bodyConfiguration; } }

		public void AddFrame (Frame frame)
		{
			// TODO: replace with something a tad more sophisticated.
			Console.WriteLine (frame.Id);
			frames.Add (frame);
			lastFrameID++;
		}

		/**
		 * Method for inserting a markpoint in the recording.
		 * @param markpoint: The markpoint to add.
		 */
		public void AddMarkPoint (MarkPoint markpoint)
		{
			markpoints.Add (markpoint);
		}
	}

	public struct Frame
	{
		private readonly List<Joint> joints;
		private readonly int id;

		public Frame (int frameId, List<Joint> joints)
		{
			id = frameId;
			this.joints = joints;
		}

		public int Id { get { return id; } }

		public List<Joint> Joints { get { return joints; } }
        
	}

	public struct Joint
	{
		private readonly int id;
		private readonly Vector3 position;
		private readonly Quaternion orientation;

		public Joint (int jointId, Vector3 position, Quaternion orientation)
		{
			id = jointId;
			this.position = position;
			this.orientation = orientation;
		}

		public int Id { get { return id; } }

		public Vector3 Position { get { return position; } }

		public Quaternion Orientation { get { return orientation; } }
	}

	public struct MarkPoint
	{
		private readonly int id;
		private readonly String description;
		private readonly int frameId;

		public MarkPoint (int id, string description, int frameId)
		{
			this.id = id;
			this.description = description;
			this.frameId = frameId;
		}

		public int Id { get { return id; } }

		public String Description { get { return description; } }

		public int Frameid { get { return frameId; } }
	}
}
