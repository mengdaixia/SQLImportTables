using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class FloatParser : FieldTypeParser
	{
		public override void SetConf(string type_str)
		{

		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append("f");
		}
		public override void Write(string source_value, ReadOnlySpan<char> value_str, BytesWrite write)
		{
			if (!float.TryParse(value_str, out float result))
			{
				result = 0;
			}
			write.Write(result);
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("Float");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("float");
		}
	}
}