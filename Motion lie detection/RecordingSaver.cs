using System;

namespace Motion_lie_detection
{
	/**
	 * Abstract class for writing a recording to file.
	 */
	public abstract class RecordingSaver
	{
		/**
		 * The file path and name to write the recording to.
		 */
		protected String file;

		public RecordingSaver(String file) {
			this.file = file;
		}

		/**
		 * Method for saving an entire recording to the file.
		 * @param recording The recording to save.
		 * @return True on success, false on failure.
		 */
		public bool saveToFile(Recording recording) {
			return saveToFile (recording, 0, recording.FrameCount);
		}

		/**
		 * Method for saving an recording from a given point to the file.
		 * @param recording The recording to save a portion of.
		 * @param start The frame index to start saving from.
		 * @return True on success, false on failure.
		 */
		public bool saveToFile(Recording recording, int start) {
			return saveToFile (recording, start, recording.FrameCount);
		}

		/**
		 * Method for saving a given segment of a recording to the file.
		 * @param recording The recording to save the segment of.
		 * @param start The frame index of the start of the segment.
		 * @param end The frame index of the end of the segment.
		 * @return True on success, false on failure.
		 */
		public abstract bool saveToFile(Recording recording, int start, int end);

	}

	/**
	 * Implementation for saving a recording to an .mvnx file.
	 */
}

