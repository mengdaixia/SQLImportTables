using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class StringParser : FieldTypeParser
	{
		public override void SetConf(string type_str)
		{

		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append("s");
		}
		public override void Write(ReadOnlySpan<char> value_str, BytesWrite write)
		{
			write.Write(value_str);
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("String");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("string");
		}
	}
}