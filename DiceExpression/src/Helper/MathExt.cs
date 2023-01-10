using System;
using System.ComponentModel;
using System.Numerics;
using DiceExpression.ShuntingYard;

namespace DiceExpression;

public static class MathExt<T> where T : INumber<T>
{
	public static T Lerp(T from, T to, T weight) => from + (to - from) * weight;
	public static T InverseLerp(T from, T to, T weight) => (weight - from) / (to - from);

	public static T Remap(T fromMin, T fromMax, T toMin, T toMax, T value) =>
		Lerp(toMin, toMax, InverseLerp(fromMin, fromMax, value));

	public static T Factorial(T input)
	{
		var result = T.One;
		for (T i = input; i > T.Zero; i--)
			result *= i;
		return result;
	}

}
