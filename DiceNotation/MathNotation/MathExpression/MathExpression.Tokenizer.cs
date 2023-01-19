
using System.Globalization;

namespace MathNotation;

public partial class MathExpression<T>
{

	#region Static Methods

	public static bool IsVarName(char ch) => char.IsLetter(ch) || ch == NAME_SEPARATOR;
	public static bool IsDigitF(char ch) => char.IsDigit(ch) || ch == DECIMAL_SEPARATOR;

	public static bool IsConstant(string ch) => Constants.ContainsKey(ch);

	//public static bool IsOperation(string str) => CharOperations.ContainsKey(str[0]) || StringOperations.ContainsKey(str);

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		var sb = new StringBuilder(UtilString.RemoveWithspaceAndInsensive(expression));
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

	public static List<IToken> ExpressionToInfix(string expression) => ExpressionToInfix(expression.AsSpan()); 
	public static List<IToken> ExpressionToInfix(ReadOnlySpan<char> expression)
	{
		List<IToken> infixTokens = new(3);

		var buffer = new StringBuilder(3);
		int bracketCount = 0;

		var expLength = expression.Length;
		IToken? priorToken = null;
		for (int i = 0; i < expLength; i++)
		{
			IToken? tk;
			char ch = expression[i];

			// #Number
			if (IsDigitF(ch))
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
				var value = T.Parse(buffer.ToStringAndClear(), CultureInfo.InvariantCulture);
				tk = new TokenNumber<T>(value);
				infixTokens.Add(tk);
			}
			// #Separators
			else if (ch == OPEN_BRACKET)
			{
				tk = Separators[ch];
				infixTokens.Add(tk);
				bracketCount++;
			}
			else if (ch == CLOSE_BRACKET)
			{
				tk = Separators[ch];
				infixTokens.Add(tk);
				bracketCount--;
			}
			else if (ch == PARAMETER_SEPARATOR)
			{
				tk = Separators[ch];
				infixTokens.Add(tk);
			}
			else
			{
				bool priorWasNumberic = priorToken != null && (priorToken.Category & (Category.Number | Category.CloseBracket | Category.Function | Category.PostOperator)) != 0;

				int r = i + 1;
				bool hasRight = r < expLength;

				bool nextIsNumber;
				bool nextIsNotNumber;
				if (hasRight)
				{
					var chR = expression[r];
					nextIsNumber = chR == OPEN_BRACKET || IsDigitF(chR) || IsVarName(chR) || PrefixOperators.ContainsKey(chR);
					nextIsNotNumber = chR == CLOSE_BRACKET || chR == PARAMETER_SEPARATOR || Operators.ContainsKey(chR) || PosfixOperators.ContainsKey(chR);
				}
				else
				{
					nextIsNumber = false;
					nextIsNotNumber = false;
				}

				// #Unary-Pre-Operators
				if ((priorToken == null || (priorToken.Category & (Category.OpenBracket | Category.ParamSeparator | Category.Operator | Category.PreOperator)) != 0)
					&& nextIsNumber && PrefixOperators.TryGetValue(ch, out tk))
				{
					if (ch == '+')
						continue;
					infixTokens.Add(tk);
				}
				// #Binary-Operators
				else if (priorWasNumberic && nextIsNumber && Operators.TryGetValue(ch, out tk))
				{
					infixTokens.Add(tk);
		}
				// #Unary-Pos-Operator
				else if (priorWasNumberic && (!hasRight || nextIsNotNumber) && PosfixOperators.TryGetValue(ch, out tk)) 
				{ 
					infixTokens.Add(tk);
				}
				// #Funcs
				else if (IsVarName(ch))
				{
					bool isFunction = false;
					buffer.Append(ch);
					//look-ahead for more text until find the bracket
					for (i++; i < expLength; i++)
					{
						ch = expression[i];
						if (Separators.ContainsKey(ch)|| Operators.ContainsKey(ch) /*|| PrefixOperators.ContainsKey(ch)*/) 
						{
							i--;
							isFunction = ch == OPEN_BRACKET;
							break;
						}
						buffer.Append(ch);
					}

					var str = buffer.ToStringAndClear();
					if (isFunction && Functions.TryGetValue(str, out tk)) { 
						infixTokens.Add(tk);
					}					
					else if (Constants.TryGetValue(str, out tk)) { 
						infixTokens.Add(tk);
					}
					else
						throw new Exception($"Function or Constant: '{str}' dont exist");
				}
				else
					throw new Exception($"Invalid equation or character: '{ch}'");
			}

			priorToken = tk;
		}

		if (bracketCount != 0)
			throw new Exception("Not all brackets have been closed");

		return infixTokens;
	}


}
