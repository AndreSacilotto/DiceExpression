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
		["kh"] = Operator.Great,
		["kl"] = Operator.Less,
		["khl"] = Operator.Equal,
		["km"] = Operator.Equal,
		["k"] = Operator.NotEqual,
		["dh"] = Operator.NotEqual,
		["dl"] = Operator.GreatEqual,
		["dm"] = Operator.Less,
		["d"] = Operator.Assign,
		["min"] = Operator.Assign,
		["max"] = Operator.Assign,
		["cap"] = Operator.Assign,
		["clamp"] = Operator.Assign,
		["r"] = Operator.Assign,
		["u"] = Operator.Assign,
		["!"] = Operator.Assign,
		["rr"] = Operator.Assign,
	};

	public readonly Dictionary<string, Operator> Funcs = new() {
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