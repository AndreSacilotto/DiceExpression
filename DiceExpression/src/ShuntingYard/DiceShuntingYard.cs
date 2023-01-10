
using System.Collections.Immutable;

namespace DiceExpression;

public static partial class DiceShuntingYard<T> where T : unmanaged, INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{
	private static IRandom<T, int> rng = new GenericRandom<T>(1000);

	public static ImmutableDictionary<Symbol, IToken> Symbols { get; } = CreateSymbolsDict();

	private static ImmutableDictionary<Symbol, IToken> CreateSymbolsDict()
	{
		var opt = new IToken[] {
			new TokenBasic(Symbol.OpenBracket, Category.Bracket),
			new TokenBasic(Symbol.CloseBracket, Category.Bracket),
			new TokenBasic(Symbol.FunctionSeparator, Category.Bracket),

			new TokenUnary(Symbol.Floor, Category.Function, T.Floor),
			new TokenUnary(Symbol.Ceil, Category.Function, T.Ceiling),
			new TokenUnary(Symbol.Round, Category.Function, T.Round),
			new TokenUnary(Symbol.Sqtr, Category.Function, T.Sqrt),
			new TokenUnary(Symbol.Abs, Category.Function, T.Abs),
			new TokenBinary(Symbol.Mult, Category.Function, (a, b) => a * b),

			new TokenUnary(Symbol.Negate, Category.UnaryPreOperator, (a) => -a),

			new TokenBinary(Symbol.Addition, Category.BinaryOperator, (a, b) => a + b) { Precedence = 2 },
			new TokenBinary(Symbol.Subtraction, Category.BinaryOperator, (a, b) => a - b) { Precedence = 2 },
			new TokenBinary(Symbol.Multiplication, Category.BinaryOperator, (a, b) => a * b) { Precedence = 4 } ,
			new TokenBinary(Symbol.Division, Category.BinaryOperator, (a, b) => a / b) { Precedence = 4 } ,
			new TokenBinary(Symbol.Remainer, Category.BinaryOperator, (a, b) => a % b) { Precedence = 4 } ,
			new TokenBinary(Symbol.Pow, Category.BinaryOperator, T.Pow) { Precedence = 6, RightAssociativity = true },

			new TokenBinary(Symbol.Dice, Category.BinaryOperator, (t, s) => new DiceRoll<T, int>(int.CreateChecked(t), s).Roll(rng) ) { Precedence = 10 },
		};

		var builder = ImmutableDictionary.CreateBuilder<Symbol, IToken>();
		foreach (var item in opt)
			builder.Add(item.Symbol, item);
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
				case Category.Function:
				{
					stack.Push(token);
					break;
				}
				case Category.BinaryOperator:
				{
					var tk = (TokenBinary)token;
					if (tk.RightAssociativity)
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.BinaryOperator && ((TokenBinary)peek).Precedence > tk.Precedence)
							queue.Enqueue(stack.Pop());
					}
					else
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.BinaryOperator && ((TokenBinary)peek).Precedence >= tk.Precedence)
							queue.Enqueue(stack.Pop());
					}
					stack.Push(token);
					break;
				}
				case Category.Bracket:
				{
					if (token.Symbol == Symbol.OpenBracket)
						stack.Push(token);
					else if (token.Symbol == Symbol.CloseBracket)
					{
						while (stack.Peek().Symbol != Symbol.OpenBracket)
							queue.Enqueue(stack.Pop());

						//Remove the Open Bracket 
						stack.Pop();

						//If top of stack is a function, output it
						if (stack.TryPeek(out var peek) && peek.Category == Category.Function)
							queue.Enqueue(stack.Pop());
					}
					else if (token.Symbol == Symbol.FunctionSeparator)
					{
						while (stack.Peek().Symbol != Symbol.OpenBracket)
							queue.Enqueue(stack.Pop());
					}
					else
						throw new Exception($"Invalid Bracket type {token.Category}");

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
				case Category.BinaryOperator:
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
			throw new Exception("Result token is not t number");
		return tn.Number;
	}

}
