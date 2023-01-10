global using System;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Numerics;

global using Number = System.Double;
global using Integer = System.Int32;

using System.Linq;

using static System.Console;

namespace DiceExpression;

internal class Program
{
	private static void Main()
	{
		var dices = new DiceExpression[]
		{
			//new("1d4"),
			//new("d6"),
			//new("explode(d6)"),
			//new("(11.25)"),
			//new("(9 + 1) * 2"),
			//new("(2.5 ^ 2.5)"),
			//new("ceil(22.5)"),
			//new("2d6"),
			//new("-20/-10"),
			new("(5)!"),
		};

		var width = dices.Max(x => x.ToString().Length);

		foreach (var item in dices)
		{
			var equation = item.ToString().PadRight(width);
			WriteLine($"{equation} = {item.Evaluate()}");
		}

	}
}
