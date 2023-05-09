using System;
using System.Collections;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace ImportTables.Utils
{
	public static partial class Utility
	{
		public static class Buffer
		{
			//简单测试了一下
			//Unsafe.CopyBlockUnaligned在数组较小时效率最高（10000左右会比Span慢一点）
			//Span.CopyTo也很快，且平稳
			//Buffer.BlockCopy和Arry.Copy做的预处理较多会慢一点儿
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void Copy(byte[] src, byte[] dest, int length)
			{
				Unsafe.CopyBlockUnaligned(ref dest[0], ref src[0], (uint)length);
			}
		}
	}
}