namespace MathNotation.ShuntingYard;

public enum Symbol : byte
{
	None = 0,

	/* Brackets */
	OpenBracket,
	CloseBracket,
	ParamsSeparator,

	/* Binary operators */
	Addition,
	Subtraction,
	Multiplication,
	Division,
	Pow,
	Remainer,

	/* Unary Pre operators */
	Negate,

	/* Unary Pos operators */
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
