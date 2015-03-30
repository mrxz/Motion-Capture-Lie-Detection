using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
	/**
	 * The algorithm class.
	 */
    public abstract class Algorithm
    {
        public LieResult Compute(Recording recording)
        {
            return Compute(recording, 0, recording.FrameCount);
        }

        public LieResult Compute(Recording recording, int framestart, int framend){
            //Check if order of frameindices is valid
            if (framend < framestart)
                throw new Exception("Frame indices must be specified from small to high");

            //Make base result by setting framestart
            LieResult result = new LieResult(recording, framestart, Frame.Empty);
            
            //Add the consecutive differences of the frames
            while (framestart < framend)
            {
                int next = result.NextFrameId;
                if (next >= recording.FrameCount)
                    break;
				result.AddDiff(next, ComputeFrame(result, recording.BodyConfiguration, recording.Frames[next]));
                framestart++;
            }
            return result;
        }

        public LieResult Compute(ref Recording recording, LieResult result)
        {
            int next = result.NextFrameId;
            if (next > -1 && next < recording.FrameCount)
                result.AddDiff(next, ComputeFrame(result, recording.BodyConfiguration, recording.Frames[next]));
            return result;
        }

        public abstract List<float> ComputeFrame(LieResult result, BodyConfiguration bodyConfiguration, Frame next);    
    }

	/**
	 * A fitler pass that is executed before the algorithm allowing the data to be pre-processed.
	 */
    public abstract class FilterPass : Algorithm
    {
		/**
		 * The next step in the chain, can be another filter or an instance of an algorithm.
		 */
        protected Algorithm BaseAlgorithm;

		public FilterPass(Algorithm baseAlgorithm) {
			BaseAlgorithm = baseAlgorithm;
		}

    }

}
