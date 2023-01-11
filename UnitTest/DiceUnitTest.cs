using DiceNotation;

namespace XTesting;

public class DiceUnitTest
{
	private static IRandomNumber<int> rng = new RandomInt(1000);

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
			Assert.InRange(d.RollAndSum(rng), times, sides * times);
	}
}
