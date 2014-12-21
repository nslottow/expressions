using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    public class ExpressionEvaluator
    {
        public readonly IDictionary<string, float> Constants;
        public readonly IDictionary<FunctionInfo, Delegate> Functions;

        public struct FunctionInfo
        {
            public string Name;
            public int ParameterCount;

            public FunctionInfo(string name, int parameterCount)
            {
                Name = name;
                ParameterCount = parameterCount;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ ParameterCount.GetHashCode();
            }
        }

        public ExpressionEvaluator()
        {
            Constants = new Dictionary<string, float>();
            Functions = new Dictionary<FunctionInfo, Delegate>();
        }

        internal ExpressionEvaluator(
            IDictionary<string, float> constants,
            IDictionary<FunctionInfo, Delegate> functions,
            Func<object, string, float> getInstanceParameter)
        {
            Constants = constants;
            Functions = functions;
        }

        public void AddFunction(string name, Func<float> func)
        {
            Functions.Add(new FunctionInfo(name, 0), func);
        }

        public void AddFunction(string name, Func<float, float> func)
        {
            Functions.Add(new FunctionInfo(name, 1), func);
        }

        public void AddFunction(string name, Func<float, float, float> func)
        {
            Functions.Add(new FunctionInfo(name, 2), func);
        }

        public void AddFunction(string name, Func<float, float, float, float> func)
        {
            Functions.Add(new FunctionInfo(name, 3), func);
        }

        public Expression ParseAndReduce(string source)
        {
            var expression = Parser.Parse(source);
            return expression.Accept(new ExpressionReducerVisitor(this));
        }

        public float Evaluate(Expression expression, IExpressionEvaluatorContext context = null)
        {
            return expression.Accept(new ExpressionEvaluatorVisitor(this, context));
        }

        private float GetInstanceParameterStub(object context, string name)
        {
            return 0;
        }
    }

    public interface IExpressionEvaluatorContext
    {
        bool GetParameter(string name, out float value);
    }

    internal struct ExpressionReducerVisitor : IExpressionVisitor<Expression>
    {
        private ExpressionEvaluator m_evaluator;

        public ExpressionReducerVisitor(ExpressionEvaluator evaluator)
        {
            m_evaluator = evaluator;
        }

        Expression IExpressionVisitor<Expression>.Visit(IdentifierExpression identifierExpression)
        {
            float value;
            if (m_evaluator.Constants.TryGetValue(identifierExpression.Name, out value))
            {
                return new NumericLiteralExpression(value);
            }

            return identifierExpression;
        }

        Expression IExpressionVisitor<Expression>.Visit(NumericLiteralExpression numericLiteralExpression)
        {
            return numericLiteralExpression;
        }

        Expression IExpressionVisitor<Expression>.Visit(PrefixExpression prefixExpression)
        {
            var reducedOperand = prefixExpression.Operand.Accept(this);
            if (reducedOperand.Kind == ExpressionKind.NumericLiteral)
            {
                float value = ((NumericLiteralExpression)reducedOperand).Value * -1;
                return new NumericLiteralExpression(value);
            }

            prefixExpression.Operand = reducedOperand;
            return prefixExpression;
        }

        Expression IExpressionVisitor<Expression>.Visit(BinaryExpression binaryExpression)
        {
            var left = binaryExpression.Left.Accept(this);
            var right = binaryExpression.Right.Accept(this);

            if (left.Kind == ExpressionKind.NumericLiteral && right.Kind == ExpressionKind.NumericLiteral)
            {
                var leftValue = ((NumericLiteralExpression)left).Value;
                var rightValue = ((NumericLiteralExpression)right).Value;

                switch (binaryExpression.Kind)
                {
                    case ExpressionKind.Addition:
                        return new NumericLiteralExpression(leftValue + rightValue);
                    case ExpressionKind.Subtraction:
                        return new NumericLiteralExpression(leftValue - rightValue);
                    case ExpressionKind.Multiplication:
                        return new NumericLiteralExpression(leftValue * rightValue);
                    case ExpressionKind.Division:
                        return new NumericLiteralExpression(leftValue / rightValue);
                }
            }

            binaryExpression.Left = left;
            binaryExpression.Right = right;

            return binaryExpression;
        }

        Expression IExpressionVisitor<Expression>.Visit(ParentheticalExpression parentheticalExpression)
        {
            return parentheticalExpression.Inner.Accept(this);
        }

        Expression IExpressionVisitor<Expression>.Visit(FunctionCallExpression functionCallExpression)
        {
            for (int i = 0; i < functionCallExpression.Parameters.Count; i++)
            {
                functionCallExpression.Parameters[i] = functionCallExpression.Parameters[i].Accept(this);
            }

            return functionCallExpression;
        }
    }

    internal struct ExpressionEvaluatorVisitor : IExpressionVisitor<float>
    {
        private ExpressionEvaluator m_evaluator;
        private IExpressionEvaluatorContext m_context;

        public ExpressionEvaluatorVisitor(ExpressionEvaluator evaluator, IExpressionEvaluatorContext context)
        {
            m_evaluator = evaluator;
            m_context = context;
        }

        public float ReduceAndEvaluate(Expression expression)
        {
            var reducedExpression = expression.Accept(new ExpressionReducerVisitor(m_evaluator));
            return reducedExpression.Accept(this);
        }

        float IExpressionVisitor<float>.Visit(IdentifierExpression identifierExpression)
        {
            float value;
            if (m_evaluator.Constants.TryGetValue(identifierExpression.Name, out value))
            {
                return value;
            }

            if (m_context != null && m_context.GetParameter(identifierExpression.Name, out value))
            {
                return value;
            }

            throw new Exception("Unknown identifier: " + identifierExpression.Name);
        }

        float IExpressionVisitor<float>.Visit(NumericLiteralExpression numericLiteralExpression)
        {
            return numericLiteralExpression.Value;
        }

        float IExpressionVisitor<float>.Visit(PrefixExpression prefixExpression)
        {
            Debug.Assert(prefixExpression.Kind == ExpressionKind.Negation);
            return prefixExpression.Operand.Accept(this) * -1;
        }

        float IExpressionVisitor<float>.Visit(BinaryExpression binaryExpression)
        {
            float left = binaryExpression.Left.Accept(this);
            float right = binaryExpression.Right.Accept(this);

            switch (binaryExpression.Kind)
            {
                case ExpressionKind.Addition:
                    return left + right;
                case ExpressionKind.Subtraction:
                    return left - right;
                case ExpressionKind.Multiplication:
                    return left * right;
                case ExpressionKind.Division:
                    return left / right;
            }

            Debug.Assert(false);
            return 0;
        }

        float IExpressionVisitor<float>.Visit(ParentheticalExpression parentheticalExpression)
        {
            return parentheticalExpression.Inner.Accept(this);
        }

        float IExpressionVisitor<float>.Visit(FunctionCallExpression functionCallExpression)
        {
            // TODO: Replace FunctionCallExpression.Identifier with a string
            Delegate function;
            if (!m_evaluator.Functions.TryGetValue(new ExpressionEvaluator.FunctionInfo(functionCallExpression.Identifier.Name, functionCallExpression.Parameters.Count), out function))
            {
                // TODO: proper error
                Console.WriteLine("error: A function named '{0}' does not exist", functionCallExpression.Identifier.Name);
                return 0;
            }

            if (functionCallExpression.Parameters.Count == 0)
            {
                return (float)function.DynamicInvoke();
            }

            // TODO: Don't have to allocate this every time, just reuse a single one with the longest parameter list
            // Hopefully avoid boxing too by storing boxed floats and not reboxing
            var parameters = new object[functionCallExpression.Parameters.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = functionCallExpression.Parameters[i].Accept(this);
            }

            return (float)function.DynamicInvoke(parameters);
        }
    }
}
