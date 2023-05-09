using ExcelReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables.Attr
{
	public abstract class AttrAction
	{
		public abstract void DoAction(IExcelReader reader, int sub_tab_index, int column, string param);
	}
}