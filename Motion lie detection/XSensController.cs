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

		public XSensController ()
		{
			listener = new XSensListener ("127.0.0.1", 9763);
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

		public void Test() {
			listener.Test ();
		}

		public void onMXTP02(Header header, List<Segment> segments)
		{
			// FIXME: Assume no fragmentation of the packet.
			// FIXME: Assume no out of order packets.

			// Construct a frame from this packet.
			int frameID = 0;//(int)header.timeCode; // FIXME: Perhaps induce the correct frameID instead of using the timeCode?
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
			// Length data, not sure how to receive this packet. 
		}
	}
}
