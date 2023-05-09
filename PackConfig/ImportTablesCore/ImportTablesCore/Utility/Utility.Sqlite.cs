using System;
using System.Collections;
using System.Data.SQLite;
using Microsoft.Win32;

namespace ImportTables.Utils
{
	public static partial class Utility
	{
		public static class Sqlite
		{
			public static void DropTable(SQLiteConnection sql, string name)
			{
				var cmd = sql.CreateCommand();
				cmd.CommandText = "drop table if exists " + name;
				cmd.ExecuteNonQuery();
			}
		}
	}
}