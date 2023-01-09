using static System.Console;


namespace DiceExpression;

public static class MathTest<TIn, TOut> where TOut : INumber<TOut> where TIn : INumber<TIn>
{
	public static TOut GetO(TIn value)
	{
		try
		{
			return (TOut)(object)value;
		}
		catch
		{
			Write("CastException - ");
		}
		return TOut.Zero;
	}

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

	public static void Test() 
	{
		var i = (TIn)typeof(TIn).GetField("MaxValue")?.GetRawConstantValue()!;
		WriteLine(GetO(i));
		WriteLine(GetC(i));
		WriteLine(GetT(i));
		WriteLine(GetS(i));
		WriteLine();
	}
}
