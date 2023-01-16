using Helper;

namespace DiceNotation;

public readonly struct Dice<T> where T : INumber<T>
{
	public readonly T sides;

	public Dice(T sides) => this.sides = sides;

	public override string ToString() => 'D' + sides.ToString();

	public double GetAverage() => double.CreateSaturating(sides + T.One) / 2.0;

	public T Roll(IRandomNumber<T> rng) => rng.Next(T.One, sides);

	public T RollExplosive(IRandomNumber<T> rng)
	{
		if (sides <= T.One)
			throw new Exception("Infinity explosion");

		T sum = T.Zero;
		T value;
		do
		{
			value = Roll(rng);
			sum += value;
		} while (value == sides);
		return sum;
	}

	public T[] RollKeepExplosive(IRandomNumber<T> rng)
	{
		if (sides <= T.One)
			throw new Exception("Infinity explosion");

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
