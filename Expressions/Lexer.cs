using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions
{
    public class Lexer
    {
        public static List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            var state = State.None;
            int tokenStart = 0;

            // TODO: This could do a better job parsing invalid tokens & knowing if parens are unmatched- although that's really the job of the parser.
            // The parser could just spit out diagnostics with positions, then the printer could change colors if it's at that position

            // TODO: This is just so that the last token gets parsed
            // Should fix this by calling Consume(' ')
            // Or a CreateToken() function that depending on the state transitions to State.None and creates the appropriate token, would call at end
            // Create Consume function with the big switch
            expression += ' ';

            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];

                switch (state)
                {
                    case State.None:
                        if (char.IsLetter(ch) || ch == '_')
                        {
                            state = State.InIdentifier;
                            tokenStart = i;
                        }
                        else if (char.IsNumber(ch))
                        {
                            state = State.InNumber;
                            tokenStart = i;
                        }
                        else if (char.IsWhiteSpace(ch))
                        {
                            // Simply consume white-space
                        }
                        else
                        {
                            switch (ch)
                            {
                                case '+':
                                    tokens.Add(new Token(TokenKind.Plus, "+", i));
                                    break;
                                case '-':
                                    tokens.Add(new Token(TokenKind.Minus, "-", i));
                                    break;
                                case '*':
                                    tokens.Add(new Token(TokenKind.Star, "*", i));
                                    break;
                                case '/':
                                    tokens.Add(new Token(TokenKind.Slash, "/", i));
                                    break;
                                case '(':
                                    tokens.Add(new Token(TokenKind.LeftParen, "(", i));
                                    break;
                                case ',':
                                    tokens.Add(new Token(TokenKind.Comma, ",", i));
                                    break;
                                case ')':
                                    tokens.Add(new Token(TokenKind.RightParen, ")", i));
                                    break;
                                default:
                                    tokenStart = i;
                                    state = State.InInvalid;
                                    break;
                            }
                        }
                        break;
                    case State.InIdentifier:
                        if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                        {
                            // TODO: This could be a function
                            tokens.Add(new Token(
                                TokenKind.Identifier,
                                expression.Substring(tokenStart, i - tokenStart),
                                tokenStart));
                            state = State.None;
                            --i;
                        }
                        break;
                    case State.InNumber:
                        if (ch == '.')
                        {
                            state = State.AfterDecimal;
                        }
                        else if (!char.IsNumber(ch))
                        {
                            tokens.Add(new Token(
                                TokenKind.Number,
                                expression.Substring(tokenStart, i - tokenStart),
                                tokenStart));
                            state = State.None;
                            --i;
                        }
                        break;
                    case State.AfterDecimal:
                        if (char.IsNumber(ch))
                        {
                            state = State.InMantissa;
                        }
                        else
                        {
                            state = State.InInvalid;
                        }
                        break;
                    case State.InMantissa:
                        if (!char.IsNumber(ch))
                        {
                            tokens.Add(new Token(
                                TokenKind.Number,
                                expression.Substring(tokenStart, i - tokenStart),
                                tokenStart));
                            state = State.None;
                            --i;
                        }
                        break;
                    case State.InInvalid:
                        if (char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch))
                        {
                            tokens.Add(new Token(
                                TokenKind.Invalid,
                                expression.Substring(tokenStart, i - tokenStart),
                                tokenStart));
                            state = State.None;
                            --i;
                        }
                        break;
                }
            }

            return tokens;
        }

        private enum State
        {
            None,
            InIdentifier,
            InNumber,
            AfterDecimal,
            InMantissa,
            InInvalid
        }
    }
}
