using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DiceExpression;

public interface IRandom<T> where T : unmanaged, INumber<T>
{
	T Seed { get; set; }

	/// <summary>Returns a non negative value between 0..MaxValue [inclusive, exclusive[</summary>
	T Next();
	/// <summary>Returns value between 0..<paramref name="max"/> [inclusive, inclusive] </summary>
	T Next(T max);
	/// <summary>Returns value between <paramref name="min"/>..<paramref name="max"/> [inclusive, inclusive] </summary>
	T Next(T min, T max);
}
