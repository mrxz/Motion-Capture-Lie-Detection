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
        /**
         * Constant to prevent some weird problems in case the algoritm suddenly has to do a lot of work.
         * Note: this will most likely not be a problem, given that it will probably run fast enough on most decent machines.
         */
        public static readonly int MaxFrameExpand = 100;


        /**
         * Method for starting a new computation using the algorithm
         * This method will compute the LieResult over the entire recording (at the time of calling).
         * @param recording The recording to use to compute the LieResult.
         * @param context The context in which the algoritm should run.
         * @return The LieResult for the entire recording.
         */
        public LieResult Compute(ref Recording recording, ref AlgorithmContext context)
        {
            return Compute(ref recording, ref context, 0, recording.FrameCount);
        }

        /**
         * Method for starting a new computation using the algorithm over a given interval.
         * @param recording The recording to use to compute the LieResult.
         * @param context The context in which the algoritm should run.
         * @param frameStart The frame id to start the computation over.
         * @param frameEnd The frame id to end the computation.
         * @return The LieResult for the requested span of the recording.
         */
        public LieResult Compute(ref Recording recording, ref AlgorithmContext context, int framestart, int framend)
        {
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
                List<double> res = ComputeFrame(ref context, recording.BodyConfiguration, recording.Frames[next]);
                if(res != null)
                    result.AddFrameDiff(res, next);
                framestart++;
            }
            return result;
        }

        /**
         * Method for expanding a lieresult with one additional frame.
         * Note: the recording, context and LieResult should not be intermixed with other instances for the result will be undefined.
         * @param recording The recording to use for the computation
         * @param context The context used in the computation of the LieResult.
         * @param result The lie result to expand upon.
         */
        public void Compute(ref Recording recording, ref AlgorithmContext context, ref LieResult result)
        {
            int next = result.NextFrameId;
            if (next > -1 && next < recording.FrameCount)
                result.AddFrameDiff(ComputeFrame(ref context, recording.BodyConfiguration, recording.Frames[next]), next);
        }

        /**
         * Method for expanding a lieresult up to a given frame.
         * Note: the recording, context and LieResult should not be intermixed with other instances for the result will be undefined.
         * @param recording The recording to use for the computation
         * @param context The context used in the computation of the LieResult.
         * @param result The lie result to expand upon.
         * @param frameEnd The frame id to expand the LieResult to.
         */
        public void Compute(ref Recording recording, ref AlgorithmContext context, ref LieResult result, int frameEnd)
        {
            int next = result.NextFrameId;
            int number = 0; // Counter to cap number of frames per update.
            while(next < frameEnd && number < MaxFrameExpand)
            {
                if (next > -1 && next < recording.FrameCount)
                    result.AddFrameDiff(ComputeFrame(ref context, recording.BodyConfiguration, recording.Frames[next]), next);
                next = result.NextFrameId;
                number++;
            }
        }

        /**
         * Abstract method that performs the actual algorithm.
         * @param context The context containing the needed information.
         * @param bodyConfiguration The configuration of the body used in the frame.
         * @param frame The frame to perform the algorithm on.
         * @return List containing the results per body node/joint.
         */
        public abstract List<double> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next);    
    }	
}
