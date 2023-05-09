using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImportTables.Utils;
using ExcelReader;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;
using System.Xml.Linq;

namespace ImportTables
{
	public class ITWorker
	{
		private Dictionary<string, SQLiteConnection> sqlDic = new Dictionary<string, SQLiteConnection>();
		private Dictionary<string, FileInfo> fileInfoDic = new Dictionary<string, FileInfo>();
		private Dictionary<string, IExcelReader> readerDic = new Dictionary<string, IExcelReader>();
		private List<string> readWorkLst = new List<string>();
		private ITWorkJobQueue workJobQueue;
		private Stopwatch timer;

		public Action OnImportOverAc;
		private ITWorker()
		{
			workJobQueue = new ITWorkJobQueue(this);
		}
		public static ITWorker Create(IDebugable debug)
		{
			Utility.Debug.SetLog(debug);
			return new ITWorker();
		}
		public SQLiteConnection GetSqlCn(string name)
		{
			lock (sqlDic)
			{
				SQLiteConnection result = null;
				if (!sqlDic.TryGetValue(name, out result))
				{
					var path = ITConf.Sqlite_Generated_Path + ITConf.Excel_2_Sqlite_Dic[name] + ".bytes";
					var cnStr = "Data Source=" + path;
					result = new SQLiteConnection(cnStr);
					result.Open();
					sqlDic.Add(name, result);
				}
				return result;
			}
		}
		public IExcelReader GetReader(string tab_name)
		{
			if (!readerDic.TryGetValue(tab_name, out IExcelReader result))
			{
				result = ExcelReaderFactory.Create(EExcelReadType.ExcelDataReader);
			}
			return result;
		}
		public FileInfo GetFileInfo(string tab_name)
		{
			lock(fileInfoDic)
			{
				FileInfo info = null;
				if (!fileInfoDic.TryGetValue(tab_name, out info))
				{
					var filePath = ITConf.Excel_File_Path + tab_name + ".xlsx";
					info = new FileInfo(filePath);
					fileInfoDic.Add(tab_name, info);
				}
				return info;
			}
		}
		#region Import
		public void ImportAll()
		{
			foreach (var item in ITConf.Csv_Conf_Dic)
			{
				readWorkLst.Add(item.Key);
			}
			IntternalImport();
		}
		public void ImportMuls(IEnumerable<string> excel_names)
		{
			foreach (var item in excel_names)
			{
				readWorkLst.Add(item);
			}
			IntternalImport();
		}
		public void ImportOne(string excel_name)
		{
			readWorkLst.Add(excel_name);
			IntternalImport();
		}
		#endregion
		private void IntternalImport()
		{
			OnImportBegin();
			workJobQueue.EnququeJob(readWorkLst);
			while (true)
			{
				workJobQueue.Update();
				if (workJobQueue.IsDone)
				{
					break;
				}
			}
			OnImportOver();
		}
		private void OnImportBegin()
		{
			timer = new Stopwatch();
			timer.Start();
			readWorkLst.Sort((c1, c2) =>
			{
				var f1 = GetFileInfo(c1);
				var f2 = GetFileInfo(c2);
				return f2.Length.CompareTo(f1.Length);
			});
		}
		private void OnImportOver()
		{
			Utility.Files.WriteAllFiles();
			Utility.Files.ClearAllReadCache();
			fileInfoDic.Clear();
			readWorkLst.Clear();
			readerDic.Clear();
			foreach (var item in sqlDic)
			{
				item.Value.Close();
			}
			sqlDic.Clear();
			OnImportOverAc?.Invoke();
			Utility.Prefers.Flush();
			timer.Stop();
			Utility.Debug.Log("导表成功:" + timer.ElapsedMilliseconds);

			Utility.Profiler.DebugMulProfiler();
		}
	}
}