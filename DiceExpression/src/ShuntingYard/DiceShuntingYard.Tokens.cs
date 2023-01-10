
namespace DiceExpression.ShuntingYard;

#region Token Base

public interface IToken
{
	Category Category { get; }
	Symbol Symbol { get; }
}

public class TokenBasic : IToken
{
	public Category Category { get; }
	public Symbol Symbol { get; }

	public TokenBasic(Symbol symbol, Category category)
	{
		Category = category;
		Symbol = symbol;
	}

	public override string ToString() => Symbol.ToString();
}

public class TokenNumber<T> : IToken where T : unmanaged, INumber<T>
{
	public Category Category => Category.Number;
	public Symbol Symbol => Symbol.None;

	public TokenNumber(T number) => Number = number;
	public T Number { get; }

	public override string ToString() => Number.ToString() + "";
}

#endregion

#region Token Funcs

public class TokenUnary : TokenBasic
{
	public delegate IToken UnaryFunc(IToken a);

	public TokenUnary(Symbol symbol, Category category, UnaryFunc function) : base(symbol, category) => UnaryFunction = function;

	public UnaryFunc UnaryFunction { get; }
}

public interface ITokenPrecedence : IToken
{
	/// <summary> Priority of different operators </summary>
	int Precedence { get; init; }

	/// <summary> 
	/// Used to distinct the precedence/resolution of operators of same symbol. <br/>
	/// <see langword="False"/> means left-associativity [left to right] (+ - / *), <br/>
	/// <see langword="True"/> means right-associativity [right to left] (^ EE √).
	/// </summary>
	bool RightAssociativity { get; init; }
}

public class TokenBinary : TokenBasic, ITokenPrecedence
{
	public delegate IToken BinaryFunc(IToken a, IToken b);
	public TokenBinary(Symbol symbol, Category category, BinaryFunc function) : base(symbol, category) => BinaryFunction = function;
	public BinaryFunc BinaryFunction { get; }

	public int Precedence { get; init; } = 0;
	public bool RightAssociativity { get; init; } = false;
}

#endregion
