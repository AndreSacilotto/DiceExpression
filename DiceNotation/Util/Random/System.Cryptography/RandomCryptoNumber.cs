namespace Helper;

public class RandomCryptoNumber<T> : IRandomNumber<T> where T : INumber<T>
{
	//private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();
	//private static readonly byte[] buffer = new byte[sizeof(TDice)];

	public T Next()
	{
		throw new NotImplementedException();
	}

	public T Next(T max)
	{
		throw new NotImplementedException();
	}

	public T Next(T min, T max)
	{
		throw new NotImplementedException();
	}
}
