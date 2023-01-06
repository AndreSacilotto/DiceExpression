
using Number = System.Double;

using static DiceExpression.DiceShuntingYard<double>;

namespace DiceExpression;

public partial class DiceExpression
{
	private static readonly Regex clean = RegexClean();

	[GeneratedRegex(@"[^0-9\-\+*\/\(\)\.]", RegexOptions.CultureInvariant)]
	private static partial Regex RegexClean();

	private IToken[] infix = Array.Empty<IToken>();
	private IToken[] postfix = Array.Empty<IToken>();

	public DiceExpression(string expression)
	{
		//expression = clean.Replace(expression.Trim().ToLowerInvariant(), "");

		Queue<IToken> match = new(3);
		StringBuilder buffer = new(5);
		for (int i = 0; i < expression.Length; i++)
		{
			char ch = expression[i];
			if (char.IsWhiteSpace(ch))
				continue;

			if (char.IsNumber(ch) || ch.IsNumberSeparator())
			{
				bool isFloat = ch.IsNumberSeparator();
				buffer.Append(ch);
				for (int j = i + 1; j < expression.Length; i++, j++)
				{
					ch = expression[j];
					if (ch.IsNumberSeparator())
					{
						if (isFloat)
							throw new Exception("Invalid number format");
						isFloat = true;
					}

					if (char.IsNumber(ch))
						buffer.Append(ch);
					else
					{
						var value = MathExt<Number>.ConvertTo(buffer.ToString());
						match.Enqueue(new TokenNumber(value));
						buffer.Clear();
						break;
					}
				}
			}
			else if (ch == '(')
				match.Enqueue(Symbols[Symbol.OpenBracket]);
			else if (ch == ')')
				match.Enqueue(Symbols[Symbol.CloseBracket]);
			else if (ch == '+')
				match.Enqueue(Symbols[Symbol.Addition]);
			else if (ch == '-')
				match.Enqueue(Symbols[Symbol.Subtraction]);
			else if (ch == '*')
				match.Enqueue(Symbols[Symbol.Multiplication]);
			else if (ch == '/')
				match.Enqueue(Symbols[Symbol.Division]);
			else
				throw new Exception($"Invalid character {ch}");

			//ch = char.ToLowerInvariant(ch);
		}

		infix = match.ToArray();
		postfix = InfixToPostfix(infix);
	}

	public double Evaluate() => EvaluatePostfixTokens(postfix);


	public IEnumerable<IToken> GetInfix()
	{
		foreach (var item in infix)
			yield return item;
	}

	public IEnumerable<IToken> GetPostfix()
	{
		foreach (var item in infix)
			yield return item;
	}

}
