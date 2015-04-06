using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
	/**
	 * Class representing a configuration of a body.
	 * This includes the mapping between the joints and jointIDs as well as the lengths of the body parts.
	 */
	public class BodyConfiguration
	{
		/**
		 * The bodyTree.
		 */
		protected BodyNode root;
		/**
		 * The bodyNode that can be used to determine the orientation.
		 */
		protected BodyNode orientationNode;

		/**
		 * Map containing the lengths between two bodyNodes.
		 */
		protected Dictionary<Tuple<BodyNode, BodyNode>, float> lengths;

		public BodyConfiguration()
		{
			root = null;
			orientationNode = null;
			lengths = new Dictionary<Tuple<BodyNode, BodyNode>, float> ();
		}

		public BodyNode getRoot()
		{
			return root;
		}

		public BodyNode getOrientationNode()
		{
			return orientationNode;
		}

		public float GetLength(BodyNode from, BodyNode to)
		{
			float result;
			if(lengths.TryGetValue(Tuple.Create(from, to), out result))
				return result;
			return -1;
		}

		public void SetLength(BodyNode from, BodyNode to, float length)
		{
			Tuple<BodyNode, BodyNode> key = Tuple.Create (from, to);
			lengths.Add (key, length);
		}

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

		public Joint getRootJoint(IList<Joint> joints) {
			return getJoint (joints, root);
		}

		public Joint getOrientationJoint(IList<Joint> joints) {
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

					float length = Vector3.Distance (first.Position, second.Position);
					lengths.Add (key, length);

					q.Enqueue (neighbour);
				}
			}
		}
	}

	/**
	 * A node in the body tree.
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
			adjacent = new List<BodyNode> ();
			this.jointId = jointId;
			this.name = name;
		}

		public List<BodyNode> getNeighbours() {
			return adjacent;
		}

		public void addNeighbour(BodyNode node) {
			adjacent.Add (node);
		}

		public BodyNode getRoot() {
			return root != null ? root : this;
		}

		public void setRoot(BodyNode node) {
			this.root = node;
		}

        public int JointId { get { return jointId; } }

		public String getName() {
			return name;
		}

	}

	/**
	 * Simple body configuration that provides a static mapping and configuration of the body.
	 * This mapping is based on the default segment layout of XSens hardware.
	 */
	public class FixedBodyConfiguration : BodyConfiguration
	{

		public FixedBodyConfiguration() : base()
		{
			// Spine
			BodyNode pelvis = new BodyNode (1, "Pelvis");
			BodyNode l5 = new BodyNode (2, "L5");
			BodyNode l3 = new BodyNode (3, "L3");
			BodyNode t12 = new BodyNode (4, "T12");
			BodyNode t8 = new BodyNode (5, "T8");
			BodyNode neck = new BodyNode (6, "Neck");
			BodyNode head = new BodyNode (7, "Head");
			l5.setRoot (pelvis);
			l3.setRoot (pelvis);
			t12.setRoot (pelvis);
			t8.setRoot (pelvis);
			neck.setRoot (pelvis);
			head.setRoot (neck);

			pelvis.addNeighbour (l5);
			l5.addNeighbour (l3);
			l3.addNeighbour (t12);
			t12.addNeighbour (t8);
			t8.addNeighbour (neck);
			neck.addNeighbour (head);


			// Left leg.
			{
				BodyNode lT = new BodyNode (23, "LeftToe");
				BodyNode lF = new BodyNode (22, "LeftFoot");
				BodyNode lL = new BodyNode (21, "LeftLowerLeg");
				BodyNode lU = new BodyNode (20, "LeftUpperLeg");
				lT.setRoot (lU);
				lF.setRoot (lU);
				lL.setRoot (lU);
				lU.setRoot (pelvis);

				lF.addNeighbour (lT);
				lL.addNeighbour (lF);
				lU.addNeighbour (lL);
				pelvis.addNeighbour (lU);
			}

			// Right leg.
			{
				BodyNode rT = new BodyNode (19, "RightToe");
				BodyNode rF = new BodyNode (18, "RightFoot");
				BodyNode rL = new BodyNode (17, "RightLowerLeg");
				BodyNode rU = new BodyNode (16, "RightUpperLeg");
				rT.setRoot (rU);
				rF.setRoot (rU);
				rL.setRoot (rU);
				rU.setRoot (pelvis);

				rF.addNeighbour (rT);
				rL.addNeighbour (rF);
				rU.addNeighbour (rL);
				pelvis.addNeighbour (rU);

				// Also use the right uppper leg as orientation node.
				orientationNode = rU;
			}

			// Left arm
			{
				BodyNode lH = new BodyNode (15, "LeftHand");
				BodyNode lF = new BodyNode (14, "LeftForeArm");
				BodyNode lU = new BodyNode (13, "LeftUpperArm");
				BodyNode lS = new BodyNode (12, "LeftShoulder");
				lH.setRoot (lS);
				lF.setRoot (lS);
				lU.setRoot (lS);
				lS.setRoot (t8);

				lF.addNeighbour (lH);
				lU.addNeighbour (lF);
				lS.addNeighbour (lU);
				t8.addNeighbour (lS);
			}

			// Right arm
			{
				BodyNode rH = new BodyNode (11, "RightHand");
				BodyNode rF = new BodyNode (10, "RightForeArm");
				BodyNode rU = new BodyNode (9, "RightUpperArm");
				BodyNode rS = new BodyNode (8, "RightShoulder");
				rH.setRoot (rS);
				rF.setRoot (rS);
				rU.setRoot (rS);
				rS.setRoot (t8);

				rF.addNeighbour (rH);
				rU.addNeighbour (rF);
				rS.addNeighbour (rU);
				t8.addNeighbour (rS);
			}

			// Make the pelvis the root of the body.
			this.root = pelvis;
		}

	}
}

