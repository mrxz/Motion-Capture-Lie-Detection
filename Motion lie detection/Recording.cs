
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
    public struct Recording
    {
        private List<Frame> frames;
        private List<MarkPoint> markpoints;
        private BodyConfiguration bconfig;

        public Recording(BodyConfiguration bconfig)
        {
            frames = new List<Frame>();
            markpoints = new List<MarkPoint>();
        }
        
        public Frame FrameRate
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public List<MarkPoint> MarkPoints { get { return markpoints; } }

        public List<Frame> Frames { get { return frames; } }

        public int FrameCount { get { return frames.Count; } }

        public BodyConfiguration BodyConfiguration { get { return bconfig; } }
        
        public void AddFrame(Frame frame){
            frames.Add(frame);
        }

        public void AddMarkPoint(MarkPoint mpoint)
        {
            markpoints.Add(mpoint);
        }
    }

    public struct Frame
    {
        private List<Joint> joints;
        private int id;

        public Frame(int num, List<Joint> joints)
        {
            id = num;
            this.joints = joints;
        }

        public int Id { get { return id; } }

        public List<Joint> Joints { get { return joints; } }
        
    }

    public struct Joint
    {        
        private int id;
        public Vector3 Position {get; set;}
        public Quaternion Orientation { get; set; }

        public Joint(int num, Vector3 position, Quaternion orientation)
        {
            id = num;
            Position = position;
            Orientation = orientation;
        }

        public int Id { get { return id; } }
    }

    public struct MarkPoint
    {
        private int id;
        public string Description { get; set; }
        public int Frameid { get; set; }

        public MarkPoint(int id, string description, int frameid)
        {
            this.id = id;
            Description = description;
            Frameid = frameid;
        }

        public int Id { get { return id; } }
    }
}
