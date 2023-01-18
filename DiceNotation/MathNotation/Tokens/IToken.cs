namespace MathNotation;

public interface IToken
{
	Category Category { get; }
	string Name { get; }
}

public interface ITokenNumber<T> : IToken where T : INumber<T>
{
	T Number { get; }
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

/// <summary>
/// Nullary = 0-ary<br/>Unary = 1-ary<br/>
/// Binary = 2-ary<br/>Ternary = 3-ary<br/>
/// Quaternary = 4-ary<br/>Quinary = 5-ary<br/>
/// Senary = 6-ary<br/>Septenary = 7-ary<br/>
/// Octary = 8-ary<br/>Nonary = 9-ary<br/>...
/// </summary>
public interface ITokenNAry : IToken { }

