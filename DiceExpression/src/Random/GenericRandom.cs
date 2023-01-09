namespace DiceExpression;

public class GenericRandom<T> : IRandom<T, int> where T : unmanaged, INumber<T>
{
	private Random rng = null!;
	private int seed;

	public GenericRandom(int seed) => Seed = seed;

	public int Seed
	{
		get => seed; set {
			seed = value;
			rng = new(seed);
		}
	}

	public T Next()
	{
		var type = typeof(T);

		if (type == typeof(int))
			return T.CreateChecked(rng.Next());

		if (type == typeof(long))
			return T.CreateChecked(rng.NextInt64());

		if (type == typeof(float))
			return T.CreateChecked(rng.NextSingle() * float.CreateTruncating(float.MaxValue-1f));

		if (type == typeof(double))
			return T.CreateChecked(rng.NextDouble() * double.CreateTruncating(double.MaxValue-1d));

		throw new NonRandomTypeException();
	}

	public T Next(T max)
	{
		var type = typeof(T);

		if (type == typeof(int))
			return T.CreateChecked(rng.Next(int.CreateSaturating(max) + 1));

		if (type == typeof(long))
			return T.CreateChecked(rng.NextInt64(long.CreateSaturating(max) + 1L));

		if (type == typeof(float))
			return T.CreateChecked(rng.NextSingle() * float.CreateSaturating(max));

		if (type == typeof(double))
			return T.CreateChecked(rng.NextDouble() * double.CreateSaturating(max));

		throw new NonRandomTypeException();

	}

	public T Next(T min, T max)
	{
		var type = typeof(T);

		if (type == typeof(int))
			return T.CreateChecked(rng.Next(int.CreateSaturating(min), int.CreateSaturating(max) + 1));

		if (type == typeof(long))
			return T.CreateChecked(rng.NextInt64(long.CreateSaturating(min), long.CreateSaturating(max) + 1L));

		if (type == typeof(float))
			return T.CreateChecked(rng.NextSingle() * float.CreateSaturating(max) + float.CreateSaturating(min));

		if (type == typeof(double))
			return T.CreateChecked(rng.NextDouble() * double.CreateSaturating(max) + double.CreateSaturating(min));

		throw new NonRandomTypeException();
	}

	private class NonRandomTypeException : Exception
	{
		public NonRandomTypeException() : base($"{typeof(T)} is not of type (int, long, float, double) so unsupported") { }
	}

}
