using ImportTables.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CSVReader
{
	private BytesReader reader = new BytesReader();
	public void Write(byte[] datas)
	{
		reader.SetPosition(datas);
	}
	public byte ReadByte()
	{
		return (byte)reader.ReadByte();
	}
	public short ReadShort()
	{
		return reader.ReadShort();
	}
	public int ReadInt()
	{
		return reader.ReadInt();
	}
	public long ReadLong()
	{
		return reader.ReadLong();
	}
	public float ReadFloat()
	{
		return reader.ReadFloat();
	}
	public double ReadDouble()
	{
		return reader.ReadFloat();
	}
	public string ReadString()
	{
		return reader.ReadString();
	}
	public List<byte> ReadByteList()
	{
		var length = ReadShort();
		var lst = new List<byte>(length);
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadByte());
		}
		return lst;
	}
	public List<short> ReadShortList()
	{
		var length = ReadShort();
		var lst = new List<short>(length);
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadShort());
		}
		return lst;
	}
	public List<int> ReadIntList()
	{
		var length = ReadShort();
		var lst = new List<int>(length);
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadInt());
		}
		return lst;
	}
	public List<long> ReadLongList()
	{
		var length = ReadShort();
		var lst = new List<long>(length);
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadLong());
		}
		return lst;
	}
	public List<float> ReadFloatList()
	{
		var length = ReadShort();
		var lst = new List<float>(length);
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadFloat());
		}
		return lst;
	}
	public List<double> ReadDoubleList()
	{
		var length = ReadShort();
		var lst = new List<double>(length);
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadDouble());
		}
		return lst;
	}
	public List<string> ReadStringList()
	{
		var length = ReadShort();
		var lst = new List<string>(length);
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadString());
		}
		return lst;
	}
	public HashSet<byte> ReadByteHash()
	{
		var length = ReadShort();
		var lst = new HashSet<byte>();
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadByte());
		}
		return lst;
	}
	public HashSet<short> ReadShortHash()
	{
		var length = ReadShort();
		var lst = new HashSet<short>();
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadShort());
		}
		return lst;
	}
	public HashSet<int> ReadIntHash()
	{
		var length = ReadShort();
		var lst = new HashSet<int>();
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadInt());
		}
		return lst;
	}
	public HashSet<long> ReadLongHash()
	{
		var length = ReadShort();
		var lst = new HashSet<long>();
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadLong());
		}
		return lst;
	}
	public HashSet<float> ReadFloatHash()
	{
		var length = ReadShort();
		var lst = new HashSet<float>();
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadFloat());
		}
		return lst;
	}
	public HashSet<double> ReadDoubleHash()
	{
		var length = ReadShort();
		var lst = new HashSet<double>();
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadDouble());
		}
		return lst;
	}
	public HashSet<string> ReadStringHash()
	{
		var length = ReadShort();
		var lst = new HashSet<string>();
		for (int i = 0; i < length; i++)
		{
			lst.Add(ReadString());
		}
		return lst;
	}
	public byte[] ReadByteArr()
	{
		var length = ReadShort();
		var arr = new byte[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = ReadByte();
		}
		return arr;
	}
	public short[] ReadShortArr()
	{
		var length = ReadShort();
		var arr = new short[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = ReadShort();
		}
		return arr;
	}
	public int[] ReadIntArr()
	{
		var length = ReadShort();
		var arr = new int[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = ReadInt();
		}
		return arr;
	}
	public long[] ReadLongArr()
	{
		var length = ReadShort();
		var arr = new long[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = ReadLong();
		}
		return arr;
	}
	public float[] ReadFloatArr()
	{
		var length = ReadShort();
		var arr = new float[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = ReadFloat();
		}
		return arr;
	}
	public double[] ReadDoubleArr()
	{
		var length = ReadShort();
		var arr = new double[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = ReadDouble();
		}
		return arr;
	}
	public string[] ReadStringArr()
	{
		var length = ReadShort();
		var arr = new string[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = ReadString();
		}
		return arr;
	}
	public bool ReadBool()
	{
		return reader.ReadByte() == 1;
	}
	//可以不存长度的，但是还是做个验证吧，否则如果配表错误则后续数据全错
	public UnityEngine.Vector3 ReadVector3()
	{
		var length = ReadByte();
		if (length != 3)
		{
			throw new System.Exception();
		}
		UnityEngine.Vector3 v3 = new UnityEngine.Vector3(ReadFloat(), ReadFloat(), ReadFloat());
		return v3;
	}
	public UnityEngine.Vector2 ReadVector2()
	{
		var length = ReadByte();
		if (length != 2)
		{
			throw new System.Exception();
		}
		UnityEngine.Vector2 v2 = new UnityEngine.Vector2(ReadFloat(), ReadFloat());
		return v2;
	}
	public DynamicValue ReadIntDynamicValue()
	{
		DynamicValue result = new DynamicValue();
		var lst = ReadIntList();
		result.Set(lst);
		return result;
	}
	public void Dispose()
	{
		reader?.Dispose();
	}
}