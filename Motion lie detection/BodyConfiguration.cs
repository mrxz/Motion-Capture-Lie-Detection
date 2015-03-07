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
		LEFT_LOWER_LEG,
		LEFT_FOOT,

		RIGHT_UPPER_LEG,
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
	}

	/**
	 * Simple body configuration that provides a static mapping and configuration of the body.
	 */
	public class FixedBodyConfiguration : BodyConfiguration
	{

		public FixedBodyConfiguration() : base()
		{
			mapping.Add (BodyPart.HEAD, 1);
			//mapping.Add (BodyPart.LEFT_SHOULDER, 2);
		}

	}
}

