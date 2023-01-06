
using System.Collections.Immutable;

namespace DiceExpression;

public static class DiceShuntingYard<T> where T : struct, INumber<T>
{
	public enum Category
	{
		Number,
		UnaryPrefixOperator,
		UnaryPostfixOperator,
		Operator,
		Function,
		Bracket,
	};

	public enum Symbol
	{
		Number,

		/* Left Operators */
		Addition,
		Subtraction,
		Multiplication,
		Division,
		//Remainer,

		/* Right Operators */
		Negate,
		//Power,
		//Sqtr,

		/* Unary Funcs */
		//Floor,
		//Ceil,
		//Round,

		/* Brackets */
		OpenBracket,
		CloseBracket,

		/* Dice */
		//Roll,
		//KeepHigest,
		//KeepLowest,
		//Explode,
	}

	#region Token
	public interface IToken
	{
		Category Category { get; }
		Symbol Symbol { get; }
	}

	public class TokenOpt : IToken
	{
		public TokenOpt(Symbol symbol, Category category)
		{
			Category = category;
			Symbol = symbol;
		}

		public Category Category { get; }
		public Symbol Symbol { get; }

		public int Precedence { get; init; } = 0;
		public bool RightAssociativity { get; init; } = false;
	
		public override string ToString() => Symbol.ToString();
	}

	public class TokenNumber : IToken
	{
		public Category Category => Category.Number;
		public Symbol Symbol => Symbol.Number;

		public TokenNumber(T number) => Number = number;
		public T Number { get; }
		public override string ToString() => Number.ToString() ?? string.Empty;
	}
	#endregion

	public static ImmutableDictionary<Symbol, TokenOpt> Symbols { get; } = new Dictionary<Symbol, TokenOpt>() {
		//[Symbol.Number] = new(Symbol.Number, Category.Number),

		[Symbol.OpenBracket] = new(Symbol.OpenBracket, Category.Bracket),
		[Symbol.CloseBracket] = new(Symbol.CloseBracket, Category.Bracket),

		[Symbol.Addition] = new(Symbol.Addition, Category.Operator) { Precedence = 2 },
		[Symbol.Subtraction] = new(Symbol.Subtraction, Category.Operator) { Precedence = 2 },
		[Symbol.Multiplication] = new(Symbol.Multiplication, Category.Operator) { Precedence = 4 },
		[Symbol.Division] = new(Symbol.Division, Category.Operator) { Precedence = 4 },

		[Symbol.Negate] = new(Symbol.Negate, Category.UnaryPrefixOperator) { Precedence = 8, RightAssociativity = true },
	}.ToImmutableDictionary();

	//https://www.andr.mu/logs/the-shunting-yard-algorithm/
	public static IToken[] InfixToPostfix(IToken[] infixTokens)
	{
		Stack<IToken> stack = new(3);
		Queue<IToken> queue = new(infixTokens.Length / 2);

		for (int i = 0; i < infixTokens.Length; i++)
		{
			var token = infixTokens[i];

			switch (token.Category)
			{
				case Category.Number:
				case Category.UnaryPostfixOperator:
				{
					queue.Enqueue(token);
					break;
				}
				case Category.UnaryPrefixOperator:
				case Category.Function:
				{
					stack.Push(token);
					break;
				}
				case Category.Operator:
				{
					var tk = (TokenOpt)token;
					if (tk.RightAssociativity)
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.Operator && ((TokenOpt)peek).Precedence > tk.Precedence)
							queue.Enqueue(stack.Pop());
					}
					else
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.Operator && ((TokenOpt)peek).Precedence >= tk.Precedence)
							queue.Enqueue(stack.Pop());
					}
					stack.Push(token);
					break;
				}
				case Category.Bracket:
				{
					if (token.Symbol == Symbol.OpenBracket)
						stack.Push(token);
					else
					{
						while (stack.Peek().Symbol != Symbol.OpenBracket)
							queue.Enqueue(stack.Pop());

						//Remove Close Bracket 
						stack.Pop();

						if (stack.TryPeek(out var peek) && peek.Category == Category.Function)
							queue.Enqueue(stack.Pop());
					}
					break;
				}

				default: throw new Exception("The symbol dont exist");
			}
		}

		while (stack.Count > 0)
			queue.Enqueue(stack.Pop());

		return queue.ToArray();
	}

	/// <summary>Evaluate the Postfix array</summary>
	public static T EvaluatePostfixTokens(IToken[] postfixTokens)
	{
		var stack = new Stack<IToken>(3);

		for (int i = 0; i < postfixTokens.Length; i++)
		{
			var token = postfixTokens[i];

			if (token.Category == Category.Number)
				stack.Push(token);
			else if (token.Category == Category.Operator)
			{
				var b = ((TokenNumber)stack.Pop()).Number;
				var a = ((TokenNumber)stack.Pop()).Number;

				IToken tk = token.Symbol switch {
					Symbol.Addition => new TokenNumber(a + b),
					Symbol.Subtraction => new TokenNumber(a - b),
					Symbol.Multiplication => new TokenNumber(a * b),
					Symbol.Division => new TokenNumber(a / b),
					_ => throw new ArithmeticException($"The {token.Symbol} is not an operator"),
				};

				stack.Push(tk);
			}
			else
				throw new Exception("Invalid postfix token");

		}

		if (stack.Count != 1)
			throw new Exception("Too many tokens on stack, invalid formula");
		var result = stack.Pop();
		if (result is not TokenNumber tn)
			throw new Exception("Result is not a Number");
		return tn.Number;
	}

}

