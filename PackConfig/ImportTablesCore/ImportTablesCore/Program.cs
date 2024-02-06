using ImportTables.Utils;
using MiniExcelLibs;
using System;
using System.Buffers;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;

namespace ImportTables
{
	public class CDebug : IDebugable
	{
		public void Log(string msg)
		{
			Console.WriteLine(msg);
		}

		public void LogError(string msg)
		{
			var path = Environment.CurrentDirectory + "/Error.txt";
			Utility.Files.Write2Cache(path, msg + "\n");
			Console.WriteLine(msg);
		}
	}
	class Program
	{
		static unsafe void Main(string[] args)
		{
			bool checkFiles = true;
			if (args.Length > 0)
			{
				checkFiles = args[0] == "1";
			}
			try
			{
				var it = ITWorker.Create(new CDebug());
				it.ImportAll(checkFiles);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.ReadLine();
				throw;
			}
			if (checkFiles)
			{
				Console.ReadLine();
			}
		}
	}
}