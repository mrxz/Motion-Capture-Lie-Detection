using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
    public class NormalizePosition : FilterPass
    {
        protected override List<float> ComputeFrame(LieResult result, Frame next)
        {
            return BaseAlgorithm.ComputeFrame(result, next);
        }
    }

    public class NormalizeOrientation : FilterPass
    {
        protected override List<float> ComputeFrame(LieResult result, Frame next)
        {
            return BaseAlgorithm.ComputeFrame(result, next);
        }
    }

    public class NormalizeLength : FilterPass
    {
        protected override List<float> ComputeFrame(LieResult result, Frame next)
        {
            return BaseAlgorithm.ComputeFrame(result, next);
        }
    }
}
