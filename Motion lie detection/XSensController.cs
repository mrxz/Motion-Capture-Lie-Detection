using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
	public class XSensController : SuitController, XSensEventHandler
	{
		/**
		 * The XSensListener used to receive packages.
		 */
		private readonly XSensListener listener;

		public XSensController () : this("localhost", 9763)
		{ }

		public XSensController (String host) : this (host, 9763)
		{}

		public XSensController(String host, int port)
		{
			listener = new XSensListener (host, port);
			listener.AddEventHandler (this);
		}

		public override bool Connect ()
		{
			return listener.Start ();
		}

		public override bool Disconnect ()
		{
			return listener.Stop ();
		}

		public override bool Calibrate ()
		{
			// Note: in the case of the network stream the calibrating is done by the MVN Studio.
			// TODO: Perhaps use this to determine the framerate?
			// Ideally we want to receive an MXTP13 packages containing the body configuration (but how???).
			return true;
		}

		public void onMXTP02(Header header, List<Segment> segments)
		{
			// FIXME: Assume no fragmentation of the packet.
			// FIXME: Assume no out of order packets.

			// Construct a frame from this packet.
			int frameID = (int)header.timeCode; // FIXME: Perhaps induce the correct frameID instead of using the timeCode?

			List<Joint> joints = new List<Joint> ();

			foreach(Segment segment in segments) {
				joints.Add(new Joint(
					segment.id,
					new Microsoft.Xna.Framework.Vector3(segment.x, segment.y, segment.z),
					new Microsoft.Xna.Framework.Quaternion(segment.q1, segment.q2, segment.q3, segment.q4)));
			}

			// Notify the observers.
			NotifyObservers (new Frame (frameID, joints));
		}

		public void onMXTP12(Header header, Metadata metadata)
		{
			// Simply ignore this data for now.
		}

		public void onMXTP13(Header header, List<Segment> segments, List<Point> point)
		{
			// TODO Note: Length data, not sure how to receive this packet. 

			// FIXME: Ideally we read and interpret the content of this packet to construct the BodyConfiguration.
			// Sadly the segments aren't labeled, but only the points. We could parse the points with a (0,0,0) offset
			// from the corresponding joint and use that point's label to determine the body part.
			// BUT, this would MOST likely result in the exact same configuration as the FixedBodyConfiguration!
			BodyConfiguration bodyConfiguration = new FixedBodyConfiguration ();
			NotifyObservers (bodyConfiguration);
		}
	}
}
