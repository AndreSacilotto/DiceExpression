global using System;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Numerics;

global using Number = System.Double;

using System.Linq;

using static System.Console;

namespace MathExpression;

internal class Program
{
	private static void Main()
	{
		var exp = new MathExpression[]
		{
			//new("(11.25)"),
			//new("(9 + 1) * 2"),
			//new("(2.5 ^ 2.5)"),
			//new("ceil(22.5)"),
			//new("ceil((3+.5))"),
			//new("-20/-10"),
			//new("(5)!"),
			new("clamp(100, 2., 35)"),
		};

		var width = exp.Max(x => x.ToString().Length);

		foreach (var item in exp)
		{
			var equation = item.ToString().PadRight(width);
			WriteLine($"{equation} = {item.Evaluate()}");
		}

	}
}
