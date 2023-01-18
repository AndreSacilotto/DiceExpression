
using System.Globalization;
using DiceNotation;
using Helper;

namespace MathNotation;

public partial class MathExpression<T> where T : unmanaged, INumber<T>, IFloatingPoint<T>, IPowerFunctions<T>, IRootFunctions<T>
{

	#region Static Vars

	public const char DECIMAL_SEPARATOR = '.';
	public const char NAME_SEPARATOR = '_';
	public const char NEGATE_SYMBOL = 'n';

	public const char OPEN_BRACKET = '(';
	public const char CLOSE_BRACKET = ')';
	public const char PARAMETER_SEPARATOR = ',';

	public static readonly Dictionary<char, IToken> Separators;
	public static readonly Dictionary<char, IToken> PosfixOperators;
	public static readonly Dictionary<char, IToken> PrefixOperators;
	public static readonly Dictionary<char, IToken> Operators;
	public static readonly Dictionary<string, IToken> Constants;
	public static readonly Dictionary<string, IToken> Functions;

	#endregion

	static MathExpression()
	{
		static void KeyToName<K, V>(Dictionary<K, V> dict) where K : notnull where V : IToken
		{
			foreach (var item in dict)
				if(item.Value is TokenBasic tb)
					tb.Name = item.Key.ToString() + "";
		}

		Separators = new() {
			[OPEN_BRACKET] = new TokenBasic(Category.OpenBracket),
			[CLOSE_BRACKET] = new TokenBasic(Category.CloseBracket),
			[PARAMETER_SEPARATOR] = new TokenBasic(Category.ParamSeparator),
		};
		KeyToName(Separators);

		PrefixOperators = new() {
			['+'] = new TokenUnary<T>((a) => a) { Category = Category.PreOperator },
			['-'] = new TokenUnary<T>((a) => -a) { Category = Category.PreOperator },
		};
		KeyToName(PrefixOperators);

		PosfixOperators = new() {
			['!'] = new TokenUnary<T>(UtilMath<T>.Factorial) { Category = Category.PostOperator },
		};
		KeyToName(PosfixOperators);

		Operators = new(){
			['+'] = new TokenBinaryOperator<T>((a, b) => a + b) { Precedence = 2 },
			['-'] = new TokenBinaryOperator<T>((a, b) => a - b) { Precedence = 2 },
			['*'] = new TokenBinaryOperator<T>((a, b) => a * b) { Precedence = 4 },
			['/'] = new TokenBinaryOperator<T>((a, b) => a / b) { Precedence = 4 },
			['%'] = new TokenBinaryOperator<T>((a, b) => a % b) { Precedence = 4 },
			['^'] = new TokenBinaryOperator<T>((a, b) => T.CreateSaturating(Math.Pow(double.CreateSaturating(a), double.CreateSaturating(b)))) { Precedence = 6, RightAssociativity = true },
		};
		KeyToName(Operators);

		Constants = new() {
			["e"] = new TokenConstant<T>("E", T.E),
			["π"] = new TokenConstant<T>("π", T.E),
			["pi"] = new TokenConstant<T>("PI", T.Pi),
			["tau"] = new TokenConstant<T>("TAU", T.Tau),
		};
		KeyToName(Constants);

		Functions = new() {
			["abs"] = new TokenUnary<T>(T.Abs) { Category = Category.Function },
			["min"] = new TokenBinary<T>(T.Min) { Category = Category.Function },
			["max"] = new TokenBinary<T>(T.Max) { Category = Category.Function },
			["clamp"] = new TokenTernary<T>(T.Clamp) { Category = Category.Function },
			["sign"] = new TokenUnary<T>((a) => T.CreateChecked(T.Sign(a))) { Category = Category.Function },

			["floor"] = new TokenUnary<T>(T.Floor) { Category = Category.Function },
			["ceil"] = new TokenUnary<T>(T.Ceiling) { Category = Category.Function },
			["round"] = new TokenUnary<T>(T.Round) { Category = Category.Function },
			["sqtr"] = new TokenUnary<T>(T.Sqrt) { Category = Category.Function },
			["root"] = new TokenBinary<T>((a, b) => T.RootN(a, int.CreateChecked(b))) { Category = Category.Function },
		};
		KeyToName(Functions);
	}

}
