global using System;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Numerics;

namespace DiceExpression;

internal class Program
{
	static void Main()
	{
		var d = new DiceExpression("(9 + 1) ^ 2");
		Console.WriteLine(d.Evaluate());
	}
}
