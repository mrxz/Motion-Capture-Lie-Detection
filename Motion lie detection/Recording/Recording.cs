using System;
using System.Collections.Generic;

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
		private SortedList<int, MarkPoint> markpoints;

		/**
		 * The recording provider that provides the frame data containing joint positions.
		 */
		private readonly RecordingProvider provider;

		/**
		 * The configuration of the body in this recording.
		 */
		private readonly BodyConfiguration bodyConfiguration;

        /**
		 * The configuration that contains the classification model.
		 */
        private ClassificationConfiguration classificationconfig;

        /**
         * Simple counter for generating unique markpoint ids.
         */
        private int markpointId = 0;
        
		public Recording (RecordingProvider provider, BodyConfiguration bodyConfiguration, ClassificationConfiguration classconfig = null)
		{
			this.provider = provider;
			this.bodyConfiguration = bodyConfiguration;
            this.classificationconfig = FixedClassification.Create();

			frames = new List<Frame> ();
			lastFrameID = -1;
			markpoints = new SortedList<int, MarkPoint> ();

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
         * Method for finalizing the recording, meaning it won't be expanded and the recording provider can be closed.
         */
        public bool Finish()
        {
            // Let the recording provider stop.
            return provider.Stop();
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

		public IList<MarkPoint> MarkPoints { get { return markpoints.Values; } }

		public IList<Frame> Frames { get { return frames; } }

		public int FrameCount { get { return frames.Count; } }

		public BodyConfiguration BodyConfiguration { get { return bodyConfiguration; } }

        public ClassificationConfiguration ClassificationConfiguration {
            get { return classificationconfig; }
            set
            {
                if (value != null)
                    classificationconfig = value;
                else                
                    throw new Exception("Classification model cannot be set to null");
            }
        }

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
            // FIXME: Perhaps disallow multiple markpoints at the exact same frame id.
			markpoints.Add (markpoint.Frameid, markpoint);
            markpointId++;
		}

        public int MarkpointId
        {
            get { return markpointId; }
        }

        public void RemoveMarkPoint(MarkPoint markpoint)
        {
            // Make sure the markpoint isn't null.
            if (markpoint == null)
                return;
            markpoints.Remove(markpoint.Frameid);
        }
	}
}
