using System;
using System.ComponentModel;
using System.Numerics;

namespace DiceExpression;

public static class MathExt<T> where T : INumber<T>
{
	public static T Lerp(T from, T to, T weight) => from + (to - from) * weight;
	public static T InverseLerp(T from, T to, T weight) => (weight - from) / (to - from);

	public static T Remap(T fromMin, T fromMax, T toMin, T toMax, T value) => 
		Lerp(toMin, toMax, InverseLerp(fromMin, fromMax, value));

	public static T ConvertTo(object value)
	{
		if (value is T variable)
			return variable;

		try
		{
			//Handling Nullable types i.e, int?, double?, bool? .. etc
			if (Nullable.GetUnderlyingType(typeof(T)) != null)
				return (T)(TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value) ?? 0);

			return (T)Convert.ChangeType(value, typeof(T));
		}
		catch (Exception)
		{
			return default!;
		}
	}
}
