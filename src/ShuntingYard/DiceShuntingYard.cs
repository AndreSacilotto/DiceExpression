
using System.Collections.Immutable;

namespace DiceExpression;

public static partial class DiceShuntingYard<T> where T : INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{
	public static ImmutableDictionary<Symbol, IToken> Symbols { get; } = new Dictionary<Symbol, IToken>() {
		// Basic
		[Symbol.OpenBracket] = new TokenBasic(Symbol.OpenBracket, Category.Bracket),
		[Symbol.CloseBracket] = new TokenBasic(Symbol.CloseBracket, Category.Bracket),

		//Un - Funcs
		[Symbol.Floor] = new TokenUnary(Symbol.Floor, Category.Function, T.Floor),
		[Symbol.Ceil] = new TokenUnary(Symbol.Ceil, Category.Function, T.Ceiling),

		//Bi - Funcs

		//Un - Operators
		[Symbol.Negate] = new TokenUnary(Symbol.Negate, Category.UnaryPreOperator, x => -x),

		//Bi - Operators
		[Symbol.Addition] = new TokenBinary(Symbol.Addition, Category.Operator, (a, b) => a + b) { Precedence = 2 },
		[Symbol.Subtraction] = new TokenBinary(Symbol.Subtraction, Category.Operator, (a, b) => a - b) { Precedence = 2 },
		[Symbol.Multiplication] = new TokenBinary(Symbol.Multiplication, Category.Operator, (a, b) => a * b) { Precedence = 4 },
		[Symbol.Division] = new TokenBinary(Symbol.Division, Category.Operator, (a, b) => a / b) { Precedence = 4 },
		[Symbol.Remainer] = new TokenBinary(Symbol.Remainer, Category.Operator, (a, b) => a % b) { Precedence = 4 },
		[Symbol.Power] = new TokenBinary(Symbol.Power, Category.Operator, T.Pow) { Precedence = 10, RightAssociativity = true },

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
				case Category.UnaryPosOperator:
				{
					queue.Enqueue(token);
					break;
				}
				case Category.UnaryPreOperator:
				case Category.Function:
				{
					stack.Push(token);
					break;
				}
				case Category.Operator:
				{
					var tk = (TokenBinary)token;
					if (tk.RightAssociativity)
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.Operator && ((TokenBinary)peek).Precedence > tk.Precedence)
							queue.Enqueue(stack.Pop());
					}
					else
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.Operator && ((TokenBinary)peek).Precedence >= tk.Precedence)
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
				default:
				throw new Exception($"The token of {token.Category} category dont exist");
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

			switch (token.Category)
			{
				case Category.Number:
				{
					stack.Push(token);
					break;
				}
				case Category.UnaryPreOperator:
				case Category.UnaryPosOperator:
				case Category.Function:
				case Category.Operator:
				{
					IToken tk;
					if (token is TokenUnary tu)
					{
						var n = ((TokenNumber)stack.Pop()).Number;
						tk = new TokenNumber(tu.UnaryFunction(n));
					}
					else if (token is TokenBinary tb)
					{
						var b = ((TokenNumber)stack.Pop()).Number;
						var a = ((TokenNumber)stack.Pop()).Number;
						tk = new TokenNumber(tb.BinaryFunction(a, b));
					}
					else
						throw new Exception($"The {token.Symbol} is not valid token type");

					stack.Push(tk);
					break;
				}
				case Category.Bracket:
				throw new Exception("Postfix cant contain brackets");
				default:
				throw new Exception($"The token of {token.Category} category dont exist");
			}

		}

		if (stack.Count != 1)
			throw new Exception("Too many tokens on stack, invalid formula");
		var result = stack.Pop();
		if (result is not TokenNumber tn)
			throw new Exception("Result token is not a number");
		return tn.Number;
	}

}

