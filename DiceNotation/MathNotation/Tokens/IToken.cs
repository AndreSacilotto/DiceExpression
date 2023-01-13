﻿namespace MathNotation;

public interface IToken
{
	Category Category { get; }
	string Name { get; }
}

public interface ITokenOperator : IToken
{ 
	
}

public interface ITokenPrecedence : IToken
{
	/// <summary> Priority of different operators </summary>
	int Precedence { get; init; }

	/// <summary> 
	/// Used to distinct the precedence/resolution of operators of same symbol. <br/>
	/// <see langword="False"/> means left-associativity [left to right] (+ - / *), <br/>
	/// <see langword="True"/> means right-associativity [right to left] (^ EE √).
	/// </summary>
	bool RightAssociativity { get; init; }
}