
using System.Collections.Immutable;
using Helper;

namespace MathNotation.ShuntingYard;

public partial class ShuntingYard<T> where T : unmanaged, INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{
	public static ImmutableDictionary<char, IToken> SymbolsChar { get; }
	public static ImmutableDictionary<string, IToken> SymbolsString { get; }

	public static IToken? FindSymbol(Symbol symbol)
	{
		foreach (var item in ShuntingYard<T>.SymbolsString)
			if (item.Value.Symbol == symbol)
				return item.Value;
		foreach (var item in ShuntingYard<T>.SymbolsString)
			if (item.Value.Symbol == symbol)
				return item.Value;
		return default;
	}

	static ShuntingYard()
	{
		var c = new Dictionary<char, IToken> {
			['('] = new TokenBasic(Symbol.OpenBracket, Category.Bracket),
			[')'] = new TokenBasic(Symbol.CloseBracket, Category.Bracket),
			[','] = new TokenBasic(Symbol.ParamsSeparator, Category.Bracket),

			['~'] = new TokenUnary<T>(Symbol.Negate, Category.UnaryPreOperator, (a) => -a),

			['!'] = new TokenUnary<T>(Symbol.Factorial, Category.UnaryPosOperator, UtilMath<T>.Factorial),

			['+'] = new TokenBinaryOperator<T>(Symbol.Addition, (a, b) => a + b) { Precedence = 2 },
			['-'] = new TokenBinaryOperator<T>(Symbol.Subtraction, (a, b) => a - b) { Precedence = 2 },
			['*'] = new TokenBinaryOperator<T>(Symbol.Multiplication, (a, b) => a * b) { Precedence = 4 },
			['/'] = new TokenBinaryOperator<T>(Symbol.Division, (a, b) => a / b) { Precedence = 4 },
			['%'] = new TokenBinaryOperator<T>(Symbol.Remainer, (a, b) => a % b) { Precedence = 4 },
			['^'] = new TokenBinaryOperator<T>(Symbol.Pow, T.Pow) { Precedence = 6, RightAssociativity = true },
		};

		var s = new Dictionary<string, IToken> {
			["floor"] = new TokenUnary<T>(Symbol.Floor, Category.Function, T.Floor),
			["round"] = new TokenUnary<T>(Symbol.Ceil, Category.Function, T.Ceiling),
			["ceil"] = new TokenUnary<T>(Symbol.Round, Category.Function, T.Round),
			["sqtr"] = new TokenUnary<T>(Symbol.Sqtr, Category.Function, T.Sqrt),
			["abs"] = new TokenUnary<T>(Symbol.Abs, Category.Function, T.Abs),
			["min"] = new TokenBinary<T>(Symbol.Min, Category.Function, T.Min),
			["max"] = new TokenBinary<T>(Symbol.Max, Category.Function, T.Max),
			["clamp"] = new TokenTernary<T>(Symbol.Clamp, Category.Function, T.Clamp),
		};
		//Aliases
		s.Add("cap", s["clamp"]);
		//ToImmutable
		SymbolsChar = c.ToImmutableDictionary();
		SymbolsString = s.ToImmutableDictionary();
	}

}
