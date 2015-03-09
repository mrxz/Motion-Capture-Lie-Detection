using System;
using System.IO;

namespace Motion_lie_detection
{
	/**
	 * A BigEndian version of the standard C# BinaryReader.
	 * The default BinaryReader assumes that the input is in little endian when 
	 * reading multi-byte types with no means of altering or specifying it.
	 * This BigEndianBinaryReader first reads the number of bytes required for the type,
	 * reverses them and returns the (correct) value.
	 */
	public class BigEndianBinaryReader : BinaryReader
	{

		public BigEndianBinaryReader(Stream input) : base(input)
		{ }

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
			byte[] bytes = ReadBytes (8);
			Array.Reverse (bytes);
			return BitConverter.ToDouble (bytes, 0);
		}

		public override short ReadInt16 ()
		{
			byte[] bytes = ReadBytes (8);
			Array.Reverse (bytes);
			return BitConverter.ToInt16 (bytes, 0);
		}

		public override long ReadInt64 ()
		{
			byte[] bytes = ReadBytes (8);
			Array.Reverse (bytes);
			return BitConverter.ToInt64 (bytes, 0);
		}

		public override ushort ReadUInt16 ()
		{
			byte[] bytes = ReadBytes (2);
			Array.Reverse (bytes);
			return BitConverter.ToUInt16 (bytes, 0);
		}

		public override ulong ReadUInt64 ()
		{
			byte[] bytes = ReadBytes (8);
			Array.Reverse (bytes);
			return BitConverter.ToUInt64 (bytes, 0);
		}

	}
}

