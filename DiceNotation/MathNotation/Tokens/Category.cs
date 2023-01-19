namespace MathNotation;

//[Flags]
public enum Category : int
{
	None = 0,
	Number = 1 << 0,
	OpenBracket = 1 << 1,
	CloseBracket = 1 << 2,
	ParamSeparator = 1 << 3,
	Function = 1 << 4,
	Operator = 1 << 5,
	PreOperator = 1 << 6,
	PostOperator = 1 << 7,
}