using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions.Tests
{
    public class EvaluatorTests
    {
        private const float k_pi = 3.14159f;

        [Fact]
        public static void Operators()
        {
            Verify("-1", -1);
            Verify("1 + 1", 2);
            Verify("2 * 3", 6);
            Verify("3 * 3", 9);
            Verify("3 / 3", 1);
        }

        [Fact]
        public static void Associativity()
        {
            Verify("-1 + 1", 0);
            Verify("2 + 3 * 4", 14);
            Verify("(2 + 3) * 4", 20);
            Verify("2 * 3 + 4", 10);
            Verify("3 * (4 + 5)", 27);
            Verify("2 + 3 / 4", 2.75f);
            Verify("(2 + 3) / 4", 1.25f);
        }

        [Fact]
        public static void Constants()
        {
            Verify("pi", k_pi);
            Verify("2 * pi", k_pi * 2.0f);
        }

        [Fact]
        public static void Functions()
        {
            var evaluator = new ExpressionEvaluator();
            evaluator.AddFunction("meaning", () => 42.0f);

            // TODO: this should throw: Verify("meaning", 0, evaluator);
            Verify("meaning()", 42, evaluator);
            Verify("(meaning())", 42, evaluator);
            Verify("2 * meaning()", 84, evaluator);
            Verify("meaning() * 2", 84, evaluator);
        }

        [Fact]
        public static void FunctionOverloads()
        {
            var evaluator = new ExpressionEvaluator();
            evaluator.AddFunction("f", () => 1.0f);
            evaluator.AddFunction("f", (a) => a);
            evaluator.AddFunction("f", (a, b) => a * b);

            Verify("f()", 1, evaluator);
            Verify("f(2)", 2, evaluator);
            Verify("f(2,3)", 6, evaluator);
        }

        [Fact]
        public static void Parameters()
        {
            var evaluator = new ExpressionEvaluator();
            var context = new TestContext(1, 2);

            Verify("a", 1, evaluator, context);
            Verify("b", 2, evaluator, context);

            // TODO: verify that any other parameter name throws
        }

        private class TestContext : IExpressionEvaluatorContext
        {
            public float A;
            public float B;

            public TestContext(float a, float b)
            {
                A = a;
                B = b;
            }

            public bool GetParameter(string name, out float value)
            {
                if (name == "a")
                {
                    value = A;
                    return true;
                }
                else if (name == "b")
                {
                    value = B;
                    return true;
                }
                else
                {
                    value = 0;
                    return false;
                }
            }
        }

        private static void Verify(string source, float expectedResult, ExpressionEvaluator evaluator = null, IExpressionEvaluatorContext context = null)
        {
            if (evaluator == null)
            {
                evaluator = new ExpressionEvaluator();
                var random = new Random();

                evaluator.Constants.Add("pi", k_pi);
                evaluator.AddFunction("rand", () => { return (float)random.NextDouble(); });
            }

            var reducerVisitor = new ExpressionReducerVisitor(evaluator);
            var evaluatorVisitor = new ExpressionEvaluatorVisitor(evaluator, context);

            var expression = Parser.Parse(source);
            var reducedExpression = expression.Accept(reducerVisitor);
            float actualResult = reducedExpression.Accept(evaluatorVisitor);

            Assert.Equal(actualResult, expectedResult,
                string.Format("Expression '{0}' evaulated to '{1}', expected '{2}'", source, actualResult, expectedResult));
        }
    }
}
