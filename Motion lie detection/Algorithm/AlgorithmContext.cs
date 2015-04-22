using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
    /**
     * The context in which the algorithm is performed.
     * This context contains all the information needed.
     */
    public class AlgorithmContext
    {
        /**
         * The last frame that is computed. This frame is stored to compute the delta values.
         * Note: this frame is from after the filter passes.
         */
        Frame lastFrame;
        
        /**
         * Special frame that can be used to down-sample and group 
         */
        Frame sampleFrame;
        /**
         * The size of the sample.
         * Generally this will be the number of frames the sameplFrame represents.
         */
        int samplesize;

        /**
         * Special body configuration that represents the 'normal' configuration for a certain classification.
         */
        BodyConfiguration normalizeconfiguration;
        /**
         * List of nodes that should be considered as root nodes when performing the algorithm computations.
         * This can be used for performing the algorithm on sub-trees of the body tree.
         */
        BodyNode[] rootnodes;

        /**
         * Constructor that sets the initial values of the AlgorithmContext.
         * @param ClassConfiguraiton Optional classification configuration.
         */
        public AlgorithmContext(ClassificationConfiguration ClassConfiguration = null)
        {
            lastFrame = Frame.Empty;
            sampleFrame = Frame.Empty;
            samplesize = 0;

            // Check if a classification configuration is given.
            // Note: this configuration will be used to determine what has to be computed by the algorithm for classification.
            if (ClassConfiguration != null)
            {
                normalizeconfiguration = ClassConfiguration.NormalBodyconfiguration;
                rootnodes = ClassConfiguration.Rootnodes;
            }
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

        public BodyNode[] RootNodes
        {
            get { return rootnodes; }
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
