namespace MathNotation.ShuntingYard;

public static partial class ShuntingYard<T>
{
	//https://en.wikipedia.org/wiki/Shunting_yard_algorithm -> explanation
	//https://www.andr.mu/logs/the-shunting-yard-algorithm/ -> UnaryOperators
	//https://stackoverflow.com/a/16392115 -> composite funcs
	public static void InfixToPostfix(Queue<IToken> queue, IEnumerable<IToken> infixTokens)
	{
		Stack<IToken> stack = new(3);

		foreach (var token in infixTokens)
		{
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
					else if (token.Symbol == Symbol.ParamsSeparator)
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

		while (stack.TryPop(out var peek))
			queue.Enqueue(peek);
	}

	public static T EvaluatePostfix(IEnumerable<IToken> postfixTokens)
	{
		var stack = new Stack<IToken>(3);

		foreach (var token in postfixTokens)
		{
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
					T PopNumber() => ((TokenNumber<T>)stack.Pop()).Number;

					T value;
					if (token is TokenUnary<T> tu)
					{
						var a = PopNumber();
						value = tu.UnaryFunction(a);
					}
					else if (token is TokenBinary<T> tb)
					{
						var b = PopNumber();
						var a = PopNumber();
						value = tb.BinaryFunction(a, b);
					}
					else if (token is TokenTernary<T> tt)
					{
						var c = PopNumber();
						var b = PopNumber();
						var a = PopNumber();
						value = tt.TernaryFunction(a, b, c);
					}
					else if (token is TokenNth<T> tth)
					{
						var arr = new T[tth.ParamsCount];
						for (int j = 0; j < tth.ParamsCount; j++)
							arr[j] = PopNumber();
						value = tth.NthFunction(arr);
					}
					else
						throw new Exception($"The {token} is not a valid operator");

					stack.Push(new TokenNumber<T>(value));
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
			throw new Exception("Result token is not a number");
		return tn.Number;
	}

}
