using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    public struct Frame
    {
        private readonly List<Joint> joints;
        private readonly int timestamp;

        public Frame(List<Joint> joints, int timestamp)
        {
            this.joints = joints;
            this.timestamp = timestamp;
        }

        public ReadOnlyCollection<Joint> Joints { get { return joints != null ? joints.AsReadOnly() : null; } }

        public int Timestamp { get { return timestamp; } }

        public static Frame Empty
        {
            get { return new Frame(null, -1); }
        }

        public static bool IsEmpty(Frame frame)
        {
            return frame.joints == null;
        }

        public static Frame MeanFrame(List<Frame> list)
        {
            List<List<Joint>> joints = new List<List<Joint>>();
            foreach (Frame frame in list)
            {
                for (int i = 0; i < frame.Joints.Count; i++)
                {
                    joints[i].Add(frame.joints[i]);
                }
            }
            return new Frame(joints.ConvertAll<Joint>(new Converter<List<Joint>, Joint>(Joint.MeanJoint)), list[0].Timestamp);
        }

    }
}
