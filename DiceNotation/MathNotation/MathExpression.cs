
using System.Globalization;
using MathNotation.ShuntingYard;
using Helper;

using Sy = MathNotation.ShuntingYard.ShuntingYard<double>;

namespace MathNotation;

public class MathExpression<T> where T : unmanaged, INumber<T>
{
	private IToken[] infix;
	private IToken[] postfix;

	private string internalExpression;

	public MathExpression(string expression)
	{
		internalExpression = CleanExpression(expression);
		infix = TokenizeExpression(internalExpression).ToArray();
		postfix = Sy.InfixToPostfix(infix);
	}

	public MathExpression(IToken[] infix)
	{
		this.infix = infix;
		internalExpression = InfixToExpression(infix);
		postfix = Sy.InfixToPostfix(infix);
	}

	public Number Evaluate() => Sy.EvaluatePostfix(postfix);

	public override string ToString() => internalExpression;

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");

		var sb = new StringBuilder(UtilString.CleanEquation(expression));
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
				match.Enqueue(new TokenNumber<Number>(value));
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

				// #Unary-preOperators
				if (hasRight && (i == 0 || (hasLeft && !(IsNumeric(chL) || chL == CLOSE_BRACKET))) && (IsNumeric(chR) || chR == OPEN_BRACKET))
				{
					var result = Array.Find(preOperators, x => x.Ch == ch);
					if (result == null)
						throw new Exception($"There is no Unary-PreOperator: '{ch}'");
					symbol = result.Symbol;
				}
				// #Bi-operators
				else if (hasRight && hasLeft)
				{
					var result = Array.Find(operators, x => x.Ch == ch);
					if (result == null)
						throw new Exception($"There is no Binary-Operator: '{ch}'");
					symbol = result.Symbol;
				}
				// #Unary-PosOperator
				else if (hasLeft && IsNumeric(chL) && (!hasRight || (hasRight && (!IsNumeric(chR) && chR != OPEN_BRACKET))))
				{
					var result = Array.Find(posOperators, x => x.Ch == ch);
					if (result == null)
						throw new Exception($"There is no Unary-PreOperator: '{ch}'");
					symbol = result.Symbol;
				}
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
					var funcStr = buffer.ToString();

					var result = Array.Find(functions, x => x.Text == funcStr);
					if (result == null)
						throw new Exception($"The funtion \"{funcStr}\" dont exist");
					
					symbol = result.Symbol;

					buffer.Clear();
				}
				else
					throw new Exception($"Invalid character {ch}");

				if (symbol != Symbol.None)
					match.Enqueue(Sy.Symbols[symbol]);
			}

		}

		if (bracketCount != 0)
			throw new Exception("Not all brackets have been closed");

		return match;
	}

	private static string InfixToExpression(IToken[] infix)
	{
		var sb = new StringBuilder(infix.Length);
		foreach (var token in infix)
		{
			int len = sb.Length;
			bool FindSymbolPredicate(ITokenSymbol ts) => ts.Symbol == token.Symbol;

			var sc = Array.Find(preOperators, FindSymbolPredicate);
			if (sc != null)
				sb.Append(sc.Ch);

			sc = Array.Find(operators, FindSymbolPredicate);
			if (sc != null)
				sb.Append(sc.Ch);

			sc = Array.Find(posOperators, FindSymbolPredicate);
			if (sc != null)
				sb.Append(sc.Ch);

			var ss = Array.Find(functions, FindSymbolPredicate);
			if (ss != null)
				sb.Append(ss.Text);

			if(len == sb.Length)
				throw new Exception($"There is no char {token.Symbol}");
		}

		return sb.ToString();
	}

	#region Operators and Funcs
	private record class SymbolChar(char Ch, Symbol Symbol) : ITokenSymbol;
	private record class SymbolString(string Text, Symbol Symbol) : ITokenSymbol;

	//Dictonarys are not faster than arrays in small collections (<20)

	private static SymbolChar[] posOperators = new[]{
		new SymbolChar('!', Symbol.Factorial),
	};
	private static SymbolChar[] preOperators = new[] {
		new SymbolChar('+', Symbol.None),
		new SymbolChar('-', Symbol.Negate),
	};
	private static SymbolChar[] operators = new[] {
		new SymbolChar('+', Symbol.Addition),
		new SymbolChar('-', Symbol.Subtraction),
		new SymbolChar('*', Symbol.Multiplication),
		new SymbolChar('/', Symbol.Division),
		new SymbolChar('%', Symbol.Remainer),
		new SymbolChar('^', Symbol.Pow),
	};
	private static SymbolString[] functions = new[] {
		new SymbolString("floor", Symbol.Floor),
		new SymbolString("ceil", Symbol.Ceil),
		new SymbolString("round", Symbol.Round),
		new SymbolString("abs", Symbol.Abs),
		new SymbolString("sqtr", Symbol.Sqtr),
		new SymbolString("min", Symbol.Min),
		new SymbolString("max", Symbol.Max),
		new SymbolString("clamp", Symbol.Clamp),
	};
	#endregion


}
