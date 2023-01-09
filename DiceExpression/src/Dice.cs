using System.Numerics;

namespace DiceExpression;


public readonly struct DiceRoll<T> where T : unmanaged, INumber<T>
{
	public readonly T times;
	public readonly Dice<T> dice;

	public DiceRoll(T times, Dice<T> dice)
	{
		this.times = times;
		this.dice = dice;
	}
	public DiceRoll(T times, T sides) : this(times, new Dice<T>(sides)) { }

	public override string ToString() => ToString(false);
	public string ToString(bool hideIfOne)
	{
		var str = "d" + dice.sides;
		if (!hideIfOne && times <= T.One)
			return str;
		return times + str;
	}

	public T RollInt(Random rng)
	{
		var sum = T.Zero;
		for (T i = T.Zero; i < times; i++)
			sum += T.CreateTruncating(dice.RollInt(rng));
		return sum;
	}
	public T RollLong(Random rng)
	{
		var sum = T.Zero;
		for (T i = T.Zero; i < times; i++)
			sum += T.CreateTruncating(dice.RollLong(rng));
		return sum;
	}
	public T RollFloat(Random rng)
	{
		var sum = T.Zero;
		for (T i = T.Zero; i < times; i++)
			sum += T.CreateTruncating(dice.RollFloat(rng));
		return sum;
	}
	public T RollDouble(Random rng)
	{
		var sum = T.Zero;
		for (T i = T.Zero; i < times; i++)
			sum += T.CreateTruncating(dice.RollDouble(rng));
		return sum;
	}

	public T Roll(Random rng)
	{
		T sum = T.Zero;
		for (T i = T.Zero; i < times; i++)
			sum += dice.Roll(rng);
		return sum;
	}

}

public readonly struct Dice<T> where T : unmanaged, INumber<T>
{
	public readonly T sides;

	public Dice(T sides) => this.sides = sides;

	public override string ToString() => 'D' + sides.ToString();

	public double GetAverage() => ((double)(object)(sides + T.One)) / 2.0;

	public int RollInt(Random rng) => rng.Next((int)(object)sides) + 1;
	public long RollLong(Random rng) => rng.NextInt64() * ((long)(object)sides) + 1;
	public float RollFloat(Random rng) => rng.NextSingle() * ((float)(object)sides) + 1;
	public double RollDouble(Random rng) => rng.NextDouble() * ((double)(object)sides) + 1;

	public T Roll(Random rng)
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
		return T.CreateSaturating(RollDouble(rng));
	}

}
