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

		var parser = DiceReader<int>.ParseDice("2d20{H3}+2D6+d6{L<2}");

		var exp = new MathExpression<double>[]
		{
			new MathExpression<double>("5+5"),
			new MathExpression<double>("-(10 / 2.5)"),
			new MathExpression<double>("20 + (-10 / 2.5)"),
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