using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace ImportTablesCore.Utility.Bytes
{
	internal static class NativeArray
	{
		public unsafe static T[] Alloc<T>(int length)
		{
			var byteCount = IntPtr.Size * 3 + Unsafe.SizeOf<T>() * length;
			var pointer = NativeMemory.AllocZeroed((uint)byteCount);
			Unsafe.Write(Unsafe.Add<nint>(pointer, 1), typeof(T[]).TypeHandle.Value);
			Unsafe.Write(Unsafe.Add<nint>(pointer, 2), length);
			T[] result = null!;
			Unsafe.Write(Unsafe.AsPointer(ref result), new IntPtr(Unsafe.Add<nint>(pointer, 1)));
			return result;
		}
		public unsafe static void Free<T>(T[] array)
		{
			var address = *(nint*)Unsafe.AsPointer(ref array);
			NativeMemory.Free(Unsafe.Add<nint>(address.ToPointer(), -1));
		}
	}
}