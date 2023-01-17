using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceNotation;

public class DiceFuncs<T> where T : IBinaryNumber<T>
{

	public static void KeepHighest(List<T> rolls, int amount)
	{
		while (rolls.Count > amount)
			rolls.RemoveAt(0);
	}

	public static void KeepLowest(List<T> rolls, int amount)
	{
		while (rolls.Count > amount)
			rolls.RemoveAt(rolls.Count - 1);
	}


}
