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

		public override List<float> ComputeFrame(LieResult result, BodyConfiguration bodyConfiguration, Frame next)
        {
			// Get the position of the root joint.
			Vector3 rootPosition = bodyConfiguration.getRootJoint (next.Joints).Position;

            // Loop over the joints and re-position them.
			for (int i = 0; i < next.Joints.Count; i++) {
				Joint joint = next.Joints [i];
				next.Joints[i] = new Joint(
					joint.Id,
					joint.Position - rootPosition,
					joint.Orientation);
			}

			return BaseAlgorithm.ComputeFrame (result, bodyConfiguration, next);
        }
    }



	/**
	 * Filter pass that normalizes the orientation of the entire body.
	 */
    public class NormalizeOrientation : FilterPass
    {
		public NormalizeOrientation(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

		// DEBUG: this variable is used for debugging purposes to rotate the visualization, i know, UGLY...
		public float AdditionalRotation = 0.0f;

		public override List<float> ComputeFrame(LieResult result, BodyConfiguration bodyConfiguration, Frame next)
        {
			Vector3 reference = bodyConfiguration.getOrientationJoint(next.Joints).Position;

            // Get the rotation of the body.
			double rotation = Math.Atan2 (reference.X, reference.Y);
			// DEBUG:
			rotation += AdditionalRotation;

			// Loop over the joints and rotate them.
			for (int i = 0; i < next.Joints.Count; i++) {
				Joint joint = next.Joints [i];

				double newX = Math.Cos (rotation) * joint.Position.X - Math.Sin (rotation) * joint.Position.Y;
				double newY = Math.Sin (rotation) * joint.Position.X + Math.Cos (rotation) * joint.Position.Y;

				next.Joints[i] = new Joint(
					joint.Id,
					new Vector3((float)newX, (float)newY, joint.Position.Z),
					joint.Orientation);
			}


			return BaseAlgorithm.ComputeFrame (result, bodyConfiguration, next);
        }
    }



	/**
	 * Filter pass that normalizes the lengths of the body parts/segments.
	 */
    public class NormalizeLength : FilterPass
    {
		public NormalizeLength(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

		public override List<float> ComputeFrame(LieResult result, BodyConfiguration bodyConfiguration, Frame next)
        {

			return BaseAlgorithm.ComputeFrame (result, bodyConfiguration, next);
        }
    }
}
