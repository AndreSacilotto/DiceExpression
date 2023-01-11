
using System.Collections.Immutable;
using Helper;

namespace MathNotation.ShuntingYard;

public partial class ShuntingYard<T> where T : unmanaged, INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{
	public static ImmutableDictionary<Symbol, IToken> Symbols { get; } = CreateSymbolsDict();

	private static ImmutableDictionary<Symbol, IToken> CreateSymbolsDict()
	{
		var opt = new IToken[] {
			new TokenBasic(Symbol.OpenBracket, Category.Bracket),
			new TokenBasic(Symbol.CloseBracket, Category.Bracket),
			new TokenBasic(Symbol.ArgsSeparator, Category.Bracket),

			new TokenUnary<T>(Symbol.Floor, Category.Function, T.Floor),
			new TokenUnary<T>(Symbol.Ceil, Category.Function, T.Ceiling),
			new TokenUnary<T>(Symbol.Round, Category.Function, T.Round),
			new TokenUnary<T>(Symbol.Sqtr, Category.Function, T.Sqrt),
			new TokenUnary<T>(Symbol.Abs, Category.Function, T.Abs),

			new TokenBinary<T>(Symbol.Min, Category.Function, T.Min),
			new TokenBinary<T>(Symbol.Max, Category.Function, T.Max),
			new TokenTernary<T>(Symbol.Clamp, Category.Function, T.Clamp),

			new TokenUnary<T>(Symbol.Negate, Category.UnaryPreOperator, (a) => -a),

			new TokenUnary<T>(Symbol.Factorial, Category.UnaryPosOperator, UtilMath<T>.Factorial),

			new TokenBinaryOperator<T>(Symbol.Addition, (a, b) => a + b) { Precedence = 2 },
			new TokenBinaryOperator<T>(Symbol.Subtraction, (a, b) => a - b) { Precedence = 2 },
			new TokenBinaryOperator<T>(Symbol.Multiplication, (a, b) => a * b) { Precedence = 4 } ,
			new TokenBinaryOperator<T>(Symbol.Division, (a, b) => a / b) { Precedence = 4 } ,
			new TokenBinaryOperator<T>(Symbol.Remainer, (a, b) => a % b) { Precedence = 4 } ,
			new TokenBinaryOperator<T>(Symbol.Pow, T.Pow) { Precedence = 6, RightAssociativity = true },
		};

		var builder = ImmutableDictionary.CreateBuilder<Symbol, IToken>();
		foreach (var item in opt)
			builder.Add(item.Symbol, item);
		return builder.ToImmutable();

	}

}
