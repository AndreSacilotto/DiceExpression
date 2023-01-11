
using System.Globalization;
using MathNotation.ShuntingYard;
using Helper;

namespace MathNotation;

public class MathExpression<T> where T : unmanaged, INumber<T>, IPowerFunctions<T>, IRootFunctions<T>, IFloatingPoint<T>
{
	private IToken[] infix;
	private IToken[] postfix;

	private string internalExpression;

	public IToken[] Infix => infix;
	public IToken[] Postfix => postfix;
	public string Expression => internalExpression;

	public MathExpression(string expression)
	{
		internalExpression = CleanExpression(expression);
		infix = TokenizeExpression(internalExpression).ToArray();
		postfix = ShuntingYard<T>.InfixToPostfix(infix);
	}

	public MathExpression(IToken[] infix)
	{
		this.infix = infix;
		internalExpression = InfixToExpression(infix);
		postfix = ShuntingYard<T>.InfixToPostfix(infix);
	}

	public T Evaluate() => ShuntingYard<T>.EvaluatePostfix(postfix);

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		var sb = new StringBuilder(UtilString.CleanEquation(expression));
		return sb.Replace("+-", "-").Replace("-+", "-").Replace("--", "+").Replace("++", "+").ToString();
	}

	private const char DECIMAL_SEPARATOR = '.';
	private const char NAME_SEPARATOR = '_';

	private const char OPEN_BRACKET = '(';
	private const char CLOSE_BRACKET = ')';
	private const char PARAMETER_SEPARATOR = ',';

	public static Queue<IToken> TokenizeExpression(string expression)
	{
		static bool IsName(char ch) => char.IsLetter(ch) || ch == NAME_SEPARATOR;
		static bool IsNumeric(char ch) => char.IsNumber(ch) || ch == DECIMAL_SEPARATOR;

		// Algoritm
		Queue<IToken> match = new(3);
		StringBuilder buffer = new(5);
		int bracketCount = 0;

		var expLength = expression.Length;
		for (int i = 0; i < expLength; i++)
		{
			char ch = expression[i];

			IToken tk;

			// #Number
			if (char.IsNumber(ch) || ch == DECIMAL_SEPARATOR)
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
					else if (!char.IsNumber(ch))
					{
						i--;
						break;
					}
					buffer.Append(ch);
				}

				// Parse Number
				var value = Number.Parse(buffer.ToString(), CultureInfo.InvariantCulture);
				tk = new TokenNumber<Number>(value);
				buffer.Clear();
			}
			// #Brackets
			else if (ch == OPEN_BRACKET)
			{
				tk = ShuntingYard<T>.SymbolsChar[ch];
				bracketCount++;
			}
			else if (ch == CLOSE_BRACKET)
			{
				tk = ShuntingYard<T>.SymbolsChar[ch];
				bracketCount--;
			}
			else if (ch == PARAMETER_SEPARATOR)
			{
				tk = ShuntingYard<T>.SymbolsChar[ch];
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
						ch = '~';
					tk = ShuntingYard<T>.SymbolsChar[ch];
				}
				// #Bi-operators
				else if (hasRight && hasLeft)
					tk = ShuntingYard<T>.SymbolsChar[ch];
				// #Unary-PosOperator
				else if (hasLeft && IsNumeric(chL) && (!hasRight || (hasRight && (!IsNumeric(chR) && chR != OPEN_BRACKET))))
					tk = ShuntingYard<T>.SymbolsChar[ch];
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
					tk = ShuntingYard<T>.SymbolsString[buffer.ToString()];
					buffer.Clear();
				}
				else
					throw new Exception($"Invalid character {ch}");
			}

			match.Enqueue(tk);
		}

		if (bracketCount != 0)
			throw new Exception("Not all brackets have been closed");

		return match;
	}

	public static string InfixToExpression(IToken[] infix)
	{
		var sb = new StringBuilder(infix.Length);
		foreach (IToken token in infix)
		{
			var len = sb.Length;
			if (token is TokenNumber<T> tn)
			{
				sb.Append(tn.Number);
				continue;
			}

			foreach (var item in ShuntingYard<T>.SymbolsChar)
				if (item.Value.Symbol == token.Symbol)
				{
					sb.Append(item.Key);
					break;
				}
			foreach (var item in ShuntingYard<T>.SymbolsString)
				if (item.Value.Symbol == token.Symbol)
				{
					sb.Append(item.Key);
					break;
				}

			if (len == sb.Length)
				throw new Exception($"There is no char {token.Symbol}");
		}

		return sb.ToString();
	}

}
