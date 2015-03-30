using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    public class AlgorithmContext
    {
        Frame lastFrame;
        Frame sampleFrame;
        int samplesize;

        public AlgorithmContext()
        {
            lastFrame = Frame.Empty;
            sampleFrame = Frame.Empty;
            samplesize = 0;
        }

        public Frame LastFrame
        {
            get { return lastFrame; }
            set { lastFrame = value; }
        }

        public int SampleSize
        {
            get { return samplesize; }
        }

        public Frame SampleFrame
        {
            get { return sampleFrame; }
        }

        public void SetSample(Frame frame, int framecount)
        {
            sampleFrame = frame;
            samplesize = framecount;
        }

        public void FlushSample()
        {
            sampleFrame = Frame.Empty;
            samplesize = 0;
        }
    }
}
