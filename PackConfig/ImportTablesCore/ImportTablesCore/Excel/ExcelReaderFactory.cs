using System;
using System.Collections.Generic;
using System.Collections;

namespace ExcelReader
{
	public enum EExcelReadType
	{
		ExcelDataReader,
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
			}
			return reader;
		}
	}
}