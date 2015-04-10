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
		public override List<float> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next)
        {
            Frame last = context.LastFrame;
            context.LastFrame = next;
            if (Frame.IsEmpty(last))
                return null;

            List<float> res = PairwiseDifference(last.Joints, next.Joints);
            if (context.RootNodes != null)
            {
                Queue<BodyNode> queue = new Queue<BodyNode>();
                BodyNode current;
                foreach (BodyNode node in context.RootNodes)
                {
                    if (node.getRoot() == node)
                    {
                        res.Add(AbsoluteMovement(last.Joints, next.Joints));
                        continue;
                    }
                    int normal = node.getRoot().JointId;
                    queue.Enqueue(node);
                    float totdiff = 0;
                    while (queue.Count > 0)
                    {
                        current = queue.Dequeue();
                        foreach (BodyNode v in current.getNeighbours())
                        {
                            queue.Enqueue(v);
                        }
                        totdiff += Vector3.Distance(last.Joints[current.JointId].Position - last.Joints[normal].Position, next.Joints[current.JointId].Position - next.Joints[normal].Position);
                    }
                    //return sum of pairwise differences
                    res.Add(totdiff);
                }
            }
            return res;
        }

        private List<float> PairwiseDifference(IList<Joint> A, IList<Joint> B)
        {
            if (A.Count != B.Count)
                throw new Exception("Number of joints, is not equal");
            List<float> res = new List<float>();
            float totdiff = 0;
            for (int i = 0; i < A.Count; i++)
            {
                float diff = Vector3.Distance(A[i].Position, B[i].Position);
                res.Add(diff);
                totdiff += diff;
            }
            //Add sum of pairwise differences
            res.Add(totdiff);
            return res;
        }

        private float AbsoluteMovement(IList<Joint> A, IList<Joint> B)
        {
            if (A.Count != B.Count)
                throw new Exception("Number of joints, is not equal");
            float totdiff = 0;
            for (int i = 0; i < A.Count; i++)
            {
                totdiff += Vector3.Distance(A[i].Position, B[i].Position);
            }
            //return sum of pairwise differences
            return totdiff;
        }

        //private float NAbsoluteMovement(List<Joint> A, List<Joint> B, int normal = 0)
        //{
        //    if (A.Count != B.Count)
        //        throw new Exception("Number of joints, is not equal");
        //    float totdiff = 0;
        //    for (int i = 0; i < A.Count; i++)
        //    {
        //        totdiff += Vector3.Distance(A[i].Position - A[normal].Position, B[i].Position - B[normal].Position);
        //    }
        //    //return sum of pairwise differences
        //    return totdiff;
        //}
    }
}
