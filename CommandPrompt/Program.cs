using System;

namespace CommandPrompt
{
    static class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {   
                Console.Write("Calculate > ");
                var equation = Console.ReadLine();
                if (equation?.ToLower() == "exit") break;
                var result = equation.Calculate();
                Console.WriteLine(result);
            }
        }
    }
}
