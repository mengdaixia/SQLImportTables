using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public class VectorParser : FieldTypeParser
	{
		private int length = 0;
		private const string VECTOR_STR2 = "vector:2";
		private const string VECTOR_STR3 = "vector:3";
		private const string VECTOR_STR4 = "vector:4";
		public override void SetConf(string type_str)
		{
			switch (type_str)
			{
				case VECTOR_STR2:
					length = 2;
					break;
				case VECTOR_STR3:
					length = 3;
					break;
				case VECTOR_STR4:
					length = 4;
					break;
			}
		}
		public override void ReadFieldPreName(StringBuilder sb)
		{
			sb.Append("v").Append(length);
		}
		public override void Write(string source_value, ReadOnlySpan<char> value_str, BytesWrite write)
		{
			if (value_str.Length == 0)
			{
				switch (length)
				{
					case 2:
						value_str = "0:0";
						break;
					case 3:
						value_str = "0:0:0";
						break;
					case 4:
						value_str = "0:0:0:0";
						break;
				}
			}
			using (var dataStr = SpanUtils.Split(value_str, ':'))
			{
				//可以不存长度的，但是还是做个验证吧，否则如果配表错误则后续数据全错
				write.WriteOne((byte)dataStr.Count);
				for (int i = 0; i < dataStr.Count; i++)
				{
					write.Write(float.Parse(dataStr.Get(value_str, i)));
				}
			}
		}
		public override void ReadMethodStr(StringBuilder sb)
		{
			sb.Append("Vector").Append(length);
		}
		public override void ReadFieldTypeName(StringBuilder sb)
		{
			sb.Append("Vector").Append(length);
		}
	}
}