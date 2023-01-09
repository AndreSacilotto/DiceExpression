
using System.Collections.Immutable;

namespace DiceExpression;

public static partial class DiceShuntingYard<T>
{
	public enum Category : byte
	{
		Number,
		Bracket,
		Function,
		/// <summary>By default operators are considered binary</summary>
		Operator,
		UnaryPreOperator,
		UnaryPosOperator,
	};

	public enum Symbol : byte
	{
		None = 0,

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

		/* Unary Funcs */
		Floor,
		Ceil,
		Round,
		Sqtr,
		Abs,

		/* Brackets */
		OpenBracket,
		CloseBracket,

		/* Dice */
		//Roll, // 'XdY' Operator 
		//KeepHighest, Func (dice, number to keep)
		//KeepLowest, Func (dice, number to keep)
		//Explode, Func (dice, number to keep)
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
		public Symbol Symbol => Symbol.None;

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

		public TokenUnary(Symbol symbol, Category category, UnaryFunc function) : base(symbol, category) => UnaryFunction = function;
		
		public UnaryFunc UnaryFunction { get; }
	}
	public class TokenBinary : TokenBasic
	{
		public delegate T BinaryFunc(T a, T b);
		public TokenBinary(Symbol symbol, Category category, BinaryFunc function) : base(symbol, category) => BinaryFunction = function;
		public BinaryFunc BinaryFunction { get; }

		/// <summary> Priority of different operators </summary>
		public int Precedence { get; init; } = 0;

		/// <summary> 
		/// Used to distinct the precedence/resolution of operators of same symbol. <br/>
		/// <see langword="False"/> means left-associativity [left to right] (+ - / *), <br/>
		/// <see langword="True"/> means right-associativity [right to left] (^ EE √).
		/// </summary>
		public bool RightAssociativity { get; init; } = false;
	}
	#endregion

}

