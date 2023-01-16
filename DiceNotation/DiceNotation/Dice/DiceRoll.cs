using Helper;

namespace DiceNotation;

public readonly record struct DiceRoll<T> where T : INumber<T>
{
	public readonly int times;
	public readonly Dice<T> dice;

	public DiceRoll(int times, Dice<T> dice)
	{
		this.times = times;
		this.dice = dice;
	}
	public DiceRoll(int times, T sides) : this(times, new Dice<T>(sides)) { }

	public override string ToString() => ToString(false);
	public string ToString(bool hideTimesIfOne)
	{
		var str = "d" + dice.sides;
		if (hideTimesIfOne && times <= 1)
			return str;
		return times + str;
	}

	public T[] Roll(IRandomNumber<T> rng)
	{
		var rolls = new T[times];
		for (int i = 0; i < times; i++)
			rolls[i] = dice.Roll(rng);
		return rolls;
	}

	public T RollAndSum(IRandomNumber<T> rng)
	{
		var sum = T.Zero;
		for (int i = 0; i < times; i++)
			sum += dice.Roll(rng);
		return sum;
	}

	public T[] RollAndKeepHighest(IRandomNumber<T> rng, int amount)
	{
		var rolls = Roll(rng);
		Array.Sort(rolls);
		var h = new T[amount];
		Array.Copy(rolls, h, amount);
		return h;
	}

	public T[] RollAndKeepLowest(IRandomNumber<T> rng, int amount)
	{
		var rolls = Roll(rng);
		Array.Sort(rolls);
		var l = new T[amount];
		Array.Copy(rolls, times - amount, l, 0, amount);
		return l;
	}

	public (T[] lowest, T[] highest) RollAndKeepLowestHighest(IRandomNumber<T> rng, int lowAmount, int highAmount)
	{
		var rolls = Roll(rng);
		Array.Sort(rolls);
		var l = new T[lowAmount];
		Array.Copy(rolls, times - lowAmount, l, 0, lowAmount);
		var h = new T[highAmount];
		Array.Copy(rolls, h, highAmount);
		return (l, h);
	}
}
