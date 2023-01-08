
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
		UnaryFunction,
		OpenBracket,
		CloseBracket,
	};

	public enum Symbol : byte
	{
		/* Operators */
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Pow,
		Remainer,

		/* Unary Operators */
		Negate,

		/* Unary Funcs */
		Floor,
		Ceil,
		Round,
		Sqtr,

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
	}

	public class TokenNumber : IToken
	{
		public Category Category => Category.Number;

		public TokenNumber(T number) => Number = number;
		public T Number { get; }
		public override string ToString() => Number.ToString() ?? string.Empty;
	}
	public class TokenBasic : IToken
	{
		public TokenBasic(Category category)
		{
			Category = category;
		}

		public Category Category { get; }
		public Symbol Symbol { get; }

		public override string ToString() => Symbol.ToString();
	}
	public class TokenUnary : TokenBasic
	{
		public delegate T UnaryFunc(T a);

		public UnaryFunc UnaryFunction { get; }
		public TokenUnary(Category category, UnaryFunc function) : base(category)
		{
			UnaryFunction = function;
		}
	}
	public class TokenBinary : TokenBasic
	{
		public delegate T BinaryFunc(T a, T b);

		public TokenBinary(Category category, BinaryFunc binaryFunction) : base(category)
		{
			BinaryFunction = binaryFunction;
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

