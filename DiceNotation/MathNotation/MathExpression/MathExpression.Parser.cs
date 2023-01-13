
using System.Globalization;
using Helper;

namespace MathNotation;

public partial class MathExpression<T> where T : unmanaged, INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{

	#region Static Vars

	private const char DECIMAL_SEPARATOR = '.';
	private const char NAME_SEPARATOR = '_';
	private const char NEGATE_SYMBOL = 'n';

	private const char OPEN_BRACKET = '(';
	private const char CLOSE_BRACKET = ')';
	private const char PARAMETER_SEPARATOR = ',';

	private static readonly ImmutableDictionary<char, IToken> CharOperations;
	private static readonly ImmutableDictionary<string, IToken> StringOperations;

	#endregion

	static MathExpression()
	{
		var c = new Dictionary<char, IToken>() {
			[OPEN_BRACKET] = new TokenBasic(Category.OpenBracket),
			[CLOSE_BRACKET] = new TokenBasic(Category.CloseBracket),
			[PARAMETER_SEPARATOR] = new TokenBasic(Category.ParamSeparator),

			[NEGATE_SYMBOL] = new TokenUnary<T>((a) => -a) { Category = Category.PreOperator, Name = "-" },

			['!'] = new TokenUnary<T>(UtilMath<T>.Factorial) { Category = Category.PostOperator },

			['+'] = new TokenBinaryOperator<T>((a, b) => a + b) { Precedence = 2 },
			['-'] = new TokenBinaryOperator<T>((a, b) => a - b) { Precedence = 2 },
			['*'] = new TokenBinaryOperator<T>((a, b) => a * b) { Precedence = 4 },
			['/'] = new TokenBinaryOperator<T>((a, b) => a / b) { Precedence = 4 },
			['%'] = new TokenBinaryOperator<T>((a, b) => a % b) { Precedence = 4 },
			['^'] = new TokenBinaryOperator<T>(T.Pow) { Precedence = 6, RightAssociativity = true },
		};

		var s = new Dictionary<string, IToken>() {
			["floor"] = new TokenUnary<T>(T.Floor) { Category = Category.Function },
			["ceil"] = new TokenUnary<T>(T.Ceiling) { Category = Category.Function },
			["round"] = new TokenUnary<T>(T.Round){ Category = Category.Function },
			["sqtr"] = new TokenUnary<T>(T.Sqrt) { Category = Category.Function },
			["abs"] = new TokenUnary<T>(T.Abs) { Category = Category.Function },
			["min"] = new TokenBinary<T>(T.Min) { Category = Category.Function },
			["max"] = new TokenBinary<T>(T.Max) { Category = Category.Function },
			["clamp"] = new TokenTernary<T>(T.Clamp) { Category = Category.Function },
		};

		var cb = ImmutableDictionary.CreateBuilder<char, IToken>();
		foreach (var item in c)
		{
			var tb = (TokenBasic)item.Value;
			if (tb.Name == string.Empty)
				tb.Name = item.Key.ToString();
			cb.Add(item.Key, item.Value);
		}
		CharOperations = cb.ToImmutable();

		var sb = ImmutableDictionary.CreateBuilder<string, IToken>();
		foreach (var item in s)
		{
			var tb = (TokenBasic)item.Value;
			if (tb.Name == string.Empty)
				tb.Name = item.Key.ToString();
			sb.Add(item.Key, item.Value);
		}
		StringOperations = sb.ToImmutable();

	}

	#region Static Methods
	
	private static bool IsName(char ch) => char.IsLetter(ch) || ch == NAME_SEPARATOR;
	private static bool IsNumeric(char ch) => char.IsDigit(ch) || ch == DECIMAL_SEPARATOR;
	//public static bool IsOperation(string str) => CharOperations.ContainsKey(str[0]) || StringOperations.ContainsKey(str);
	
	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		var sb = new StringBuilder(UtilString.CleanEquation(expression));
		return sb.Replace("+-", "-").Replace("-+", "-").Replace("--", "+").Replace("++", "+").ToString();
	}
	#endregion


	public static StringBuilder InfixToExpression(IEnumerable<IToken> infix) 
	{
		var sb = new StringBuilder();
		foreach (var token in infix)
			sb.Append(token.Name);
		sb.TrimExcess();
		return sb;
	}

	public static Queue<IToken> ExpressionToInfix(string expression)
	{
		Queue<IToken> infixTokens = new(3);

		var buffer = new StringBuilder(3);
		int bracketCount = 0;

		var expLength = expression.Length;
		for (int i = 0; i < expLength; i++)
		{
			char ch = expression[i];

			// #Number
			if (char.IsDigit(ch) || ch == DECIMAL_SEPARATOR)
			{
				bool isFloat = ch == DECIMAL_SEPARATOR;
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
					else if (!char.IsDigit(ch))
					{
						i--;
						break;
					}
					buffer.Append(ch);
				}

				// Parse Number
				var value = Number.Parse(buffer.ToString(), CultureInfo.InvariantCulture);
				infixTokens.Enqueue(new TokenNumber<Number>(value));
				buffer.Clear();
			}
			// #Brackets
			else if (ch == OPEN_BRACKET)
			{
				infixTokens.Enqueue(CharOperations[ch]);
				bracketCount++;
			}
			else if (ch == CLOSE_BRACKET)
			{
				infixTokens.Enqueue(CharOperations[ch]);
				bracketCount--;
			}
			else if (ch == PARAMETER_SEPARATOR)
			{
				infixTokens.Enqueue(CharOperations[ch]);
			}
			else
			{
				int r = i + 1;
				bool hasRight = r < expLength;
				char chR = hasRight ? expression[r] : '\0';

				int l = i - 1;
				bool hasLeft = l >= 0;
				char chL = hasLeft ? expression[l] : '\0';

				// #Unary-preOperators
				if (hasRight && (i == 0 || (hasLeft && !(IsNumeric(chL) || chL == CLOSE_BRACKET))) && (IsNumeric(chR) || chR == OPEN_BRACKET))
				{
					if (ch == '+')
						continue;
					if (ch == '-')
						ch = 'n';
					infixTokens.Enqueue(CharOperations[ch]);
				}
				// #Bi-operators
				else if (hasRight && hasLeft)
				{
					infixTokens.Enqueue(CharOperations[ch]);
				}
				// #Unary-PosOperator
				else if (hasLeft && IsNumeric(chL) && (!hasRight || (hasRight && (!IsNumeric(chR) && chR != OPEN_BRACKET))))
					infixTokens.Enqueue(CharOperations[ch]);
				// #Funcs
				else if (IsName(ch) && hasRight)
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
					infixTokens.Enqueue(StringOperations[buffer.ToString()]);
					buffer.Clear();
				}
				else
					throw new Exception($"Invalid character {ch}");
			}

		}

		if (bracketCount != 0)
			throw new Exception("Not all brackets have been closed");

		return infixTokens;
	}


}
