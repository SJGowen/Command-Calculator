using System;
using Xunit;

namespace CommandCalculatorTests
{
    public class CommandCalculatorTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("", " ")]
        [InlineData("1", "1")]
        [InlineData("123", "123")]
        [InlineData("2", "1+1")]
        [InlineData("444", "123+321")]
        [InlineData("15", "1+2 + 3 +4+5")]
        [InlineData("5", "1+2 + 3 +4-5")]
        [InlineData("26", "1+2 + 3 +4*5")]
        [InlineData("8", "1+2 + 3 +10/5")]
        [InlineData("10", "1+2+2 + 3 +10/5")]
        [InlineData("13", "1+2+3 + 3 +4%5")]
        [InlineData("7", "1+2+5 % 3 +12%5")]
        [InlineData("21", "1+4^2+5 % 3 +12%5")]
        public void CalculationsTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("3", "1+5-(6/2)")]
        [InlineData("14", "10+5-((8/2)-(6/2))")]
        [InlineData("9", "10+5-(6/2)-(6/2)")]
        public void CalculationsWithBracketsTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("-46", "-23+-23")]
        [InlineData("0", "-23--23")]
        [InlineData("0", "-23+23")]
        [InlineData("4", "-24/-6")]
        [InlineData("-4", "24/-6")]
        public void CalculationsWithNegativeNumbersTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("Invalid expression.", "-23+&23")]
        [InlineData("Invalid expression.", "-23+$23.5")]
        [InlineData("Invalid expression.", "-24//6")]
        [InlineData("Invalid expression.", "1+(24/-6))")]
        [InlineData("Invalid expression.", "1+(24/-6O3)")]
        [InlineData("Invalid expression.", "1+(2O4/-63)")]
        [InlineData("Invalid expression.", ")3+5(")]
        [InlineData("Invalid expression.", "-23---23")]
        public void CalculationsWithErrorsTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("3.00", "24.3/8.1")]
        [InlineData("3.00", "24.0/8.0")]
        [InlineData("-31.50", "-4.5*7")]
        [InlineData("31.50", "8%5-3+4.5*7")]
        [InlineData("27.00", "3.0^3")]
        public void CalculationsForFloatingPointTests(string expectedResult, string equation)
        {
            var result = equation.Calculate();
            Assert.Equal(expectedResult, result);
        }
    }
}
