using System.Reflection.Emit;
using System.Security.Cryptography;

namespace DiceNotation;

public class RandomCryptoNumber<T> : IRandomNumber<T> where T : INumber<T>
{
	//private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
	//private static readonly byte[] buffer = new byte[sizeof(T)];

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
