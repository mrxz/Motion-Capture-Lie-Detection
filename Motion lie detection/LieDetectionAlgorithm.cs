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
        protected List<BodyNode> calculationnodes;

        public LieDetectionAlgorithm(List<BodyNode> rootnodes = null)
        {
            calculationnodes = (rootnodes == null) ? new List<BodyNode>() : rootnodes;
        }

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
            Queue<BodyNode> queue = new Queue<BodyNode>();
            BodyNode current;
            foreach (BodyNode node in calculationnodes)
            {
                if(node.getRoot() == node){
                    res.Add(AbsoluteMovement(last.Joints, next.Joints));
                    continue;
                }
                int normal = node.getRoot().JointId;
                queue.Enqueue(node);
                float totdiff = 0;
                while(queue.Count > 0)
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
            return res;

            /*full body
            //res.Add(AbsoluteMovement(last.Joints, next.Joints));
            ////left arm
            //res.Add(NAbsoluteMovement(last.Joints.GetRange((int)BodyPart.LEFT_SHOULDER, 4), next.Joints.GetRange((int)BodyPart.LEFT_SHOULDER, 4), 0));
            ////right arm
            //res.Add(NAbsoluteMovement(last.Joints.GetRange((int)BodyPart.RIGHT_SHOULDER, 4), next.Joints.GetRange((int)BodyPart.RIGHT_SHOULDER, 4), 0));
            ////left leg
            //res.Add(NAbsoluteMovement(last.Joints.GetRange((int)BodyPart.LEFT_UPPER_LEG, 4), next.Joints.GetRange((int)BodyPart.LEFT_UPPER_LEG, 4), 0));
            ////right leg
            //res.Add(NAbsoluteMovement(last.Joints.GetRange((int)BodyPart.RIGHT_UPPER_LEG, 4), next.Joints.GetRange((int)BodyPart.RIGHT_UPPER_LEG, 4), 0));
            ////head
            //res.Add(NAbsoluteMovement(last.Joints.GetRange((int)BodyPart.T8, 3), next.Joints.GetRange((int)BodyPart.T8, 3), 0));
            ////body
            //res.Add(NAbsoluteMovement(last.Joints.GetRange((int)BodyPart.PELVIS, 4), next.Joints.GetRange((int)BodyPart.PELVIS, 4), 0));
            ////upper body
            //res.Add(NAbsoluteMovement(last.Joints.GetRange((int)BodyPart.PELVIS, 15), next.Joints.GetRange((int)BodyPart.PELVIS, 15), 0));
            //return res; */
        }

        private List<float> PairwiseDifference(List<Joint> A, List<Joint> B)
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

        private float AbsoluteMovement(List<Joint> A, List<Joint> B)
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
