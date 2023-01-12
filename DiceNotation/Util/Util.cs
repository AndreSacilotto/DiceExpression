using System.Runtime.InteropServices;

namespace Helper;

public static class Util
{
	public static void TrimExcess(this StringBuilder sb) => sb.Capacity = sb.Length;

	public static T[] Combine<T>(T[] first, T[] second) where T : unmanaged
	{
		var size = Marshal.SizeOf<T>();
		var arr = new T[first.Length + second.Length];
		Buffer.BlockCopy(first, 0, arr, 0, count: first.Length * size);
		Buffer.BlockCopy(second, 0, arr, first.Length, second.Length * size);
		return arr;
	}
}
