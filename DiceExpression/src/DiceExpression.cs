
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

	public double Evaluate() => EvaluatePostfixTokens(postfix);

	public override string ToString() => internalExpression;

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		expression = whitespaceRegex.Replace(expression, string.Empty);
		var sb = new StringBuilder(expression.ToLowerInvariant());
		return sb.Replace("+-", "-").Replace("-+", "-").Replace("--", "+").Replace("++", "+").ToString();
	}

	private const char OPEN_BRACKET = '(';
	private const char CLOSE_BRACKET = ')';
	private const char PARAMETER_SEPARATOR = ',';
	private const char DECIMAL_SEPARATOR = '.';
	private static bool IsName(char ch) => char.IsLetter(ch) || ch == '_';
	private static bool IsNumeric(char ch) => char.IsNumber(ch) || ch == DECIMAL_SEPARATOR;

	public static Queue<IToken> TokenizeExpression(string expression)
	{
		// Algoritm
		Queue<IToken> match = new(3);
		StringBuilder buffer = new(5);
		int bracketCount = 0;

		var expLength = expression.Length;
		for (int i = 0; i < expLength; i++)
		{
			char ch = expression[i];

			// #Number
			if (char.IsNumber(ch) || ch == DECIMAL_SEPARATOR)
			{
				bool isFloat = false;
				buffer.Append(ch);
				//loop-ahead for more decimals
				for (i++; i < expLength; i++)
				{
					ch = expression[i];
					if (ch == DECIMAL_SEPARATOR)
					{
						if (isFloat)
							throw new Exception("Invalid number format, too many decimal separators");
						isFloat = true;
					}
					else if (!char.IsNumber(ch))
					{
						i--;
						break;
					}
					buffer.Append(ch);
				}

				// Parse
				var value = double.Parse(buffer.ToString(), CultureInfo.InvariantCulture);
				buffer.Clear();

				match.Enqueue(new TokenNumber(value));
			}
			else
			{
				Symbol symbol;
				int r = i + 1;

				// #Brackets
				if (ch == OPEN_BRACKET)
				{
					symbol = Symbol.OpenBracket;
					bracketCount++;
				}
				else if (ch == CLOSE_BRACKET)
				{
					symbol = Symbol.CloseBracket;
					bracketCount--;
				}
				else if (r < expLength) // hasRight
				{
					int l = i - 1;
					bool hasLeft = l >= 0;
					char chR = expression[r];
					char chL = hasLeft ? expression[l] : '\0';

					// #Unary-PreOperators
					if ((IsNumeric(chR) || chR == OPEN_BRACKET) && (i == 0 || (hasLeft && !IsNumeric(chL) && chL != CLOSE_BRACKET)))
					{
						if (ch == '+')
							continue;
						else if (ch == '-')
							symbol = Symbol.Negate;
						else
							throw new Exception($"There is no Unary Pre Operator {ch}");
					}
					// #Bi-Operators
					else if (ch == '+')
						symbol = Symbol.Addition;
					else if (ch == '-')
						symbol = Symbol.Subtraction;
					else if (ch == '*')
						symbol = Symbol.Multiplication;
					else if (ch == '/')
						symbol = Symbol.Division;
					else if (ch == '%')
						symbol = Symbol.Remainer;
					else if (ch == '^')
						symbol = Symbol.Pow;
					else if (ch == 'd')
						symbol = Symbol.Dice;
					// #Bi-Operators with 2 letters
					else if (ch == '&' && expression[r] == '&' && (i + 2) < expLength)
					{
						//i += 2;
						throw new NotImplementedException($"{ch}{chR} is not implemented");
					}
					// #Unary-PosOperator
					else if (hasLeft && IsNumeric(chL) && !IsNumeric(chR))
					{
						throw new NotImplementedException($"There is no Unary Pos BinaryOperator '{ch}'");
					}
					// #Funcs
					else if (IsName(ch) && !IsNumeric(chL))
					{
						buffer.Append(ch);
						//look-ahead for more text until find the bracket
						for (i++; i < expLength; i++)
						{
							ch = expression[i];
							if (ch == OPEN_BRACKET)
							{
								i--;
								break;
							}
							buffer.Append(ch);
						}

						if (ch != OPEN_BRACKET)
							throw new Exception("Function has no open bracket");

						// Try find Func
						var funcStr = buffer.ToString();
						symbol = funcStr switch {
							"floor" => Symbol.Floor,
							"ceil" => Symbol.Ceil,
							"round" => Symbol.Round,
							"abs" => Symbol.Abs,
							"sqtr" => Symbol.Sqtr,
							_ => throw new Exception($"The funtion \"{funcStr}\" dont exist")
						};

						buffer.Clear();
					}
					else
						throw new Exception($"Expression is too short");
				}
				else
					throw new Exception($"Invalid character {ch}");

				match.Enqueue(Symbols[symbol]);
			}

		}

		if (bracketCount != 0)
			throw new Exception("Not all brackets have been closed");

		return match;
	}



}
