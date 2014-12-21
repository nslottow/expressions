using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions.Repl
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = new ConsolePrettyWriter();
            var context = new ExpressionEvaluator();
            var random = new Random();

            context.Constants.Add("pi", 3.14159f);
            context.AddFunction("rand", () => { return (float)random.NextDouble(); });
            context.AddFunction("rand", (min, max) => { return (float)random.NextDouble() * (max - min) + min; });

            while (true)
            {
                Console.Write("expression: ");
                var input = Console.ReadLine();
                writer.Write(input);

                try
                {
                    var expression = Parser.Parse(input);
                    Console.WriteLine(" = {0}", context.Evaluate(expression));
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0}{1}", Environment.NewLine, e);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
    }
}
