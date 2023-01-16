namespace DiceNotation;

public class TokenNullary<T> : TokenBasic, ITokenNAry where T : unmanaged, INumber<T>
{
	public delegate T NullaryFunc();

	public TokenNullary(NullaryFunc function) => UnaryFunction = function;

	public NullaryFunc UnaryFunction { get; }
}

public class TokenUnary<T> : TokenBasic, ITokenNAry where T : unmanaged, INumber<T>
{
	public delegate T UnaryFunc(T a);

	public TokenUnary(UnaryFunc function) => UnaryFunction = function;

	public UnaryFunc UnaryFunction { get; }
}

public class TokenBinary<T> : TokenBasic, ITokenNAry where T : unmanaged, INumber<T>
{
	public delegate T BinaryFunc(T a, T b);

	public TokenBinary(BinaryFunc function) => BinaryFunction = function;
	public BinaryFunc BinaryFunction { get; }
}

public class TokenTernary<T> : TokenBasic, ITokenNAry where T : unmanaged, INumber<T>
{
	public delegate T TernaryFunc(T a, T b, T c);

	public TokenTernary(TernaryFunc function) => TernaryFunction = function;
	public TernaryFunc TernaryFunction { get; }
}

public class TokenBinaryOperator<T> : TokenBinary<T>, ITokenPrecedence where T : unmanaged, INumber<T>
{
	public TokenBinaryOperator(BinaryFunc function) : base(function) => Category = Category.Operator;
	public int Precedence { get; init; } = 0;
	public bool RightAssociativity { get; init; } = false;
}