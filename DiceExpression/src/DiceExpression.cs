
using System.Globalization;
using DiceExpression.ShuntingYard;

using Sy = DiceExpression.ShuntingYard.DiceShuntingYard<double>;
using Rand = DiceExpression.IRandom<double, int>;

namespace DiceExpression;

public partial class DiceExpression
{
	private static readonly Regex whitespaceRegex = Whitespace();

	[GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
	private static partial Regex Whitespace();

	private IToken[] infix;
	private IToken[] postfix;

	private string internalExpression;

	public Rand Random { get; set; }

	public DiceExpression(string expression, Rand? rand = null)
	{
		Random = rand ?? new GenericRandom<Number>();
		internalExpression = CleanExpression(expression);
		infix = TokenizeExpression(internalExpression).ToArray();
		postfix = Sy.InfixToPostfix(infix);
	}

	public Number Evaluate() => Sy.EvaluatePostfixTokens(postfix);

	public override string ToString() => internalExpression;

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		expression = whitespaceRegex.Replace(expression, string.Empty);
		var sb = new StringBuilder(expression.ToLowerInvariant());
		return sb.Replace("+-", "-").Replace("-+", "-").Replace("--", "+").Replace("++", "+").ToString();
	}

	public static Queue<IToken> TokenizeExpression(string expression)
	{
		const char OPEN_BRACKET = '(';
		const char CLOSE_BRACKET = ')';
		const char PARAMETER_SEPARATOR = ',';
		const char DECIMAL_SEPARATOR = '.';
		static bool IsName(char ch) => char.IsLetter(ch) || ch == '_';
		static bool IsNumeric(char ch) => char.IsNumber(ch) || ch == DECIMAL_SEPARATOR;

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

				// Parse Number
				if (isFloat)
				{
					var value = Number.Parse(buffer.ToString(), CultureInfo.InvariantCulture);
					match.Enqueue(new TokenNumber<Number>(value));
				}
				else
				{
					var value = Integer.Parse(buffer.ToString(), CultureInfo.InvariantCulture);
					match.Enqueue(new TokenNumber<Integer>(value));
				}
				buffer.Clear();
			}
			// #Brackets
			else if (ch == OPEN_BRACKET)
			{
				match.Enqueue(Sy.Symbols[Symbol.OpenBracket]);
				bracketCount++;
			}
			else if (ch == CLOSE_BRACKET)
			{
				match.Enqueue(Sy.Symbols[Symbol.CloseBracket]);
				bracketCount--;
			}
			else if (ch == PARAMETER_SEPARATOR)
			{
				match.Enqueue(Sy.Symbols[Symbol.ArgsSeparator]);
				continue;
			}
			else
			{
				Symbol symbol;

				int r = i + 1;
				bool hasRight = r < expLength;
				char chR = hasRight ? expression[r] : '\0';

				int l = i - 1;
				bool hasLeft = l >= 0;
				char chL = hasLeft ? expression[l] : '\0';

				// #Unary-PreOperators
				if (hasRight && (i == 0 || (hasLeft && !(IsNumeric(chL) || chL == CLOSE_BRACKET))) && (IsNumeric(chR) || chR == OPEN_BRACKET))
				{
					if (ch == '+')
						continue;
					else if (ch == '-')
						symbol = Symbol.Negate;
					else if (ch == 'd')
						symbol = Symbol.Dice;
					else
						throw new Exception($"There is no Unary-PreOperator: '{ch}'");
				}
				// #Bi-Operators
				else if (hasRight && hasLeft)
				{
					if (ch == '+')
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
						symbol = Symbol.DiceRoll;
					else
						throw new Exception($"There is no Binary-Operator: '{ch}'");
				}
				// #Unary-PosOperator
				else if (hasLeft && IsNumeric(chL) && (!hasRight || (hasRight && (!IsNumeric(chR) && chR != OPEN_BRACKET))))
				{
					if (ch == '!')
						symbol = Symbol.Factorial;
					else
						throw new Exception($"There is no Unary-PreOperator: '{ch}'");
				}
				// #Funcs
				else if (IsName(ch) && hasRight && hasLeft && !IsNumeric(chL))
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

						"explode" => Symbol.Explode,
						"highest" => Symbol.KeepHighest,
						"lowest" => Symbol.KeepLowest,
						"rh" => Symbol.KeepLowestHighest,
						_ => throw new Exception($"The funtion \"{funcStr}\" dont exist")
					};

					buffer.Clear();
				}
				else
					throw new Exception($"Invalid character {ch}");

				match.Enqueue(Sy.Symbols[symbol]);
			}

		}

		if (bracketCount != 0)
			throw new Exception("Not all brackets have been closed");

		return match;
	}



}
