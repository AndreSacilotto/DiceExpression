using Helper;

namespace DiceNotation;


public partial class DiceExpression<T> where T : unmanaged, INumber<T>, IBinaryInteger<T>
{
	public record class DiceRollResult(T[] Rolls, T Result);

	private IRandomNumber<T> rng;

	//private IToken[] infix;
	//private IToken[] postfix;

	private string internalExpressions;

	public string Expressions => internalExpressions;

	public DiceExpression(IRandomNumber<T> random, string expression)
	{
		rng = random;
		internalExpressions = CleanExpression(expression);
	}

	//	public Number Evaluate() => Sy.EvaluatePostfix(postfix);

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

