using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
    /**
     * Result class from the algorithm.
     * This result contains all the information to classify the result as either a lie or a truth.
     */
    public class LieResult
    {
        /**
         * The start frame of the interval the lie result was computed over.
         */
        private int framestart;
        /**
         * The end frame of the interval the lie result was computed over.
         */
        private int framend;

        /**
         * The list of framedifferences.
         */
        private List<List<double>> frameDifferences;


        /**
         * Convenienve list containing the means of the entire lie result interval.
         */
        private List<double> means;
        /**
         * The number of elements the means are computed over.
         */
        private int meancount = 0;

        public LieResult(int framestart = 0)
        {
            this.means = new List<double>();
            this.frameDifferences = new List<List<double>>();
            this.framestart = framestart;
            this.framend = framestart - 1;
        }        

        public static LieResult Empty
        {
            get { return new LieResult(); }
        }

        public void AddFrameDiff(List<double> diff, int next)
        {            
            framend = next;
            if (diff != null) {
                if (means.Count == 0)
                {
                    means = diff;
                    meancount = 1;
                }
                else
                {
                    frameDifferences.Add(diff);
                    for (int i = 0; i < means.Count; i++)
                    {
                        double mcount = (double)meancount;
                        means[i] *= mcount / (mcount + 1);
                        means[i] += diff[i] / (mcount + 1);
                    }
                    meancount++;
                }
            }
        }

        public int NextFrameId { get { return framend + 1; } }

        public int Start { get { return framestart; } }

        public int End { get { return framend; } }

        public List<double> Means
        {
            get { return means; }
        }

        public List<double> ComputeAbsoluteMovements(int start, int end)
        {
            if (frameDifferences.Count == 0 || start >= end)
                return null;

            // First convert the start and end to (downsampled) index.
            int m = (framend - framestart) / frameDifferences.Count;
            int startIndex = (start - framestart) / m;
            int endIndex = Math.Min((end - framestart) / m, frameDifferences.Count - 1);
            if (startIndex >= endIndex)
                return null;

            // Sum all the framedifferences between start and end.
            List<double> result = new List<double>();
            for (int i = startIndex; i < endIndex; i++)
            {
                for (int j = 0; j < frameDifferences[i].Count; j++)
                {
                    if(i == startIndex) result.Add(0);
                    result[j] += frameDifferences[i][j];
                }
            }

            // Compute the mean and multiply by 500
            for(int i = 0; i < result.Count; i++)
            {
                result[i] /= endIndex - startIndex;
                result[i] *= 500;
            }

            return result;
        }

        public List<double> FrameDifference(int id)
        {
            if (id < framestart || id > framend)
                throw new Exception("Id out of range of the Lieresult, must be greater than Framestart and smaller or equal to Framend");

            //TODO: make finding right framedifference for given frameid better
            if (frameDifferences.Count > 0)
            {
                double m = (double)(framend - framestart) / (frameDifferences.Count - 1);
                return frameDifferences[(int)((double)(id - framestart) / m)];
            }
            else
            {
                return null;
            }
        }

        public IList<List<double>> FrameDifferences
        {
            get { return frameDifferences.AsReadOnly(); }
        }

        public List<double> this[int frameid] { get { return FrameDifference(frameid); } }
    }
}
