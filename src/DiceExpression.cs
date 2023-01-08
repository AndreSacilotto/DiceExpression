
using Number = System.Double;

using static DiceExpression.src.ShuntingYard.DiceShuntingYard<double>;

namespace DiceExpression;

public partial class DiceExpression
{
	private static readonly Regex whitespaceRegex = Whitespace();

	[GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
	private static partial Regex Whitespace();

	private IToken[] infix;
	private IToken[] postfix;

	public DiceExpression(string expression)
	{
		expression = CleanExpression(expression);
		infix = TokenizeExpression(expression).ToArray();
		postfix = InfixToPostfix(infix);
	}

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		expression = expression.Trim().ToLowerInvariant();
		expression = whitespaceRegex.Replace(expression, "");
		expression = expression.Replace("+-", "-").Replace("-+", "-").Replace("--", "+");
		return expression;
	}

	private static bool IsNumberSeparator(char ch) => ch == '.' || ch == ',';

	public double Evaluate() => EvaluatePostfixTokens(postfix);
	public static Queue<IToken> TokenizeExpression(string expression)
	{
		Queue<IToken> match = new(3);
		StringBuilder buffer = new(5);

		for (int i = 0; i < expression.Length; i++)
		{
			char ch = expression[i];
			if (char.IsWhiteSpace(ch))
				continue;

			if (char.IsNumber(ch) || IsNumberSeparator(ch))
			{
				bool isFloat = IsNumberSeparator(ch);
				buffer.Append(ch);
				for (int j = i + 1; j < expression.Length; i++, j++)
				{
					ch = expression[j];
					if (IsNumberSeparator(ch))
					{
						if (isFloat)
							throw new Exception("Invalid number format");
						isFloat = true;
					}
					else if (!char.IsNumber(ch))
						break;
					buffer.Append(ch);
				}

				var value = double.Parse(buffer.ToString());
				match.Enqueue(new TokenNumber(value));
				buffer.Clear();
			}
			else if (ch == '(')
				match.Enqueue(Symbols[Symbol.OpenBracket]);
			else if (ch == ')')
				match.Enqueue(Symbols[Symbol.CloseBracket]);
			else if (char.IsLetter(ch))
			{

			}
			else if (ch == '+')
			{
				match.Enqueue(Symbols[Symbol.Addition]);
			}
			else if (ch == '-')
			{
				match.Enqueue(Symbols[Symbol.Subtraction]);
			}
			else if (ch == '*')
				match.Enqueue(Symbols[Symbol.Multiplication]);
			else if (ch == '/')
				match.Enqueue(Symbols[Symbol.Division]);
			else if (ch == '^')
				match.Enqueue(Symbols[Symbol.Power]);
			else
				throw new Exception($"Invalid character {ch}");

			//ch = char.ToLowerInvariant(ch);
		}

		return match;
	}



}
