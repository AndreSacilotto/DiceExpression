global using System;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Numerics;

using System.Linq;
using DiceExpression;

using static System.Console;

using from = System.UInt32;
using to = System.Int32;


var rng = new Random(1000);
var i = from.MaxValue;
WriteLine(MathTest<from, to>.GetC(i));
WriteLine(MathTest<from, to>.GetT(i));
WriteLine(MathTest<from, to>.GetS(i));
WriteLine();

public static class MathTest<TIn, TOut> where TOut : INumber<TOut> where TIn : INumber<TIn>
{
	//public static T GetO(TIn value) => (T)(object)value;
	public static TOut GetC(TIn value)
	{
		try
		{
			return TOut.CreateChecked(value);
		}
		catch (ArithmeticException)
		{
			Write("ArithmeticException - ");
		}
		return TOut.Zero;
	}

	public static TOut GetT(TIn value) => TOut.CreateTruncating(value);
	public static TOut GetS(TIn value) => TOut.CreateSaturating(value);
}
