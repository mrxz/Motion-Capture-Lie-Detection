using System;
using System.Collections.Generic;

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
		protected readonly Dictionary<BodyPart, int> mapping;

		public BodyConfiguration()
		{
			mapping = new Dictionary<BodyPart, int> ();
		}

		public Dictionary<BodyPart, int> GetMapping()
		{
			return mapping;
		}

		public void AddMapping(BodyPart part, int id)
		{
			mapping [part] = id;
		}

		public int GetJointFor(BodyPart part)
		{
			int jointID;
			if (!mapping.TryGetValue (part, out jointID))
				return -1;
			return jointID;
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
			// The mapping between body part and segment id.
			// Source: http://issuu.com/xsensmvn/docs/mvn_studio_real-time_network_stream/16?e=14522406/10381348
			mapping.Add (BodyPart.PELVIS, 1);
			mapping.Add (BodyPart.L5, 2);
			mapping.Add (BodyPart.L3, 3);
			mapping.Add (BodyPart.T12, 4);
			mapping.Add (BodyPart.T8, 5);
			mapping.Add (BodyPart.NECK, 6);
			mapping.Add (BodyPart.HEAD, 7);

			mapping.Add (BodyPart.RIGHT_SHOULDER, 8);
			mapping.Add (BodyPart.RIGHT_UPPER_ARM, 9);
			mapping.Add (BodyPart.RIGHT_FORE_ARM, 10);
			mapping.Add (BodyPart.RIGHT_HAND, 11);

			mapping.Add (BodyPart.LEFT_SHOULDER, 12);
			mapping.Add (BodyPart.LEFT_UPPER_ARM, 13);
			mapping.Add (BodyPart.LEFT_FORE_ARM, 14);
			mapping.Add (BodyPart.LEFT_HAND, 15);

			mapping.Add (BodyPart.RIGHT_UPPER_LEG, 16);
			mapping.Add (BodyPart.RIGHT_LOWER_LEG, 17);
			mapping.Add (BodyPart.RIGHT_FOOT, 18);
			mapping.Add (BodyPart.RIGHT_TOE, 19);

			mapping.Add (BodyPart.LEFT_UPPER_LEG, 20);
			mapping.Add (BodyPart.LEFT_LOWER_LEG, 21);
			mapping.Add (BodyPart.LEFT_FOOT, 22);
			mapping.Add (BodyPart.LEFT_TOE, 23);
		}

	}
}

