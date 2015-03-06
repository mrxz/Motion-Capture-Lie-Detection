﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace Motion_lie_detection
{
	/**
	 * UdpListener that listens for packets generated by the MVN Studio.
	 * This listener only supports MXTP02, MXTP12, MXTP13 packets.
	 */ 
	class XSensListener
	{
		/**
		 * The logger for the XSensListener.
		 */
		public static readonly Logger LOG = Logger.getInstance ("XSensListener");

		/**
		 * The UdpClient 
		 */
		private readonly UdpClient client;
		/**
		 * Flag indicating whether or not the listener is listening.
		 */
		private bool running;
		/**
		 * Listening thread.
		 */
		private Thread listeningThread;


		public XSensListener (String host, int port)
		{
			client = new UdpClient (new IPEndPoint (IPAddress.Any, port));
			LOG.info ("XSensListener created for " + host + ":" + port);
		}

		public bool Start ()
		{
			// Only start a thread if not already running.
			if (running) {
				LOG.warn ("Already running");
				return false;
			}

			// Start thread for reading incoming packets.
			listeningThread = new Thread (Listen);
			listeningThread.Start ();
			running = true;

			return true;
		}

		public bool Stop ()
		{
			// Only stop listening if we're listening.
			if (!running)
				return false;

			// Stop the listening thread.
			listeningThread.Abort ();
			running = false;
			return true;
		}

		public void Listen ()
		{
			// Listen
			LOG.info ("Started listening on port 9763");

			// Spawn a new thread that listens for the packages.
			IPEndPoint endPoint = new IPEndPoint (IPAddress.Any, 0);
			while (true) {
				byte[] packet = client.Receive (ref endPoint);

				handlePacket (packet);
			}
		}

		public static void handlePacket (byte[] packet)
		{
			BinaryReader reader = new BigEndianBinaryReader (new MemoryStream (packet));

			Header header = Header.fromBytes (reader);
			Console.WriteLine ("Received: " + header.id);

			// Check the package type.
			switch (header.id.Substring (4)) {
			case "02":
				{
					// Load the positions and quaternions.
					for (int i = 0; i < header.numberOfItems; i++) {
						Segment segment = Segment.fromBytes (reader);
						Console.WriteLine ("id:{0} x:{1} y:{2} z:{3}", segment.id, segment.x, segment.y, segment.z);
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
						Segment segment = Segment.nullPoseFromBytes (reader);
					}

					// Part Two: points
					UInt32 numberOfPoints = reader.ReadUInt32 ();
					for (int i = 0; i < numberOfPoints; i++) {
						Point point = Point.fromBytes (reader);
					}
				}
				break;
			default:
				LOG.warn ("Unsupported package type: " + header.id);
				break;
			}
		}
	}

	/**
	 * Header that is shared among the different packet types.
	 * The total size of the header is 24 bytes.
	 * 6 bytes		String id (e.g. MXTP02)
	 * 4 bytes		Sample counter
	 * 1 byte		Diagram counter
	 * 1 byte		Number of items, in case of MXTP02 it's the number of segments.
	 * 4 bytes		Time code
	 * 1 byte		Character id
	 * 7 bytes		Unused/reserved bytes
	 */
	class Header
	{
		/**
		 * Six character long 
		 */
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
			reader.ReadBytes (7); // Unused space.

			return header;
		}
	}

	/**
	 * Class representing the body of an MXTP12 packet.
	 * The MXTP12 is a metadata packet.
	 * It differs from the other types by not having a fixed size, but instead using string key/values.
	 */
	class Metadata
	{
		/**
		 * Dictionary containing the 
		 */
		public readonly Dictionary<String, String> properties = new Dictionary<String, String> ();

		public Metadata (Dictionary<String, String> properties)
		{
			this.properties = properties;
		}

		public static Metadata fromBytes (byte[] bytes, int offset)
		{
			Dictionary<String, String> properties = new Dictionary<string, string> ();

			// Continue reading after the 
			StreamReader reader = new StreamReader (new MemoryStream (bytes, offset, bytes.Length - offset));
			while (!reader.EndOfStream) {
				String line = reader.ReadLine ();
				String[] values = line.Split (':');

				// Note: Weird quirk in C# where a string '\0' is read as-is.
				if (line != "\0")
					properties.Add (values [0], values [1]);
			}

			return new Metadata (properties);
		}
	}

	/**
	 * The segment represents an item in the body of the MXTP02 and MXTP13 packets.
	 */
	class Segment
	{
		public int id;

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

			segment.id = reader.ReadInt32 ();
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

			segment.id = reader.ReadInt32 ();
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

	/**
	 * Class representing the point items in the body of the MXTP13 packet.
	 */
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
