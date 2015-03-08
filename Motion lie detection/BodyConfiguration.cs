using System;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	/**
	 * Enumeration containing the body parts.
	 */
	public enum BodyPart
	{
		HEAD,
		STERN,

		LEFT_SHOULDER,
		LEFT_UPPER_ARM,
		LEFT_LOWER_ARM,
		LEFT_HAND,

		RIGHT_SHOULDER,
		RIGHT_UPPER_ARM,
		RIGHT_LOWER_ARM,
		RIGHT_HAND,

		PELVIS,

		LEFT_UPPER_LEG,
		LEFT_KNEE,
		LEFT_LOWER_LEG,
		LEFT_FOOT,

		RIGHT_UPPER_LEG,
		RIGHT_KNEE,
		RIGHT_LOWER_LEG,
		RIGHT_FOOT

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

		public Dictionary<BodyPart, int> getMapping()
		{
			return mapping;
		}

		public int getJointFor(BodyPart part)
		{
			int jointID;
			if (!mapping.TryGetValue (part, out jointID))
				return -1;
			return jointID;
		}
	}

	/**
	 * Simple body configuration that provides a static mapping and configuration of the body.
	 */
	public class FixedBodyConfiguration : BodyConfiguration
	{

		public FixedBodyConfiguration() : base()
		{
			mapping.Add (BodyPart.HEAD, 7);
			mapping.Add (BodyPart.LEFT_KNEE, 20);
			mapping.Add (BodyPart.LEFT_FOOT, 21);
			//mapping.Add (BodyPart.LEFT_SHOULDER, 9);
			//mapping.Add (BodyPart.STERN, 8);
		}

	}
}

