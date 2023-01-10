
using System.Collections.Immutable;

namespace DiceExpression.ShuntingYard;

public partial class DiceShuntingYard<T> where T : unmanaged, INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{
	public static ImmutableDictionary<Symbol, IToken> Symbols { get; } = CreateSymbolsDict();

	private static ImmutableDictionary<Symbol, IToken> CreateSymbolsDict()
	{
		static T AsNumber(IToken token)
		{
			if (token is TokenNumber<Integer> ti)
				return T.CreateTruncating(ti.Number);
			return ((TokenNumber<T>)token).Number;
		}

		static int AsInt(IToken token) => int.CreateTruncating(((TokenNumber<T>)token).Number);

		var opt = new IToken[] {
			new TokenBasic(Symbol.OpenBracket, Category.Bracket),
			new TokenBasic(Symbol.CloseBracket, Category.Bracket),
			new TokenBasic(Symbol.ArgsSeparator, Category.Bracket),

			new TokenUnary(Symbol.Floor, Category.Function, (a) => new TokenNumber<T>(T.Floor(AsNumber(a))) ),
			new TokenUnary(Symbol.Ceil, Category.Function, (a) => new TokenNumber<T>(T.Ceiling(AsNumber(a))) ),
			new TokenUnary(Symbol.Round, Category.Function, (a) => new TokenNumber<T>(T.Round(AsNumber(a))) ),
			new TokenUnary(Symbol.Sqtr, Category.Function, (a) => new TokenNumber<T>(T.Sqrt(AsNumber(a))) ),
			new TokenUnary(Symbol.Abs, Category.Function, (a) => new TokenNumber<T>(T.Abs(AsNumber(a))) ),

			new TokenUnary(Symbol.Negate, Category.UnaryPreOperator, (a) => new TokenNumber<T>(-AsNumber(a))),
			new TokenUnary(Symbol.Factorial, Category.UnaryPosOperator, (a) => new TokenNumber<T>(MathExt<T>.Factorial(AsNumber(a)))),

			new TokenBinary(Symbol.Addition, Category.BinaryOperator, (a, b) => new TokenNumber<T>(AsNumber(a) + AsNumber(b)) ) { Precedence = 2 },
			new TokenBinary(Symbol.Subtraction, Category.BinaryOperator, (a, b) => new TokenNumber<T>(AsNumber(a) - AsNumber(b))) { Precedence = 2 },
			new TokenBinary(Symbol.Multiplication, Category.BinaryOperator, (a, b) => new TokenNumber<T>(AsNumber(a) * AsNumber(b))) { Precedence = 4 } ,
			new TokenBinary(Symbol.Division, Category.BinaryOperator, (a, b) => new TokenNumber<T>(AsNumber(a) / AsNumber(b))) { Precedence = 4 } ,
			new TokenBinary(Symbol.Remainer, Category.BinaryOperator, (a, b) => new TokenNumber<T>(AsNumber(a) % AsNumber(b))) { Precedence = 4 } ,
			new TokenBinary(Symbol.Pow, Category.BinaryOperator, (a, b) => new TokenNumber<T>(T.Pow(AsNumber(a), AsNumber(b)))) { Precedence = 6, RightAssociativity = true },

			//new TokenUnary(Symbol.Dice, Category.UnaryPreOperator, (s) => new TokenNumber<T>(new Dice<T, int>(AsNumber(s)).Roll(Random)) ),
			//new TokenBinary(Symbol.DiceRoll, Category.BinaryOperator, (t, s) => new TokenNumber<T>(new DiceRoll<T, int>(AsInt(t), AsNumber(s)).RollAndSum(Random)) ) { Precedence = 10 },
			//new TokenBinary(Symbol.Explode, Category.Function, (a, s) => new TokenNumber<T>(new Dice<T, int>(AsNumber(s)).RollExplosive(Random)) ),
		};

		var builder = ImmutableDictionary.CreateBuilder<Symbol, IToken>();
		foreach (var item in opt)
			builder.Add(item.Symbol, item);
		return builder.ToImmutable();

	}

}
