namespace Helper;

public class RandomGeneric<T> : IRandom<T, int> where T : INumber<T>
{
	private Random rng = null!;
	private int seed;

	public RandomGeneric() => rng = new();
	public RandomGeneric(int seed) => Seed = seed;

	public int Seed
	{
		get => seed; set {
			seed = value;
			rng = new(seed);
		}
	}

	public T Next()
	{
		switch (Type.GetTypeCode(typeof(T)))
		{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			{
				return T.CreateChecked(rng.Next());
			}
			case TypeCode.Int64:
			case TypeCode.UInt64:
			{
				return T.CreateChecked(rng.NextInt64());
			}
			case TypeCode.Single:
			{
				return T.CreateChecked(rng.NextSingle() * float.CreateTruncating(float.MaxValue - 1f));
			}
			case TypeCode.Double:
			case TypeCode.Decimal:
			{
				return T.CreateChecked(rng.NextDouble() * double.CreateTruncating(double.MaxValue - 1d));
			}
		}
		throw new NotSupportedException(ErrorMsg());
	}

	public T Next(T max)
	{
		switch (Type.GetTypeCode(typeof(T)))
		{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			{
				return T.CreateChecked(rng.Next(int.CreateSaturating(max) + 1));
			}
			case TypeCode.Int64:
			case TypeCode.UInt64:
			{
				return T.CreateChecked(rng.NextInt64(long.CreateSaturating(max) + 1L));
			}
			case TypeCode.Single:
			{
				return T.CreateChecked(rng.NextSingle() * float.CreateSaturating(max));
			}
			case TypeCode.Double:
			case TypeCode.Decimal:
			{
				return T.CreateChecked(rng.NextDouble() * double.CreateSaturating(max));
			}
		}
		throw new NotSupportedException(ErrorMsg());
	}

	public T Next(T min, T max)
	{
		switch (Type.GetTypeCode(typeof(T)))
		{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			{
				return T.CreateChecked(rng.Next(int.CreateSaturating(min), int.CreateSaturating(max) + 1));
			}
			case TypeCode.Int64:
			case TypeCode.UInt64:
			{
				return T.CreateChecked(rng.NextInt64(long.CreateSaturating(min), long.CreateSaturating(max) + 1L));
			}
			case TypeCode.Single:
			{
				return T.CreateChecked(rng.NextSingle() * float.CreateSaturating(max) + float.CreateSaturating(min));
			}
			case TypeCode.Double:
			case TypeCode.Decimal:
			{
				return T.CreateChecked(rng.NextDouble() * double.CreateSaturating(max) + double.CreateSaturating(min));
			}
		}

		throw new NotSupportedException(ErrorMsg());
	}

	private static string ErrorMsg() =>
		$"{typeof(T)} is not of type(int, long, float, double) so unsupported";

}
