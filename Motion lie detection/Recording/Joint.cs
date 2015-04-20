using System;
using mxf = Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
    /**
     * Struct for a single joint in a single frame of a recording.
     * The joint contains the position and orientation data.
     */
    public struct Joint
    {
        /**
         * The id of the joint.
         */
        private readonly int id;

        /**
         * The absolute position of the joint.
         */
        private Vector3d position;
        /**
         * The orientation of the joint.
         */
        private mxf.Quaternion orientation;

        public Joint(int jointId, Vector3d position, mxf.Quaternion orientation)
        {
            id = jointId;
            this.position = position;
            this.orientation = orientation;
        }

        public int Id { get { return id; } }

        public Vector3d Position { get { return position; } }

        public mxf.Quaternion Orientation { get { return orientation; } }
    }
}
