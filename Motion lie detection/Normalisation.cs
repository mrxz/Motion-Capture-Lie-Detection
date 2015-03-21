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

        public override List<float> ComputeFrame(LieResult result, Frame next)
        {
			// Get the position of the root joint.
			Vector3 rootPosition = Vector3.Zero;
			foreach (Joint joint in next.Joints) {
				// FIXME: Hard-coded pelvis joint id.
				if (joint.Id == 1) { 
					rootPosition = joint.Position;
					break;
				}
			}

            // Loop over the joints and disposition them.
			for (int i = 0; i < next.Joints.Count; i++) {
				Joint joint = next.Joints [i];
				next.Joints[i] = new Joint(
					joint.Id,
					joint.Position - rootPosition,
					joint.Orientation);
			}

			return BaseAlgorithm.ComputeFrame (result, next);
        }
    }



	/**
	 * Filter pass that normalizes the orientation of the entire body.
	 */
    public class NormalizeOrientation : FilterPass
    {
		public NormalizeOrientation(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

        public override List<float> ComputeFrame(LieResult result, Frame next)
        {
            

			return BaseAlgorithm.ComputeFrame (result, next);
        }
    }



	/**
	 * Filter pass that normalizes the lengths of the body parts/segments.
	 */
    public class NormalizeLength : FilterPass
    {
		public NormalizeLength(Algorithm baseAlgorithm) : base(baseAlgorithm) {}

        public override List<float> ComputeFrame(LieResult result, Frame next)
        {


			return BaseAlgorithm.ComputeFrame (result, next);
        }
    }
}
