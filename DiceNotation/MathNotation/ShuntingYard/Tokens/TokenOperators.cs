namespace MathNotation.ShuntingYard;

public class TokenBinaryOperator<T> : TokenBinary<T>, ITokenPrecedence where T : unmanaged, INumber<T>
{
	public TokenBinaryOperator(Symbol symbol, BinaryFunc function) : base(symbol, Category.BinaryOperator, function) { }
	public int Precedence { get; init; } = 0;
	public bool RightAssociativity { get; init; } = false;
}
