using DiceExpression;

namespace DiceExpressionTest;

public class DiceUnitTest
{
	private static IRandom<int, int> rng = new RandomInt(1000);

	[Theory]
	[InlineData(2)]
	[InlineData(4)]
	[InlineData(6)]
	[InlineData(8)]
	[InlineData(10)]
	[InlineData(12)]
	[InlineData(20)]
	public void Dice(int sides)
	{
		var d = new Dice<int, int>(sides);
		for (int i = sides * 3; i >= 0; i--)
			Assert.InRange(d.Roll(rng), 1, sides);
	}

	[Theory]
	[InlineData(5, 2)]
	[InlineData(5, 4)]
	[InlineData(5, 6)]
	[InlineData(5, 8)]
	[InlineData(5, 10)]
	[InlineData(5, 12)]
	[InlineData(5, 20)]
	public void DiceRoll(int times, int sides)
	{
		var d = new DiceRoll<int, int>(times, sides);
		for (int i = sides * 3; i >= 0; i--)
			Assert.InRange(d.Roll(rng), times, sides * times);
	}
}

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