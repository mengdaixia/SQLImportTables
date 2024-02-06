using System;
using System.IO;
using ExcelReader;
using MiniExcelLibs;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MiniExcelLibs.OpenXml;
using System.Dynamic;

namespace ImportTablesCore.Excel.Reader
{
	internal class MiniDataReader : IExcelReader
	{
		private string[][][] dataArr;
		private Stream fileStream;
		private string mainTabName = null;
		private List<string> sheetLst;
		private ReadAsyncOptions readOptions;

		string IExcelReader.MainTabName => mainTabName;

		int IExcelReader.TableCount => sheetLst.Count;

		int IExcelReader.GetColCount(int tab_index)
		{
			return fileStream.GetColumns(true, sheetLst[tab_index]).Count;
		}
		int IExcelReader.GetRowCount(int tab_index)
		{
			return fileStream.Query(false, sheetLst[tab_index]).Count();
		}
		string IExcelReader.GetSubTabName(int tab_index)
		{
			return sheetLst[tab_index];
		}
		string IExcelReader.GetValue(int tab_index, int row, int col)
		{
			if (dataArr[tab_index] == null)
			{
				var rows = fileStream.Query(false, sheetLst[tab_index]);
				var count = rows.Count();
				dataArr[tab_index] = new string[count][];
			}
			if (dataArr[tab_index][row] == null)
			{
				var rows = fileStream.Query(false, sheetLst[tab_index]).ToList();
				var rowData = rows[row];
				int idx = 0;
				foreach (var item in rowData)
				{
					idx++;
				}
				dataArr[tab_index][row] = new string[idx];
				idx = 0;
				foreach (var item in rowData)
				{
					var val = item.Value != null ? item.Value.ToString() : "";
					dataArr[tab_index][row][idx++] = val;
				}
			}
			var result = dataArr[tab_index][row][col];
			return result != null ? result : "";
		}
		void IExcelReader.Read(string path, int start_index)
		{
			mainTabName = Path.GetFileNameWithoutExtension(path);
			fileStream = File.OpenRead(path);
			sheetLst = MiniExcel.GetSheetNames(fileStream);
			dataArr = new string[sheetLst.Count][][];
			for (int i = 0; i < sheetLst.Count; i++)
			{
				readOptions.OnReadSubTabStartAc?.Invoke(mainTabName, i);
				readOptions.OnReadSubTabEndAc?.Invoke(mainTabName, i);
			}
			readOptions.OnReadFinishAc?.Invoke(mainTabName);
		}
		void IExcelReader.ReadAsync(string path, int start_index, ReadAsyncOptions options)
		{
			readOptions = options;
			(this as IExcelReader).Read(path, start_index);
		}
		void IExcelReader.Close()
		{
			mainTabName = null;
			sheetLst = null;
			fileStream.Dispose();
			fileStream?.Close();
			fileStream = null;
		}
	}
}