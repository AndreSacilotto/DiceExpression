using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiceNotation;


public partial class DiceReader<T> where T : INumber<T>
{

	//[1]|d|[3]|{[4]}|
	[GeneratedRegex(@"(\d*)d(\d+)(?:{(.*?)})?", RegexOptions.CultureInvariant)]
	private static partial Regex DiceExpressionParser();

	public static Regex DiceParser { get; } = DiceExpressionParser();

	private const char OPEN_BRACKET = '{';
	private const char CLOSE_BRACKET = '}';
	private const char PARAM_SEPARATOR = ',';
	private const char DICE_OPERATOR = 'd';

	public static DiceRollResult<T>[] ParseDice(string diceExpression)
	{
		diceExpression = UtilString.RemoveWithspaceAndInsensive(diceExpression);

		var matches = DiceParser.Matches(diceExpression);

		var dices = new DiceRollResult<T>[matches.Count];

		for (int i = 0; i < matches.Count; i++)
		{
			var match = matches[i];
			//Group[0] is the Match, so $1..N is the captures 
			var captures = match.Groups.Values.Skip(1).ToArray();

			int times;
			T sides;
			string[] args;

			//Console.WriteLine((captures[0].ValueSpan == ReadOnlySpan<char>.Empty) + " + " + (captures[0].ValueSpan == null));
			//Console.WriteLine((captures[0].ValueSpan.IsEmpty) + " + " + (captures[0].ValueSpan.IsWhiteSpace()));
			//Console.WriteLine((captures[0].Value == string.Empty) + " + " + (captures[0].Value == null));

			times = captures[0].ValueSpan.IsEmpty ? 1 : int.Parse(captures[0].ValueSpan, CultureInfo.InvariantCulture);

			sides = T.Parse(captures[1].ValueSpan, CultureInfo.InvariantCulture);

			args = captures[2].ValueSpan.IsEmpty ? Array.Empty<string>() : captures[2].Value.Split(PARAM_SEPARATOR);

			dices[i] = ReadDice(match.ValueSpan, times, sides, args);
		}

		return dices;
	}


	public static DiceRollResult<T> ReadDice(ReadOnlySpan<char> expression, int times, T sides, params string[] args)
	{
		var dice = new DiceRollResult<T>(expression.ToString(), new DiceRoll<T>(times, sides));

		return dice;
	}

}
public record class DiceRollResult<T>(string Expression, DiceRoll<T> Dice) where T : INumber<T>;

//public record class DiceRollResult<TDice>(string Expression, DiceRoll<TDice> Dice, TDice[] Rolls, TDice Result) where TDice : INumber<TDice> 
//{ 

//}
