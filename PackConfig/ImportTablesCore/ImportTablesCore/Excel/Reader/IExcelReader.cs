using System;
using ExcelDataReader;

namespace ExcelReader
{
	public class ReadAsyncOptions
	{
		public Action<string, int> OnReadSubTabStartAc;
		public Action<string, int> OnReadSubTabEndAc;
		public Action<string> OnReadFinishAc;
	}
	public interface IExcelReader
	{
		string MainTabName { get; }
		int TableCount { get; }
		string GetSubTabName(int tab_index);
		//横竖都以第一列第一排为准
		int GetColCount(int tab_index);
		int GetRowCount(int tab_index);
		string GetValue(int tab_index, int row, int col);
		//start_index可以用于多表融合，方便多人协同配表，防止冲突
		void Read(string path, int start_index);
		void ReadAsync(string path, int start_index, ReadAsyncOptions options);
	}
}