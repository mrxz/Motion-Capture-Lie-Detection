using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Motion_lie_detection
{
	public partial class DummyStream
	{
		private byte[][] packets;
		private int frameRate = 60;

		private int currentPacket;
		private UdpClient client;

		public DummyStream ()
		{
			packets = new byte[594][];
			init();
		}

		public void Start() 
		{
			if (client == null) {
				client = new UdpClient (new IPEndPoint (IPAddress.Any, 48962));
				client.Connect ("127.0.0.1", 9763);
			}
			currentPacket = 0;

			new Thread (send).Start();
		}

		private void send()
		{
			while(currentPacket < packets.Length)
			{
				client.Send (packets [currentPacket], packets [currentPacket].Length);

				Thread.Sleep (1000 / frameRate);
				currentPacket++;
			}

		}
	}
}

