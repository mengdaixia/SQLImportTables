using ImportTables.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.FieldTypeParse
{
	public interface IFieldName
	{
		string Name { get; }
	}
	public abstract class FieldTypeParser
	{
		public abstract void SetConf(string type_str);
		//把数据转换成Bytes
		public abstract void Write(string source_value, ReadOnlySpan<char> value_str, BytesWrite write);
		//读取的方法
		public abstract void ReadMethodStr(StringBuilder sb);
		//字段名前缀
		public abstract void ReadFieldPreName(StringBuilder sb);
		//字段的类型
		public abstract void ReadFieldTypeName(StringBuilder sb);

		public virtual void Recycle()
		{

		}
	}
}