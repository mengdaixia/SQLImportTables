using System;
using System.Collections;

namespace ImportTables.Utils
{
	public interface IDebugable
	{
		void Log(string msg);
		void LogError(string msg);
	}
	public static partial class Utility
	{
		public static class Debug
		{
			private static IDebugable debug;

			public static void SetLog(IDebugable l)
			{
				debug = l;
			}
			public static void Log(string msg)
			{
				debug?.Log(msg);
			}
			public static void LogError(string msg)
			{
				debug?.LogError(msg);
			}
		}
	}
}