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
		private List<MarkPoint> markpoints;

		/**
		 * The recording provider that provides the frame data containing joint positions.
		 */
		private readonly RecordingProvider provider;
		/**
		 * The configuration of the body in this recording.
		 */
		private readonly BodyConfiguration bodyConfiguration;

        /**
		 * Number of frames that are averaged
		 */
        private int downsamplerate = 1;

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

		public Recording (RecordingProvider provider) : this (provider, provider.GetBodyConfiguration ()) {}		

		/**
		 * Method for requesting a specific frame of the recording.
		 * @param frameID The id of the frame to return
		 * @return The requested frame if available, Frame.Empty otherwise.
		 */
		public Frame GetFrame(int frameID) {
			if (frameID < 0 || frameID >= frames.Count)
				return Frame.Empty;
			return frames [frameID];
		}

		/**
		 * Method for updating the recording by appending any new frames from the underlying recording provider.
		 * This method should be called when the recording is allowed to change.
		 * @return True if new frames are added, false if none are added.
		 */
		public bool Update() {
			List<Frame> frames = provider.GetNewFrames ();
			if (frames != null && frames.Count > 0) {
				foreach (Frame frame in frames) {
					AddFrame (frame);
				}

				// New frames are added.
				return true;
			}

			// No new frames.
			return false;
		}

		/**
		 * Method that returns the frameID of the last (or latest) frame of the recording.
		 * @return The last frame ID of the recording.
		 */
		public int LastFrame() {
			return lastFrameID;
		}

		public int FrameRate {
			get {
				return provider.GetFrameRate();
			}
		}

		public List<MarkPoint> MarkPoints { get { return markpoints; } }

		public List<Frame> Frames { get { return frames; } }

		public int FrameCount { get { return frames.Count; } }

		public BodyConfiguration BodyConfiguration { get { return bodyConfiguration; } }

		public void AddFrame (Frame frame)
		{
			// TODO: replace with something a tad more sophisticated.
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

        public int DownSampleRate
        {
            get { return downsamplerate; }
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
        
        public static Frame Empty
        {
            get { return new Frame(-1, null); }
        }

        public static bool IsEmpty(Frame frame)
        {
            return frame.joints == null;
        }

        public static Frame MeanFrame(List<Frame> list)
        {
            List<List<Joint>> joints = new List<List<Joint>>();
            foreach(Frame frame in list){
                for (int i = 0; i < frame.Joints.Count; i++){
                    joints[i].Add(frame.joints[i]);
                }
            }
            return new Frame(list[0].Id, joints.ConvertAll<Joint>(new Converter<List<Joint>, Joint>(Joint.MeanJoint)));
        }
    }

	public struct Joint
	{
		private readonly int id;
		private Vector3 position;
		private Quaternion orientation;

		public Joint (int jointId, Vector3 position, Quaternion orientation)
		{
			id = jointId;
			this.position = position;
			this.orientation = orientation;
		}

		public int Id { get { return id; } }

        public Vector3 Position { get { return position; } set { position = value; } }

		public Quaternion Orientation { get { return orientation; } }

        public static Joint MeanJoint(List<Joint> joints)
        {
            //TODO: calculate mean for quaternions
            Joint res = joints[0];
            for (int i = 1; i < joints.Count; i++)
            {
                res.position += joints[i].Position;
                //res.orientation += joints[i].orientation;
            }
            res.position /= joints.Count;
            //res.orientation /= joints.Count;
            return res;
        }
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
