using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    public struct Parser
    {
        private List<Token> m_tokens;

        // TODO: This needs to be done before putting in editor: Figure out how to do errors so they can be shown.
        // TODO: Clean the interface

        // TODO: Figure out what state to keep in here
        // TODO: Figure out what state to keep in the Tokenizer
        private int m_tokenIndex;
        private Token m_token;

        private bool Done
        {
            get { return m_tokenIndex == m_tokens.Count; }
        }

        private Parser(List<Token> tokens)
        {
            m_tokens = tokens;
            m_tokenIndex = 0;
            m_token = null;
        }

        public static Expression Parse(string source)
        {
            return Parse(Lexer.Tokenize(source));
        }

        public static Expression Parse(List<Token> tokens)
        {
            var parser = new Parser(tokens);
            return parser.ParseExpression(OperatorPrecedence.None);
        }

        private void Advance()
        {
            if (m_tokenIndex < m_tokens.Count)
            {
                m_token = m_tokens[m_tokenIndex++];
            }
            else
            {
                m_token = new Token(TokenKind.Missing, null, m_token.End);
            }
        }

        private void LookAhead(int i)
        {
            int index = m_tokenIndex + i;
            if (index < m_tokens.Count)
            {
                m_token = m_tokens[index];
            }
            else
            {
                m_token = new Token(TokenKind.Missing, null, m_tokens[m_tokens.Count - 1].End);
            }
        }

        private Expression ParseExpression(OperatorPrecedence precedence)
        {
            Advance();
            Debug.Assert(m_token != null);

            Expression left = null;

            // Prefix semantic tokens
            switch (m_token.Kind)
            {
                case TokenKind.Minus:
                    left = new PrefixExpression(m_token.Kind, ParseExpression(OperatorPrecedence.Prefix));
                    break;
                case TokenKind.LeftParen:
                    var inner = ParseExpression(OperatorPrecedence.None);
                    Advance();
                    if (m_token.Kind != TokenKind.RightParen)
                    {
                        Console.WriteLine("error: Expected ')' at position {0}", m_token.Start);
                    }
                    left = new ParentheticalExpression(inner);
                    break;
                case TokenKind.Identifier:
                    left = new IdentifierExpression(m_token.Value);
                    break;
                case TokenKind.Number:
                    left = new NumericLiteralExpression(m_token.Value);
                    break;
                default:
                    Console.WriteLine("error: Failed to parse token '{0}' at position {1}", m_token.Value, m_token.Start);
                    break;
            }

            // Infix semantic tokens
            while (precedence < GetPrecedence())
            {
                Advance();
                switch (m_token.Kind)
                {
                    case TokenKind.Plus:
                    case TokenKind.Minus:
                        left = new BinaryExpression(m_token.Kind, left, ParseExpression(OperatorPrecedence.Addition));
                        break;
                    case TokenKind.Star:
                    case TokenKind.Slash:
                        left = new BinaryExpression(m_token.Kind, left, ParseExpression(OperatorPrecedence.Multiplication));
                        break;
                    case TokenKind.LeftParen:
                        left = new FunctionCallExpression((IdentifierExpression)left, ParseParameterList());
                        break;
                }
            }

            return left;
        }

        private OperatorPrecedence GetPrecedence()
        {
            LookAhead(0);
            switch (m_token.Kind)
            {
                case TokenKind.Plus:
                case TokenKind.Minus:
                    return OperatorPrecedence.Addition;
                case TokenKind.Star:
                case TokenKind.Slash:
                    return OperatorPrecedence.Multiplication;
                case TokenKind.LeftParen:
                    return OperatorPrecedence.FunctionCall;
                default:
                    return OperatorPrecedence.None;
            }
        }

        private List<Expression> ParseParameterList()
        {
            var parameters = new List<Expression>();

            LookAhead(0);
            while (m_token.Kind != TokenKind.RightParen && m_token.Kind != TokenKind.Missing)
            {
                parameters.Add(ParseExpression(OperatorPrecedence.None));

                if (m_token.Kind == TokenKind.Comma)
                {
                    Advance();
                }
                else if (m_token.Kind == TokenKind.RightParen)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("error: Expected ',' at position {0}", m_token.Start);
                }
            }

            // Skip over the right-paren
            Advance();
            return parameters;
        }

        private enum OperatorPrecedence
        {
            None = 0,
            Addition = 1,
            Multiplication = 2,
            Prefix = 3,
            FunctionCall = 4
        }
    }
}
