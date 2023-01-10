namespace DiceExpressionTest;

public class DiceExpressionUnitTest
{
	public static IEnumerable<object[]> GetValues()
	{
		yield return new object[] { "(11.25)", 11.25 };
		yield return new object[] { "(9 + 1) * 2", 20 };
		yield return new object[] { "(2.5 ^ 2.5)", 9.882 };
		yield return new object[] { "ceil(2.5 ^ 2.5)", 10 };
		yield return new object[] { "floor(3.5)", 3 };
		yield return new object[] { "ceil((3+.5))", 4 };
		yield return new object[] { "-10", -10 };
		yield return new object[] { "(-2)-3", -5 };
		yield return new object[] { "-(-4)", 4 };
		yield return new object[] { "-(+(+6))", -6 };
		yield return new object[] { "8-5+(-(3))", 0 };
		yield return new object[] { "-30/-3", 10 };
		yield return new object[] { "(-50)/-(-25)", -2 };
		yield return new object[] { "10!", 3628800 };
	}

	[Theory]
	[MemberData(nameof(GetValues))]
	public void NoDiceExpression(string exp, double result) 
	{
		var de = new DiceExpression.DiceExpression(exp);
		var v = Math.Round(de.Evaluate(), 3, MidpointRounding.ToZero);
		Assert.Equal(v, result);
	}
}