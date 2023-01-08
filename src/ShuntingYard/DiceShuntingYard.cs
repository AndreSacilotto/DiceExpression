
using System.Collections.Immutable;

namespace DiceExpression;

public static partial class DiceShuntingYard<T> where T : unmanaged, INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{
	public static ImmutableDictionary<Symbol, IToken> Symbols { get; } = CreateSymbolsDict();

	private static ImmutableDictionary<Symbol, IToken> CreateSymbolsDict()
	{
		Category category;
		var builder = ImmutableDictionary.CreateBuilder<Symbol, IToken>();

		builder.Add(Symbol.OpenBracket, new TokenBasic(Category.OpenBracket));
		builder.Add(Symbol.CloseBracket, new TokenBasic(Category.CloseBracket));

		category = Category.UnaryFunction;
		builder.Add(Symbol.Floor, new TokenUnary(category, T.Floor));
		builder.Add(Symbol.Ceil, new TokenUnary(category, T.Ceiling));
		builder.Add(Symbol.Round, new TokenUnary(category, T.Round));
		builder.Add(Symbol.Sqtr, new TokenUnary(category, T.Sqrt));

		category = Category.UnaryPreOperator;
		builder.Add(Symbol.Negate, new TokenUnary(category, (a) => -a));

		category = Category.UnaryPosOperator;

		category = Category.Operator;
		builder.Add(Symbol.Addition, new TokenBinary(category, (a, b) => a + b) { Precedence = 2 });
		builder.Add(Symbol.Subtraction, new TokenBinary(category, (a, b) => a - b) { Precedence = 2 });
		builder.Add(Symbol.Multiplication, new TokenBinary(category, (a, b) => a * b) { Precedence = 4 } );
		builder.Add(Symbol.Division, new TokenBinary(category, (a, b) => a / b) { Precedence = 4 } );
		builder.Add(Symbol.Remainer, new TokenBinary(category, (a, b) => a % b) { Precedence = 4 } );
		builder.Add(Symbol.Pow, new TokenBinary(category, T.Pow) { Precedence = 6, RightAssociativity = true });

		return builder.ToImmutable();
	}

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
				case Category.UnaryFunction:
				case Category.OpenBracket:
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
				case Category.CloseBracket:
				{
					while (stack.Peek().Category != Category.OpenBracket)
						queue.Enqueue(stack.Pop());

					//Remove Close Bracket 
					stack.Pop();

					if (stack.TryPeek(out var peek) && peek.Category == Category.UnaryFunction)
						queue.Enqueue(stack.Pop());
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
				case Category.UnaryFunction:
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
						throw new Exception($"The {token} is not valid operator");

					stack.Push(tk);
					break;
				}
				case Category.OpenBracket:
				case Category.CloseBracket:
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

