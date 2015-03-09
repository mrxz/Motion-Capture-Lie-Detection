using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;

namespace Motion_lie_detection
{
	/**
	 * Recording provider that reads the frame and joint data from a file.
	 * Supported file types are: .mvnx
	 */
	public class FileRecordingProvider : RecordingProvider
	{
		/**
		 * The logger for the FileRecordingProvider.
		 */
		public static readonly Logger LOG = Logger.getInstance ("FileRecordingProvider");

		/**
		 * The file the recording provider reads.
		 */
		private readonly String file;

		/**
		 * The frame rate of this recording.
		 */
		private int frameRate = -1;

		/**
		 * The body configuration from the file.
		 */
		private BodyConfiguration bodyConfiguration;

		/**
		 * Buffer containing new frames (and unfinished/partial frames).
		 */
		private List<Frame> newFrames;

		public FileRecordingProvider(String file)
		{
			this.file = file;
			LOG.info ("New FileRecordingProvider for file " + file);
			// FIXME: Check the file extensions (or not).
		}


		public override bool Init ()
		{
			// Construct an XML reader for the file.
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.IgnoreWhitespace = true;
			settings.IgnoreComments = true;

			XmlReader reader = XmlReader.Create(file, settings);

			// Read frameRate.
			reader.ReadToFollowing ("subject");
			frameRate = int.Parse(reader.GetAttribute ("frameRate"));
			int segmentCount = int.Parse (reader.GetAttribute ("segmentCount"));

			// Read the segments.
			bodyConfiguration = new BodyConfiguration ();
			reader.ReadToFollowing ("segments");
			for (int i = 0; i < segmentCount; i++) {
				reader.ReadToFollowing ("segment");
				String label = reader.GetAttribute ("label");
				int id = int.Parse (reader.GetAttribute ("id"));

				// Convert the label to BodyPart
				bodyConfiguration.AddMapping (fromXSensString(label), id);
			}

			// Read the frames.
			newFrames = new List<Frame> ();
			reader.ReadToFollowing ("frames");
			while (reader.Read ()) {
				if (reader.Name != "frame")
					reader.ReadToFollowing ("frame");

				// Not interested in poses, only 'normal' frames.
				if (reader.GetAttribute ("type") != "normal") {
					reader.ReadToFollowing ("frame");
					continue;
				}

				// The frame.
				int frameId = int.Parse(reader.GetAttribute("time"));

				reader.Read (); // orientation
				float[] orientations = reader.ReadString().Split().Select(n => Convert.ToSingle(n)).ToArray();
				reader.Read (); // end orientation

				reader.Read (); // position
				float[] positions = reader.ReadString().Split().Select(n => Convert.ToSingle(n)).ToArray();
				reader.Read (); // end of position

				// Convert the data into a frame and add it to the newFrames list.
				List<Joint> joints = new List<Joint> (segmentCount);
				for (int i = 0; i < segmentCount; i++) {
					joints.Add(
						new Joint (
							i + 1,
							new Microsoft.Xna.Framework.Vector3 (positions [3 * i], positions [3 * i + 1], positions [3 * i + 2]),
							new Microsoft.Xna.Framework.Quaternion (orientations [4 * i], orientations [4 * i + 1], orientations [4 * i + 2], orientations [4 * i + 3])));
				}
				newFrames.Add(new Frame(frameId, joints));

				reader.ReadToFollowing ("frame");
			}
			LOG.info("Read " + newFrames.Count + " frames from file " + file);

			return true;
		}

		public override bool Start ()
		{
			// Nothing to do here.
			LOG.info ("Starting");
			return true;
		}

		public override int GetFrameRate ()
		{
			return frameRate;
		}

		public override BodyConfiguration GetBodyConfiguration ()
		{
			return bodyConfiguration;
		}

		public override List<Frame> GetNewFrames()
		{
			List<Frame> result = newFrames;
			newFrames = new List<Frame> ();
			return result;
		}

		/**
		 * Method for retrieving the BodyPart that corresponds with a segment label.
		 * @param label The label to return the BodyPart for.
		 * @return The corresponding body part.
		 */
		private BodyPart fromXSensString(String label)
		{
			switch (label) {
			case "Pelvis":
				return BodyPart.PELVIS;
			case "L5":
				return BodyPart.L5;
			case "L3":
				return BodyPart.L3;
			case "T12":
				return BodyPart.T12;
			case "T8":
				return BodyPart.T8;
			case "Neck":
				return BodyPart.NECK;
			case "Head":
				return BodyPart.HEAD;
			case "RightShoulder":
				return BodyPart.RIGHT_SHOULDER;
			case "RightUpperArm":
				return BodyPart.RIGHT_UPPER_ARM;
			case "RightForeArm":
				return BodyPart.RIGHT_FORE_ARM;
			case "RightHand":
				return BodyPart.RIGHT_HAND;
			case "LeftShoulder":
				return BodyPart.LEFT_SHOULDER;
			case "LeftUpperArm":
				return BodyPart.LEFT_UPPER_ARM;
			case "LeftForeArm":
				return BodyPart.LEFT_FORE_ARM;
			case "LeftHand":
				return BodyPart.LEFT_HAND;
			case "RightUpperLeg":
				return BodyPart.RIGHT_UPPER_LEG;
			case "RightLowerLeg":
				return BodyPart.RIGHT_LOWER_LEG;
			case "RightFoot":
				return BodyPart.RIGHT_FOOT;
			case "RightToe":
				return BodyPart.RIGHT_TOE;
			case "LeftUpperLeg":
				return BodyPart.LEFT_UPPER_LEG;
			case "LeftLowerLeg":
				return BodyPart.LEFT_LOWER_LEG;
			case "LeftFoot":
				return BodyPart.LEFT_FOOT;
			case "LeftToe":
				return BodyPart.LEFT_TOE;
			default:
				throw new Exception ();
			}
		}
	}
}
