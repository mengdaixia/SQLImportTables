using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables
{
	public static class StringEx
	{
		public static unsafe string ToUpperFirst(this string str)
		{
			if (str == null) return null;
			fixed (char* ptr = str)
				*ptr = char.ToUpper(*ptr);
			return str;
		}
		public static StringBuilder Tab(this StringBuilder sb, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				sb.Append("    ");
			}
			return sb;
		}
		public static StringBuilder Space(this StringBuilder sb, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				sb.Append(" ");
			}
			return sb;
		}
		public static StringBuilder Enter(this StringBuilder sb)
		{
			sb.Append("\n");
			return sb;
		}
		public static StringBuilder Equal2Space(this StringBuilder sb)
		{
			sb.Append(" = ");
			return sb;
		}
	}
}
