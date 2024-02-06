using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ImportTables.Utils
{
	//检测路径是否直接存在，不存在则创建
	public class DirectoryCheckAttribute : Attribute { };
	public static partial class Utility
	{
		public static class Files
		{
			private static Dictionary<string, string> readCacheDic = new Dictionary<string, string>();
			private static Dictionary<string, StringBuilder> path2ContentDic = new Dictionary<string, StringBuilder>();
			static Files()
			{

			}
			public static void Write2Cache(string path, string content)
			{
				lock (path2ContentDic)
				{
					if (!path2ContentDic.TryGetValue(path, out StringBuilder sb))
					{
						sb = new StringBuilder();
						path2ContentDic[path] = sb;
					}
					sb.Append(content);
				}
			}
			public static void WriteAllFiles()
			{
				var utf8NoBom = new UTF8Encoding(false);
				foreach (var item in path2ContentDic)
				{
					var path = item.Key;
					var content = item.Value;
					if (File.Exists(path))
					{
						File.Delete(path);
					}
					var fs = File.Create(path);
					var sw = new StreamWriter(fs, utf8NoBom);
					sw.Write(content);
					sw.Dispose();
					fs.Dispose();
				}
				path2ContentDic.Clear();
			}
			public static StringBuilder ReadFromWriteCache(string path)
			{
				lock (path2ContentDic)
				{
					if (path2ContentDic.TryGetValue(path, out StringBuilder result))
					{
						return result;
					}
					return null;
				}
			}
			public static string ReadFromCache(string path)
			{
				lock (readCacheDic)
				{
					string result;
					if (!readCacheDic.TryGetValue(path, out result))
					{
						result = Read(path);
						readCacheDic[path] = result;
					}
					return result;
				}
			}
			public static void ClearAllReadCache()
			{
				readCacheDic.Clear();
			}
			public static string Read(string path)
			{
				if (!File.Exists(path))
				{
					Debug.LogError("不存在文件路径:" + path);
					return "";
				}
				using (FileStream fs = File.Open(path, FileMode.Open))
				{
					using (var sw = new StreamReader(fs, Encoding.UTF8))
					{
						return sw.ReadToEnd();
					}
				}
			}
			public static string ReadSafety(string path)
			{
				if (!File.Exists(path))
				{
					Debug.LogError("不存在文件路径:" + path);
					return "";
				}
				using (FileSafetyReader fsr = new FileSafetyReader(path))
				{
					using (var sw = new StreamReader(fsr.Fs, Encoding.UTF8))
					{
						return sw.ReadToEnd();
					}
				}
			}
			//仅适用于静态成员
			public static void DirectoryCheck(Type type)
			{
				var pros = type.GetProperties();
				for (int i = 0; i < pros.Length; i++)
				{
					if (pros[i].IsDefined(typeof(DirectoryCheckAttribute), false))
					{
						var value = pros[i].GetValue(null);
						if (value != null)
						{
							var path = value.ToString();
							if (!Directory.Exists(path))
							{
								Directory.CreateDirectory(path);
							}
						}
					}
				}
			}
		}
	}
}