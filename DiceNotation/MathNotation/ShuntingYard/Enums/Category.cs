namespace MathNotation.ShuntingYard;

public enum Category : byte
{
	Number,
	Bracket,
	Function,
	/// <summary>By default operators are considered binary</summary>
	BinaryOperator,
	UnaryPreOperator,
	UnaryPosOperator,
};
