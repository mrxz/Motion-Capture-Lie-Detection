using System;
using System.IO;

namespace Motion_lie_detection
{
	public class BigEndianBinaryReader : BinaryReader
	{

		public BigEndianBinaryReader(Stream input) : base(input)
		{

		}

		public override int ReadInt32 ()
		{
			byte[] bytes = ReadBytes (4);
			Array.Reverse (bytes);
			return BitConverter.ToInt32(bytes, 0);
		}

		public override uint ReadUInt32 ()
		{
			byte[] bytes = ReadBytes (4);
			Array.Reverse (bytes);
			return BitConverter.ToUInt32(bytes, 0);
		}

		public override float ReadSingle ()
		{
			byte[] bytes = ReadBytes (4);
			Array.Reverse (bytes);
			return BitConverter.ToSingle(bytes, 0);
		}

		public override decimal ReadDecimal ()
		{
			throw new NotSupportedException ();
		}

		public override double ReadDouble ()
		{
			throw new NotSupportedException ();
		}

		public override short ReadInt16 ()
		{
			throw new NotSupportedException ();
		}

		public override long ReadInt64 ()
		{
			throw new NotSupportedException ();
		}

		public override ushort ReadUInt16 ()
		{
			throw new NotSupportedException ();
		}

		public override ulong ReadUInt64 ()
		{
			throw new NotSupportedException ();
		}

	}
}

