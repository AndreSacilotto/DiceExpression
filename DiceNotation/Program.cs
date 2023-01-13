global using System;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Numerics;
global using System.Text;
global using System.Text.RegularExpressions;
global using Number = System.Double;
using System.Globalization;
using System.Linq;
using DiceNotation;
using MathNotation;
using static System.Console;

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

		var exp = new MathExpression<double>[]
		{
			new MathExpression<double>("5+5"),
			//new MathExpression<double>("-(10 / 2.5)"),
			//new MathExpression<double>("20 + (-10 / 2.5)"),
		};

		//exp[0].AddNumber(10.0);
		//exp[0].AddExpression("+10.0");

		var width = exp.Max(x => x.Expression.Length);

		foreach (var item in exp)
		{
			var equation = item.Expression.PadRight(width);
			WriteLine($"{equation} = {item.Evaluate()}");
		}
	}
}