using Code.CustomValue;
using System;
using System.Collections.Generic;

public class DynamicValue
{
	private List<int> formulaLst;
	public int Value
	{
		get
		{
			var value = formulaLst[0];
			for (int i = 1; i < formulaLst.Count; i++)
			{
				if (CustomValueMgr.Instance.TryGetValue(formulaLst[i], out int varValue))
				{
					value += varValue;
				}
			}
			return value;
		}
	}
	public void Set(List<int> formula_lst)
	{
		formulaLst = formula_lst;
	}
}