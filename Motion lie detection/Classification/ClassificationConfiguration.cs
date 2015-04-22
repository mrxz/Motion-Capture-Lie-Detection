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
        private double[][] rootparam, jointparam;
        private double uptreshold, lowtreshold, factor, min, max;

        public ClassificationConfiguration(BodyConfiguration config, BodyNode[] rootnodes, double[][] rootparam, double uptreshold = 0.5, double lowtreshold = 0.5, double factor = 1, double min = 0, double max = 1, double[][] jointparam = null)
        {   
            //Check if length match
            if (rootnodes.Length != rootparam.Length)
                throw new Exception("The number of rootnodes and the number of classification paramteres does not match");
           
            this.normalisationconfiguration = config;
            this.rootnodes = rootnodes;
            this.rootparam = rootparam;
            this.uptreshold = uptreshold;
            this.lowtreshold = lowtreshold;
            this.factor = factor;
            this.min = min;
            this.max = max;
            //make dictionary to make indexing possible for bodynodes
            this.partindex = new Dictionary<BodyNode, int>();
            for (int j = 0; j < rootnodes.Length; j++)
            {
                partindex.Add(rootnodes[j], j);
            }
            
            //Add jointparam if they are given
            if (jointparam != null)
            {
                this.jointparam = new double[config.Size + 1][];
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

        public double[] GetPartParam(BodyNode node)
        {
            int i;
            if (partindex.TryGetValue(node, out i))
                return rootparam[i];
            else
                return null;
        }

        public double[] this[int index]
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

        public double UpTreshold { get { return uptreshold; } }

        public double LowTreshold { get { return lowtreshold; } }

        public double Factor { get { return factor; } }

        public double MinAccept { get { return min; } }

        public double MaxAccept { get { return max; } }
    }    
}
