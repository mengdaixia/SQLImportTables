using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTablesCore.Utility.Pool
{
	internal static class ListPool<T>
	{
		private static ConcurrentQueue<List<T>> queues = new ConcurrentQueue<List<T>>();
		public static List<T> Get()
		{
			if (queues.Count == 0 || !queues.TryDequeue(out var result))
			{
				return new List<T>();
			}
			return result;
		}
		public static void Release(List<T> lst)
		{
			queues.Enqueue(lst);
		}
	}
}