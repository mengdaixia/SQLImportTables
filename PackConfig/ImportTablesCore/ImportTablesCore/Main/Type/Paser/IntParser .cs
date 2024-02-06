using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class IntParser : FieldTypeParser
	{
		public override void SetConf(string type_str)
		{

		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append("i");
		}
		public override void Write(string source_value, ReadOnlySpan<char> value_str, BytesWrite write)
		{
			if (!int.TryParse(value_str, out int result))
			{
				result = 0;
			}
			write.Write(result);
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("Int");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("int");
		}
	}
}