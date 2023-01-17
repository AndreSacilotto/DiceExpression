using DiceNotation;

namespace XTesting;

public class DiceUnitTest
{
	private static IRandomNumber<int> rng = new RandomInt(1000);

	private static readonly int[] realWorldDices = new int[] { 2, 4, 6, 8, 10, 12, 16, 20 };

	public static IEnumerable<object[]> DefaultDices()
	{
		foreach (var item in realWorldDices)
			yield return new object[] { item };
	}

	[Theory]
	[MemberData(nameof(DefaultDices))]
	public void Dice(int sides)
	{
		var d = new Dice<int>(sides);
		for (int i = sides * 3; i >= 0; i--)
			Assert.InRange(d.Roll(rng), 1, sides);
	}


	public static IEnumerable<object[]> DefaultRolls()
	{
		foreach (var item in realWorldDices)
			yield return new object[] { rng.Next(10), item };
	}

	[Theory]
	[MemberData(nameof(DefaultRolls))]
	public void DiceRoll(int times, int sides)
	{
		var d = new DiceRoll<int, int>(times, sides);
		for (int i = sides * 3; i >= 0; i--)
			Assert.InRange(d.RollAndSum(rng), times, sides * times);
	}
}
