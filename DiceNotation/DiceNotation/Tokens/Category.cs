namespace DiceNotation;

public enum Category
{
	Number,
	OpenBracket,
	CloseBracket,
	ParamSeparator,
	Function,
	Operator,
	PreOperator,
	PostOperator,
}

public enum Operator 
{ 
	Great,
	Less,
	Equal,
	NotEqual,
	GreatEqual,
	LessEqual,
	Assign,
}

public class Temp {

	public readonly Dictionary<string, Operator> Operators = new() {
		[">"] = Operator.Great,
		["<"] = Operator.Less,
		["="] = Operator.Equal,
		["=="] = Operator.Equal,
		["!="] = Operator.NotEqual,
		["<>"] = Operator.NotEqual,
		[">="] = Operator.GreatEqual,
		["<="] = Operator.Less,
		["->"] = Operator.Assign,
	};
}