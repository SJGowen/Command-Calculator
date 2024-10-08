using System.Linq;

namespace Utilities;

public static class StringUtils
{

    public static bool ParenthesisIsBalanced(string equation) =>
        equation.Count(x => x == '(') == equation.Count(x => x == ')');

    public static bool ParenthesisIsValid(string equation) =>
        equation.IndexOf('(') <= equation.IndexOf(')') && ParenthesisIsBalanced(equation);

    public static string RemoveSpaces(string equation) => equation.Replace(" ", string.Empty);

    public static string ReplaceDoubleStarWithUpArrow(string equation) => equation.Replace("**", "^");
}