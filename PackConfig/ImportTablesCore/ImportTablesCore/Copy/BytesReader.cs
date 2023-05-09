using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.Utils
{
	public sealed class BytesReader
	{
		private byte[] buffer;
		private int position;

		public int Capcity => buffer.Length;
		public BytesReader()
		{

		}
		public void SetPosition(byte[] buf)
		{
			buffer = buf;
			position = 0;
		}
		public short ReadShort()
		{
			FastBitConvert.GetValue(buffer, ref position, out short value);
			return value;
		}
		public int ReadInt()
		{
			FastBitConvert.GetValue(buffer, ref position, out int value);
			return value;
		}
		public float ReadFloat()
		{
			FastBitConvert.GetValue(buffer, ref position, out float value);
			return value;
		}
		public long ReadLong()
		{
			FastBitConvert.GetValue(buffer, ref position, out long value);
			return value;
		}
		public string ReadString()
		{
			FastBitConvert.GetValue(buffer, ref position, out string value);
			return value;
		}
		public void Dispose()
		{
			buffer = null;
		}
	}
}
