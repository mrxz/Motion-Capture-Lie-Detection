using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Motion_lie_detection
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			byte[] custom = new byte[] { 
				0x4D, 0x58, 0x54, 0x50, 0x31, 0x32,			// String ID
				0x00, 0x00, 0x00, 0x00,						// 4 bytes
				0x00,										// 1 byte
				0x01,										// 1 byte
				0x00, 0x00, 0x00, 0x00,						// time code
				0x00,										// Character ID
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,	// UNUSED
				//////////////////////
				/// END OF HEADER	//
				//////////////////////
				0x00, 0x00, 0x00, 0x01,						// Segment id
				0x00, 0x00, 0x00, 0x00,						// X
				0x00, 0x00, 0x00, 0x00,						// Y
				0x00, 0x00, 0x00, 0x00,						// Z
				0x00, 0x00, 0x00, 0x00,						// q1
				0x00, 0x00, 0x00, 0x00,						// q2
				0x00, 0x00, 0x00, 0x00,						// q3
				0x00, 0x00, 0x00, 0x00						// q4
			};
			// Example
			handlePacket (custom);


			// Listen
			UdpClient client = new UdpClient ();
			IPEndPoint endPoint = new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9763);
			client.Connect (endPoint);

			while (true) {
				byte[] packet = client.Receive (ref endPoint);

				handlePacket (packet);
			}
		}

		public static void handlePacket (byte[] packet)
		{
			BinaryReader reader = new BinaryReader (new MemoryStream (packet));

			Header header = Header.fromBytes (reader);
			Console.WriteLine ("Received: " + header.id);

			// Check the package type.
			switch (header.id.Substring (4)) {
			case "01":
				{
					// Load the positions and quaternions.
					for (int i = 0; i < header.numberOfItems; i++) {
						Segment segment = Segment.fromBytes (reader);
					}
				}
				break;
			case "12":
				{
					// Read the metadata.
					Metadata metadata = Metadata.fromBytes (packet, 24);
				}
				break;
			case "13":
				{
					// Part One: null-pose
					UInt32 numberOfSegments = reader.ReadUInt32 ();
					for (int i = 0; i < numberOfSegments; i++) {
						Segment segment = Segment.fromBytes (reader);
					}

					// Part Two: points
					UInt32 numberOfPoints = reader.ReadUInt32 ();
					for (int i = 0; i < numberOfPoints; i++) {
						Point point = Point.fromBytes (reader);
					}
				}
				break;
			default:
				Console.WriteLine ("Unsupported package type");
				break;
			}
		}
	}

	class Header
	{
		public String id;

		public uint sampleCounter;
		public byte diagramCounter;
		public byte numberOfItems;

		public uint timeCode;
		public byte characterId;

		public static Header fromBytes (BinaryReader reader)
		{
			Header header = new Header ();

			header.id = "";
			byte[] idBytes = reader.ReadBytes (6);
			for (int i = 0; i < idBytes.Length; i++)
				header.id += (char)idBytes [i];
			header.sampleCounter = reader.ReadUInt32 ();
			header.diagramCounter = reader.ReadByte ();
			header.numberOfItems = reader.ReadByte ();

			header.timeCode = reader.ReadUInt32 ();
			header.characterId = reader.ReadByte ();

			return header;
		}
	}

	class Metadata
	{
		public Dictionary<String, String> properties = new Dictionary<String, String> ();

		public Metadata (Dictionary<String, String> properties)
		{
			this.properties = properties;
		}

		public static Metadata fromBytes (byte[] bytes, int offset)
		{
			Dictionary<String, String> properties = new Dictionary<string, string> ();

			StreamReader reader = new StreamReader (new MemoryStream (bytes, offset, bytes.Length - offset));
			while (!reader.EndOfStream) {
				String line = reader.ReadLine ();
				String[] values = line.Split (':');

				properties.Add (values [0], values [1]);
			}

			return new Metadata (properties);
		}
	}


	class Segment
	{
		public uint id;

		public float x;
		public float y;
		public float z;

		public float q1;
		public float q2;
		public float q3;
		public float q4;

		public static Segment fromBytes (BinaryReader reader)
		{
			Segment segment = new Segment ();

			segment.id = reader.ReadUInt32 ();
			segment.x = reader.ReadSingle ();
			segment.y = reader.ReadSingle ();
			segment.z = reader.ReadSingle ();
			segment.q1 = reader.ReadSingle ();
			segment.q2 = reader.ReadSingle ();
			segment.q3 = reader.ReadSingle ();
			segment.q4 = reader.ReadSingle ();

			return segment;
		}

		public static Segment nullPoseFromBytes (BinaryReader reader)
		{
			Segment segment = new Segment ();

			segment.id = reader.ReadUInt32 ();
			segment.x = reader.ReadSingle ();
			segment.y = reader.ReadSingle ();
			segment.z = reader.ReadSingle ();
			segment.q1 = 0;
			segment.q2 = 0;
			segment.q3 = 0;
			segment.q4 = 1;

			return segment;
		}
	}

	class Point
	{
		public ushort id;
		public ushort pointId;
		public String name;
		public uint flags;
		public float x;
		public float y;
		public float z;

		public static Point fromBytes (BinaryReader reader)
		{
			Point point = new Point ();

			point.id = reader.ReadUInt16 ();
			point.pointId = reader.ReadUInt16 ();
			point.name = reader.ReadString ();
			point.flags = reader.ReadUInt32 ();
			point.x = reader.ReadSingle ();
			point.y = reader.ReadSingle ();
			point.z = reader.ReadSingle ();

			return point;
		}
	}
}
