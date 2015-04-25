using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	/**
	 * Class responsible for providing the information needed to construct a recording.
	 * This includes the positions and orientations of the 
	 */
	public abstract class RecordingProvider
	{

		/**
		 * Method that allows the recording provider to be initialized.
		 */
		public abstract bool Init();

		/**
		 * Method that signals the recording provider that the recording starts.
		 */
		public abstract bool Start();

        /**
         * Method that signals the recording provider to stop.
         */
        public abstract bool Stop();

		/**
		 * Method for returning the framerate for the recording in frames per second.
		 * Note that the framerate MUST be fixed and so subsequential calls to GetFrameRate should return the same value.
		 */
		public abstract int GetFrameRate ();

		/**
		 * Method for getting the body configuration of the body in the recording source.
		 * @return BodyConfiguration of the body if known, null otherwise.
		 */
		public abstract BodyConfiguration GetBodyConfiguration ();

		/**
		 * Method for retrieving the (buffered) frames since the last retrieval.
		 * NOTE: Might be better to switch to a push based system rather than pull...
		 */
		public abstract List<Frame> GetNewFrames ();
	}
}
