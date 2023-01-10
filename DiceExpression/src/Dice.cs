namespace DiceExpression;

public readonly struct DiceRoll<T, S> where T : unmanaged, INumber<T>
{
	public readonly int times;
	public readonly Dice<T, S> dice;

	public DiceRoll(int times, Dice<T, S> dice)
	{
		this.times = times;
		this.dice = dice;
	}
	public DiceRoll(int times, T sides) : this(times, new Dice<T, S>(sides)) { }

	public override string ToString() => ToString(false);
	public string ToString(bool hideIfOne)
	{
		var str = "d" + dice.sides;
		if (!hideIfOne && times <= 1)
			return str;
		return times + str;
	}

	public T[] Roll(IRandom<T, S> rng)
	{
		var rolls = new T[times];
		for (int i = 0; i < times; i++)
			rolls[i] = dice.Roll(rng);
		return rolls;
	}

	public T RollAndSum(IRandom<T, S> rng)
	{
		var sum = T.Zero;
		for (int i = 0; i < times; i++)
			sum += dice.Roll(rng);
		return sum;
	}

	public T[] RollAndKeepHighest(IRandom<T, S> rng, int amount)
	{
		var rolls = Roll(rng);
		Array.Sort(rolls);
		var h = new T[amount];
		Array.Copy(rolls, h, amount);
		return h;
	}

	public T[] RollAndKeepLowest(IRandom<T, S> rng, int amount)
	{
		var rolls = Roll(rng);
		Array.Sort(rolls);
		var l = new T[amount];
		Array.Copy(rolls, times - amount, l, 0, amount);
		return l;
	}

	public (T[] lowest, T[] highest) RollAndKeepLowestHighest(IRandom<T, S> rng, int lowAmount, int highAmount)
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

public readonly struct Dice<T, S> where T : unmanaged, INumber<T>
{
	public readonly T sides;

	public Dice(T sides) => this.sides = sides;

	public override string ToString() => 'D' + sides.ToString();

	public double GetAverage() => double.CreateSaturating(sides + T.One) / 2.0;

	public T Roll(IRandom<T, S> rng) => rng.Next(T.One, sides);

	public T RollExplosive(IRandom<T, S> rng) 
	{
		T sum = T.Zero;
		T value;
		do
		{
			value = Roll(rng);
			sum += value;
		} while (value == sides);
		return sum;
	}

	public T[] RollKeepExplosive(IRandom<T, S> rng)
	{
		var list = new List<T>(1);
		T value;
		do
		{
			value = Roll(rng);
			list.Add(value);
		} while (value == sides);
		return list.ToArray();
	}


}
