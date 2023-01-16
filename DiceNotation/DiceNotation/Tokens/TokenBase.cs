namespace DiceNotation;

public class TokenBasic : IToken
{
	public Category Category { get; init; }
	public string Name { get; set; } = string.Empty;

	protected TokenBasic() { }

	public TokenBasic(Category category)
	{
		Category = category;
	}

	public override string ToString() => Name;
}

public class TokenNumber<T> : IToken where T : INumber<T>
{
	public T Number { get; }

	public Category Category => Category.Number;
	public string Name => Number.ToString() + "";

	public TokenNumber(T number) => Number = number;
	public override string ToString() => Name;
}
