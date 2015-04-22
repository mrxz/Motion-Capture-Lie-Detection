using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    /**
     * Fixed classification model that can be used in combination with XSens hard/software.
     */
    public class FixedClassification
    {
        public static ClassificationConfiguration Create()
        {
            // Create a fixed body configuration as basis.
            BodyConfiguration bodyConfiguration = new FixedBodyConfiguration();
            BodyNode[] rootNodes = new BodyNode[6];
            String[] names = new String[] { "Pelvis", "LeftShoulder", "RightShoulder", "LeftUpperLeg", "RightUpperLeg", "T8" };

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

            double[][] param = new double[6][];
            param[0] = new double[] { 4.0819881, 5.488, 4.2840368, 4.031 };
            param[1] = new double[] { 1.7914401, 1.388, 1.6371702, 0.729 };
            param[2] = new double[] { 1.6843104, 1.099, 1.6997891, 0.973 };
            param[3] = new double[] { 0.7801710, 0.478, 1.0049744, 0.538 };
            param[4] = new double[] { 0.8432133, 1.026, 1.1260909, 1.210 };
            param[5] = new double[] { 0.6695878, 0.099, 0.6819028, 0.068 };

            return new ClassificationConfiguration(new FixedBodyConfiguration(), rootNodes, param, 0.5, 0.5, 500);
        }
    }
}
