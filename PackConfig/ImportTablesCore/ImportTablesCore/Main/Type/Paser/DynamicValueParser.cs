using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class DynamicValueParser : FieldTypeParser
	{
		private static string PreName = "dyn";

		private (string LType, string CType) fieldConf;
		private static Dictionary<string, (string Type, string CType)> tDic = new Dictionary<string, (string, string)>();
		private FieldTypeParser container;
		public override void SetConf(string type_str)
		{
			if (!tDic.TryGetValue(type_str, out fieldConf))
			{
				var datas = type_str.Split(':');
				var cType = datas[1];
				fieldConf = (string.Format("DynamicValue<{0}>", cType), cType);
				tDic[type_str] = fieldConf;
			}
			container = FieldTypeParseUtils.GetParser(fieldConf.CType);
		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append(PreName);
		}
		public override void Write(ReadOnlySpan<char> value_str, BytesWrite write)
		{
			//DefaultValueCheck(EFieldType.Int, ref value_str);
			//var dataStr = value_str.Split(';');
			//var length = dataStr.Length;
			//write.Write((short)length);
			//for (int i = 0; i < dataStr.Length; i++)
			//{
			//	container.Write(dataStr[i], write);
			//}
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append(fieldConf.CType.ToUpperFirst()).Append("DynamicValue");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("DynamicValue").Append("");
		}
	}
}