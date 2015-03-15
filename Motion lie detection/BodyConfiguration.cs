using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
	/**
	 * Enumeration containing the body parts.
	 */
	public enum BodyPart
	{
		// From the Pelvis up to the Head.
		PELVIS,
		L5,
		L3,
		T12,
		T8,
		NECK,
		HEAD,

		// Right arm
		RIGHT_SHOULDER,
		RIGHT_UPPER_ARM,
		RIGHT_FORE_ARM,
		RIGHT_HAND,

		// Left arm
		LEFT_SHOULDER,
		LEFT_UPPER_ARM,
		LEFT_FORE_ARM,
		LEFT_HAND,

		// Right leg
		RIGHT_UPPER_LEG,
		RIGHT_LOWER_LEG,
		RIGHT_FOOT,
		RIGHT_TOE,

		// Left leg
		LEFT_UPPER_LEG,
		LEFT_LOWER_LEG,
		LEFT_FOOT,
		LEFT_TOE
	}

	/**
	 * Class representing a configuration of a body.
	 * This includes the mapping between the joints and jointIDs as well as the lengths of the body parts.
	 */
	public class BodyConfiguration
	{
		/**
		 * The mapping between BodyPart and joint id.
		 */
		protected readonly Dictionary<BodyPart, int> mapping;
		/**
		 * The different connections that exist in the body.
		 */
		protected readonly List<Tuple<BodyPart, BodyPart>> connections;
		/**
		 * Mapping between connections and their length (float in meters).
		 */
		protected readonly Dictionary<Tuple<BodyPart, BodyPart>, float> lengths;

		public BodyConfiguration()
		{
			mapping = new Dictionary<BodyPart, int> ();
			connections = new List<Tuple<BodyPart, BodyPart>> ();
			lengths = new Dictionary<Tuple<BodyPart, BodyPart>, float> ();
		}

		public Dictionary<BodyPart, int> GetMapping()
		{
			return mapping;
		}

		public void AddMapping(BodyPart part, int id)
		{
			mapping [part] = id;
		}

		public List<Tuple<BodyPart, BodyPart>> GetConnections()
		{
			return connections;
		}

		public void AddConnection(BodyPart from, BodyPart to)
		{
			connections.Add (Tuple.Create (from, to));
		}

		public float GetLength(BodyPart from, BodyPart to)
		{
			float result;
			if (!lengths.TryGetValue (Tuple.Create (from, to), out result))
				return -1;
			return result;
		}

		public void SetLength(BodyPart from, BodyPart to, float length)
		{
			// FIXME: Make sure connection exists perhaps?

			lengths.Add (Tuple.Create (from, to), length);
		}

		public int GetJointFor(BodyPart part)
		{
			int jointID;
			if (!mapping.TryGetValue (part, out jointID))
				return -1;
			return jointID;
		}

		/**
		 * Method for determining the connection lengths from an n-pose.
		 * Note: this requires the connections to be already present.
		 * @param nposeFrame Frame of the person in an n-pose.
		 */
		public void LengthsFromNPose(Frame nposeFrame) 
		{
			// 
			foreach(Tuple<BodyPart, BodyPart> connection in connections) {
				// FIXME: At the moment we utilse the fact that the jointId -1 is the index in the Joints array.
				Joint first = nposeFrame.Joints[mapping[connection.Item1] - 1];
				Joint second = nposeFrame.Joints[mapping[connection.Item2] - 1];

				float distance = Vector3.Distance (first.Position, second.Position);
				lengths.Add (connection, distance);
			}
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
			PopulateMapping (this);
			PopulateConnections (this);
		}


		public static void PopulateMapping(BodyConfiguration configuration) {
			// The mapping between body part and segment id.
			// Source: http://issuu.com/xsensmvn/docs/mvn_studio_real-time_network_stream/16?e=14522406/10381348
			configuration.AddMapping(BodyPart.PELVIS, 1);
			configuration.AddMapping (BodyPart.L5, 2);
			configuration.AddMapping (BodyPart.L3, 3);
			configuration.AddMapping (BodyPart.T12, 4);
			configuration.AddMapping (BodyPart.T8, 5);
			configuration.AddMapping (BodyPart.NECK, 6);
			configuration.AddMapping (BodyPart.HEAD, 7);

			configuration.AddMapping (BodyPart.RIGHT_SHOULDER, 8);
			configuration.AddMapping (BodyPart.RIGHT_UPPER_ARM, 9);
			configuration.AddMapping (BodyPart.RIGHT_FORE_ARM, 10);
			configuration.AddMapping (BodyPart.RIGHT_HAND, 11);

			configuration.AddMapping (BodyPart.LEFT_SHOULDER, 12);
			configuration.AddMapping (BodyPart.LEFT_UPPER_ARM, 13);
			configuration.AddMapping (BodyPart.LEFT_FORE_ARM, 14);
			configuration.AddMapping (BodyPart.LEFT_HAND, 15);

			configuration.AddMapping (BodyPart.RIGHT_UPPER_LEG, 16);
			configuration.AddMapping (BodyPart.RIGHT_LOWER_LEG, 17);
			configuration.AddMapping (BodyPart.RIGHT_FOOT, 18);
			configuration.AddMapping (BodyPart.RIGHT_TOE, 19);

			configuration.AddMapping (BodyPart.LEFT_UPPER_LEG, 20);
			configuration.AddMapping (BodyPart.LEFT_LOWER_LEG, 21);
			configuration.AddMapping (BodyPart.LEFT_FOOT, 22);
			configuration.AddMapping (BodyPart.LEFT_TOE, 23);
		}

		public static void PopulateConnections(BodyConfiguration configuration) {
			// Add the connections.
			// SPINE
			configuration.AddConnection(BodyPart.PELVIS, BodyPart.L5);
			configuration.AddConnection(BodyPart.L5, BodyPart.L3);
			configuration.AddConnection(BodyPart.L3, BodyPart.T12);
			configuration.AddConnection(BodyPart.T12, BodyPart.NECK);
			configuration.AddConnection(BodyPart.NECK, BodyPart.HEAD);

			// RIGHT-ARM
			configuration.AddConnection(BodyPart.RIGHT_SHOULDER, BodyPart.RIGHT_UPPER_ARM);
			configuration.AddConnection(BodyPart.RIGHT_UPPER_ARM, BodyPart.RIGHT_FORE_ARM);
			configuration.AddConnection(BodyPart.RIGHT_FORE_ARM, BodyPart.RIGHT_HAND);

			// LEFT-ARM
			configuration.AddConnection(BodyPart.LEFT_SHOULDER, BodyPart.LEFT_UPPER_ARM);
			configuration.AddConnection(BodyPart.LEFT_UPPER_ARM, BodyPart.LEFT_FORE_ARM);
			configuration.AddConnection(BodyPart.LEFT_FORE_ARM, BodyPart.LEFT_HAND);

			// RIGHT-LEG
			configuration.AddConnection(BodyPart.RIGHT_UPPER_LEG, BodyPart.RIGHT_LOWER_LEG);
			configuration.AddConnection(BodyPart.RIGHT_LOWER_LEG, BodyPart.RIGHT_FOOT);
			configuration.AddConnection(BodyPart.RIGHT_FOOT, BodyPart.RIGHT_TOE);

			// LEFT-LEG
			configuration.AddConnection(BodyPart.LEFT_UPPER_LEG, BodyPart.LEFT_LOWER_LEG);
			configuration.AddConnection(BodyPart.LEFT_LOWER_LEG, BodyPart.LEFT_FOOT);
			configuration.AddConnection(BodyPart.LEFT_FOOT, BodyPart.LEFT_TOE);
		}
	}
}

