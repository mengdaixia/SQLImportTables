using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class DoubleParser : FieldTypeParser
	{
		public override void SetConf(string type_str)
		{

		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append("d");
		}
		public override void Write(ReadOnlySpan<char> value_str, BytesWrite write)
		{
			if (!double.TryParse(value_str, out double result))
			{
				result = 0;
			}
			write.Write(result);
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("Double");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("double");
		}
	}
}