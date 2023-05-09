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
				switch (datas[0])
				{
					case "list":
						fieldConf = (string.Format("List<{0}>", datas[1]), "List", datas[1]);
						break;
					case "hash":
						fieldConf = (string.Format("HashSet<{0}>", datas[1]), "Hash", datas[1]);
						break;
					case "arr":
						fieldConf = (string.Format("{0}[]", datas[1]), "Arr", datas[1]);
						break;
				}
				tDic[type_str] = fieldConf;
			}
			container = FieldTypeParseUtils.GetParser(fieldConf.CType);
		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			container.ReadFieldPreName(sb);
			sb.Append(fieldConf.LName);
		}
		public override void Write(ReadOnlySpan<char> value_str, BytesWrite write)
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
					container.Write(dataStr.Get(value_str, i), write);
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