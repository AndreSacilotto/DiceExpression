namespace XTesting;

public class MathExpressionUnitTest
{
	public static IEnumerable<object[]> GetValues()
	{
		yield return new object[] { "(11.25)", 11.25 };
		yield return new object[] { "(9 + 1) * 2", 20 };
		yield return new object[] { "(2.5 ^ 2.5)", 9.882 };
		yield return new object[] { "ceil(2.5 ^ 2.5)", 10 };
		yield return new object[] { "floor(3.5)", 3 };
		yield return new object[] { "ceil((3+.5))", 4 };
		yield return new object[] { "4!", 24 };
		yield return new object[] { "1+3!+1", 8 };
		yield return new object[] { "-10", -10 };
		yield return new object[] { "(-2)-3", -5 };
		yield return new object[] { "-(-4)", 4 };
		yield return new object[] { "-(+(+6))", -6 };
		yield return new object[] { "-(10 / 2.5)", -4.0 };
		yield return new object[] { "8-5+(-(3))", 0 };
		yield return new object[] { "-30/-3", 10 };
		yield return new object[] { "(-50)/-(-25)", -2 };
		yield return new object[] { "10!", 3628800 };
		yield return new object[] { "clamp(-100, 2., 35)", 2.0 };
		yield return new object[] { "e + (pi * tau)", 22.457 };
		yield return new object[] { "min(3, 10.5)!+1", 7 };
		yield return new object[] { "-max(PI, TAU)", -6.283 };
	}

	[Theory]
	[MemberData(nameof(GetValues))]
	public void Expression(string exp, double result)
	{
		var de = new MathNotation.MathExpression<double>(exp);
		var v = Math.Round(de.Evaluate(), 3, MidpointRounding.ToZero);
		Assert.Equal(v, result);
	}
}