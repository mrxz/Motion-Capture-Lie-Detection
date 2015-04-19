using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxf = Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
    public struct Joint
    {
        private readonly int id;
        private Vector3d position;
        private mxf.Quaternion orientation;

        public Joint(int jointId, Vector3d position, mxf.Quaternion orientation)
        {
            id = jointId;
            this.position = position;
            this.orientation = orientation;
        }

        public int Id { get { return id; } }

        public Vector3d Position { get { return position; } set { position = value; } }

        public mxf.Quaternion Orientation { get { return orientation; } }

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
}
