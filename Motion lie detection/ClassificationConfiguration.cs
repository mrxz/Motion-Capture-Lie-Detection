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
        private float uptreshold, lowtreshold;
        public ClassificationConfiguration(BodyConfiguration config, BodyNode[] rootnodes, float[][] rootparam,float uptreshold = 0.5f, float lowtreshold = 0.5f, float[][] jointparam = null)
        {   
            //Check if length match
            if (rootnodes.Length != rootparam.Length)
                throw new Exception("The number of rootnodes and the number of classification paramteres does not match");
           
            this.normalisationconfiguration = config;
            this.rootnodes = rootnodes;
            this.rootparam = rootparam;
            this.uptreshold = uptreshold;
            this.lowtreshold = lowtreshold;
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

        public float UpTreshold { get { return uptreshold; } }

        public float LowTreshold { get { return lowtreshold; } }
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
            param[0] = new float[] { 4.0819881f, 5.488f, 4.2840368f, 4.031f};
            param[1] = new float[] { 1.7914401f, 1.388f, 1.6371702f, 0.729f};
            param[2] = new float[] { 1.6843104f, 1.099f, 1.6997891f, 0.973f};
            param[3] = new float[] { 0.7801710f, 0.478f, 1.0049744f, 0.538f };
            param[4] = new float[] { 0.8432133f, 1.026f, 1.1260909f, 1.210f};
            param[5] = new float[] { 0.6695878f, 0.099f, 0.6819028f, 0.068f};

            return new ClassificationConfiguration(new FixedBodyConfiguration(), rootNodes, param);
        }
    }
}
