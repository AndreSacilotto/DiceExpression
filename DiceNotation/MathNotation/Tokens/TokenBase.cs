using DiceNotation;

namespace MathNotation;

public class TokenBasic : IToken
{
	public Category Category { get; init; }
	public string Name { get; set; } = "";

	public TokenBasic(Category category)
	{
		Category = category;
	}

	public override string ToString() => Name.ToString();
}


public class TokenNumber<T> : ITokenNumber<T> where T : INumber<T>
{
	public T Number { get; }
	public string Name => ToString();

	public Category Category => Category.Number;

	public TokenNumber(T number) => Number = number;
	public override string ToString() => Number.ToString() + "";
}


public class TokenConstant<T> : ITokenNumber<T> where T : INumber<T>
{
	public T Number { get; }
	public string Name { get; }

	public Category Category => Category.Number;

	public TokenConstant(string name, T number)
	{
		Number = number;
		Name = name;
	}

	public override string ToString() => Name;
}
