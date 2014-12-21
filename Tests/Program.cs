using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions.Tests
{
    class Program
    {
        public static void Main(string[] args)
        {
            var runner = new Runner();
            runner.AddTestSuite(typeof(EvaluatorTests));
            runner.Run();
        }
    }
}
