namespace DiceExpression;

public class Dice
{
	public int sides;
	public int times;

	public string ToString(bool timesIfOne)
	{
		var str = "d" + sides;
		if (!timesIfOne && times <= 1)
			return str;
		return times + str;
	}

	public int RollInt(Random rng) => rng.Next(1, sides + 1);
	public long RollLong(Random rng) => rng.NextInt64() * sides + 1;
	public float RollFloat(Random rng) => rng.NextSingle() * sides + 1;
	public double RollDouble(Random rng) => rng.NextDouble() * sides + 1;
}
