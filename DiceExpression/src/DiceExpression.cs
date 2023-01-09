
using System.Globalization;

using Number = System.Double;

using static DiceExpression.DiceShuntingYard<double>;

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

	//Parse dice = "2d20" "10d6k2" "2d6x"

	public double Evaluate() => EvaluatePostfixTokens(postfix);
	public static Queue<IToken> TokenizeExpression(string expression)
	{
		// Util
		const char open = '(';
		const char close = ')';
		static bool IsNumberSeparator(char ch) => ch == '.' || ch == ',';
		static bool IsNumberF(char ch) => char.IsNumber(ch) || IsNumberSeparator(ch);
		static bool IsName(char ch) => char.IsLetter(ch) || ch == '_';

		// Algoritm
		Queue<IToken> match = new(3);
		StringBuilder buffer = new(5);

		var expLength = expression.Length;
		for (int i = 0; i < expLength; i++)
		{
			char ch = expression[i];

			var r = i + 1;
			bool hasRight = r < expLength;

			// #Number
			if (IsNumberF(ch))
			{
				bool isFloat = IsNumberSeparator(ch);
				buffer.Append(ch);
				//look-ahead for more decimals
				for (r = i + 1; r < expLength; r++, i++)
				{
					ch = expression[r];
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
				buffer.Clear();
				match.Enqueue(new TokenNumber(value));
			}
			// #Brackets
			else if (ch == open)
				match.Enqueue(Symbols[Symbol.OpenBracket]);
			else if (ch == close)
				match.Enqueue(Symbols[Symbol.CloseBracket]);

			else if (hasRight)
			{
				// #Bi-Operators
				if (ch == '+')
					match.Enqueue(Symbols[Symbol.Addition]);
				else if (ch == '-')
					match.Enqueue(Symbols[Symbol.Subtraction]);
				else if (ch == '*')
					match.Enqueue(Symbols[Symbol.Multiplication]);
				else if (ch == '/')
					match.Enqueue(Symbols[Symbol.Division]);
				else if (ch == '^')
					match.Enqueue(Symbols[Symbol.Pow]);
				else if (IsName(ch))
				{
					// #Unary-PreOperators
					if (ch == '&' && expression[r] == '&')
					{
						throw new NotImplementedException($"{ch} is not implemented");
					}
					// #Funcs
					else
					{
						buffer.Append(ch);
						//look-ahead for more text
						for (; r < expLength; r++, i++)
						{
							ch = expression[r];
							if (ch == open)
								break;
							buffer.Append(ch);
						}

						var str = buffer.ToString();
						buffer.Clear();

						// Try find Func
						if (str == "floor")
							match.Enqueue(Symbols[Symbol.Floor]);
						else if (str == "ceil")
							match.Enqueue(Symbols[Symbol.Ceil]);
						else if (str == "abs")
							match.Enqueue(Symbols[Symbol.Abs]);
						else
							throw new Exception($"The funtion \"{str}\" dont exist");
					}
				}
				else
					throw new Exception($"Expression is too short for the last opt");
			}
			// #Unary-PosOperator
			else if (char.IsLetter(ch))
			{
				throw new NotImplementedException($"There is no Unary Pos Operator: {ch}");
			}
			else
				throw new Exception($"Invalid character {ch}");
		}

		return match;
	}



}
