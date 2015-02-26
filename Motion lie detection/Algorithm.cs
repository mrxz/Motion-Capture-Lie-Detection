using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
    public abstract class Algorithm
    {
        public LieResult compute(Recording recording)
        {
            return compute(recording, 0, recording.FrameCount);
        }

        public abstract LieResult compute(Recording recording, int framestart, int framend);
    }

    public abstract class FilterPass : Algorithm
    {
        protected Algorithm BaseAlgorithm;

    }

}
