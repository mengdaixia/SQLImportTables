using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class ByteParser : FieldTypeParser
	{
		public override void SetConf(string type_str)
		{

		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append("b");
		}
		public override void Write(string source_value, ReadOnlySpan<char> value_str, BytesWrite write)
		{
			if (!byte.TryParse(value_str, out byte result))
			{
				result = 0;
			}
			write.WriteOne(result);
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("Byte");
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("byte");
		}
	}
}