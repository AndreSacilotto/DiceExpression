namespace Helper;

public partial class UtilString
{
	[GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
	private static partial Regex Whitespace();

	public static Regex WhitespaceRegex { get; } = Whitespace();


	public static string CleanEquation(string equation)
	{
		return WhitespaceRegex.Replace(equation, string.Empty).ToLower();
	}

}
