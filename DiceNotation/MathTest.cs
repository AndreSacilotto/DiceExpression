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
			Console.Write("CastException - ");
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
			Console.Write("ArithmeticException - ");
		}
		return OUT.Zero;
	}

	public static OUT GetT(IN value) => OUT.CreateTruncating(value);
	public static OUT GetS(IN value) => OUT.CreateSaturating(value);

	public static void Test()
	{
		var i = (IN)typeof(IN).GetField("MaxValue")?.GetRawConstantValue()!;
		Console.WriteLine(GetO(i));
		Console.WriteLine(GetC(i));
		Console.WriteLine(GetT(i));
		Console.WriteLine(GetS(i));
		Console.WriteLine();
	}
}
