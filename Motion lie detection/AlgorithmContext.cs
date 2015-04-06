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
        BodyConfiguration normalizeconfiguration;

        public AlgorithmContext(BodyConfiguration baseConfiguration = null)
        {
            lastFrame = Frame.Empty;
            sampleFrame = Frame.Empty;
            samplesize = 0;
            normalizeconfiguration = baseConfiguration;
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

        public BodyConfiguration Normalizeconfiguration
        {
            get { return normalizeconfiguration; }
            set { normalizeconfiguration = value; }
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
