using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using ImportTables.Utils;

namespace ImportTables
{
	//导表前几行的下标
	public static class ITRowConf
	{
		public const int FIELD_CHINESE_NAME = 0;     //字段中文名
		public const int FIELD_NAME = 1;             //字段名
		public const int FIELD_TIPS = 2;             //字段描述
		public const int FIELD_TYPE = 3;             //字段类型
		public const int FIELD_ATTRINBUTE = 4;       //字段属性
		public const int DATA_BEGIN = 5;             //数据开始
	}
	public static class ITConf
	{
		//Excel文件->>>Sqlite库名
		public static Dictionary<string, string> Excel_2_Sqlite_Dic { get; private set; }
		//Excel主表->>>子表名->>>CSV名
		public static Dictionary<string, Dictionary<string, string>> Csv_Conf_Dic { get; private set; }
		//Sqlite生成路径
		[DirectoryCheck]
		public static string Sqlite_Generated_Path { get; private set; }
		[DirectoryCheck]
		//CSV文件生成路径
		public static string Csv_Generated_Path { get; private set; }
		[DirectoryCheck]
		public static string Csv_Custom_Path { get; private set; }
		//统一生成路径
		[DirectoryCheck]
		public static string Common_Generated_Path { get; private set; }
		//程序执行的当前路径
		public static string Excel_File_Path { get; private set; }
		//CSV模板路径
		public static string CSV_Template_Path { get; private set; }
		public static string CSV_Custom_Template_Path { get; private set; }
		//枚举模板路径
		public static string Enum_Template_Path { get; private set; }
		//Select模板路径
		public static string Select_Template_Path { get; private set; }

		public const string CSV_FIELD_NAME_PREFIX = "m_";
		public const string CSV_FILE_PREFIX = "CSV";
		public const string FILE_LAST_CHANGE_TIME_KEY = "FILE_LAST_CHANGE_TIME_KEY_";
		static ITConf()
		{
			Init();
		}
		private static void Init()
		{

			var currPath = Environment.CurrentDirectory + "/";

			XmlDocument xmlRoot = new XmlDocument();
			xmlRoot.Load(currPath + "Config.txt");
			var rootNode = xmlRoot.SelectSingleNode("Root");

			Sqlite_Generated_Path = currPath + rootNode.SelectSingleNode("SqliteGeneratedPath").InnerText;
			Csv_Generated_Path = currPath + rootNode.SelectSingleNode("CSVGeneratedPath").InnerText;
			Csv_Custom_Path = currPath + rootNode.SelectSingleNode("CSVCustomePath").InnerText;
			Common_Generated_Path = currPath + rootNode.SelectSingleNode("CommonGeneratedPath").InnerText;
			CSV_Template_Path = currPath + rootNode.SelectSingleNode("CSVTemplatePath").InnerText;
			CSV_Custom_Template_Path = currPath + rootNode.SelectSingleNode("CSVCustomTemplatePath").InnerText;
			Enum_Template_Path = currPath + rootNode.SelectSingleNode("EnumTemplatePath").InnerText;
			Select_Template_Path = currPath + rootNode.SelectSingleNode("SelectTemplatePath").InnerText;
			Excel_File_Path = currPath + rootNode.SelectSingleNode("ExcelPath").InnerText;

			Utility.Files.ReadFromCache(Select_Template_Path);
			Utility.Files.ReadFromCache(CSV_Template_Path);
			Utility.Files.ReadFromCache(Enum_Template_Path);
			Utility.Files.ReadFromCache(CSV_Custom_Template_Path);

			Utility.Files.DirectoryCheck(typeof(ITConf));

			var csvConfNode = rootNode.SelectSingleNode("ExcelConf");
			Csv_Conf_Dic = new Dictionary<string, Dictionary<string, string>>();
			Excel_2_Sqlite_Dic = new Dictionary<string, string>();
			var nodes = csvConfNode.ChildNodes;
			for (int i = 0; i < nodes.Count; i++)
			{
				var node = nodes[i];
				var mainTab = node.Attributes["Name"];
				var eName = node.Attributes["EName"];
				Excel_2_Sqlite_Dic[mainTab.Value] = eName.Value;
				Csv_Conf_Dic.Add(mainTab.Value, new Dictionary<string, string>());
				for (int j = 0; j < node.ChildNodes.Count; j++)
				{
					var subNode = node.ChildNodes[j];
					Csv_Conf_Dic[mainTab.Value][subNode.Attributes["Name"].Value] = subNode.InnerText;
				}
			}
		}
		public static void Reload()
		{
			Init();
		}
	}
}