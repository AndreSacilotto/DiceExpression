namespace MathExpression.ShuntingYard;

public class TokenBasic : IToken
{
    public Category Category { get; }
    public Symbol Symbol { get; }

    public TokenBasic(Symbol symbol, Category category)
    {
        Category = category;
        Symbol = symbol;
    }

    public override string ToString() => Symbol.ToString();
}
