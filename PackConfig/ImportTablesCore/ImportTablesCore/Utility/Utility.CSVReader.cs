using System;
using System.Collections;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace ImportTables.Utils
{
	public static partial class Utility
	{
		public static partial class CSVReader
		{
			public static short ReadShort(byte[] bytes, ref int position)
			{
				var result = BitConverter.ToInt16(bytes, position);
				position += 2;
				return result;
			}
			public static int ReadInt(byte[] bytes, ref int position)
			{
				var result = BitConverter.ToInt32(bytes, position);
				position += 4;
				return result;
			}
			public static long ReadLong(byte[] bytes, ref int position)
			{
				var result = BitConverter.ToInt64(bytes, position);
				position += 8;
				return result;
			}
			public static float ReadFloat(byte[] bytes, ref int position)
			{
				var result = BitConverter.ToSingle(bytes, position);
				position += 4;
				return result;
			}
			public static double ReadDouble(byte[] bytes, ref int position)
			{
				var result = BitConverter.ToDouble(bytes, position);
				position += 8;
				return result;
			}
			public static string ReadString(byte[] bytes, ref int position)
			{
				var length = BitConverter.ToInt32(bytes, position);
				position += 4;
				var result = Encoding.UTF8.GetString(bytes, position, length);
				position += length;
				return result;
			}
		}
	}
}