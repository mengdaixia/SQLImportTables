using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.Utils
{
	//Byte和Double懒得加了
	public static class FastBitConvert
	{
		public static void GetBytes(byte[] buffer, ref int pos, short value)
		{
			buffer[pos++] = (byte)value;
			buffer[pos++] = (byte)(value >> 8);
		}
		public static void GetValue(byte[] buffer, ref int pos, out short value)
		{
			value = (short)(buffer[pos++] | buffer[pos++] << 8);
		}
		public static void GetSBytes(byte[] buffer, ref int pos, short value)
		{
			while (value > 0)
			{
				buffer[pos++] = (byte)((value & 0x7f) | 0x80);
				value >>= 7;
			}
			buffer[pos - 1] &= 0x7f;
		}
		public static void GetSValue(byte[] buffer, ref int pos, out short value)
		{
			int index = 0;
			value = 0;
			while ((buffer[pos] & 0x80) != 0)
			{
				value += (short)(buffer[pos++] << (index * 7));
				index++;
			}
			value += (short)(buffer[pos++] << (index * 7));
		}
		public static void GetBytes(byte[] buffer, ref int pos, int value)
		{
			buffer[pos++] = (byte)value;
			buffer[pos++] = (byte)(value >> 8);
			buffer[pos++] = (byte)(value >> 16);
			buffer[pos++] = (byte)(value >> 24);
		}
		public static void GetSValue(byte[] buffer, ref int pos, out int value)
		{
			int index = 0;
			value = 0;
			while ((buffer[pos] & 0x80) != 0)
			{
				value += (buffer[pos++] & 0x7f) << (index * 7);
				index++;
			}
			value += (buffer[pos++] << (index * 7));
		}
		public static void GetSBytes(byte[] buffer, ref int pos, int value)
		{
			while (value > 0)
			{
				buffer[pos++] = (byte)((value & 0x7f) | 0x80);
				value >>= 7;
			}
			buffer[pos - 1] &= 0x7f;
		}
		public static void GetValue(byte[] buffer, ref int pos, out int value)
		{
			value = buffer[pos++] | buffer[pos++] << 8 | buffer[pos++] << 16 | buffer[pos++] << 24;
		}
		public static void GetBytes(byte[] buffer, ref int pos, long value)
		{
			buffer[pos++] = (byte)value;
			buffer[pos++] = (byte)(value >> 8);
			buffer[pos++] = (byte)(value >> 16);
			buffer[pos++] = (byte)(value >> 24);
			buffer[pos++] = (byte)(value >> 32);
			buffer[pos++] = (byte)(value >> 40);
			buffer[pos++] = (byte)(value >> 48);
			buffer[pos++] = (byte)(value >> 56);
		}
		public static void GetValue(byte[] buffer, ref int pos, out long value)
		{
			uint lo = (uint)(buffer[pos++] | buffer[pos++] << 8 |
							  buffer[pos++] << 16 | buffer[pos++] << 24);
			uint hi = (uint)(buffer[pos++] | buffer[pos++] << 8 |
							 buffer[pos++] << 16 | buffer[pos++] << 24);
			value = (long)((ulong)hi) << 32 | lo;
		}
		public unsafe static void GetBytes(byte[] buffer, ref int pos, float value)
		{
			uint tValue = *(uint*)&value;
			buffer[pos++] = (byte)tValue;
			buffer[pos++] = (byte)(tValue >> 8);
			buffer[pos++] = (byte)(tValue >> 16);
			buffer[pos++] = (byte)(tValue >> 24);
		}
		public unsafe static void GetValue(byte[] buffer, ref int pos, out float value)
		{
			uint tValue = (uint)(buffer[pos++] | buffer[pos++] << 8 | buffer[pos++] << 16 | buffer[pos++] << 24);
			value = *((float*)&tValue);
		}
		public static void GetBytes(byte[] buffer, ref int pos, string value)
		{
			var len = Encoding.UTF8.GetByteCount(value);
			GetBytes(buffer, ref pos, len);
			Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, pos);
			pos += len;
		}
		public static void GetValue(byte[] buffer, ref int pos, out string value)
		{
			int len = 0;
			GetValue(buffer, ref pos, out len);
			value = Encoding.UTF8.GetString(buffer, pos, len);
			pos += len;
		}
	}
}