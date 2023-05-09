using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class CSVAttribute : Attribute
{
	public string MainTab { get; private set; }
	public string SubTab { get; private set; }
	public CSVAttribute(string main_tab, string sub_tab)
	{
		MainTab = main_tab;
		SubTab = sub_tab;
	}
}
public static class SqlUtils
{
	private static Dictionary<string, SqliteConnection> path2CnDic = new Dictionary<string, SqliteConnection>();
	private static string sqlitePath;

	static SqlUtils()
	{
		sqlitePath = Application.persistentDataPath + "/Res/Sqlite/";
		if (!Directory.Exists(sqlitePath))
		{
			Directory.CreateDirectory(sqlitePath);
		}
	}
	public static SqliteConnection GetSqlCn(string name)
	{
		SqliteConnection result = null;
		if (path2CnDic.TryGetValue(name, out result))
		{
			return result;
		}

		var path = Application.streamingAssetsPath + "/Sqlite/" + name;
		var readPath = sqlitePath + name;
		WWW loadDB = loadDB = new WWW(path);
		while (!loadDB.isDone)
		{

		}
		if (File.Exists(readPath))
		{
			File.Delete(readPath);
		}
		var fs = File.Create(readPath);
		fs.Write(loadDB.bytes, 0, loadDB.bytes.Length);
		fs.Close();

		switch (Application.platform)
		{
			case RuntimePlatform.WindowsEditor:
				result = new SqliteConnection("data source =" + readPath);
				break;
			case RuntimePlatform.Android:
				result = new SqliteConnection("URI=file:" + readPath);
				break;
			case RuntimePlatform.IPhonePlayer:
				result = new SqliteConnection("data source =" + readPath);
				break;
		}
		result.Open();
		path2CnDic[name] = result;
		return result;
	}
	public static void Close()
	{
		foreach (var item in path2CnDic)
		{
			item.Value.Close();
		}
		path2CnDic.Clear();
	}
}