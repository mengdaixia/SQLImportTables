using ExcelReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.Attr
{
	public static class AttrUtils
	{
		public const string SELECT_DATA = "select";
		public const string IGNORE_DATA = "ignore";
		private static Dictionary<string, AttrAction> actionDic = new Dictionary<string, AttrAction>(); 
		static AttrUtils()
		{
			var assembly = typeof(AttrAction).Assembly;
			var types = assembly.GetTypes();
			foreach (var type in types)
			{
				if (type.IsDefined(typeof(ITFieldAttribute), false))
				{
					var attrs = type.GetCustomAttributes(typeof(ITFieldAttribute), false);
					foreach (var attr in attrs)
					{
						if (attr is ITFieldAttribute itfa)
						{
							actionDic[itfa.AttrKey] = Activator.CreateInstance(type) as AttrAction;
						}
					}
				}
			}
		}

		public static void DoFieldAttrAction(IExcelReader reader, int sub_tab_index, int column)
		{
			lock (actionDic)
			{
				var attrValue = reader.GetValue(sub_tab_index, ITRowConf.FIELD_ATTRINBUTE, column);
				if (attrValue.Length == 0)
				{
					return;
				}
				var attrs = attrValue.Split('|');
				for (int i = 0; i < attrs.Length; i++)
				{
					var attrData = attrs[i].Split(':');
					if (attrData.Length > 0)
					{
						var key = attrData[0];
						var param = string.Empty;
						if (attrData.Length > 1)
						{
							param = attrData[1];
						}
						if (actionDic.TryGetValue(key, out AttrAction action))
						{
							action.DoAction(reader, sub_tab_index, column, param);
						}
					}
				}
			}
		}
		public static bool IsDefine(IExcelReader reader, int sub_tab_index, int column, string attr_key)
		{
			var attrValue = reader.GetValue(sub_tab_index, ITRowConf.FIELD_ATTRINBUTE, column);
			if (attrValue.Length == 0)
			{
				return false;
			}
			return attrValue.Contains(attr_key);
		}
	}
}
