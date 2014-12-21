using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    public enum TokenKind
    {
        Identifier,
        Number,
        Plus,
        Minus,
        Star,
        Slash,
        LeftParen,
        RightParen,
        Comma,
        Invalid,
        Missing
    }

    public class Token
    {
        public TokenKind Kind;
        public string Value;
        public int Start;
        public int End { get { return Start + Value.Length; } }

        public Token(TokenKind kind, string value, int start)
        {
            Kind = kind;
            Value = value;
            Start = start;
        }
    }
}
