using static System.Console;


namespace MathNotation;

public static class MathTest<IN, OUT> where OUT : INumber<OUT> where IN : INumber<IN>
{
	public static OUT GetO(IN value)
	{
		try
		{
			return (OUT)(object)value;
		}
		catch
		{
			Write("CastException - ");
		}
		return OUT.Zero;
	}

	public static OUT GetC(IN value)
	{
		try
		{
			return OUT.CreateChecked(value);
		}
		catch (ArithmeticException)
		{
			Write("ArithmeticException - ");
		}
		return OUT.Zero;
	}

	public static OUT GetT(IN value) => OUT.CreateTruncating(value);
	public static OUT GetS(IN value) => OUT.CreateSaturating(value);

	public static void Test() 
	{
		var i = (IN)typeof(IN).GetField("MaxValue")?.GetRawConstantValue()!;
		WriteLine(GetO(i));
		WriteLine(GetC(i));
		WriteLine(GetT(i));
		WriteLine(GetS(i));
		WriteLine();
	}
}
