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
        private float[] means;
        private float meancount = 0;

        public LieResult(int framestart = 0)
        {
            this.means = new float[8];
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
                frameDifferences.Add(diff);
                for (int i = 0; i < 8; i++)
                {
                    means[i] *= meancount / (meancount + 1);
                    means[i] += diff[i] / (meancount + 1);
                }
                meancount++;
            }
        }

        public int NextFrameId { get { return framend + 1; } }

        public int Start { get { return framestart; } }

        public int End { get { return framend; } }

        public float[] Means
        {
            get { return means; }
        }

        public List<float> FrameDifference(int id)
        {
            if (id <= framestart || id > framend)
                throw new Exception("Id out of range of the Lieresult, must be greater than Framestart and smaller or equal to Framend");
            //TODO: make finding right framedifference for given frameid better
            int m = (framend - framestart - 1) / frameDifferences.Count;
            return frameDifferences[((id - framestart) / m) - 1];
        }

        public List<float> this[int frameid] { get { return FrameDifference(frameid); } }
    }
}
