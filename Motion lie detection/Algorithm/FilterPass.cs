using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    /**
	 * A fitler pass that is executed before the algorithm allowing the data to be pre-processed.
	 */
    public abstract class FilterPass : Algorithm
    {
        /**
         * The next step in the chain. This can be another filter or an instance of an algorithm.
         */
        protected Algorithm BaseAlgorithm;

        public FilterPass(Algorithm baseAlgorithm)
        {
            BaseAlgorithm = baseAlgorithm;
        }

    }
}
