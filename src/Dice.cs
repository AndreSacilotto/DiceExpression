namespace DiceExpression;

public interface IDice 
{ 

}

public readonly struct DiceRoll
{
	public readonly int times;
	public readonly Dice dice;

	public DiceRoll(int times, Dice dice)
	{
		this.times = times;
		this.dice = dice;
	}
	public DiceRoll(int times, int sides) : this(times, new Dice(sides)) { }

	public override string ToString() => ToString(false);
	public string ToString(bool hideIfOne)
	{
		var str = "d" + dice.sides;
		if (!hideIfOne && times <= 1)
			return str;
		return times + str;
	}

	public int RollInt(Random rng)
	{
		int sum = 0;
		for (int i = 0; i < times; i++)
			sum += dice.RollInt(rng);
		return sum;
	}
	public long RollLong(Random rng)
	{
		long sum = 0;
		for (int i = 0; i < times; i++)
			sum += dice.RollLong(rng);
		return sum;
	}
	public float RollFloat(Random rng)
	{
		float sum = 0;
		for (int i = 0; i < times; i++)
			sum += dice.RollFloat(rng);
		return sum;
	}
	public double RollDouble(Random rng)
	{
		double sum = 0;
		for (int i = 0; i < times; i++)
			sum += dice.RollDouble(rng);
		return sum;
	}

	public T Roll<T>(Random rng) where T : INumber<T>
	{
		T sum = T.Zero;
		for (int i = 0; i < times; i++)
			sum += dice.Roll<T>(rng);
		return sum;
	}

}

public readonly struct Dice
{
	public readonly int sides;

	public Dice(int sides) => this.sides = sides;

	public override string ToString() => sides.ToString();

	public double GetAverage() => (sides + 1) / 2.0;

	public int RollInt(Random rng) => rng.Next(1, sides + 1);
	public long RollLong(Random rng) => rng.NextInt64() * sides + 1;
	public float RollFloat(Random rng) => rng.NextSingle() * sides + 1;
	public double RollDouble(Random rng) => rng.NextDouble() * sides + 1;

	public T Roll<T>(Random rng) where T : INumber<T> 
	{
		var type = typeof(T);
		if (type == typeof(int))
			return (T)(object)RollInt(rng);
		if (type == typeof(long))
			return (T)(object)RollLong(rng);
		if (type == typeof(float))
			return (T)(object)RollFloat(rng);
		if (type == typeof(double))
			return (T)(object)RollDouble(rng);
		return T.One;
	}

}
