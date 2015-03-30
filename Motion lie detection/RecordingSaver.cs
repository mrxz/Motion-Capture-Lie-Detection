using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Motion_lie_detection
{
	/**
	 * Abstract class for writing a recording to file.
	 */
	public abstract class RecordingSaver
	{
		/**
		 * FIXME: Currently hard-coded segment count.
		 */
		public static readonly int segmentCount = 23;

		/**
		 * The file path and name to write the recording to.
		 */
		protected String file;

		public RecordingSaver(String file) {
			this.file = file;
		}

		/**
		 * Method for saving an entire recording to the file.
		 * @param recording The recording to save.
		 * @return True on success, false on failure.
		 */
		public bool saveToFile(Recording recording) {
			return saveToFile (recording, 0, recording.FrameCount);
		}

		/**
		 * Method for saving an recording from a given point to the file.
		 * @param recording The recording to save a portion of.
		 * @param start The frame index to start saving from.
		 * @return True on success, false on failure.
		 */
		public bool saveToFile(Recording recording, int start) {
			return saveToFile (recording, start, recording.FrameCount);
		}

		/**
		 * Method for saving a given segment of a recording to the file.
		 * @param recording The recording to save the segment of.
		 * @param start The frame index of the start of the segment.
		 * @param end The frame index of the end of the segment.
		 * @return True on success, false on failure.
		 */
		public abstract bool saveToFile(Recording recording, int start, int end);

	}

	/**
	 * Implementation for saving a recording to an .mvnx file.
	 */
	public class MVNXSaver : RecordingSaver
	{
		public MVNXSaver(String file) : base(file)
		{

		}

		public override bool saveToFile(Recording recording, int start, int end) {
			Console.WriteLine ("Saving from {0} to {1}", start, end);

			XmlWriterSettings settings = new XmlWriterSettings ();
			settings.Indent = true;
			settings.NewLineOnAttributes = false;
			settings.NewLineChars = "\n";
			settings.NewLineHandling = NewLineHandling.Entitize;

			using (XmlWriter writer = XmlWriter.Create (file, settings)) 
			{
				// Write the header.
				writeHeader (writer);

				// The real body of the xml document.
				writer.WriteStartElement ("subject");
				writer.WriteAttributeString ("label", "Suit MLD"); // FIXME: No use trying to copy the suit-id, so now simply using Suit MLD (Motion Lie Detection)
				writer.WriteAttributeString ("frameRate", recording.FrameRate.ToString());
				writer.WriteAttributeString ("segmentCount", segmentCount.ToString()); // FIXME: Hard-coded 23 segments, could work if for lower numbers numbers are made up for the missing segments.
				writer.WriteAttributeString ("recDate", "Mon 14. May 15:31:16 2012"); // FIXME: Hard-coded rec-date, not important.
				writer.WriteAttributeString ("originalFilename", file);

				// Comment
				writeComment (writer);

				// Segments
				writeSegments (writer, recording.BodyConfiguration);

				// Sensors.
				writeSensors (writer);

				// Joints
				writeJoints (writer);

				// Frames.
				writeFrames (writer, recording, start, end);

				writer.WriteEndElement ();

				// Write the footer.
				writeFooter (writer);
			}
			return true;
		}

		private void writeHeader(XmlWriter writer) {
			// <?xml version="1.0" encoding="UTF-8"?>
			writer.WriteStartDocument ();

			// <mvnx version="3">
			writer.WriteStartElement ("mvnx");
			writer.WriteAttributeString ("version", "3");

			// <mvn version="3.1.5" build="54 2011-07-15 16:03 11670"/>
			writer.WriteStartElement ("mvn");
			writer.WriteAttributeString ("version", "3.1.5");
			writer.WriteAttributeString ("build", "54 2011-07-15 16:03 11670");
			writer.WriteEndElement();

			// <comment></comment>
			writeComment (writer);
		}

		private void writeSegments(XmlWriter writer, BodyConfiguration configuration) {
			writer.WriteStartElement ("segments");

			Queue<BodyNode> q = new Queue<BodyNode> ();
			q.Enqueue (configuration.getRoot ());
			while (q.Count > 0) {
				BodyNode node = q.Dequeue ();

				writer.WriteStartElement ("segment");
				writer.WriteAttributeString ("label", toXSensString(node.getJointId()));
				writer.WriteAttributeString ("id", node.getJointId().ToString());

				// Points
				writer.WriteStartElement ("points");
				// FIXME: Perhaps at least one points is needed with offset (0,0,0) ???
				writer.WriteFullEndElement ();

				writer.WriteEndElement ();

				foreach (BodyNode neighbour in node.getNeighbours())
					q.Enqueue (neighbour);
			}

			writer.WriteFullEndElement ();
		}

		private void writeSensors(XmlWriter writer) {
			// We don't have any sensor data available, so we simply use dummy data.
			writer.WriteStartElement ("sensors");

			foreach (String label in new String[] {"Pelvis", "T8", "Head", "RightShoulder", "RightUpperArm", "RightForeArm", "RightHand", "LeftShoulder", "LeftUpperArm", "LeftForeArm",
				"LeftHand", "RightUpperLeg", "RightLowerLeg", "RightFoot", "LeftUpperLeg", "LeftLowerLeg", "LeftFoot"}) {
				writer.WriteStartElement ("sensor");
				writer.WriteAttributeString ("label", label);
				writer.WriteEndElement ();
			}

			writer.WriteEndElement ();
		}

		private void writeJoints(XmlWriter writer) {
			writer.WriteStartElement ("joints");
			// TODO: Perhaps joints are needed.
			// Note: if we want MVN Studio compatibility we might get away with simply injecting the same as seen in other .mvnx files.
			// This also applies to the points.
			writer.WriteFullEndElement ();
		}

		private void writeFrames(XmlWriter writer, Recording recording, int start, int end) {
			writer.WriteStartElement ("frames");
			writer.WriteAttributeString ("segmentCount", segmentCount.ToString());
			writer.WriteAttributeString ("sensorCount", 0.ToString()); // FIXME: This is (and should be) 0 ???
			writer.WriteAttributeString ("jointCount", (segmentCount - 1).ToString()); // FIXME: Assumming this will be one less than the segment count?

			// Loop over the frames.
			// TODO: Add npose and/or tpose (perhaps derive from body configuration).
			int frameId = start;
			int written = 0;
			while (frameId < end) {
				Frame frame = recording.GetFrame (frameId);
				writeFrame (writer, frame, "normal");
				written++;
				frameId++;
			}
			Console.WriteLine ("Written {0} frames", written);

			writer.WriteEndElement ();
		}

		private void writeFrame(XmlWriter writer, Frame frame, String type) {
			writer.WriteStartElement ("frame");
			writer.WriteAttributeString ("time", frame.Id.ToString()); // FIXME: Id is not the frameId, but the time code ...
			writer.WriteAttributeString ("type", type);

			StringBuilder orientations = new StringBuilder ();
			StringBuilder positions = new StringBuilder ();
			foreach (Joint joint in frame.Joints) {
				if (orientations.Length != 0) orientations.Append (" ");
				orientations.Append (String.Format("{0} {1} {2} {3}", joint.Orientation.X, joint.Orientation.Y, joint.Orientation.Z, joint.Orientation.W));
				if (positions.Length != 0) positions.Append (" ");
				positions.Append (String.Format("{0} {1} {2}", joint.Position.X, joint.Position.Y, joint.Position.Z));
			}

			// Orientation.
			writer.WriteStartElement ("orientation");
			writer.WriteString (orientations.ToString());
			writer.WriteEndElement ();

			// Position
			writer.WriteStartElement ("position");
			writer.WriteString (positions.ToString());
			writer.WriteEndElement ();

			// FIXME: Should velocity and center of mass also be added?
			writer.WriteStartElement ("velocity");
			writer.WriteFullEndElement ();

			writer.WriteStartElement ("centerOfMass");
			writer.WriteFullEndElement ();

			writer.WriteEndElement ();
		}

		private void writeFooter(XmlWriter writer) {
			//<securityCode code="sample"/>
			writer.WriteStartElement ("securityCode");
			writer.WriteAttributeString ("code", "random"); // FIXME: What should the code say?
			writer.WriteEndElement ();

			//</mvnx>
			writer.WriteEndElement ();

			writer.WriteEndDocument ();
		}

		private void writeComment(XmlWriter writer) {
			// <comment></comment>
			writer.WriteStartElement ("comment");
			writer.WriteFullEndElement ();
		}

		private String toXSensString(int jointId)
		{
			String[] names = new String[] { 
				"Pelvis", "L5","L3", "T12", "T8", "Neck", "Head", "RightShoulder", "RightUpperArm", "RightForeArm",
				"RightHand",
				"LeftShoulder",
				"LeftUpperArm",
				"LeftForeArm",
				"LeftHand",
				"RightUpperLeg",
				"RightLowerLeg",
				"RightFoot",
				"RightToe",
				"LeftUpperLeg",
				"LeftLowerLeg",
				"LeftFoot",
				"LeftToe"
				};


			return names[jointId - 1];
		}
	}
}

