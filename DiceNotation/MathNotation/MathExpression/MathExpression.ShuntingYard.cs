namespace MathNotation;

public partial class MathExpression<T>
{
	//Postfix is also know as RPN (Reverse polish notation)
	//Infix is also know as equation

	//https://en.wikipedia.org/wiki/Shunting_yard_algorithm -> explanation
	//https://www.andr.mu/logs/the-shunting-yard-algorithm/ -> UnaryOperators
	//https://stackoverflow.com/a/16392115 -> composite funcs
	public static Queue<IToken> InfixToPostfix(IEnumerable<IToken> infix)
	{
		Stack<IToken> stack = new(3);
		Queue<IToken> output = new();

		foreach (var token in infix)
		{
			switch (token.Category)
			{
				case Category.Number or Category.PostOperator:
				{
					output.Enqueue(token);
					break;
				}

				case Category.Operator:
				{
					var tb = (ITokenPrecedence)token;
					if (tb.RightAssociativity)
					{
						while (stack.TryPeek(out var peek) && peek is ITokenPrecedence t && t.Precedence > tb.Precedence)
							output.Enqueue(stack.Pop());
					}
					else
					{
						while (stack.TryPeek(out var peek) && peek is ITokenPrecedence t && t.Precedence >= tb.Precedence)
							output.Enqueue(stack.Pop());
					}
					stack.Push(token);
					break;
				}

				case Category.OpenBracket or Category.PreOperator or Category.Function:
				{
					stack.Push(token);
					break;
				}

				case Category.CloseBracket:
				{
					while (stack.Peek().Category != Category.OpenBracket)
						output.Enqueue(stack.Pop());

					//Remove the Open Bracket 
					stack.Pop();

					//If top of stack is a function, output it
					if (stack.TryPeek(out var peek) && peek.Category == Category.Function)
						output.Enqueue(stack.Pop());

					break;
				}
				case Category.ParamSeparator:
				{
					while (stack.Peek().Category != Category.OpenBracket)
						output.Enqueue(stack.Pop());
					break;
				}

				default:
				throw new Exception($"The token of {token.Category} category dont exist");
			}

		}

		while (stack.TryPop(out var peek))
			output.Enqueue(peek);

		return output;
	}

	public static Stack<IToken> PostfixToInfix(IEnumerable<IToken> postfix)
	{
		Stack<IToken> stack = new(3);

		var open = CharOperations[OPEN_BRACKET];
		var close = CharOperations[CLOSE_BRACKET];
		var argSpt = CharOperations[PARAMETER_SEPARATOR];

		foreach (var token in postfix)
		{
			if (token is TokenNumber<T>)
			{
				stack.Push(token);
			}
			else if (token is ITokenNAry)
			{
				stack.Push(open);

				if (token is TokenUnary<T> tu)
				{
					var a = stack.Pop();

					switch (token.Category)
					{
						case Category.PreOperator:
						stack.Push(token);
						stack.Push(a);
						break;
						case Category.Function:
						stack.Push(token);
						stack.Push(open);
						stack.Push(a);
						stack.Push(close);
						break;
						case Category.PostOperator:
						stack.Push(a);
						stack.Push(token);
						break;
						default:
						throw new Exception($"Invalid Token Category {token.Category}");
					}
				}
				else if (token is TokenBinary<T> tb)
				{
					var b = stack.Pop();
					var a = stack.Pop();

					switch (token.Category)
					{
						case Category.Function:
						stack.Push(token);
						stack.Push(open);
						stack.Push(a);
						stack.Push(argSpt);
						stack.Push(b);
						stack.Push(close);
						break;
						case Category.Operator:
						stack.Push(a);
						stack.Push(token);
						stack.Push(b);
						break;
						default:
						throw new Exception($"Invalid Token Category {token.Category}");
					}

				}
				//Onward it can only be funtion
				else if (token is TokenTernary<T> tt)
				{
					var c = stack.Pop();
					var b = stack.Pop();
					var a = stack.Pop();

					stack.Push(token);
					stack.Push(open);
					stack.Push(a);
					stack.Push(argSpt);
					stack.Push(b);
					stack.Push(argSpt);
					stack.Push(c);
					stack.Push(close);
				}
				else
					throw new Exception($"The {token} is not a valid operation");

				stack.Push(close);
			}

		}

		return stack;
	}

	public static T PosfixEvaluation(IEnumerable<IToken> postfix)
	{
		var stack = new Stack<TokenNumber<T>>(3);

		foreach (var token in postfix)
		{
			if (token is TokenNumber<T> tn)
				stack.Push(tn);
			else if (token is ITokenNAry)
			{
				T PopNumber() => stack.Pop().Number;

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
				else
					throw new Exception($"The {token} is not a valid operation");

				stack.Push(new TokenNumber<T>(value));
			}
			else
				throw new Exception($"The token {token} category dont exist");

		}

		if (stack.Count != 1)
			throw new Exception("Too many tokens on stack, invalid formula");

		return stack.Pop().Number;
	}

}
