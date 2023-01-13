
namespace MathNotation;

public partial class MathExpression<T>
{
	private readonly List<IToken> infix = new();
	private Queue<IToken> postfix = null!;

	private readonly StringBuilder expressionBuilder;

	private bool dirty = true;

	#region Props
	public IReadOnlyCollection<IToken> Infix => infix;
	public IReadOnlyCollection<IToken> Postfix
	{
		get {
			GeneratePosfix();
			return postfix;
		}
	}
	public string Expression => expressionBuilder.ToString();

	#endregion

	#region Ctors

	public MathExpression(IEnumerable<IToken> infixTokens)
	{
		infix.AddRange(infixTokens);
		expressionBuilder = InfixToExpression(infix);
	}

	public MathExpression(string expression)
	{
		expression = CleanExpression(expression);
		infix.AddRange(ExpressionToInfix(expression));
		expressionBuilder = new(expression);
	}

	#endregion

	#region Evaluate

	private void GeneratePosfix()
	{
		if (!dirty)
			return;

		postfix = InfixToPostfix(infix);
		postfix.TrimExcess();

		dirty = false;
	}

	public T Evaluate()
	{
		GeneratePosfix();
		return PosfixEvaluation(postfix);
	}

	#endregion

	#region AddExpressions

	public void AddExpression(string expression)
	{
		expression = CleanExpression(expression);

		infix.AddRange(ExpressionToInfix(expression));
		expressionBuilder.Append(expression);

		dirty = true;
	}

	public void AddNumber(T number)
	{
		infix.Add(new TokenNumber<T>(number));
		dirty = true;
	}
	public bool AddOperation(char opr)
	{
		if (CharOperations.TryGetValue(opr, out var token))
		{
			infix.Add(token);
			expressionBuilder.Append(token.Name);
			dirty = true;
			return true;
		}
		return false;
	}
	public bool AddOperation(string opr)
	{
		if (CharOperations.TryGetValue(opr[0], out var token) || StringOperations.TryGetValue(opr, out token))
		{
			infix.Add(token);
			expressionBuilder.Append(token.Name);
			dirty = true;
			return true;
		}
		return false;
	}

	#endregion

}
