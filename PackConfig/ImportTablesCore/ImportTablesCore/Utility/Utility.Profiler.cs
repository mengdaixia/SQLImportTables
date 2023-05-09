using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImportTables.Utils
{
	public static partial class Utility
	{
		public static class Profiler
		{
			private static Dictionary<string, Stopwatch> mulWatchDic = new Dictionary<string, Stopwatch>();
			private static Dictionary<string, Stopwatch> watchDic = new Dictionary<string, Stopwatch>();
			public static void BeginProfiler(string name)
			{
				var sw = new Stopwatch();
				watchDic[name] = sw;
				sw.Start();
			}
			public static void EndProfiler(string name)
			{
				watchDic[name].Stop();
				Utility.Debug.Log(name + ":" + watchDic[name].ElapsedMilliseconds);
				watchDic.Remove(name);
			}
			public static void BeginMulProfiler(string name)
			{
				if (!mulWatchDic.TryGetValue(name, out Stopwatch sw))
				{
					sw = new Stopwatch();
					mulWatchDic[name] = sw;
				}
				sw.Start();
			}
			public static void EndMulProfiler(string name)
			{
				if (mulWatchDic.TryGetValue(name, out Stopwatch sw))
				{
					sw.Stop();
				}
			}
			public static void DebugMulProfiler()
			{
				foreach (var item in mulWatchDic)
				{
					Utility.Debug.Log(item.Key + ":" + mulWatchDic[item.Key].ElapsedMilliseconds);
				}
			}
		}
	}
}