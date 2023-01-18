namespace MathNotation;

public class TokenNullary<T> : TokenBasic, ITokenNAry where T : INumber<T>
{
	public delegate T NullaryFunc();

	public TokenNullary(NullaryFunc function) : base(Category.Function) => NullaryFunction = function;

	public NullaryFunc NullaryFunction { get; }
}

public class TokenUnary<T> : TokenBasic, ITokenNAry where T : INumber<T>
{
	public delegate T UnaryFunc(T a);

	public TokenUnary(UnaryFunc function) : base(Category.Function) => UnaryFunction = function;

	public UnaryFunc UnaryFunction { get; }
}

public class TokenBinary<T> : TokenBasic, ITokenNAry where T : INumber<T>
{
	public delegate T BinaryFunc(T a, T b);

	protected TokenBinary(Category category, BinaryFunc function) : base(category) => BinaryFunction = function;
	public TokenBinary(BinaryFunc function) : base(Category.Function) => BinaryFunction = function;
	public BinaryFunc BinaryFunction { get; }
}

public class TokenTernary<T> : TokenBasic, ITokenNAry where T : INumber<T>
{
	public delegate T TernaryFunc(T a, T b, T c);

	public TokenTernary(TernaryFunc function) : base(Category.Function) => TernaryFunction = function;
	public TernaryFunc TernaryFunction { get; }
}

public class TokenBinaryOperator<T> : TokenBinary<T>, ITokenPrecedence where T : INumber<T>
{
	public TokenBinaryOperator(BinaryFunc function) : base(Category.Operator, function) { }
	public int Precedence { get; init; } = 0;
	public bool RightAssociativity { get; init; } = false;
}