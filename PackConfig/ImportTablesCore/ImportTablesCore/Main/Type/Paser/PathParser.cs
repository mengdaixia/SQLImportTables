using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class PathParser : FieldTypeParser, IFieldName
	{
		public string Name => "(int PathHash, string AssetName)";

		public override void SetConf(string type_str)
		{

		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append("s");
		}
		public override void Write(string source_value, ReadOnlySpan<char> value_str, BytesWrite write)
		{
			var code = (0, "");
			if (value_str.Length != 0)
			{
				if (source_value.AsSpan().Length == value_str.Length)
				{
					code = ITPath.GetHashCode(source_value);
				}
				else
				{
					code = ITPath.GetHashCode(value_str.ToString());
				}
			}	
			write.Write(code.Item1);
			write.Write(code.Item2);
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("ISTuple");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append(Name);
		}
	}
}