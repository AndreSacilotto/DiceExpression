using System.Numerics;

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

	public T Roll(IRandom<T, S> rng)
	{
		T sum = T.Zero;
		for (int i = 0; i < times; i++)
			sum += dice.Roll(rng);
		return sum;
	}

}

public readonly struct Dice<T, S> where T : unmanaged, INumber<T>
{
	public readonly T sides;

	public Dice(T sides) => this.sides = sides;

	public override string ToString() => 'D' + sides.ToString();

	public double GetAverage() => double.CreateSaturating(sides + T.One) / 2.0;

	public T Roll(IRandom<T, S> rng) => rng.Next(T.One, sides);

}
