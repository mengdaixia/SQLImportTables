using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using Microsoft.Win32;

namespace ImportTables.Utils
{
	public static partial class Utility
	{
		public static class Prefers
		{
			private const string MAIN_KEY = "IMPORTABLES";
			private const string SUB_KEY = "PREFERS";
			private static string PATH;
			private static Dictionary<string, string> valueDic = new Dictionary<string, string>();
			static Prefers()
			{
				PATH = Environment.CurrentDirectory + "/ITPrefers.txt";
				if (File.Exists(PATH))
				{
					var dat = File.ReadAllText(PATH);
					var line = dat.Split('\n');
					for (int i = 0; i < line.Length; i++)
					{
						var datas = line[i].Split('-');
						valueDic[datas[0]] = datas[1];
					}
				}
			}
			public static string GeString(string key, string default_value = "")
			{
				//RegistryKey html = Registry.CurrentUser;
				//var main = html.CreateSubKey(MAIN_KEY);
				//var sub = main.CreateSubKey(SUB_KEY);
				//return sub.GetValue(key, default_value).ToString();
				lock (valueDic)
				{
					valueDic.TryGetValue(key, out default_value);
					return default_value;
				}
			}
			public static void SetString(string key, string value)
			{
				//RegistryKey html = Registry.CurrentUser;
				//var main = html.CreateSubKey(MAIN_KEY);
				//var sub = main.CreateSubKey(SUB_KEY);
				//sub.SetValue(key, value);
				lock (valueDic)
				{
					valueDic[key] = value;
				}
			}
			public static void Flush()
			{
				if (File.Exists(PATH))
				{
					File.Delete(PATH);
				}
				var fs = File.Create(PATH);
				var sw = new StreamWriter(fs);

				var str = "";
				int index = 0;
				foreach (var item in valueDic)
				{
					index++;
					str += item.Key + "-" + item.Value;
					if (index != valueDic.Count)
					{
						str += "\n";
					}
				}

				sw.Write(str);
				sw.Dispose();
				fs.Dispose();
				File.SetAttributes(PATH, FileAttributes.Hidden);
			}
		}
	}
}