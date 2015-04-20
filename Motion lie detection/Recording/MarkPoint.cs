using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
    /**
     * Class representing a markpoint in a recording.
     */ 
    public class MarkPoint
    {
        /**
         * The id of the markpoint unique within the recording.
         */
        private readonly int id;

        /**
         * The description of the markpoint.
         */
        private String description;

        /**
         * The frame id the markpoint belongs to.
         */
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
            set { this.description = value; }
        }

        public int Frameid
        {
            get { return frameId; }
            set { this.frameId = value; }
        }

        public override string ToString()
        {
            return description;
        }
    }
}
