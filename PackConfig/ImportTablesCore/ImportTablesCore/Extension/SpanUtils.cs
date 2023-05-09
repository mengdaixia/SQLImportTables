using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables
{
	public static class SpanUtils
	{
		private static Queue<List<(int, int)>> lstPool = new Queue<List<(int, int)>>();

		public static SplitResult Split(this string str, char split_char = ';')
		{
			return Split(str.AsSpan(), split_char);
		}
		public static SplitResult Split(this ReadOnlySpan<char> str, char split_char = ';')
		{
			var result = new SplitResult();
			int start = 0;
			int end = 0;
			var length = str.Length;
			while (end <= str.Length)
			{
				end++;
				if (end == length)
				{
					result.Add((start, end - start));
					break;
				}
				if (str[end].Equals(split_char))
				{
					result.Add((start, end - start));
					start = end + 1;
				}
			}
			return result;
		}

		private static List<(int, int)> GetLst()
		{
			lock (lstPool)
			{
				if (lstPool.Count > 0)
				{
					return lstPool.Dequeue();
				}
				return new List<(int, int)>();
			}
		}
		private static void ReturnLst(List<(int, int)> lst)
		{
			lock (lstPool)
			{
				lst.Clear();
				lstPool.Enqueue(lst);
			}
		}
		//这么用IDisposable不太好，但是Span用起来确实比较麻烦
		public struct SplitResult : IDisposable
		{
			public readonly List<(int, int)> SplitIndexLst;
			public int Count => SplitIndexLst.Count;

			public SplitResult()
			{
				SplitIndexLst = GetLst();
			}
			public ReadOnlySpan<char> Get(ReadOnlySpan<char> span, int index)
			{
				var idx = SplitIndexLst[index];
				return span.Slice(idx.Item1, idx.Item2);
			}
			public ReadOnlySpan<char> Get(string str, int index)
			{
				var idx = SplitIndexLst[index];
				return str.AsSpan().Slice(idx.Item1, idx.Item2);
			}
			public void Add((int,int) index)
			{
				SplitIndexLst.Add(index);
			}
			public void Dispose()
			{
				ReturnLst(SplitIndexLst);
			}
		}
	}
}