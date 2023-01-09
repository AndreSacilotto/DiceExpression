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
	private static void Main()
	{
		var dices = new DiceExpression[]
		{
			new("(11.25)"),
			new("(9 + 1) * 2"),
			new("(2.5 ^ 2.5)"),
			new("ceil(2.5 ^ 2.5)"),
			new("floor(3.5)"),
			new("ceil((3+.5))"),
			new("2d6"),
			new("-10"),
			new("(-2)-3"),
			new("-(-4)"),
			new("-(+(+6))"),
			new("8-5+(-(3))"),
			new("-30/-3"),
			new("(-50)/-(-25)"),
		};

		var width = dices.Max(x => x.ToString().Length);

		foreach (var item in dices)
		{
			var equation = item.ToString().PadRight(width);
			WriteLine($"{equation} = {item.Evaluate()}");
		}

	}
}
