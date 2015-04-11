using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
    public class LieResult
    {
        private int framestart;
        private int framend;
        private List<List<float>> frameDifferences;
        private List<float> means;
        private float meancount = 0;

        public LieResult(int framestart = 0)
        {
            this.means = new List<float>();
            this.frameDifferences = new List<List<float>>();
            this.framestart = framestart;
            this.framend = framestart - 1;
        }        

        public static LieResult Empty
        {
            get { return new LieResult(); }
        }

        public void AddFrameDiff(List<float> diff, int next)
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
                        means[i] *= meancount / (meancount + 1);
                        means[i] += diff[i] / (meancount + 1);
                    }
                    meancount++;
                }
            }
        }

        public int NextFrameId { get { return framend + 1; } }

        public int Start { get { return framestart; } }

        public int End { get { return framend; } }

        public List<float> Means
        {
            get { return means; }
        }

        public List<float> ComputeAbsoluteMovements(int start, int end)
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
            List<float> result = new List<float>();
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
                result[i] /= frameDifferences.Count;
                result[i] *= 500;
            }

            return result;
        }

        public List<float> FrameDifference(int id)
        {
            if (id < framestart || id > framend)
                throw new Exception("Id out of range of the Lieresult, must be greater than Framestart and smaller or equal to Framend");

            //TODO: make finding right framedifference for given frameid better
            int m = (framend - framestart) / frameDifferences.Count;
            int index = Math.Min((id - framestart) / m, frameDifferences.Count - 1);
            return frameDifferences[index];
        }

        public IList<List<float>> FrameDifferences
        {
            get { return frameDifferences.AsReadOnly(); }
        }

        public List<float> this[int frameid] { get { return FrameDifference(frameid); } }
    }
}
