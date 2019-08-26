using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace System
{
    public static class StringExtensionsCalculate
    {
        private const string InvalidExpression = "Invalid expression.";
        private static readonly string DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private static bool _floatingPointExpression;

        public static string Calculate(this string equation)
        {
            equation = RemoveSpaces(equation);
            _floatingPointExpression = equation.Contains(DecimalSeparator[0]);
            if (!ParenthesisIsValid(equation)) return InvalidExpression;
            equation = EvaluateParenthesisedPiecesOfEquation(equation);
            return WeightedCalculate(equation);
        }

        private static string RemoveSpaces(string equation)
        {
            return equation.Replace(" ", string.Empty);
        }

        private static bool ParenthesisIsValid(string equation)
        {
            return equation.IndexOf('(') <= equation.IndexOf(')') && ParenthesisIsBalanced(equation);
        }

        private static bool ParenthesisIsBalanced(string equation)
        {
            return equation.Count(x => x == '(') == equation.Count(x => x == ')');
        }

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

                var subEquation = equation.Substring(startIndex, length);
                var result = WeightedCalculate(subEquation);
                equation = equation.Replace("(" + subEquation + ")", result);
            }

            return equation;
        }

        private static string WeightedCalculate(string equation)
        {
            if (equation.Length == 0) return string.Empty;

            var list = BreakUpEquation(equation);
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
            var item = string.Empty;
            var list = new List<string>();
            foreach (var character in equation)
            {
                if (int.TryParse(character.ToString(), out var _) || character == DecimalSeparator[0])
                {
                    item += character;
                }
                else
                {
                    if (item != string.Empty)
                    {
                        list.Add(item);
                        item = string.Empty;
                    }
                    else if (character == '-')
                    {
                        item += character;
                        continue;
                    }

                    list.Add(character.ToString());
                }
            }

            if (item != string.Empty) list.Add(item);

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
                    WriteDebugMessageAndArray($"Looking at = '{list[itemIndex]}'.\tLooking for = '{mathsOp.ToString()}'.\t", list);
                    if (list[itemIndex] != mathsOp.ToString()) continue;
                    list[itemIndex] = _floatingPointExpression
                        ? CalculateAsFloat(list[itemIndex - 1], list[itemIndex], list[itemIndex + 1])
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
        private static void WriteDebugMessageAndArray(string message, IEnumerable<string> list)
        {
            Debug.WriteLine($"{message}list = '{string.Join(' ', list)}'.");
        }

        private static string CalculateAsFloat(string number1, string operation, string number2)
        {
            if (!float.TryParse(number1, out var float1) ||
                (!float.TryParse(number2, out var float2))) return InvalidExpression;
            switch (operation)
            {
                case "^": return Math.Pow(float1, float2).ToString("F");
                case "*": return (float1 * float2).ToString("F");
                case "/": return (float1 / float2).ToString("F");
                case "%": return (float1 % float2).ToString("F");
                case "+": return (float1 + float2).ToString("F");
                case "-": return (float1 - float2).ToString("F");
                default: return InvalidExpression;
            }
        }

        private static string CalculateAsInteger(string number1, string operation, string number2)
        {
            if (!int.TryParse(number1, out var integer1) ||
                (!int.TryParse(number2, out var integer2))) return InvalidExpression;
            switch (operation)
            {
                case "^": return Math.Pow(integer1, integer2).ToString("F0");
                case "*": return (integer1 * integer2).ToString();
                case "/": return (integer1 / integer2).ToString();
                case "%": return (integer1 % integer2).ToString();
                case "+": return (integer1 + integer2).ToString();
                case "-": return (integer1 - integer2).ToString();
                default: return InvalidExpression;
            }
        }
    }
}