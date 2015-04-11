using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    public class ClassificationConfiguration
    {
        private BodyConfiguration normalisationconfiguration;
        private BodyNode[] rootnodes;
        private Dictionary<BodyNode, int> partindex;
        private float[][] rootparam, jointparam;

        public ClassificationConfiguration(BodyConfiguration config, BodyNode[] rootnodes, float[][] rootparam, float[][] jointparam = null)
        {   
            //Check if length match
            if (rootnodes.Length != rootparam.Length)
                throw new Exception("The number of rootnodes and the number of classification paramteres does not match");
           
            this.normalisationconfiguration = config;
            this.rootnodes = rootnodes;
            this.rootparam = rootparam;

            //make dictionary to make indexing possible for bodynodes
            this.partindex = new Dictionary<BodyNode, int>();
            for (int j = 0; j < rootnodes.Length; j++)
            {
                partindex.Add(rootnodes[j], j);
            }
            
            //Add jointparam if they are given
            if (jointparam != null)
            {
                this.jointparam = new float[config.Size + 1][];
                for (int i = 0; i < jointparam.Length + 1; i++)
                {
                    this.jointparam[i] = jointparam[i];
                }
            }
        }

        public BodyNode[] Rootnodes
        {
            get { return rootnodes; }
        }

        public float[] GetPartParam(BodyNode node)
        {
            int i;
            if (partindex.TryGetValue(node, out i))
                return rootparam[i];
            else
                return null;
        }

        public float[] this[int index]
        {
            get
            {
                if (index <= normalisationconfiguration.Size)
                    return (jointparam != null) ? jointparam[index] : null;
                else
                {
                    index -= (normalisationconfiguration.Size + 1);
                    if (index < rootnodes.Length)
                        return rootparam[index];
                    else
                        throw new Exception("Index out of range");
                }

            }
        }

        public BodyConfiguration NormalBodyconfiguration
        {
            get { return normalisationconfiguration; }
        }
    }

    /**
     * Fixed classification model that can be used in combination with XSens hard/software.
     */
    public class FixedClassification {

        public static ClassificationConfiguration Create(){
            // Create a fixed body configuration as basis.
            BodyConfiguration bodyConfiguration = new FixedBodyConfiguration();
            BodyNode[] rootNodes = new BodyNode[6];
            String[] names = new String[] { "Pelvis", "LeftShoulder", "RightShoulder", "LeftUpperLeg", "RightUpperLeg", "T8"};

            // BFS over the body configuration in search of the root nodes.
            Queue<BodyNode> queue = new Queue<BodyNode>();
            queue.Enqueue(bodyConfiguration.getRoot());
            while (queue.Count > 0)
            {
                BodyNode current = queue.Dequeue();
                foreach (BodyNode v in current.getNeighbours())
                    queue.Enqueue(v);

                // Check if the node is one of the requested root nodes.
                int index = Array.IndexOf(names, current.getName());
                if (index != -1)
                    rootNodes[index] = current;
            }

            float[][] param = new float[6][];
            param[0] = new float[] { 123.1333f, 50001.310f, 235.8511f, 14329.630f };
            param[1] = new float[] { 47.3356f, 1000.482f, 116.904444f, 6314.596798f };
            param[2] = new float[] { 73.9956f, 3295.712f, 137.3156f, 6601.916f};
            param[3] = new float[] { 14.3289f, 81.369f, 18.7489f, 411.240f};
            param[4] = new float[] { 15.6422f, 128.037f, 22.2667f, 987.294f};
            param[5] = new float[] { 15.0489f, 50.747f, 19.3667f, 54.922f};

            return new ClassificationConfiguration(new FixedBodyConfiguration(), rootNodes, param);
        }
    }
}
