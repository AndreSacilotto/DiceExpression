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

	public static Queue<IToken> PostfixToInfix(IEnumerable<IToken> postfix)
	{
		Stack<IToken> stack = new(3);
		Queue<IToken> infix = new();


		foreach (var item in postfix)
		{

		}

		return infix;
	}

	public static T PosfixEvaluation(IEnumerable<IToken> postfix)
	{
		var stack = new Stack<TokenNumber<T>>(3);

		foreach (var token in postfix)
		{
			if (token is TokenNumber<T> tn)
				stack.Push(tn);
			else if (token is ITokenOperator)
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
				else if (token is TokenNth<T> tth)
				{
					var arr = new T[tth.ParamsCount];
					for (int j = tth.ParamsCount - 1; j >= 0; j--)
						arr[j] = PopNumber();
					value = tth.NthFunction(arr);
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
