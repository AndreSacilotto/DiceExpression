namespace DiceExpression.ShuntingYard;

public enum Symbol : byte
{
	None = 0,

	/* Brackets */
	OpenBracket,
	CloseBracket,
	ArgsSeparator,

	/* Binary Operators */
	Addition,
	Subtraction,
	Multiplication,
	Division,
	Pow,
	Remainer,

	/* Unary Pre Operators */
	Negate,

	/* Unary Pos Operators */
	Factorial,

	/* Funcs */
	Floor,
	Ceil,
	Round,
	Sqtr,
	Abs,

	/* Dice */
	Dice, // 'dX' BinaryOperator : dice
	DiceRoll, // 'YdX' BinaryOperator : dice[]
	Explode, // explode -> Func(dice) : dice[]
	KeepHighest, // highest -> Func (dice, number to keep) : dice[]
	KeepLowest, // lowest -> Func(dice, number to keep) : dice[]
	KeepLowestHighest, // rh -> Func(dice, lowest to keep, highest to keep) : dice[]
}
