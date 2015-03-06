namespace Motion_lie_detection
{
	/**
	 * Class responsible for providing the information needed to construct a recording.
	 * This includes the positions and orientations of the 
	 */
	public abstract class RecordingProvider
	{
		/**
		 * Method for returning the framerate for the recording in frames per second.
		 * Note that the framerate MUST be fixed and so subsequential calls to GetFrameRate should return the same value.
		 */
		public abstract int GetFrameRate ();

		/**
		 * Method for getting the body configuration of the body in the recording source.
		 * @return BodyConfiguration of the body if known, null otherwise.
		 */
		public abstract BodyConfiguration getBodyConfiguration ();
	}
}
