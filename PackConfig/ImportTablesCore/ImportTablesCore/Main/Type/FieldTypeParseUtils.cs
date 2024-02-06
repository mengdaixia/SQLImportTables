using System;
using System.Collections.Generic;

namespace ImportTables.FieldTypeParse
{
	public enum EFieldType
	{
		Bool,
		Byte,
		Short,
		Int,
		Long,
		Float,
		Double,
		String,
		List,
		Vector2,
		Vector3,
		Vector4,
	}
	public static class FieldTypeParseUtils
	{
		private static Dictionary<Type, Queue<FieldTypeParser>> poolDic = new Dictionary<Type, Queue<FieldTypeParser>>();
		private static object lockObj = new object();
		static FieldTypeParseUtils()
		{
			poolDic[typeof(BoolParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(ByteParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(ShortParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(IntParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(LongParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(FloatParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(DoubleParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(StringParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(ListParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(DynamicValueParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(VectorParser)] = new Queue<FieldTypeParser>();
			poolDic[typeof(PathParser)] = new Queue<FieldTypeParser>();
		}
		public static FieldTypeParser GetParser(string type)
		{
			lock (lockObj)
			{
				FieldTypeParser result = null;
				switch (type)
				{
					case "bool":
						result = GetFromPool<BoolParser>();
						break;
					case "byte":
						result = GetFromPool<ByteParser>();
						break;
					case "short":
						result = GetFromPool<ShortParser>();
						break;
					case "int":
						result = GetFromPool<IntParser>();
						break;
					case "long":
						result = GetFromPool<LongParser>();
						break;
					case "float":
						result = GetFromPool<FloatParser>();
						break;
					case "double":
						result = GetFromPool<DoubleParser>();
						break;
					case "string":
						result = GetFromPool<StringParser>();
						break;
					case "path":
						result = GetFromPool<PathParser>();
						break;
					case var c when c.StartsWith("dynamic"):
						result = GetFromPool<DynamicValueParser>();
						break;
					case var c when c.StartsWith("vector"):
						result = GetFromPool<VectorParser>();
						break;
					default:
						result = GetFromPool<ListParser>();
						break;
				}
				result.SetConf(type);
				return result;
			}
		}
		private static FieldTypeParser GetFromPool<T>() where T : FieldTypeParser, new()
		{
			FieldTypeParser result = null;
			if (poolDic.TryGetValue(typeof(T), out Queue<FieldTypeParser> queue))
			{
				if (queue.Count > 0)
				{
					return queue.Dequeue();
				}
				result = new T();
			}
			return result;
		}
		public static void Recycle(FieldTypeParser parser)
		{
			lock (lockObj)
			{
				parser.Recycle();
				poolDic[parser.GetType()].Enqueue(parser);
			}
		}
		public static string GetTyepParse(string type)
		{
			return type == "int" ? "int.Parse(first)" : "first";
		}
	}
}