global using System;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Numerics;
global using Number = System.Double;


using System.Linq;
using DiceNotation;
using MathNotation;
using System.Globalization;

using static System.Console;

namespace Main;

internal class Program
{
	static void Main()
	{
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

		//var exp = new DiceExpression<int>[]
		//{
		//	new("1d6"),
		//};

		var exp = new MathExpression<double>[]
		{
			new MathExpression<double>("20 + (-10 / 2.5)"),
		};


		WriteLine(MathExpression<double>.InfixToExpression(exp[0].Infix));

		var width = exp.Max(x => x.Expression.Length);

		foreach (var item in exp)
		{
			var equation = item.Expression.PadRight(width);
			WriteLine($"{equation} = {item.Evaluate()}");
		}
	}
}