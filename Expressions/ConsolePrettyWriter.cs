using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    public class ConsolePrettyWriter
    {
        private int m_position;

        public ConsoleColor IdentifierColor;
        public ConsoleColor NumericLiteralColor;
        public ConsoleColor OperatorColor;
        public ConsoleColor ParenthesisColor;
        public ConsoleColor InvalidColor;

        public ConsolePrettyWriter()
        {
            IdentifierColor = ConsoleColor.White;
            NumericLiteralColor = ConsoleColor.Cyan;
            OperatorColor = ConsoleColor.Gray;
            ParenthesisColor = ConsoleColor.DarkGray;
            InvalidColor = ConsoleColor.Red;
        }

        private void WriteToken(Token token)
        {
            switch (token.Kind)
            {
                case TokenKind.Identifier:
                    Console.ForegroundColor = IdentifierColor;
                    break;
                case TokenKind.Number:
                    Console.ForegroundColor = NumericLiteralColor;
                    break;
                case TokenKind.Plus:
                case TokenKind.Minus:
                case TokenKind.Star:
                case TokenKind.Slash:
                case TokenKind.Comma:
                    Console.ForegroundColor = OperatorColor;
                    break;
                case TokenKind.LeftParen:
                case TokenKind.RightParen:
                    Console.ForegroundColor = ParenthesisColor;
                    break;
                case TokenKind.Invalid:
                    Console.ForegroundColor = InvalidColor;
                    break;
            }

            while (m_position < token.Start)
            {
                Console.Write(' ');
                ++m_position;
            }

            Console.Write(token.Value);
            m_position = token.End;
        }

        public void Write(string source)
        {
            var originalForegroundColor = Console.ForegroundColor;

            m_position = 0;
            var tokens = Lexer.Tokenize(source);
            foreach (var token in tokens)
            {
                WriteToken(token);
            }
            
            Console.ForegroundColor = originalForegroundColor;
        }

        public void WriteLine(string source)
        {
            Write(source);
            Console.WriteLine();
        }
    }
}
