//using ImportTables.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ImportTables.FieldTypeParse
//{
//	public class DicParser : MultipleFieldTypeParser
//	{
//		public override void SetConf(string type_str)
//		{
//			var datas = type_str.Split(',');
//			containParser.Add(FieldTypeParseUtils.GetParser(datas[0]));
//			containParser.Add(FieldTypeParseUtils.GetParser(datas[1]));
//		}
//		public override byte[] Parse(string value_str)
//		{
//			var splitCount = FieldTypeParseUtils.DEPTH_SPLIT_CHAR_DIC[depth + 1];
//			var splitKV = FieldTypeParseUtils.DEPTH_SPLIT_CHAR_DIC[depth];
//			var datas = value_str.Split(splitCount);
//			if (datas.Length == 0)
//			{
//				return Utility.Bytes.ZeroBytes;
//			}
//			byte[] results = null;
//			for (int i = 0; i < datas.Length; i++)
//			{
//				var kv = datas[i].Split(splitKV);
//				if (kv.Length == 2)
//				{
//					var kBytes = containParser[0].Parse(kv[0]);
//					var vBytes = containParser[1].Parse(kv[1]);
//					results = Utility.Bytes.Concat(kBytes, vBytes);
//				}
//			}
//			results = results == null ? Utility.Bytes.ZeroBytes : results;
//			return results;
//		}
//		public override void ReadMethodNameStr(StringBuilder sb)
//		{
//			containParser.ForEach(c => c.ReadMethodNameStr(sb));
//			sb.Append("Dic");
//		}
//		public override void ReadMethodStr(StringBuilder sb)
//		{

//		}
//		//字典占两层
//		public override int CalculateDepth()
//		{
//			depth = base.CalculateDepth();
//			return depth + 1;
//		}
//		public override string ReadTyepStr()
//		{
//			return string.Format("Dictionary<{0},{1}>", containParser[0].ReadTyepStr(), containParser[1].ReadTyepStr());
//		}
//	}
//}