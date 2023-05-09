using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.Attr
{
	public class ITFieldAttribute : Attribute
	{
		public string AttrKey { get; }

		public ITFieldAttribute(string attr)
		{
			AttrKey = attr;
		}
	}
}