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
         * The result will look as follows:
         *  n times   - Abs. movement of the joints
         *  1 time    - Abs. movement of the entire body
         *  rootNodes - Abs. movement of the rootNodes
		 * @param context The context in which the algorithm runs. 
         * @param bodyConfiguration The configuration of the body as in the frame and context.
		 * @param next The new frame to run the computations on.
		 * @return List of absolute movements in the order as described above.
		 */
		public override List<double> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next)
        {
            // Get the last frame to compute the diff.
            Frame last = context.LastFrame;
            context.LastFrame = next;

            // In case the last frame was empty, no diff can be calcualted.
            if (Frame.IsEmpty(last))
                return null;

            // Pairwise compare the joints of the last and next frames.
            List<double> res = PairwiseDifference(last.Joints, next.Joints);
            // Check if rootnodes are specified, needed for the classification
            if (context.RootNodes != null)
            {
                // Perform a BFS from each of the rootnodes.
                Queue<BodyNode> queue = new Queue<BodyNode>();
                foreach (BodyNode node in context.RootNodes)
                {
                    BodyNode normalNode;
                    // In case the node is the root, use the absolute movement.
                    if (node == bodyConfiguration.getRoot())
                        normalNode = node;
                    else
                        normalNode = node.getRoot();

                    // Actual BFS
                    Vector3d nextNormal = bodyConfiguration.getJoint(next.Joints, normalNode).Position;
                    Vector3d lastNormal = bodyConfiguration.getJoint(last.Joints, normalNode).Position;
                    queue.Enqueue(node);
                    double totdiff = 0;
                    while (queue.Count > 0)
                    {
                        BodyNode current = queue.Dequeue();
                        foreach (BodyNode v in current.getNeighbours())
                            queue.Enqueue(v);

                        // Normalize joint positions
                        Vector3d A = bodyConfiguration.getJoint(last.Joints, current).Position - lastNormal;
                        Vector3d B = bodyConfiguration.getJoint(next.Joints, current).Position - nextNormal;

                        totdiff += (A - B).LengthSquared;
                    }
                    totdiff = Math.Sqrt(totdiff);

                    // return sum of pairwise differences
                    res.Add(totdiff);
                }
            }

            return res;
        }

        private List<double> PairwiseDifference(IList<Joint> A, IList<Joint> B)
        {
            // Make sure the lengths of the joint lists match.
            if (A.Count != B.Count)
                throw new Exception("Number of joints, is not equal");

            // Iterate over the joints and compute the distance.
            List<double> res = new List<double>();
            double totdiff = 0;
            for (int i = 0; i < A.Count; i++)
            {
                double diff = (A[i].Position - B[i].Position).LengthSquared;
                res.Add(diff);
                totdiff += diff;
            }
            totdiff = Math.Sqrt(totdiff);

            // Add sum of pairwise differences
            res.Add(totdiff);
            return res;
        }
    }
}
