using System;
using System.Collections;
using System.Data.SQLite;
using Microsoft.Win32;

namespace ImportTables.Utils
{
	public static partial class Utility
	{
		public static class Bytes
		{
			public static byte[] ZeroBytes = BitConverter.GetBytes(0);
			public static byte[] Concat(byte[] data1, byte[] data2)
			{
				byte[] data3 = new byte[data1.Length + data2.Length];
				data1.CopyTo(data3, 0);
				data2.CopyTo(data3, data1.Length);
				return data3;
			}
		}
	}
}