using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
    public abstract class Algorithm
    {
        public LieResult Compute(ref Recording recording)
        {
            return Compute(ref recording, 0, recording.FrameCount);
        }

        public LieResult Compute(ref Recording recording, int framestart, int framend){
            //Check if order of frameindices is valid
            if (framend < framestart)
                throw new Exception("Frame indices must be specified from small to high");

            //Make base result by setting framestart
            LieResult result = new LieResult(ref recording, framestart, Frame.Empty);
            
            //Add the consecutive differences of the frames
            while (framestart < framend)
            {
                int next = result.NextFrameId;
                if (next >= recording.FrameCount)
                    break;
                result.AddDiff(next, ComputeFrame(result, recording.Frames[next]));
                framestart++;
            }
            return result;
        }

        protected abstract List<float> ComputeFrame(LieResult result, Frame next);    
    }

    public abstract class FilterPass : Algorithm
    {
        protected Algorithm BaseAlgorithm;

    }

}
