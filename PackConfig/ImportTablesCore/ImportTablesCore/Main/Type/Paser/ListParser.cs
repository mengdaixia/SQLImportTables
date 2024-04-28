using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class ListParser : FieldTypeParser
	{
		private (string LType, string LName, string CType) fieldConf;
		private static Dictionary<string, (string Type, string Name, string CType)> tDic = new Dictionary<string, (string, string, string)>();
		private FieldTypeParser container;

		public override void SetConf(string type_str)
		{
			if (!tDic.TryGetValue(type_str, out fieldConf))
			{
				var datas = type_str.Split(':');
				var cType = datas[1];
				var contain = FieldTypeParseUtils.GetParser(cType);
				var containerType = cType;
				if (contain is IFieldName ifn)
				{
					containerType = ifn.Name;
				}
				switch (datas[0])
				{
					case "list":
						fieldConf = (string.Format("List<{0}>", containerType), "List", cType);
						break;
					case "hash":
						fieldConf = (string.Format("HashSet<{0}>", containerType), "Hash", cType);
						break;
					case "arr":
						fieldConf = (string.Format("{0}[]", containerType), "Arr", cType);
						break;
				}
				tDic[type_str] = fieldConf;
				FieldTypeParseUtils.Recycle(contain);
			}
			container = FieldTypeParseUtils.GetParser(fieldConf.CType);
		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			container.ReadFieldPreName(sb);
			sb.Append(fieldConf.LName);
		}
		public override void Write(string source_value, ReadOnlySpan<char> value_str, BytesWrite write)
		{
			if (value_str.Length == 0)
			{
				write.Write((short)0);
				return;
			}
			using (var dataStr = SpanUtils.Split(value_str))
			{
				write.Write((short)dataStr.Count);
				for (int i = 0; i < dataStr.Count; i++)
				{
					container.Write(source_value, dataStr.Get(value_str, i), write);
				}
			}
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			container.ReadMethodStr(sb);
			sb.Append(fieldConf.LName);
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append(fieldConf.LType);
		}
		public override void Recycle()
		{
			base.Recycle();
			FieldTypeParseUtils.Recycle(container);
			container = null;
		}
	}
}