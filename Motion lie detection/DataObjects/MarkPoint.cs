using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    public class MarkPoint
    {
        private readonly int id;
        private String description;
        private int frameId;

        public MarkPoint(int id, string description, int frameId)
        {
            this.id = id;
            this.description = description;
            this.frameId = frameId;
        }

        public int Id { get { return id; } }

        public String Description
        {
            get { return description; }
            set
            {
                this.description = value;
            }
        }

        public int Frameid
        {
            get { return frameId; }
            set
            {
                this.frameId = value;
            }
        }

        public override string ToString()
        {
            return description;
        }
    }
}
