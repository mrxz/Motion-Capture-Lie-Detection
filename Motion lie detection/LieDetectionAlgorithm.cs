using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	/**
	 * Lie detection algorithm.
	 */
    public class LieDetectionAlgorithm : Algorithm
    {
		/**
		 * Method for computing the result for one single frame.
		 * @param result
		 * @param next
		 * @return
		 */
		public override List<float> ComputeFrame(LieResult result, BodyConfiguration bodyConfiguration, Frame next)
        {
            Frame last = result.LastFrame;
            result.AddFrame(next);
            if (Frame.IsEmpty(last))
                return null;
            return PairwiseDifference(last, next);
        }

        //public override LieResult NextFrame(LieResult result)
        //{
        //    Frame next = result.NextFrame;
        //    Frame last = result.LastFrame;
        //    //Check if next frame does exist
        //    if (!Frame.IsEmpty(next))
        //        //if there is now last frame, no differences can be calculated onlt framestaret can be set
        //        if (!Frame.IsEmpty(next))
        //            result.AddFrame(next, null);
        //        else
        //        result.AddFrame(next, PairwiseDifference(last, next));
        //    else
        //        result = null;
        //    return result;
        //}

        private List<float> PairwiseDifference(Frame A, Frame B)
        {
            if (A.Joints.Count != B.Joints.Count)
                throw new Exception("Number of joints, is not equal");
            List<float> res = new List<float>();
            float totdiff = 0;
            for (int i = 0; i < A.Joints.Count; i++)
            {
                float diff = Vector3.Distance(A.Joints[i].Position, B.Joints[i].Position);
                res.Add(diff);
                totdiff += diff;
            }
            //Add sum of pairwise differences
            res.Add(totdiff);
            return res;
        }
    }
}
