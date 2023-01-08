
using System.Collections.Immutable;

namespace DiceExpression;

public static partial class DiceShuntingYard<T>
{
	public enum Category : byte
	{
		Number = 1,
		UnaryPreOperator,
		UnaryPosOperator,
		Operator,
		Function,
		Bracket,
	};

	public enum Symbol : byte
	{
		Number = 1,

		/* Operators */
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Power,
		Remainer,

		/* Unary Operators */
		Negate,

		/* Unary Funcs */
		Floor,
		Ceil,
		//Round,
		//Sqtr,

		/* Brackets */
		OpenBracket,
		CloseBracket,

		/* Dice */
		//Roll,
		//KeepHighest,
		//KeepLowest,
		//Explode,
	}

	#region Token
	public interface IToken
	{
		Category Category { get; }
		Symbol Symbol { get; }
	}
	public class TokenNumber : IToken
	{
		public Category Category => Category.Number;
		public Symbol Symbol => Symbol.Number;

		public TokenNumber(T number) => Number = number;
		public T Number { get; }
		public override string ToString() => "" + Number.ToString();
	}
	public class TokenBasic : IToken
	{
		public TokenBasic(Symbol symbol, Category category)
		{
			Category = category;
			Symbol = symbol;
		}

		public Category Category { get; }
		public Symbol Symbol { get; }

		public override string ToString() => Symbol.ToString();
	}
	public class TokenUnary : TokenBasic
	{
		public delegate T UnaryFunc(T a);

		public UnaryFunc UnaryFunction { get; }
		public TokenUnary(Symbol symbol, Category category, UnaryFunc function) : base(symbol, category)
		{
			UnaryFunction = function;
		}
	}
	public class TokenBinary : TokenBasic
	{
		public delegate T BinaryFunc(T a, T b);
		public TokenBinary(Symbol symbol, Category category, BinaryFunc function) : base(symbol, category)
		{
			BinaryFunction = function;
		}
		public BinaryFunc BinaryFunction { get; }
		/// <summary> Priority of different operators </summary>
		public int Precedence { get; init; }
		/// <summary> 
		/// Used to distinct the precedence of operators of same symbol.
		/// <see langword="false"/> means left-associativity, <see langword="true"/> means right-associativity. 
		/// </summary>
		public bool RightAssociativity { get; init; }
	}
	#endregion

}

