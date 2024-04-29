using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ImportTables.Utils;
using ExcelReader;
using ImportTables.FieldTypeParse;
using ImportTables.Attr;
using System.IO;
using ImportTablesCore.Utility.Bytes;
using ImportTablesCore.Utility.Pool;
using static ImportTables.Utils.Utility;

namespace ImportTables
{
	internal class ITWorkJob
	{
		private ITWorker mgr;
		private Thread workThread;
		private Queue<(string TabName, int Index)> jobQueue = new Queue<(string TabName, int Index)>();
		private bool isReading = false;

		private Dictionary<string, int> sub2RealIndexDic = new Dictionary<string, int>();

		private StringBuilder tempSb = new StringBuilder(100);
		private StringBuilder tempSb2 = new StringBuilder(100);
		private List<int> selectLst = new List<int>();
		private BytesWrite bytesWirte = new BytesWrite(100);

		private bool isTheadDone = false;
		private FieldTypeParser[] parserArr = new FieldTypeParser[20];
		private IExcelReader currReader;

		private int currSubTabIndex;
		private int currRow;
		private int currCow;
		private string currValue;

		private ReadAsyncOptions readOptions = new ReadAsyncOptions();
		public bool IsDone => !isReading && jobQueue.Count == 0;
		public ITWorkJob(ITWorker mg)
		{
			mgr = mg;
			mgr.OnImportOverAc += Stop;
			readOptions.OnReadSubTabStartAc += OnReadSubTabStartHandler;
			readOptions.OnReadSubTabEndAc += OnReadSubTabEndHandler;
			readOptions.OnReadFinishAc += OnReadMainTabFinishHandler;
		}
		private void Start()
		{
			isTheadDone = false;
			workThread = new Thread(() =>
			{
				while (true)
				{
					if (isTheadDone)
					{
						break;
					}
					lock (jobQueue)
					{
						while (jobQueue.Count > 0)
						{
							var job = jobQueue.Peek();
							try
							{
								HandleRunningData(job.TabName, job.Index);
								jobQueue.Dequeue();
							}
							catch (Exception e)
							{
								Utility.Debug.LogError(currReader.MainTabName + "-" + currReader.GetSubTabName(currSubTabIndex) + "-第" + (currRow + 1) + "行第" + (currCow + 1) + "列数据-" + (currValue) + "解析错误!");
								Utility.Debug.LogError(e.ToString());
								throw;
							}
						}
					}
				}
			});
			workThread.Start();
		}
		public void Start(IExcelReader reader, string main_tab_name)
		{
			var fileInfo = mgr.GetFileInfo(main_tab_name);
			currReader = reader;
			isReading = true;
			sub2RealIndexDic.Clear();
			currReader.ReadAsync(fileInfo.FullName, ITRowConf.DATA_BEGIN, readOptions);
			Start();
		}
		private void EnqueueJob(string tab_name, int index)
		{
			var subCTabName = currReader.GetSubTabName(index);
			if (ITConf.Csv_Conf_Dic[currReader.MainTabName].ContainsKey(subCTabName))
			{
				lock (jobQueue)
				{
					if (!sub2RealIndexDic.TryGetValue(currReader.MainTabName, out var idx))
					{
						sub2RealIndexDic[currReader.MainTabName] = 0;
					}
					else
					{
						sub2RealIndexDic[currReader.MainTabName]++;
					}
					jobQueue.Enqueue((tab_name, index));
				}
			}
		}
		private unsafe void HandleRunningData(string tab_name, int index)
		{
			currSubTabIndex = index;
			var sql = mgr.GetSqlCn(tab_name);
			var subCTabName = currReader.GetSubTabName(index);

			var sqlTabName = ITConf.Csv_Conf_Dic[tab_name][subCTabName];
			var colCount = currReader.GetColCount(index);
			selectLst.Clear();

			for (int i = 1; i < colCount; i++)
			{
				var isSelect = AttrUtils.IsDefine(currReader, index, i, AttrUtils.SELECT_DATA);
				if (isSelect)
				{
					selectLst.Add(i);
				}
			}
			var freeLst = ListPool<byte[]>.Get();
			var rowCount = currReader.GetRowCount(index);
			if (rowCount >= ITRowConf.DATA_BEGIN)
			{
				//删除表
				Utility.Sqlite.DropTable(sql, sqlTabName);

				tempSb.Clear();
				//创建表
				var cmd = sql.CreateCommand();
				var firstFiledName = currReader.GetValue(index, ITRowConf.FIELD_NAME, 0);
				tempSb.Append(firstFiledName).Append(" text, Bytes blog");

				for (int i = 0; i < selectLst.Count; i++)
				{
					var fName = currReader.GetValue(index, ITRowConf.FIELD_NAME, selectLst[i]);
					tempSb.Append(", ").Append(fName).Append(" text");
				}
				cmd.CommandText = "create table if not exists " + sqlTabName + "(" + tempSb.ToString() + ")";
				cmd.ExecuteNonQuery();

				var trans = sql.BeginTransaction();
				cmd = sql.CreateCommand();
				cmd.Transaction = trans;
				var param = cmd.CreateParameter();
				cmd.Parameters.Add(param);

				//插入数据
				for (int i = ITRowConf.DATA_BEGIN; i < rowCount; i++)
				{
					tempSb.Clear();
					currRow = i;
					var fValue = currReader.GetValue(index, i, 0);
					if (string.IsNullOrEmpty(fValue))
					{
						continue;
					}
					#region 查错
					currCow = 0;
					currValue = fValue;
					var type = currReader.GetValue(currSubTabIndex, ITRowConf.FIELD_TYPE, 0);
					var isInt = type == "int";
					if (isInt)
					{
						int.Parse(fValue);
					}
					#endregion

					bytesWirte.SetPosition(0);
					WriteBytes(currReader, index, i);

					var bytes = NativeArray.Alloc<byte>(bytesWirte.Length);
					bytesWirte.Buffer.AsSpan().Slice(0, bytesWirte.Length).CopyTo(bytes);
					freeLst.Add(bytes);

					param.ParameterName = "@bytes";
					param.DbType = System.Data.DbType.Binary;
					param.Value = bytes;
					tempSb.Append("insert into ").Append(sqlTabName).Append(" values(\'").Append(fValue).Append("\', @bytes");
					for (int j = 0; j < selectLst.Count; j++)
					{
						var sValue = currReader.GetValue(index, i, selectLst[j]);
						tempSb.Append(", \'").Append(sValue).Append("\'");
					}
					tempSb.Append(")");
					cmd.CommandText = tempSb.ToString();
					cmd.ExecuteNonQueryAsync();
				}
				trans.Commit();
				trans.Dispose();
			}
			foreach (var item in freeLst)
			{
				NativeArray.Free(item);
			}
			freeLst.Clear();
			GenerateCSVFile(currReader, tab_name, index);
			for (int i = 0; i < colCount; i++)
			{
				AttrUtils.DoFieldAttrAction(currReader, index, i);
			}

			RecycleParser();
			if (sub2RealIndexDic[tab_name] == ITConf.Csv_Conf_Dic[currReader.MainTabName].Count - 1)
			{
				Utility.Debug.Log("导出:" + tab_name);
				var lTicks = mgr.GetFileInfo(tab_name).LastWriteTime.Ticks.ToString();
				Utility.Prefers.SetString(ITConf.FILE_LAST_CHANGE_TIME_KEY + tab_name, lTicks);
			}
		}
		private void WriteBytes(IExcelReader reader, int tab_index, int row)
		{
			var cCount = reader.GetColCount(tab_index);
			for (int i = 1; i < cCount; i++)
			{
				currCow = i;
				currValue = reader.GetValue(tab_index, row, i);
				var type = reader.GetValue(tab_index, ITRowConf.FIELD_TYPE, i);
				var name = reader.GetValue(tab_index, ITRowConf.FIELD_NAME, i);
				bool isIgnore = AttrUtils.IsDefine(reader, tab_index, i, AttrUtils.IGNORE_DATA);
				var isSelect = AttrUtils.IsDefine(reader, tab_index, i, AttrUtils.SELECT_DATA);
				if (type.Length * name.Length == 0 || isIgnore || isSelect)
				{
					continue;
				}

				var tParser = GetParser(i, type);
				tParser.Write(currValue, currValue.AsSpan(), bytesWirte);
			}
		}
		private void GenerateCSVFile(IExcelReader reader, string tab_name, int tab_index)
		{
			var fileContent = Utility.Files.ReadFromCache(ITConf.CSV_Template_Path);
			tempSb.Clear();
			tempSb.Append(fileContent);
			var mainCTabName = tab_name;
			var subCTabName = reader.GetSubTabName(tab_index);
			var sqlName = ITConf.Excel_2_Sqlite_Dic[mainCTabName];
			var sqlTabName = ITConf.Csv_Conf_Dic[mainCTabName][subCTabName];
			var className = ITConf.CSV_FILE_PREFIX + sqlTabName;
			var dataTotolCount = (reader.GetRowCount(tab_index) - ITRowConf.DATA_BEGIN).ToString();
			var firstColumnName = reader.GetValue(tab_index, ITRowConf.FIELD_NAME, 0);
			var firstFieldType = reader.GetValue(tab_index, ITRowConf.FIELD_TYPE, 0);
			var firstFieldName = ITConf.CSV_FIELD_NAME_PREFIX + firstColumnName;
			var firstFieldParse = FieldTypeParseUtils.GetTyepParse(firstFieldType);
			//主表中文名
			tempSb.Replace(ITGeneratedConf.MAIN_C_TAB, mainCTabName);
			//子表中文名
			tempSb.Replace(ITGeneratedConf.SUB_C_TAB, subCTabName);
			//数据库名
			tempSb.Replace(ITGeneratedConf.MAIN_TAB, sqlName);
			//数据库表名
			tempSb.Replace(ITGeneratedConf.SUB_TAB, sqlTabName);
			//数据总行数
			tempSb.Replace(ITGeneratedConf.TOTOL_COUNT, dataTotolCount);
			//类名
			tempSb.Replace(ITGeneratedConf.CLASS, className);
			//首列字段类型
			tempSb.Replace(ITGeneratedConf.FIRST_FIELD_TYPE, firstFieldType);
			//首列名
			tempSb.Replace(ITGeneratedConf.FIRST_COLUMN_NAME, firstColumnName);
			//首列字段名
			tempSb.Replace(ITGeneratedConf.FIRST_FIELD_NAME, firstFieldName);
			//首列字段解析
			tempSb.Replace(ITGeneratedConf.FIRST_FIELD_PARSE, firstFieldParse);

			tempSb2.Clear();
			var cCount = reader.GetColCount(tab_index);
			for (int i = 0; i < cCount; i++)
			{
				var type = reader.GetValue(tab_index, ITRowConf.FIELD_TYPE, i);
				var name = reader.GetValue(tab_index, ITRowConf.FIELD_NAME, i);
				bool isIgnore = AttrUtils.IsDefine(reader, tab_index, i, AttrUtils.IGNORE_DATA);
				if (type.Length * name.Length == 0 || isIgnore)
				{
					continue;
				}
				var tParser = GetParser(i, type);
				tempSb2.Append("private ");
				tParser.ReadFieldTypeName(tempSb2);
				tempSb2.Append(" ").Append(ITConf.CSV_FIELD_NAME_PREFIX);
				tempSb2.Append(name).Append(";\n").Tab();
			}
			//私有字段
			tempSb.Replace(ITGeneratedConf.PRIVATE_FIELDS, tempSb2.ToString());
			tempSb2.Clear();

			for (int i = 0; i < cCount; i++)
			{
				var type = reader.GetValue(tab_index, ITRowConf.FIELD_TYPE, i);
				var name = reader.GetValue(tab_index, ITRowConf.FIELD_NAME, i);
				bool isIgnore = AttrUtils.IsDefine(reader, tab_index, i, AttrUtils.IGNORE_DATA);
				bool isSelect = AttrUtils.IsDefine(reader, tab_index, i, AttrUtils.SELECT_DATA);
				if (type.Length * name.Length == 0 || isIgnore)
				{
					continue;
				}
				var tParser = GetParser(i, type);
				tempSb2.Append("public ");
				tParser.ReadFieldTypeName(tempSb2);
				tParser.ReadFieldPreName(tempSb2.Space());
				tempSb2.Append(name).Space();
				tempSb2.Append("{ get { ");
				if (i != 0 && !isSelect)
				{
					tempSb2.Append("Deserialized(); ");
				}
				tempSb2.Append("return ");
				tempSb2.Append(ITConf.CSV_FIELD_NAME_PREFIX);
				tempSb2.Append(name).Append("; } }");
				tempSb2.Append("\n").Tab();
			}

			//共有属性
			tempSb.Replace(ITGeneratedConf.PUBLIC_PROPERTYS, tempSb2.ToString());
			tempSb2.Clear();

			for (int i = 1; i < cCount; i++)
			{
				var type = reader.GetValue(tab_index, ITRowConf.FIELD_TYPE, i);
				var name = reader.GetValue(tab_index, ITRowConf.FIELD_NAME, i);
				bool isIgnore = AttrUtils.IsDefine(reader, tab_index, i, AttrUtils.IGNORE_DATA);
				var isSelect = AttrUtils.IsDefine(reader, tab_index, i, AttrUtils.SELECT_DATA);
				if (type.Length * name.Length == 0 || isIgnore || isSelect)
				{
					continue;
				}
				var tParser = GetParser(i, type);
				tempSb2.Append(ITConf.CSV_FIELD_NAME_PREFIX);
				tempSb2.Append(name).Append(" = reader.Read");
				tParser.ReadMethodStr(tempSb2);
				tempSb2.Append("();");
				tempSb2.Enter().Tab(3);
			}

			//反序列化
			tempSb.Replace(ITGeneratedConf.DESERIALISED, tempSb2.ToString());

			//Select初始化
			tempSb2.Clear();
			for (int i = 0; i < selectLst.Count; i++)
			{
				var type = reader.GetValue(tab_index, ITRowConf.FIELD_TYPE, selectLst[i]);
				var name = reader.GetValue(tab_index, ITRowConf.FIELD_NAME, selectLst[i]);
				var tParser = GetParser(selectLst[i], type);

				//只处理了int和string，应该用不到其他类型了
				tempSb2.Append(ITConf.CSV_FIELD_NAME_PREFIX).Append(name).Append(" = ");
				if (type == "int")
				{
					tempSb2.Append("int.Parse(").Append("reader.GetString(").Append(2 + i).Append("));\n").Tab(2);
				}
				else
				{
					tempSb2.Append("reader.GetString(").Append(2 + i).Append(");\n").Tab(2);
				}
			}
			tempSb.Replace(ITGeneratedConf.DESERIALISE_SELECT, tempSb2.ToString());
			Utility.Files.Write2Cache(ITConf.Csv_Generated_Path + "/" + className + ".cs", tempSb.ToString());

			var cusFileName = ITConf.Csv_Custom_Path + "/" + className + ".cs";
			if (!File.Exists(cusFileName))
			{
				var content = Utility.Files.ReadFromCache(ITConf.CSV_Custom_Template_Path);
				content = content.Replace(ITGeneratedConf.CLASS, className);
				Utility.Files.Write2Cache(cusFileName, content);
			}
		}
		private FieldTypeParser GetParser(int colume, string type)
		{
			if (parserArr.Length <= colume)
			{
				var newArr = new FieldTypeParser[parserArr.Length * 2];
				Array.Copy(parserArr, newArr, parserArr.Length);
				parserArr = newArr;
			}
			if (parserArr[colume] == null)
			{
				parserArr[colume] = FieldTypeParseUtils.GetParser(type);
			}
			return parserArr[colume];
		}
		private void RecycleParser()
		{
			for (int i = 0; i < parserArr.Length; i++)
			{
				if (parserArr[i] != null)
				{
					FieldTypeParseUtils.Recycle(parserArr[i]);
					parserArr[i] = null;
				}
			}
		}
		public void Stop()
		{
			RecycleParser();
			jobQueue.Clear();
			isTheadDone = true;
			isReading = false;
			workThread = null;
		}

		#region Handler
		private void OnReadSubTabStartHandler(string tab_name, int index)
		{

		}
		private void OnReadSubTabEndHandler(string tab_name, int index)
		{
			EnqueueJob(tab_name, index);
		}
		private void OnReadMainTabFinishHandler(string main_tab_name)
		{
			isReading = false;
		}
		#endregion
	}
}