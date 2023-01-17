
using Helper;

namespace MathNotation;

public partial class MathExpression<T> : IExpression
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
		infix = new List<IToken>(infixTokens);
		expressionBuilder = InfixToExpression(infix);
	}

	public MathExpression(string expression)
	{
		expression = CleanExpression(expression);
		infix = new List<IToken>(ExpressionToInfix(expression.AsSpan()));
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
		var expSpan = CleanExpression(expression).AsSpan();
		infix.AddRange(ExpressionToInfix(expSpan));
		expressionBuilder.Append(expSpan);

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
