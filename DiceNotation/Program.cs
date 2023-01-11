global using System;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Numerics;

global using Number = System.Double;

using System.Linq;

using static System.Console;

namespace DiceNotation;

internal class Program
{
	static void Main()
	{
		var exp = new DiceExpression<int>[]
		{
			new("1d6"),
		};

		//var width = exp.Max(x => x.ToString().Length);

		//foreach (var item in exp)
		//{
		//	var equation = item.ToString().PadRight(width);
		//	WriteLine($"{equation} = {item.Evaluate()}");
		//}
	}
}