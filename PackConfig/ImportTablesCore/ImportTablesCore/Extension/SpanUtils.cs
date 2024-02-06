using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables
{
	public static class SpanUtils
	{
		private static Queue<List<int>> lstPool = new Queue<List<int>>();

		public static SplitResult Split(this string str, char split_char = ';')
		{
			return Split(str.AsSpan(), split_char);
		}
		public static SplitResult Split(this ReadOnlySpan<char> str, char split_char = ';')
		{
			var result = new SplitResult();
			int index = 0;
			var length = str.Length;
			while (index < length)
			{
				var idd = str[index];
				if (idd.Equals(split_char))
				{
					result.Add(index);
				}
				index++;
			}
			return result;
		}

		private static List<int> GetLst()
		{
			lock (lstPool)
			{
				if (lstPool.Count > 0)
				{
					return lstPool.Dequeue();
				}
				return new List<int>();
			}
		}
		private static void ReturnLst(List<int> lst)
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
			public List<int> SplitIndexLst = new List<int>();
			public int Count => SplitIndexLst.Count + 1;

			public SplitResult()
			{
				SplitIndexLst = GetLst();
			}
			public ReadOnlySpan<char> Get(ReadOnlySpan<char> span, int index)
			{
				var idx = index == 0 ? -1 : index - 1;
				var start = index == 0 ? 0 : SplitIndexLst[idx] + 1;
				var end = idx + 1 < SplitIndexLst.Count ? SplitIndexLst[idx + 1] - start : span.Length - start;
				return span.Slice(start, end);
			}
			public ReadOnlySpan<char> Get(string str, int index)
			{
				var idx = index == 0 ? -1 : index - 1;
				var start = index == 0 ? 0 : SplitIndexLst[idx] + 1;
				var end = idx + 1 < SplitIndexLst.Count ? SplitIndexLst[idx + 1] - start : str.Length - start;
				return str.AsSpan().Slice(start, end);
			}
			public void Add(int index)
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