using System;
using System.Collections.Generic;
using System.Collections;
using ImportTables.Utils;
using System.Threading;
using System.IO;

namespace ExcelReader
{
	public class ExcelDatReader : IExcelReader
	{
		private string[][][] datasArr;
		private string mainTabName;
		private string[] subTanNameArr;
		private ReadAsyncOptions readOptions;
		public int TableCount => datasArr.Length;
		string IExcelReader.MainTabName => mainTabName;

		public ExcelDatReader()
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
		}
		public string GetSubTabName(int tab_index)
		{
			return subTanNameArr[tab_index];
		}
		public int GetColCount(int tab_index)
		{
			return datasArr[tab_index][0].Length;
		}
		public int GetRowCount(int tab_index)
		{
			return datasArr[tab_index].Length;
		}
		public string GetValue(int tab_index, int row, int col)
		{
			return datasArr[tab_index][row][col];
		}
		public void Read(string path, int start_index)
		{
			mainTabName = Path.GetFileNameWithoutExtension(path);
			using (var fsr = new FileSafetyReader(path))
			{
				var reader = ExcelDataReader.ExcelReaderFactory.CreateOpenXmlReader(fsr.Fs, new ExcelDataReader.ExcelReaderConfiguration() { LeaveOpen = true });
				var tabCount = reader.ResultsCount;
				datasArr = new string[tabCount][][];
				subTanNameArr = new string[tabCount];
				for (int i = 0; i < tabCount; i++)
				{
					readOptions?.OnReadSubTabStartAc?.Invoke(mainTabName, i);
					var rowCount = reader.RowCount;
					var cCount = reader.FieldCount;
					var rowArr = new string[rowCount][];
					datasArr[i] = rowArr;
					for (int j = 0; j < rowCount; j++)
					{
						reader.Read();
						var cowArr = new string[cCount];
						rowArr[j] = cowArr;
						for (int k = 0; k < cCount; k++)
						{
							var value = reader.GetValue(k);
							cowArr[k] = value != null ? value.ToString() : "";
						}
					}
					subTanNameArr[i] = reader.Name;
                    readOptions?.OnReadSubTabEndAc?.Invoke(mainTabName, i);
					reader.NextResult();
				}
				readOptions?.OnReadFinishAc?.Invoke(mainTabName);
			}
		}
		public void ReadAsync(string path, int start_index, ReadAsyncOptions options)
		{
			readOptions = options;
			void IntnernalRead()
			{
				Read(path, start_index);
			};
			//IntnernalRead();
			var thread = new Thread(IntnernalRead);
			thread.Start();
		}
	}
}