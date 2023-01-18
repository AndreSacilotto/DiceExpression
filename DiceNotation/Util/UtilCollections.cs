using System.Runtime.InteropServices;

namespace Helper;

public static class UtilCollections
{
	public static void TrimExcess(this StringBuilder sb) => sb.Capacity = sb.Length;

	#region AddRange
	public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enumerable)
	{
		foreach (T obj in enumerable)
			queue.Enqueue(obj);
	}
	public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enumerable, int lenght)
	{
		queue.EnsureCapacity(queue.Count + lenght);
		foreach (T obj in enumerable)
			queue.Enqueue(obj);
	}

	public static void AddRange<T>(this Stack<T> stack, IEnumerable<T> enumerable)
	{
		foreach (T obj in enumerable)
			stack.Push(obj);
	}
	public static void AddRange<T>(this Stack<T> stack, IEnumerable<T> enumerable, int lenght)
	{
		stack.EnsureCapacity(stack.Count + lenght);
		foreach (T obj in enumerable)
			stack.Push(obj);
	}
	#endregion

	public static T[] Combine<T>(T[] first, T[] second) where T : unmanaged
	{
		var size = Marshal.SizeOf<T>();
		var arr = new T[first.Length + second.Length];
		Buffer.BlockCopy(first, 0, arr, 0, count: first.Length * size);
		Buffer.BlockCopy(second, 0, arr, first.Length, second.Length * size);
		return arr;
	}

	#region Shuffle
	public static void Shuffle<T>(T[] array, IRandomNumber<int> rng)
	{
		int i = array.Length;
		while (i > 1)
		{
			int j = rng.Next(i--);
			(array[j], array[i]) = (array[i], array[j]);
		}
	}
	public static void Shuffle<T>(List<T> list, IRandomNumber<int> rng)
	{
		int i = list.Count;
		while (i > 1)
		{
			int j = rng.Next(i--);
			(list[j], list[i]) = (list[i], list[j]);
		}
	}
	#endregion


	public static bool TryFindValue<K, V>(K key, out V value, params IDictionary<K, V>[] dicts) where K : notnull
	{
		foreach (var dict in dicts)
			if (dict.TryGetValue(key, out var v))
			{
				value = v;
				return true;
			}
		value = default!;
		return false;
	}
	public static K? TryFindKey<K, V>(V value, params IDictionary<K, V>[] dicts) where K : notnull where V : IEquatable<V>
	{
		foreach (var dict in dicts)
			foreach (var item in dict)
				if (value.Equals(item.Value))
					return item.Key;
		return default;
	}

}
