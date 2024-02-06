using System;
using System.Collections.Generic;
using System.Collections;
using MiniExcelLibs;
using ImportTablesCore.Excel.Reader;

namespace ExcelReader
{
	public enum EExcelReadType
	{
		ExcelDataReader,
		MiniExcel,
	}
	public static class ExcelReaderFactory
	{
		public static IExcelReader Create(EExcelReadType type)
		{
			IExcelReader reader = null;
			switch (type)
			{
				case EExcelReadType.ExcelDataReader:
					reader = new ExcelDatReader();
					break;
				case EExcelReadType.MiniExcel:
					reader = new MiniDataReader();
					break;
			}
			return reader;
		}
	}
}