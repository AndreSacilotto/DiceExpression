namespace Helper;

public partial class UtilString
{
	[GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
	private static partial Regex Whitespace();
	public static Regex WhitespaceRegex { get; } = Whitespace();

	public static string RemoveWithspaceAndInsensive(string input) => 
		WhitespaceRegex.Replace(input, string.Empty).ToLower();

	#region Char&To&String
	public static bool IsDigit(string input) => IsDigit(input.AsSpan());
	public static bool IsDigit(ReadOnlySpan<char> input)
	{
		foreach (char c in input)
			if (!char.IsDigit(c))
				return false;
		return true;
	}
	public static bool IsNumber(string input) => IsNumber(input.AsSpan());
	public static bool IsNumber(ReadOnlySpan<char> input)
	{
		foreach (char c in input)
			if (!char.IsNumber(c))
				return false;
		return true;
	}
	public static bool IsLetter(string input) => IsLetter(input.AsSpan());
	public static bool IsLetter(ReadOnlySpan<char> input)
	{
		foreach (char c in input)
			if (!char.IsLetter(c))
				return false;
		return true;
	}
	#endregion


}
