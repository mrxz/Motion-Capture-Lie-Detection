using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	/**
	 * Class representing a configuration of a body.
	 * This includes the mapping between the joints and jointIDs as well as the lengths of the body parts.
	 */
	public class BodyConfiguration
	{
		/**
		 * The root node of the bodyTree.
		 */
		protected BodyNode root;
		/**
		 * The bodyNode that can be used to determine the orientation of the body.
		 */
		protected BodyNode orientationNode;

		/**
		 * Map containing the lengths between two bodyNodes.
		 */
		protected Dictionary<Tuple<BodyNode, BodyNode>, double> lengths;

		public BodyConfiguration(BodyNode rootnode = null)
		{
            root = rootnode;
			orientationNode = rootnode;
			lengths = new Dictionary<Tuple<BodyNode, BodyNode>, double> ();
		}

		public BodyNode getRoot()
		{
			return root;
		}

		public BodyNode getOrientationNode()
		{
			return orientationNode;
		}

        /**
         * Method for getting the length between two body nodes.
         * Note: there's no guarantee that from-to returns the same as to-from.
         * @param from The first BodyNode
         * @param to The second BodyNode.
         * @return The length of the segment betwen the bodynodes, -1 if no such connection (or length) is present.
         */
		public double GetLength(BodyNode from, BodyNode to)
		{
			double result;
			if(lengths.TryGetValue(Tuple.Create(from, to), out result))
				return result;
			return -1;
		}

        /**
         * Method for setting the lenght for a given segment between BodyNodes.
         * @param from The first BodyNode.
         * @param to The second BodyNode.
         * @param length The new length to set.
         */
		public void SetLength(BodyNode from, BodyNode to, float length)
		{
			Tuple<BodyNode, BodyNode> key = Tuple.Create (from, to);
			lengths.Add (key, length);
		}

        /**
         * Method for getting the joint from a given list of joints corresponding to the requested BodyNode.
         * @param joints The list of joints to return the joint from.
         * @param node The node to return the joint for.
         * @return The corresponding Joint if found, an empyt Joint otherwise.
         */
		public Joint getJoint(IList<Joint> joints, BodyNode node) {
			// TODO: Add alternative in case the joint order is known.
			// Loop over the joints to find the correct one.
			foreach (Joint joint in joints) {
				if (joint.Id == node.JointId)
					return joint;
			}

			// Not found.
			return new Joint ();
		}

        /**
         * Convenience method for getting the root joint from a list of joints.
         * @param joints The list of joints to return the joint from.
         * @return The root Joint if found, an empty Joint otherwise.
         */
		public Joint getRootJoint(IList<Joint> joints) {
			return getJoint (joints, root);
		}

        /**
         * Convenience method for getting the orientation joint from a list of joints.
         * @param joints The list of joints to return the joint from.
         * @return The orientation Joint if found, an empty Joint otherwise.
         */
        public Joint getOrientationJoint(IList<Joint> joints)
        {
			return getJoint (joints, orientationNode);
		}

		/**
		 * Method for determining the connection lengths from an n-pose.
		 * Note: this requires the connections to be already present.
		 * @param nposeFrame Frame of the person in an n-pose.
		 */
		public void LengthsFromNPose(Frame nposeFrame) 
		{
			Queue<BodyNode> q = new Queue<BodyNode> ();
			q.Enqueue (root);
			while (q.Count > 0) {
				BodyNode node = q.Dequeue ();

				foreach (BodyNode neighbour in node.getNeighbours()) { 
					Tuple<BodyNode, BodyNode> key = Tuple.Create (node, neighbour);
					Joint first = getJoint (nposeFrame.Joints, node);
					Joint second = getJoint (nposeFrame.Joints, neighbour);

					double length = Vector3d.Distance (first.Position, second.Position);
					lengths.Add (key, length);

					q.Enqueue (neighbour);
				}
			}
		}

        /**
         * Returns the size of the body tree.
         * The size is the number of nodes in the tree.
         */
        public int Size {
            get { return (root != null) ? root.Size : 0; }
        }
	}

	/**
	 * A node in the body tree.
     * Note: BodyNode should not be intermixed between different instances of a BodyConfiguration.
	 */
    public class BodyNode
    {
        /**
         * The bodyNode this node should be normalized against.
         */
        private BodyNode root;

        /**
         * List containing the adjacent body nodes. 
         */
        private List<BodyNode> adjacent;

        /**
         * The joint id that corresponds with this bodypart.
         */
        private int jointId;
        /**
         * The name of the bodyNode.
         */
        private String name;

        public BodyNode(int jointId, String name)
        {
            root = null;
            adjacent = new List<BodyNode>();
            this.jointId = jointId;
            this.name = name;
        }

        public List<BodyNode> getNeighbours()
        {
            return adjacent;
        }

        public void addNeighbour(BodyNode node)
        {
            adjacent.Add(node);
        }

        public BodyNode getRoot()
        {
            return root != null ? root : this;
        }

        public void setRoot(BodyNode node)
        {
            this.root = node;
        }

        public int JointId { get { return jointId; } }

        public String getName()
        {
            return name;
        }

        /**
         * Returns the size of the body tree from this node onwards.
         * The size is the number of nodes in the sub-tree.
         */
        public int Size
        {
            get
            {
                // Start with one, the node itself.
                int res = 1;
                foreach (BodyNode node in adjacent)
                    res += node.Size;
                return res;
            }
        }
    }	
}

