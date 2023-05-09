using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class ShortParser : FieldTypeParser
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
			if (!short.TryParse(value_str, out short result))
			{
				result = 0;
			}
			write.Write(result);
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("Short");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("short");
		}
	}
}