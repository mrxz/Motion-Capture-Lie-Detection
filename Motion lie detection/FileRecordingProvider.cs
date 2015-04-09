using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Globalization;
using System.IO;
using mxf = Microsoft.Xna.Framework;

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
		protected readonly String file;

		/**
		 * The frame rate of this recording.
		 */
		protected int frameRate = -1;

		/**
		 * The body configuration from the file.
		 */
		protected BodyConfiguration bodyConfiguration;

		/**
		 * Buffer containing new frames (and unfinished/partial frames).
		 */
		protected List<Frame> newFrames;

		public FileRecordingProvider(String file)
		{
			this.file = file;
			LOG.info ("New FileRecordingProvider for file " + file);
			// FIXME: Check the file extensions (or not).
		}

        public static FileRecordingProvider AppropriateReader(string fname)
        {
            string ext = Path.GetExtension(fname);
            switch (ext) // bad code smell
            {
                case ".bmocap":
                    return new BinaryMocapReader(fname);
                case ".mvnx":
                    return new MVNXReader(fname);
                default:
                    // kan eventueel ook defaulten naar mvnx ofzo?
                    throw new ArgumentException("Select a .mvnx or .bmocap file.");
            }
        }

        public override bool Init()
        {
            throw new NotImplementedException();
        }
        public override bool Start()
        {
            // Nothing to do here.
            LOG.info("Starting");
            return true;
        }

        public override int GetFrameRate()
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
	}

    public class MVNXReader : FileRecordingProvider
    {
        public MVNXReader(string f) : base(f) { }

        public override bool Init()
        {
            // Construct an XML reader for the file.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(file, settings);

            // Read frameRate.
            reader.ReadToFollowing("subject");
            frameRate = int.Parse(reader.GetAttribute("frameRate"));
            int segmentCount = int.Parse(reader.GetAttribute("segmentCount"));

            // Read the segments.
            bodyConfiguration = new FixedBodyConfiguration();
            // FIXME: Read the connections between the segments.
            // Note: isn't really worth the effort probably since it will always be the same.
            // Note: We simply skip the reading the body configuration from the file for the time being, since we know it's an .mvnx file from XSens, meaning the layout is constant.
            /*
            reader.ReadToFollowing ("segments");
            for (int i = 0; i < segmentCount; i++) {
                reader.ReadToFollowing ("segment");
                String label = reader.GetAttribute ("label");
                int id = int.Parse (reader.GetAttribute ("id"));

                // Convert the label to BodyPart
                //bodyConfiguration.AddMapping (fromXSensString(label), id);
            }*/



            // Read the frames.
            newFrames = new List<Frame>();
            reader.ReadToFollowing("frames");
            while (reader.ReadToFollowing("frame"))
            {
                // Not interested in poses, only 'normal' frames.
                switch (reader.GetAttribute("type"))
                {
                    case "npose":
                        // The npose can be used to determine the lengths of the body-part.
                        Frame npose = readFrame(reader);
                        bodyConfiguration.LengthsFromNPose(npose);
                        break;
                    case "normal":
                        // Read the frame and add it to the newFrames list.
                        Frame newFrame = readFrame(reader);
                        newFrames.Add(newFrame);
                        break;
                    default:
                        // This type of frame doesn't interest us, so don't do anything.
                        break;
                }
            }
            LOG.info("Read " + newFrames.Count + " frames from file " + file);

            return true;
        }


        /**
         * Method for reading a frame given an XmlReader positioned at a frame.
         * @param reader The XmlReader to read the frame of.
         * @return The Frame from the xmlReader, Frame.Empty on failure.
         */
        private Frame readFrame(XmlReader reader)
        {
            int timeCode = int.Parse(reader.GetAttribute("time"));

            reader.Read(); // orientation
            float[] orientations = reader.ReadString().Split().Select(n => float.Parse(n, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
            reader.Read(); // end orientation

            reader.Read(); // position
            float[] positions = reader.ReadString().Split().Select(n => float.Parse(n, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
            reader.Read(); // end of position

            // Convert the data into a frame and add it to the newFrames list.
            List<Joint> joints = new List<Joint>();
            for (int i = 0; i < positions.Length / 3; i++)
            {
                joints.Add(
                    new Joint(
                        i + 1,
                        new mxf.Vector3(positions[3 * i], positions[3 * i + 1], positions[3 * i + 2]),
                        new mxf.Quaternion(orientations[4 * i], orientations[4 * i + 1], orientations[4 * i + 2], orientations[4 * i + 3])));
            }

            return new Frame(joints, timeCode);
        }
    }

    public class BinaryMocapReader : FileRecordingProvider
    {
        private int jCount = 0;

        public BinaryMocapReader(string f) : base(f) { }

        public override bool Init()
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                using (BinaryReader br = new BinaryReader(fs)) {
                    frameRate = br.ReadInt32();

                    // Read all segments
                    int segmentCount = br.ReadInt32();
                    bodyConfiguration = new FixedBodyConfiguration();
                    // For later: Read body configuration

                    // Read frames
                    int frameCount = br.ReadInt32();
                    jCount = br.ReadInt32();
                    newFrames = new List<Frame>(frameCount);
                    for (int f = 0; f < frameCount; ++f) {
                        // Nposes are used to determine the lengths of body parts,
                        // otherwise we read a regular frame.
                        bool isNPose = br.ReadBoolean();
                        if (isNPose) {
                            Frame npose = readFrame(br);
                            bodyConfiguration.LengthsFromNPose(npose);
                        }
                        else {
                            Frame frame = readFrame(br);
                            newFrames.Add(frame);
                        }
                    }

                    LOG.info("Read " + frameCount + " frames from file " + file);
                }
            }

            return true;
        }


        /**
         * Method for reading a frame given an BinaryReader positioned at a frame.
         * @param br The BinaryReader to read the frame of.
         * @return The Frame from the BinaryReader, Frame.Empty on failure.
         */
        private Frame readFrame(BinaryReader br)
        {
            int timeCode = br.ReadInt32();

            List<Joint> joints = new List<Joint>(jCount);
            for (int i = 0; i < jCount; ++i) {
                joints.Add(
                    new Joint(
                        i + 1,
                        new mxf.Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                        new mxf.Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle())));
            }

            return new Frame(joints, timeCode);
        }
    }
}
