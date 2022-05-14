using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace System
{
    public static class StringExtensionsCalculate
    {
        private const string InvalidExpression = "Invalid expression.";
        private static readonly char DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
        private static bool floatingPointExpression;

        public static string Calculate(this string equation)
        {
            equation = RemoveSpaces(equation);
            equation = ReplaceDoubleStarWithUpArrow(equation);
            floatingPointExpression = equation.Contains(DecimalSeparator);
            if (!ParenthesisIsValid(equation)) 
                return InvalidExpression;
            equation = EvaluateParenthesisedPiecesOfEquation(equation);
            return WeightedCalculate(equation);
        }

        private static string RemoveSpaces(string equation) => equation.Replace(" ", string.Empty);

        private static string ReplaceDoubleStarWithUpArrow(string equation) => equation.Replace("**", "^");

        private static bool ParenthesisIsValid(string equation) => 
            equation.IndexOf('(') <= equation.IndexOf(')') && ParenthesisIsBalanced(equation);

        private static bool ParenthesisIsBalanced(string equation) => 
            equation.Count(x => x == '(') == equation.Count(x => x == ')');

        private static string EvaluateParenthesisedPiecesOfEquation(string equation)
        {
            while (equation.Contains("("))
            {
                var length = 0;
                var startIndex = 0;
                var equationIndex = 0;
                foreach (var character in equation)
                {
                    if (character == '(')
                    {
                        startIndex = equationIndex + 1;
                        length = 0;
                    }
                    else if (character == ')' && length == 0)
                    {
                        length = equationIndex - startIndex;
                    }

                    equationIndex++;
                }

                var result = WeightedCalculate(equation.Substring(startIndex, length));
                result = ReplaceNoOpBeforeBracketsWithTimes(equation, startIndex - 1, result);
                equation = equation.Substring(0, startIndex - 1) + result + equation.Substring(startIndex + length + 1);
            }

            return equation;
        }

        private static string ReplaceNoOpBeforeBracketsWithTimes(string equation, int startIndex, string result)
        {
            if (startIndex > 0 && IsANumber(equation[startIndex - 1]))
                result = "*" + result;
            return result;
        }

        private static bool IsANumber(char character) => IsANumber(character.ToString());

        private static bool IsANumber(string characters) => 
            int.TryParse(characters, out var _) || double.TryParse(characters, out var _);

        private static string WeightedCalculate(string equation)
        {
            if (equation.Length == 0) return string.Empty;

            var list = BreakUpEquation(equation);
            list = CheckForNegatives(list);
            try
            {
                CondenseListByCalculating(list, "^");
                CondenseListByCalculating(list, "/%*");
                CondenseListByCalculating(list, "+-");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return InvalidExpression;
            }

            return list.Count == 1 ? list[0] : InvalidExpression;
        }

        private static List<string> BreakUpEquation(string equation)
        {
            var stringRepresentingNumber = string.Empty;
            var list = new List<string>();
            foreach (var character in equation)
            {
                if (IsCharacterNumericOrDecimalSeparator(character))
                {
                    stringRepresentingNumber += character;
                }
                else
                {
                    if (stringRepresentingNumber != string.Empty)
                    {
                        list.Add(stringRepresentingNumber);
                        stringRepresentingNumber = string.Empty;
                    }

                    list.Add(character.ToString());
                }
            }

            if (stringRepresentingNumber != string.Empty) list.Add(stringRepresentingNumber);

            return list;
        }

        private static bool IsCharacterNumericOrDecimalSeparator(char character) => 
            (int.TryParse(character.ToString(), out var _)  || character == DecimalSeparator);

        private static List<string> CheckForNegatives(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == "-" && (i == 0 || !IsANumber(list[i - 1])))
                {
                    list[i + 1] = "-" + list[i + 1];
                    list.RemoveAt(i);
                }
            }

            return list;
        }

        private static void CondenseListByCalculating(IList<string> list, string mathsOperator)
        {
            if (list.Count == 1) return;

            var itemIndex = 0;
            while (itemIndex < list.Count)
            {
                foreach (var mathsOp in mathsOperator)
                {
                    if (list[itemIndex] != mathsOp.ToString()) continue;
                    WriteDebugMessageAndArray($"Looking at = '{list[itemIndex]}'.\tLooking for = '{mathsOp}'.\t", list);
                    list[itemIndex] = floatingPointExpression
                        ? CalculateAsDouble(list[itemIndex - 1], list[itemIndex], list[itemIndex + 1])
                        : CalculateAsInteger(list[itemIndex - 1], list[itemIndex], list[itemIndex + 1]);

                    list.RemoveAt(itemIndex + 1);
                    list.RemoveAt(itemIndex - 1);
                    itemIndex -= 2;
                    break;
                }

                itemIndex++;
            }

            WriteDebugMessageAndArray($"After ConsolidateListByDoing ({mathsOperator}).\t", list);
        }

        [Conditional("DEBUG")]
        private static void WriteDebugMessageAndArray(string message, IEnumerable<string> list) =>
            Debug.WriteLine($"{message}list = '{string.Join(' ', list)}'.");

        private static string CalculateAsDouble(string number1, string operation, string number2)
        {
            if (!double.TryParse(number1, out var double1) ||
                (!double.TryParse(number2, out var double2))) return InvalidExpression;
            return operation switch
            {
                "^" => Math.Pow(double1, double2).ToString("F"),
                "*" => (double1 * double2).ToString("F"),
                "/" => (double1 / double2).ToString("F"),
                "%" => (double1 % double2).ToString("F"),
                "+" => (double1 + double2).ToString("F"),
                "-" => (double1 - double2).ToString("F"),
                _ => InvalidExpression,
            };
        }

        private static string CalculateAsInteger(string number1, string operation, string number2)
        {
            if (!long.TryParse(number1, out var integer1) ||
                (!long.TryParse(number2, out var integer2))) return InvalidExpression;
            return operation switch
            {
                "^" => Math.Pow(integer1, integer2).ToString("F0"),
                "*" => (integer1 * integer2).ToString("F0"),
                "/" => (integer1 / integer2).ToString("F0"),
                "%" => (integer1 % integer2).ToString("F0"),
                "+" => (integer1 + integer2).ToString("F0"),
                "-" => (integer1 - integer2).ToString("F0"),
                _ => InvalidExpression,
            };
        }
    }
}