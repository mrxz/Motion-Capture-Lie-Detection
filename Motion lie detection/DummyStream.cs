using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Motion_lie_detection
{
	public partial class DummyStream
	{
		public static readonly Logger LOG = Logger.getInstance("DummyStream");

		private byte[][] packets;
		private int frameRate = 60;

		private int currentPacket;
		private UdpClient client;

		public DummyStream ()
		{
			LOG.info ("New dummy stream created");
			packets = new byte[594][];
			init();
		}

		public void Start(bool endless) 
		{
			if (client == null) {
				client = new UdpClient (new IPEndPoint (IPAddress.Any, 48962));
				client.Connect ("127.0.0.1", 9763);
			}
			currentPacket = 0;

			LOG.info ("Starting streaming thread");
			new Thread (send).Start(endless);
		}

		private void send(object data)
		{
            bool endless = (bool)data;
			while(currentPacket < packets.Length || endless)
			{
				LOG.fine ("Sending packet #" + currentPacket);
				client.Send (packets [currentPacket], packets [currentPacket].Length);

				Thread.Sleep (1000 / frameRate);
                currentPacket = (currentPacket + 1) % packets.Length;
			}

		}
	}
}

