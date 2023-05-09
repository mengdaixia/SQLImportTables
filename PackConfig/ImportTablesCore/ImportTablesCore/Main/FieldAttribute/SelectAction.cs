using ExcelReader;
using ImportTables.FieldTypeParse;
using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.Attr
{
	[ITField("select")]
	public class SelectAction : AttrAction
	{
		public override void DoAction(IExcelReader reader, int sub_tab_index, int column, string param)
		{
			//因为多线程，暂时直接new了
			var contentSb = new StringBuilder(50);

			var mainCTabName = reader.MainTabName;
			var subCTabName = reader.GetSubTabName(sub_tab_index);
			var sqlTabName = ITConf.Csv_Conf_Dic[mainCTabName][subCTabName];
			var className = ITConf.CSV_FILE_PREFIX + sqlTabName;
			var firstColumnName = reader.GetValue(sub_tab_index, ITRowConf.FIELD_NAME, 0);
			var firstFieldType = reader.GetValue(sub_tab_index, ITRowConf.FIELD_TYPE, 0);
			var firstFieldName = ITConf.CSV_FIELD_NAME_PREFIX + firstColumnName;
			var firstFieldParse = FieldTypeParseUtils.GetTyepParse(firstFieldType);

			var template = Utility.Files.ReadFromCache(ITConf.Select_Template_Path);
			contentSb.Append(template);

			var type = reader.GetValue(sub_tab_index, ITRowConf.FIELD_TYPE, column);
			var name = reader.GetValue(sub_tab_index, ITRowConf.FIELD_NAME, column);

			contentSb.Replace(ITGeneratedConf.SUB_TAB, sqlTabName);
			contentSb.Replace(ITGeneratedConf.SELECT_FIELD_TYPE, type);
			contentSb.Replace(ITGeneratedConf.SELECT_FIELD_NAME, name);
			contentSb.Replace(ITGeneratedConf.FIRST_FIELD_PARSE, firstFieldParse);
			contentSb.Replace(ITGeneratedConf.CLASS, className);
			contentSb.Replace(ITGeneratedConf.FIRST_FIELD_NAME, firstFieldName);
			contentSb.Enter().Tab().Append(ITGeneratedConf.OTHER);

			var path = ITConf.Csv_Generated_Path + "/" + className + ".cs";
			var content = Utility.Files.ReadFromWriteCache(path);
			content.Replace(ITGeneratedConf.OTHER, contentSb.ToString());
		}
	}
}