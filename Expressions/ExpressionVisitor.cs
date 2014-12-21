using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    internal interface IExpressionVisitor
    {
        void Visit(IdentifierExpression identifierExpression);
        void Visit(NumericLiteralExpression numericLiteralExpression);
        void Visit(PrefixExpression prefixExpression);
        void Visit(BinaryExpression binaryExpression);
        void Visit(ParentheticalExpression parentheticalExpression);
        void Visit(FunctionCallExpression functionCallExpression);
    }

    internal interface IExpressionVisitor<T>
    {
        T Visit(IdentifierExpression identifierExpression);
        T Visit(NumericLiteralExpression numericLiteralExpression);
        T Visit(PrefixExpression prefixExpression);
        T Visit(BinaryExpression binaryExpression);
        T Visit(ParentheticalExpression parentheticalExpression);
        T Visit(FunctionCallExpression functionCallExpression);
    }
}
