
namespace DiceExpression.ShuntingYard;

public static partial class DiceShuntingYard<T>
{
	//https://en.wikipedia.org/wiki/Shunting_yard_algorithm -> explanation
	//https://www.andr.mu/logs/the-shunting-yard-algorithm/ -> UnaryOperators
	//https://stackoverflow.com/a/16392115 -> composite funcs
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
					var tk = (ITokenPrecedence)token;
					if (tk.RightAssociativity)
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.BinaryOperator && ((ITokenPrecedence)peek).Precedence > tk.Precedence)
							queue.Enqueue(stack.Pop());
					}
					else
					{
						while (stack.TryPeek(out var peek) && peek.Category == Category.BinaryOperator && ((ITokenPrecedence)peek).Precedence >= tk.Precedence)
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
					else if (token.Symbol == Symbol.ArgsSeparator)
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
						tk = tu.UnaryFunction(stack.Pop());
					}
					else if (token is TokenBinary tb)
					{
						var b = stack.Pop();
						var a = stack.Pop();
						tk = tb.BinaryFunction(a, b);
					}
					else
						throw new Exception($"The {token} is not valid operator");

					stack.Push(tk);
					break;
				}
				case Category.Bracket:
				throw new Exception($"Postfix cant contain brackets. Found {token.Symbol}");
				default:
				throw new Exception($"The token of {token.Category} category dont exist");
			}

		}

		if (stack.Count != 1)
			throw new Exception("Too many tokens on stack, invalid formula");
		var result = stack.Pop();
		if (result is not TokenNumber<T> tn)
			throw new Exception("Result token is not t number");
		return tn.Number;
	}

}
