using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.Utils
{
	public sealed class BytesWrite
	{
		private byte[] buffer;
		private int position;

		public byte[] Buffer => buffer;
		public int Length => position;
		public int Capcity => buffer.Length;
		public BytesWrite(int capcity)
		{
			buffer = new byte[capcity];
		}
		public void SetPosition(int pos)
		{
			position = pos;
		}
		public void WriteOne(int value)
		{
			EnsureCapcity(1);
			FastBitConvert.GetBytes(buffer, ref position, (byte)value);
		}
		public void WriteOne(short value)
		{
			EnsureCapcity(1);
			FastBitConvert.GetBytes(buffer, ref position, (byte)value);
		}
		public void Write(short value)
		{
			EnsureCapcity(2);
			FastBitConvert.GetBytes(buffer, ref position, value);
		}
		public void Write(int value)
		{
			EnsureCapcity(4);
			FastBitConvert.GetBytes(buffer, ref position, value);
		}
		public void Write(float value)
		{
			EnsureCapcity(4);
			FastBitConvert.GetBytes(buffer, ref position, value);
		}
		public void Write(double value)
		{
			EnsureCapcity(8);
		}
		public void Write(ReadOnlySpan<char> value)
		{
			//UTF8Encoding会直接操作指针，并不会申请Char[],可以直接用
			var bytesCount = Encoding.UTF8.GetByteCount(value);
			EnsureCapcity(4 + bytesCount);
			FastBitConvert.GetBytes(buffer, ref position, value);
		}
		public void Write(string value)
		{
			//UTF8Encoding会直接操作指针，并不会申请Char[],可以直接用
			var bytesCount = Encoding.UTF8.GetByteCount(value);
			EnsureCapcity(4 + bytesCount);
			FastBitConvert.GetBytes(buffer, ref position, value);
		}
		public byte[] GetBuffer()
		{
			if (position == 0)
			{
				return Utility.Bytes.ZeroBytes;
			}
			//可用对象池
			var bytes = new byte[Length];
			Utility.Buffer.Copy(buffer, bytes, Length);
			return bytes;
		}
		private void EnsureCapcity(int cap)
		{
			if (position + cap > Capcity)
			{
				byte[] newBytes = null;
				if (position + cap < Capcity * 2)
				{
					newBytes = new byte[Capcity * 2];
				}
				else
				{
					newBytes = new byte[Capcity + cap];
				}
				System.Buffer.BlockCopy(buffer, 0, newBytes, 0, buffer.Length);
				buffer = newBytes;
			}
		}
	}
}