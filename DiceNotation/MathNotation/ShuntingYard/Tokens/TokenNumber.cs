namespace MathNotation.ShuntingYard;

public class TokenNumber<T> : IToken where T : unmanaged, INumber<T>
{
    public Category Category => Category.Number;
    public Symbol Symbol => Symbol.None;

    public TokenNumber(T number) => Number = number;
    public T Number { get; }

    public override string ToString() => Number.ToString() + "";
}
