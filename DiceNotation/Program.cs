using System.Globalization;
using System.Linq;

using DiceNotation;
using MathNotation;

namespace Main;

internal class Program
{
	static void Main()
	{
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

		var rng = new RandomInt();

		//var exp = new DiceExpression<int>[]
		//{
		//	new(rng, "1d6"),
		//	new(rng, "1d6"),
		//};

		//var parser = DiceReader<int>.ParseDice("2d20{H3, C(<2,>2,=2,)}+2D6+d6{L<2}+2-(10/6-4)*3%floor(e * r)", out var no);
		var parser = DiceReader<int>.ParseDice("2d20+d6", out var no);

		var e = new StringBuilder[parser.Length];
		var r = new object[parser.Length];

		for (int i = 0; i < parser.Length; i++)
		{
			var item = parser[i];
			WriteLine(item);

			e[i] = DiceReader<int>.CalculateDiceRollExpression(item, rng);
			r[i] = DiceReader<int>.CalculateDiceRollResult(item, rng);

			WriteLine(e[i] + " | " + r[i]);
		}

		var w1 = string.Format(no.ToString(), e);
		var w2 = string.Format(no.ToString(), r);

		var exp = new MathExpression<double>[]
		{
			//new MathExpression<double>("5+5"),
			new MathExpression<double>(w1),
			//new MathExpression<double>(w2),
		};

		//exp[0].AddNumber(10.0);
		//exp[0].AddExpression("+10.0");

		var width = exp.Max(x => x.Expression.Length);

		foreach (var item in exp)
		{
			var equation = item.Expression.PadRight(width);
			Console.WriteLine($"{equation} = {item.Evaluate()}");
		}

	}
}