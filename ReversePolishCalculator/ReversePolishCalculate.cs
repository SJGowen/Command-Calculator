using static Utilities.StringUtils;

namespace System;

public static class ReversePolishCalculate
{
    private const string InvalidExpression = "Invalid expression.";
    private const string Operators = "^*/%+-";
    private static readonly char DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
    private static bool FloatingPointExpression;
                                      
    public static string CalculateReversePolish(this string equation)
    {
        equation = RemoveSpaces(equation);
        equation = ReplaceDoubleStarWithUpArrow(equation);
        FloatingPointExpression = equation.Contains(DecimalSeparator);
        if (!ParenthesisIsValid(equation))
            return InvalidExpression;
        return EvaluateEquation(equation);
    }

    private static string EvaluateEquation(string equation)
    {
        var outputQueue = StringToReversePolish(equation);
        var output = ReversePolishToString(outputQueue);
        return FloatingPointExpression ? output : StringFloatToInt(output);
    }

    private static string StringFloatToInt(string output)
    {
        if (output.Contains(DecimalSeparator))
        {
            var result = string.Empty;
            var index = 0;
            while (output[index] != DecimalSeparator)
            {
                result += output[index];
                index++;
            }

            return result;
        }

        return output;
    }

    private static string ReversePolishToString(Queue<string>? toEvaluate)
    {
        var stack = new Stack<string>();

        while (toEvaluate?.Count > 0)
        {
            string token = toEvaluate.Dequeue();

            if (IsOperator(token))
            {
                string operand2 = stack.Pop();
                string operand1 = stack.Pop();
                string result = EvaluateEquation(operand1, operand2, token);
                stack.Push(result);
            }
            else
            {
                stack.Push(token);
            }
        }

        return stack.Pop();
    }

    private static string EvaluateEquation(string operand1, string operand2, string token)
    {
        double num1 = double.Parse(operand1);
        double num2 = double.Parse(operand2);
        double result = token switch
        {
            "^" => Math.Pow(num1, num2),
            "*" => num1 * num2,
            "/" => num1 / num2,
            "%" => num1 % num2,
            "+" => num1 + num2,
            "-" => num1 - num2,
            _ => throw new InvalidOperationException("Invalid Operator")
        };

        return result.ToString();
    }

    private static Queue<string>? StringToReversePolish(string equation)
    {
        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<string>();
        var numberBeingRead = "";
        foreach (char character in equation)
        {
            if (IsNumberChar(character))
            {
                numberBeingRead += character;
            }
            else
            {
                if (numberBeingRead != "")
                {
                    outputQueue.Enqueue(numberBeingRead);
                    numberBeingRead = "";
                }

                if (IsOperator(character))
                {
                    while (operatorStack.Count > 0 && GreaterPrecedence(operatorStack.Peek(), character.ToString()))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Push(character.ToString());
                }

                if (IsLeftBracket(character.ToString()))
                {
                    operatorStack.Push(character.ToString());
                }

                if (IsRightBracket(character.ToString()))
                {
                    while (!IsLeftBracket(operatorStack.Peek()))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Pop();
                }

            }
        }

        if (numberBeingRead != "")
        {
            outputQueue.Enqueue(numberBeingRead);
        }

        while (operatorStack.Count > 0)
        {
            outputQueue.Enqueue(operatorStack.Pop());
        }

        return outputQueue;
    }

    private static bool IsRightBracket(string potentialRightBracket)
    {
        return potentialRightBracket == ")";
    }

    private static bool IsLeftBracket(string potentialLeftBracket)
    {
        return potentialLeftBracket == "(";
    }

    private static bool GreaterPrecedence(string a, string b)
    {
        return (Operators.Contains(a) && Operators.Contains(b) && Operators.IndexOf(a) < Operators.IndexOf(b));
    }

    private static bool IsOperator(char character)
    {
        return Operators.Contains(character);
    }

    private static bool IsOperator(string a)
    {
        return Operators.Contains(a[0]);
    }


    private static bool IsNumberChar(char character)
    {
        return char.IsDigit(character) || character == DecimalSeparator;
    }
}