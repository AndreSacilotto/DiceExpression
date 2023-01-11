
using System.Globalization;
using MathNotation.ShuntingYard;
using Helper;

namespace DiceNotation;


public partial class DiceExpression<T> where T : unmanaged, INumber<T>, IBinaryInteger<T>
{
	public record class DiceRollResult(T[] Rolls, T Result);

	//private IToken[] infix;
	//private IToken[] postfix;

	private string internalExpression;

	public string Expression => internalExpression;

	public DiceExpression(string expression, IRandomNumber<T> random)
	{
		internalExpression = CleanExpression(expression);
	}

	//	public Number Evaluate() => Sy.EvaluatePostfix(postfix);

	//	public override string ToString() => internalExpression;

	private static string CleanExpression(string expression)
	{
		if (string.IsNullOrWhiteSpace(expression))
			throw new Exception("Empty Expression");
		return UtilString.CleanEquation(expression);
	}


	public static void EvaluateDicesFromExpression(string expression)
	{
	}

}

