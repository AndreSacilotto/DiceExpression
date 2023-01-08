
using Number = System.Double;

using static DiceExpression.DiceShuntingYard<double>;
using System.Globalization;

namespace DiceExpression;

public partial class DiceExpression
{
	private static readonly Regex whitespaceRegex = Whitespace();

	[GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
	private static partial Regex Whitespace();

	private IToken[] infix;
	private IToken[] postfix;

	private string internalExpression;

	public DiceExpression(string expression)
	{
		internalExpression = CleanExpression(expression);
		infix = TokenizeExpression(internalExpression).ToArray();
		postfix = InfixToPostfix(infix);
	}

	public override string ToString() => internalExpression;

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		expression = expression.Trim().ToLowerInvariant();
		expression = whitespaceRegex.Replace(expression, "");
		expression = expression.Replace("+-", "-").Replace("-+", "-").Replace("--", "+").Replace("++", "+");
		return expression;
	}


	public double Evaluate() => EvaluatePostfixTokens(postfix);
	public static Queue<IToken> TokenizeExpression(string expression)
	{
		// Util
		const char open = '(';
		const char close = ')';
		static bool IsNumberSeparator(char ch) => ch == '.' || ch == ',';
		static bool IsName(char ch) => char.IsLetter(ch) || ch == '_';

		// Algoritm
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
				//look-ahead for more decimals
				for (int j = i + 1; j < expression.Length; j++, i++)
				{
					ch = expression[j];
					if (char.IsNumber(ch))
						buffer.Append(ch);
					else if (IsNumberSeparator(ch))
					{
						if (isFloat)
							throw new Exception("Invalid number format, too many number separators ('.' or ',')");
						isFloat = true;
						buffer.Append('.');
					}
					else
						break;
				}
				// Parse
				var value = double.Parse(buffer.ToString(), CultureInfo.InvariantCulture);
				match.Enqueue(new TokenNumber(value));
				buffer.Clear();
			}
			else if (ch == open)
				match.Enqueue(Symbols[Symbol.OpenBracket]);
			else if (ch == close)
				match.Enqueue(Symbols[Symbol.CloseBracket]);
			else if (ch == '*')
				match.Enqueue(Symbols[Symbol.Multiplication]);
			else if (ch == '/')
				match.Enqueue(Symbols[Symbol.Division]);
			else if (ch == '^')
				match.Enqueue(Symbols[Symbol.Pow]);
			else if (ch == '+')
			{
				//TODO: CHECK UNARY
				match.Enqueue(Symbols[Symbol.Addition]);
			}
			else if (ch == '-')
			{
				//TODO: CHECK UNARY
				match.Enqueue(Symbols[Symbol.Subtraction]);
			}
			else if (IsName(ch))
			{
				//TODO: LETTER OPERATORS

				buffer.Append(ch);
				//look-ahead
				for (int j = i + 1; j < expression.Length; j++, i++)
				{
					ch = expression[j];
					if (ch == open)
						break;
					buffer.Append(ch);
				}

				var str = buffer.ToString();
				buffer.Clear();

				//Find Func
				if (str == "floor")
					match.Enqueue(Symbols[Symbol.Floor]);
				else if (str == "ceil")
					match.Enqueue(Symbols[Symbol.Ceil]);
				else
					throw new Exception($"The funtion \"{str}\" dont exist");
			}
			else
				throw new Exception($"Invalid character {ch}");

		}

		return match;
	}



}
