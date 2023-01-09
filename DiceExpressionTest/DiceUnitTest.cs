using DiceExpression;

namespace DiceExpressionTest;

public class DiceUnitTest
{
	private static Random rng = new(1000);

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
		var d = new Dice<int>(sides);
		for (int i = sides * 3; i >= 0; i--)
			Assert.InRange(d.Roll(rng), 1, sides);
	}

	[Theory]
	public void DiceRoll(int times, int sides)
	{
		var d = new DiceRoll<int>(times, sides);
		for (int i = sides * 3; i >= 0; i--)
			Assert.InRange(d.Roll(rng), 1, sides);
	}
}