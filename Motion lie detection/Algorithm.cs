﻿using System;
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
        public LieResult Compute(ref Recording recording, ref AlgorithmContext context)
        {
            return Compute(ref recording, ref context, 0, recording.FrameCount);
        }

        public LieResult Compute(ref Recording recording, ref AlgorithmContext context, int framestart, int framend){
            //Check if order of frameindices is valid
            if (framend < framestart)
                throw new Exception("Frame indices must be specified from small to high");

            //Make base result by setting framestart
            LieResult result = new LieResult(framestart);
            
            //Add the consecutive differences of the frames
            while (framestart < framend)
            {
                int next = result.NextFrameId;
                if (next >= recording.FrameCount)
                    break;
                result.AddFrameDiff(ComputeFrame(ref context, recording.BodyConfiguration, recording.Frames[next]), next);
                framestart++;
            }
            return result;
        }

        public void Compute(ref Recording recording, ref AlgorithmContext context, ref LieResult result)
        {
            int next = result.NextFrameId;
            if (next > -1 && next < recording.FrameCount)
                result.AddFrameDiff(ComputeFrame(ref context, recording.BodyConfiguration, recording.Frames[next]), next);
        }

        public abstract List<float> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next);    
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
