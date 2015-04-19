using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
	/**
	 * Filter pass that places the pelvis joint at the origin of the world (0,0,0)
	 */
    public class NormalizePosition : FilterPass
    {
		public NormalizePosition(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

		public override List<double> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next)
        {
			// Get the position of the root joint.
			Vector3d rootPosition = bodyConfiguration.getRootJoint (next.Joints).Position;

            // Loop over the joints and re-position them.
			List<Joint> newJoints = new List<Joint> ();
			for (int i = 0; i < next.Joints.Count; i++) {
				Joint joint = next.Joints [i];
				newJoints.Add(new Joint(
					joint.Id,
					joint.Position - rootPosition,
					joint.Orientation));
			}
			Frame newFrame = new Frame (newJoints, next.Timestamp);

			return BaseAlgorithm.ComputeFrame (ref context, bodyConfiguration, newFrame);
        }
    }



	/**
	 * Filter pass that normalizes the orientation of the entire body.
	 */
    public class NormalizeOrientation : FilterPass
    {
		public NormalizeOrientation(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

        public override List<double> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next)
        {
			Vector3d reference = bodyConfiguration.getOrientationJoint(next.Joints).Position;

            // Get the rotation of the body.
			double rotation = Math.Atan2 (reference.X, reference.Y);

			// Loop over the joints and rotate them.
			List<Joint> newJoints = new List<Joint> ();
			for (int i = 0; i < next.Joints.Count; i++) {
				Joint joint = next.Joints [i];

				double newX = Math.Cos (rotation) * joint.Position.X - Math.Sin (rotation) * joint.Position.Y;
				double newY = Math.Sin (rotation) * joint.Position.X + Math.Cos (rotation) * joint.Position.Y;

				newJoints.Add(new Joint(
					joint.Id,
					new Vector3d(newX, newY, joint.Position.Z),
					joint.Orientation));
			}
			Frame newFrame = new Frame (newJoints, next.Timestamp);

			return BaseAlgorithm.ComputeFrame (ref context, bodyConfiguration, newFrame);
        }
    }

    /**
	 * Filter pass that normalizes the orientation of the entire body.
	 */
    public class DownsamplePass : FilterPass
    {
        public DownsamplePass(Algorithm baseAlgorithm, int samplerate) : base(baseAlgorithm) { DownsampleRate = samplerate; }

        public int DownsampleRate;

        public override List<double> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next)
        {
            double n = context.SampleSize;
            if (n < DownsampleRate && n > 0)
            {
                IList<Joint> samplejoints = context.SampleFrame.Joints;
				List<Joint> newJoints = new List<Joint> ();
                for (int i = 0; i < samplejoints.Count; i++)
                {
                    Joint j = samplejoints[i];
                    j.Position = j.Position * (n / (n + 1)) + next.Joints[i].Position / (n + 1);
					newJoints.Add(j);
                }

				next = new Frame(newJoints, next.Timestamp);
                
            }
            n++;

            if (n >= DownsampleRate)
            {
                context.FlushSample();
                return BaseAlgorithm.ComputeFrame(ref context, bodyConfiguration, next);
            }
            context.SetSample(next, (int)n);
            return null;
        }
    }

	/**
	 * Filter pass that normalizes the lengths of the body parts/segments.
	 */
    public class NormalizeLength : FilterPass
    {        
		public NormalizeLength(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

        public override List<double> ComputeFrame(ref AlgorithmContext context, BodyConfiguration bodyConfiguration, Frame next)
        {            
            if (context.Normalizeconfiguration != null)
            {
                Vector3d[] nvectors = new Vector3d[next.Joints.Count];
				bool[] normalised = new bool[next.Joints.Count]; //?? maybe we can assume that that it is not cyclic, I think we may :-)
                Queue<BodyNode> queue = new Queue<BodyNode>();
               
				List<Joint> newJoints = new List<Joint> ();
				newJoints.AddRange(next.Joints);

                BodyNode node = context.Normalizeconfiguration.getRoot();
                nvectors[node.JointId] = new Vector3d(0,0,0);
                normalised[node.JointId] = true;
                queue.Enqueue(node);
                while (queue.Count > 0)
                {
                    node = queue.Dequeue();
                    //look at all child nodes
                    foreach (BodyNode child in node.getNeighbours())
                    {
                        if (!normalised[child.JointId - 1])
                        {
                            double nlength = context.Normalizeconfiguration.GetLength(node, child);
                            double rlength = bodyConfiguration.GetLength(node, child);
                            if (nlength == -1 || rlength == -1)
                            {
                                if (nlength != rlength)
                                    throw new Exception("Mismatch between classification configuration and bodyconfiguration"); //Well not lethal but would give some stange figures
                                continue;
                            }
							Vector3d diff = newJoints[node.JointId - 1].Position - newJoints[child.JointId - 1].Position;
                            //calculate normalise vector for the childnode
                            nvectors[child.JointId - 1] = nvectors[node.JointId - 1] + diff * ((nlength - rlength) / rlength);
                            normalised[child.JointId - 1] = true;
                            queue.Enqueue(child);
                        }
                    }

                    //normalise nodejoint
					Joint nodejoint = newJoints[node.JointId-1];
                    nodejoint.Position += nvectors[node.JointId - 1];
                    //put normalised joint back in the frame
					newJoints[node.JointId -1] = nodejoint;
                }

				next = new Frame (newJoints, next.Timestamp);
            }


			return BaseAlgorithm.ComputeFrame (ref context, bodyConfiguration, next);
        }
    }
}
