namespace MathExpression.ShuntingYard;

public enum Symbol : byte
{
	None = 0,

	/* Brackets */
	OpenBracket,
	CloseBracket,
	ArgsSeparator,

	/* Binary Operators */
	Addition,
	Subtraction,
	Multiplication,
	Division,
	Pow,
	Remainer,

	/* Unary Pre Operators */
	Negate,

	/* Unary Pos Operators */
	Factorial,

	/* Funcs */
	Floor,
	Ceil,
	Round,
	Sqtr,
	Abs,
	Min,
	Max,
	Clamp,
}
