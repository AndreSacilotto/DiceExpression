namespace Helper;

public interface ISeed<T> where T : struct
{
	T Seed { get; set; }
}

public interface IRandomNumber<T> where T : INumber<T>
{
	/// <summary>Returns a non negative value between 0..MaxValue [inclusive, exclusive[</summary>
	T Next();
	/// <summary>Returns value between 0..<paramref name="max"/> [inclusive, inclusive] </summary>
	T Next(T max);
	/// <summary>Returns value between <paramref name="min"/>..<paramref name="max"/> [inclusive, inclusive] </summary>
	T Next(T min, T max);
}

public interface IRandom<T, TSeed> : IRandomNumber<T>, ISeed<TSeed> where T : INumber<T> where TSeed : struct
{
}
