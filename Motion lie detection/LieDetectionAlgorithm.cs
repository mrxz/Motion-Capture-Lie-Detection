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
         *  - Abs. movement x Joints
         *  - Abs. movement
         *  - Abs. movement x rootNodes
		 * @param result
		 * @param next
		 * @return
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
                    // In case the node is it's own root, use the absolute movement.
                    if (node == bodyConfiguration.getRoot()) // DEBUG
                    {
                        res.Add(AbsoluteMovement(last.Joints, next.Joints));
                        continue;
                    }

                    // Actual BFS
                    BodyNode normalNode = node.getRoot();
                    queue.Enqueue(node);
                    double totdiff = 0;
                    while (queue.Count > 0)
                    {
                        BodyNode current = queue.Dequeue();
                        foreach (BodyNode v in current.getNeighbours())
                            queue.Enqueue(v);

                        // Add the distance between the joint and the 'normal' joint.
                        // 
                        Joint lastNode = bodyConfiguration.getJoint(last.Joints, current);
                        Joint lastNormal = bodyConfiguration.getJoint(last.Joints, normalNode);
                        Joint nextNode = bodyConfiguration.getJoint(next.Joints, current);
                        Joint nextNormal = bodyConfiguration.getJoint(next.Joints, normalNode);
                        Vector3d A = lastNode.Position - lastNormal.Position;
                        Vector3d B = nextNode.Position - nextNormal.Position;

                        double diffX = A.X - B.X;
                        double diffY = A.Y - B.Y;
                        double diffZ = A.Z - B.Z;
                        double diff = diffX * diffX + diffY * diffY + diffZ * diffZ;

                        totdiff += diff;
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
                double diffX = A[i].Position.X - B[i].Position.X;
                double diffY = A[i].Position.Y - B[i].Position.Y;
                double diffZ = A[i].Position.Z - B[i].Position.Z;
                double diff = diffX * diffX + diffY * diffY + diffZ * diffZ;

                res.Add(diff);
                totdiff += diff;
            }
            totdiff = Math.Sqrt(totdiff);

            // Add sum of pairwise differences
            res.Add(totdiff);
            return res;
        }

        private double AbsoluteMovement(IList<Joint> A, IList<Joint> B)
        {
            // Make sure the lengths of the joint lists match.
            if (A.Count != B.Count)
                throw new Exception("Number of joints, is not equal");

            // Iterate over the joints and sum the distances
            double totdiff = 0;
            for (int i = 0; i < A.Count; i++)
            {
                double diffX = A[i].Position.X - B[i].Position.X;
                double diffY = A[i].Position.Y - B[i].Position.Y;
                double diffZ = A[i].Position.Z - B[i].Position.Z;
                totdiff += diffX * diffX + diffY * diffY + diffZ * diffZ;
            }
            totdiff = Math.Sqrt(totdiff);
            
            // Return sum of pairwise differences
            return totdiff;
        }

    }
}
