using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ImportTables.Utils;

namespace ImportTables
{
	public static class ITPath
	{
		private static ConcurrentDictionary<string, (int, string)> path2HashDic = new ConcurrentDictionary<string, (int, string)>();
		private static ConcurrentDictionary<string, int> pathPrefix2HashDic = new ConcurrentDictionary<string, int>();
		private static ConcurrentDictionary<string, string> pathLowerDic = new ConcurrentDictionary<string, string>();
		private static HashSet<int> pathCodeHash = new HashSet<int>();

		public static (int, string) GetHashCode(string path)
		{
			if (!path2HashDic.TryGetValue(path, out var result))
			{
				var idx = path.LastIndexOf('/');
				var assetPath = path.AsSpan().Slice(0, idx).ToString();
				var assetName = path.AsSpan().Slice(idx + 1, path.Length - idx - 1).ToString();
				if (!pathLowerDic.TryGetValue(assetPath, out var lowerAssetPath))
				{
					lowerAssetPath = assetPath.ToLower();
					pathLowerDic.TryAdd(assetPath, lowerAssetPath);
				}
				if (!pathPrefix2HashDic.TryGetValue(lowerAssetPath, out var hash))
				{
					hash = GetStringHashCode(lowerAssetPath);
					pathPrefix2HashDic.TryAdd(lowerAssetPath, hash);
				}
				result = (hash, assetName);
				path2HashDic.TryAdd(path, result);
			}
			return result;
		}
		private static int GetStringHashCode(string str)
		{
			lock (path2HashDic)
			{
				var hash = Random.Shared.Next(int.MinValue, int.MaxValue);
				while (!pathCodeHash.Add(hash))
				{
					hash = Random.Shared.Next(int.MinValue, int.MaxValue);
				}
				return hash;
			}
		}
		public static void BeforeImport()
		{
			var path = ITConf.Data_Path + "AssetPathHashInfo.bytes";
			if (File.Exists(path))
			{
				var text = Utility.Files.Read(path);
				if (text.Length > 0)
				{
					using (var dataStr = SpanUtils.Split(text, '|'))
					{		
						for (int i = 0; i < dataStr.Count; i++)
						{
							var span = dataStr.Get(text, i);
							using (var dataStr2 = SpanUtils.Split(span, ':'))
							{
								if (dataStr2.Count == 2)
								{
									var assetPath = dataStr2.Get(span, 0).ToString();
									var hash = int.Parse(dataStr2.Get(span, 1));
									pathPrefix2HashDic[assetPath] = hash;
								}
							}
						}
					}
				}
			}
		}
		public static void ExportAllUsedPath()
		{
			var sb = new StringBuilder(512);
			int idx = 0;
			foreach (var item in pathPrefix2HashDic)
			{
				sb.Append(item.Key.ToLower());
				sb.Append(":");
				sb.Append(item.Value);
				idx++;
				if (idx != pathPrefix2HashDic.Count)
				{
					sb.Append("|");
				}
            }
			var path = ITConf.Data_Path + "AssetPathHashInfo.bytes";
			var datas = sb.ToString();
			var arr = datas.ToArray();
			Utility.Files.Write2Cache(path, datas);
			path2HashDic.Clear();
		}
	}
}