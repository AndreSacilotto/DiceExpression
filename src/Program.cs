global using System;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Numerics;

using System.Linq;

using static System.Console;

namespace DiceExpression;

internal class Program
{
	static void Main()
	{
		var dices = new DiceExpression[]
		{
			new DiceExpression("(11.25)"),
			new DiceExpression("(9 + 1) * 2"),
			new DiceExpression("(2.5 ^ 2.5)"),
			new DiceExpression("ceil(2.5 ^ 2.5)"),
		};

		var width = dices.Max(x => x.ToString().Length);

		foreach (var item in dices)
		{
			var equation = item.ToString().PadRight(width);
			WriteLine($"{equation} = {item.Evaluate()}");
		}

	}
}
