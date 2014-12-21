using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    public enum ExpressionKind
    {
        Identifier,
        NumericLiteral,
        FunctionCall,
        Negation,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Parenthetical
    }

    public abstract class Expression
    {
        public ExpressionKind Kind { get; protected set; }

        internal abstract void Accept(IExpressionVisitor visitor);

        internal abstract T Accept<T>(IExpressionVisitor<T> visitor);
    }

    internal class IdentifierExpression : Expression
    {
        public string Name { get; private set; }

        public IdentifierExpression(string name)
        {
            Kind = ExpressionKind.Identifier;
            Name = name;
        }

        internal override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class NumericLiteralExpression : Expression
    {
        public float Value { get; private set; }

        public NumericLiteralExpression(string text)
        {
            Kind = ExpressionKind.NumericLiteral;
            Value = float.Parse(text);
        }

        public NumericLiteralExpression(float value)
        {
            Kind = ExpressionKind.NumericLiteral;
            Value = value;
        }

        internal override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class PrefixExpression : Expression
    {
        public Expression Operand { get; set; }

        public PrefixExpression(TokenKind kind, Expression operand)
        {
            Kind = ExpressionKind.Negation;
            Operand = operand;
        }

        internal override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class BinaryExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public BinaryExpression(TokenKind kind, Expression left, Expression right)
        {
            switch (kind)
            {
                case TokenKind.Plus:
                    Kind = ExpressionKind.Addition;
                    break;
                case TokenKind.Minus:
                    Kind = ExpressionKind.Subtraction;
                    break;
                case TokenKind.Star:
                    Kind = ExpressionKind.Multiplication;
                    break;
                case TokenKind.Slash:
                    Kind = ExpressionKind.Division;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            Left = left;
            Right = right;
        }

        internal override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class ParentheticalExpression : Expression
    {
        public Expression Inner { get; private set; }

        public ParentheticalExpression(Expression inner)
        {
            Inner = inner;
        }

        internal override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class FunctionCallExpression : Expression
    {
        public IdentifierExpression Identifier { get; private set; }
        public List<Expression> Parameters { get; private set; }

        public FunctionCallExpression(IdentifierExpression identifier, List<Expression> parameters)
        {
            Identifier = identifier;
            Parameters = parameters;
        }

        internal override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
