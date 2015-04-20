using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
    /**
     * Struct of a single frame int a Recording.
     * 
     * Note: frames are read-only and can't be altered.
     * In case a modified frame is needed compute the modified values and construct a new Frame.
     */
    public struct Frame
    {
        /**
         * A special empty frame that can be used as replacement for invalid or otherwise null frames.
         */
        public static readonly Frame Empty = new Frame(null, -1);

        /**
         * List with the joints for this frame containing the positions and orientations.
         */
        private readonly List<Joint> joints;
        /**
         * The timestamp of the frame within the recording.
         */
        private readonly int timestamp;

        public Frame(List<Joint> joints, int timestamp)
        {
            this.joints = joints;
            this.timestamp = timestamp;
        }

        public IList<Joint> Joints { get { return joints != null ? joints.AsReadOnly() : null; } }

        public int Timestamp { get { return timestamp; } }

        /**
         * Method for checking the frame is empty, meaning it contains no joint data.
         * @return True if the frame doesn't contain joint data, false otherwise.
         */
        public static bool IsEmpty(Frame frame)
        {
            return frame.joints == null;
        }

    }
}
