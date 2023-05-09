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
	[ITField("enum")]
	public class EnumAction : AttrAction
	{
		public override void DoAction(IExcelReader reader, int sub_tab_index, int column, string param)
		{
			var datas = param.Split('_');
			if (datas.Length != 2)
			{
				return;
			}
			var enumName = datas[0];
			var addCol = 0;
			if (!int.TryParse(datas[1], out addCol))
			{
				return;
			}
			if (addCol + column < 0)
			{
				return;
			}
			//因为多线程，暂时直接new了
			var contentSb = new StringBuilder(50);
			var tempSb = new StringBuilder(50);
			var enumHash = new HashSet<string>();

			var content = Utility.Files.ReadFromCache(ITConf.Enum_Template_Path);
			enumHash.Clear();
			tempSb.Clear();
			contentSb.Clear();
			contentSb.Append(content);
			contentSb.Replace(ITGeneratedConf.ENUM_NAME, enumName);

			var rowCount = reader.GetRowCount(sub_tab_index);
			for (int i = ITRowConf.DATA_BEGIN; i < rowCount; i++)
			{
				var enumValue = reader.GetValue(sub_tab_index, i, column);
				if (enumValue.Length == 0)
				{
					continue;
				}
				var enumKey = reader.GetValue(sub_tab_index, i, column + addCol);
				if (enumHash.Add(enumKey))
				{
					tempSb.Append(enumKey).Equal2Space().Append(enumValue).Append(",").Enter().Tab(1);
				}
			}
			contentSb.Replace(ITGeneratedConf.ENUMS, tempSb.ToString());
			Utility.Files.Write2Cache(ITConf.Common_Generated_Path + "/" + enumName + ".cs", contentSb.ToString());


			var mainCTabName = reader.MainTabName;
			var subCTabName = reader.GetSubTabName(sub_tab_index);
			var fieldName = reader.GetValue(sub_tab_index, ITRowConf.FIELD_NAME, column);
			var sqlTabName = ITConf.Csv_Conf_Dic[mainCTabName][subCTabName];
			var className = ITConf.CSV_FILE_PREFIX + sqlTabName;
			var type = reader.GetValue(sub_tab_index, ITRowConf.FIELD_TYPE, column);
			var path = ITConf.Csv_Generated_Path + "/" + className + ".cs";

			tempSb.Clear();
			var parser = FieldTypeParseUtils.GetParser(type);
			tempSb.Append("public ").Append(datas[0]).Space().Append(fieldName).Append(" => ").Append("(").Append(datas[0]).Append(")");
			parser.ReadFieldPreName(tempSb);
			tempSb.Append(fieldName).Append(";");
			tempSb.Enter().Tab().Append(ITGeneratedConf.OTHER);
			FieldTypeParseUtils.Recycle(parser);
			var csContent = Utility.Files.ReadFromWriteCache(path);
			csContent.Replace(ITGeneratedConf.OTHER, tempSb.ToString());
		}
	}
}