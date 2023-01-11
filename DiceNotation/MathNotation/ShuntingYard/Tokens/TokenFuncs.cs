namespace MathNotation.ShuntingYard;

public class TokenUnary<T> : TokenBasic where T : unmanaged, INumber<T>
{
	public delegate T UnaryFunc(T a);

	public TokenUnary(Symbol symbol, Category category, UnaryFunc function) : base(symbol, category) => UnaryFunction = function;

	public UnaryFunc UnaryFunction { get; }
}

public class TokenBinary<T> : TokenBasic where T : unmanaged, INumber<T>
{
    public delegate T BinaryFunc(T a, T b);

    public TokenBinary(Symbol symbol, Category category, BinaryFunc function) : base(symbol, category) => BinaryFunction = function;
    public BinaryFunc BinaryFunction { get; }
}

public class TokenTernary<T> : TokenBasic where T : unmanaged, INumber<T>
{
	public delegate T TernaryFunc(T a, T b, T c);

	public TokenTernary(Symbol symbol, Category category, TernaryFunc function) : base(symbol, category) => TernaryFunction = function;
	public TernaryFunc TernaryFunction { get; }
}

public class TokenNth<T> : TokenBasic where T : unmanaged, INumber<T>
{
	public delegate T NthFunc(params T[] tokens);

	public TokenNth(Symbol symbol, Category category, NthFunc function) : base(symbol, category) => NthFunction = function;
	public NthFunc NthFunction { get; }

	public required int ParamsCount { get; init; }
}