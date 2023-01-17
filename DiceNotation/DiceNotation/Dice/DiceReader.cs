using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiceNotation;


public partial class DiceReader<T> where T : IBinaryInteger<T>
{
	private const char OPEN_BRACKET = '{';
	private const char CLOSE_BRACKET = '}';
	private const char PARAM_SEPARATOR = ',';
	private const char DICE_OPERATOR = 'd';

	//[1]d[2]{[3]}
	[GeneratedRegex(@"(\d*)d(\d+)(?:{(.*?)})?", RegexOptions.CultureInvariant)]
	private static partial Regex DiceExpressionParser();

	public static readonly Regex DiceParser = DiceExpressionParser();

	public record class DiceRollExpression(string Expression, DiceRoll<T> Dice, string[] Args);

	public static DiceRollExpression[] ParseDice(string expression, out StringBuilder noDiceExpression)
	{
		expression = UtilString.RemoveWithspaceAndInsensive(expression);

		var matches = DiceParser.Matches(expression);

		noDiceExpression = UtilString.RegexRemoveGroupsForFormat(new(expression), matches);

		var dices = new DiceRollExpression[matches.Count];

		for (int i = 0; i < matches.Count; i++)
		{
			var match = matches[i];
			//Group[0] is the Match, so $1..N is the captures 
			var groups = match.Groups;

			var times = groups[1].ValueSpan.IsEmpty ? 1 : int.Parse(groups[1].ValueSpan, CultureInfo.InvariantCulture);

			var sides = T.Parse(groups[2].ValueSpan, CultureInfo.InvariantCulture);

			var args = groups[3].ValueSpan.IsEmpty ? Array.Empty<string>() : groups[3].Value.Split(PARAM_SEPARATOR);

			dices[i] = new DiceRollExpression(match.Value, new DiceRoll<T>(times, sides), args);
		}

		return dices;
	}

	public static T CalculateDiceRollResult(DiceRollExpression diceExpression, IRandomNumber<T> random)
	{
		return diceExpression.Dice.RollAndSum(random);
	}


	public static StringBuilder CalculateDiceRollExpression(DiceRollExpression diceExpression, IRandomNumber<T> random, string separator = " + ")
	{
		var rolls = diceExpression.Dice.Roll(random);
		var sb = new StringBuilder(rolls.Length);
		sb.Append('(');
		for (int i = 0; i < rolls.Length-1; i++)
			sb.Append(rolls[i].ToString() + separator);
		sb.Append(rolls[rolls.Length-1].ToString() + ')');
		return sb;
	}

	//public static DiceRollExpression<T> CalculateDiceRollExpression(DiceRollExpression<T> diceExpression)
	//{
	//	var diceRoll = new DiceRoll<T>(times, sides);
	//	var dre = new DiceRollExpression<T>(expression.ToString(), diceRoll);

	//	var rolled = diceRoll.Roll(rng);

	//	var rolls = new List<T>(diceRoll.Roll(rng));
	//	rolls.Sort(); // ascending order (small -> big)
	//	foreach (var item in args)
	//		GenerateFunc(item, rolls, rng);

	//	return dre;
	//}




}


//public record struct DiceRollRolls<T>(DiceRollExpression<T> Dice, T[] Rolls, T Result) where T : INumber<T>
//{

//}
