using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
	/**
	 * Class representing a recording.
	 */
	public struct Recording
	{
		/**
		 * List of frames in the recording.
		 * The index in the list is the frame id, divided by the framerate will result in the time.
		 */
		private readonly List<Frame> frames;
		/**
		 * List containing markpoints made in this recording.
		 */
		private readonly List<MarkPoint> markpoints;

		/**
		 * The configuration of the body in this recording.
		 */
		private readonly BodyConfiguration bodyConfiguration;

		public Recording (RecordingProvider provider, BodyConfiguration bodyConfiguration)
		{
			frames = new List<Frame> ();
			markpoints = new List<MarkPoint> ();
			this.bodyConfiguration = bodyConfiguration;
		}

		public Recording (RecordingProvider provider) : this (provider, provider.getBodyConfiguration ())
		{
		}

		public Frame FrameRate {
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
			frames.Add (frame);
		}

		public void AddMarkPoint (MarkPoint mpoint)
		{
			markpoints.Add (mpoint);
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
