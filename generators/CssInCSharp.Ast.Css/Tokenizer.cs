﻿namespace CssInCSharp.Ast.Css
{
    public class Tokenizer
    {
        private List<Token> tokens = [];
        private bool urlMode = false;
        private int blockMode = 0;
        private int pos = 0;
        private int tn = 0;
        private int ln = 1;
        private int col = 1;
        private int cssLength = 0;

        private Dictionary<char, string> Punctuation = new()
        {
            { ' ', TokenType.Space },
            { '\n', TokenType.Newline },
            { '\r', TokenType.Newline },
            { '\t', TokenType.Tab },
            { '!', TokenType.ExclamationMark },
            { '"', TokenType.QuotationMark },
            { '#', TokenType.NumberSign },
            { '$', TokenType.DollarSign },
            { '%', TokenType.PercentSign },
            { '&', TokenType.Ampersand },
            { '\'', TokenType.Apostrophe },
            { '(', TokenType.LeftParenthesis },
            { ')', TokenType.RightParenthesis },
            { '*', TokenType.Asterisk },
            { '+', TokenType.PlusSign },
            { ',', TokenType.Comma },
            { '-', TokenType.HyphenMinus },
            { '.', TokenType.FullStop },
            { '/', TokenType.Solidus },
            { ':', TokenType.Colon },
            { ';', TokenType.Semicolon },
            { '<', TokenType.LessThanSign },
            { '=', TokenType.EqualsSign },
            { '>', TokenType.GreaterThanSign },
            { '?', TokenType.QuestionMark },
            { '@', TokenType.CommercialAt },
            { '[', TokenType.LeftSquareBracket },
            { ']', TokenType.RightSquareBracket },
            { '^', TokenType.CircumflexAccent },
            { '_', TokenType.LowLine },
            { '{', TokenType.LeftCurlyBracket },
            { '|', TokenType.VerticalLine },
            { '}', TokenType.RightCurlyBracket },
            { '~', TokenType.Tilde }
        };

        public List<Token> getTokens(string css, int tabSize)
        {
            char c; // Current character
            char cn; // Next character

            cssLength = css.Length;

            // Parse string, character by character:
            for (pos = 0; pos < cssLength; col++, pos++)
            {
                c = css.charAt(pos);
                cn = css.charAt(pos + 1);

                // If we meet `/*`, it's a start of a multiline comment.
                // Parse following characters as a multiline comment:
                if (c == '/' && cn == '*')
                {
                    parseMLComment(css);
                }

                // If we meet `//` and it is not a part of url:
                else if (!urlMode && c == '/' && cn == '/')
                {
                    // If we're currently inside a block, treat `//` as a start
                    // of identifier. Else treat `//` as a start of a single-line
                    // comment:
                    if (blockMode > 0) parseIdentifier(css);
                    else parseSLComment(css);
                }

                // If current character is a double or single quote, it's a start
                // of a string:
                else if (c == '"' || c == '\'')
                {
                    parseString(css, c);
                }

                // If current character is a space:
                else if (c == ' ')
                {
                    parseSpaces(css);
                }

                // If current character is a punctuation mark:
                else if (Punctuation.ContainsKey(c))
                {
                    // Add it to the list of tokens:
                    pushToken(Punctuation[c], c, col);
                    if (c == '\n' || c == '\r')
                    {
                        ln++;
                        col = 0;
                    } // Go to next line
                    else if (c == ')') urlMode = false; // Exit url mode
                    else if (c == '{') blockMode++; // Enter a block
                    else if (c == '}') blockMode--; // Exit a block
                    else if (c == '\t' && tabSize > 1) col += (tabSize - 1);
                }

                // If current character is a decimal digit:
                else if (isDecimalDigit(c))
                {
                    parseDecimalNumber(css);
                }

                // If current character is anything else:
                else
                {
                    parseIdentifier(css);
                }
            }

            return tokens;
        }

        public void parseMLComment(string css)
        {
            var start = pos;

            // Read the string until we meet `*/`.
            // Since we already know first 2 characters (`/*`), start reading
            // from `pos + 2`:
            for (pos = pos + 2; pos < cssLength; pos++)
            {
                if (css.charAt(pos) == '*' && css.charAt(pos + 1) == '/')
                {
                    pos++;
                    break;
                }
            }

            // Add full comment (including `/*` and `*/`) to the list of tokens:
            var comment = css.substring(start, pos + 1);
            pushToken(TokenType.CommentML, comment, col);

            var newlines = comment.Split('\n');
            if (newlines.Length > 1)
            {
                ln += newlines.Length - 1;
                col = newlines[newlines.Length - 1].Length;
            }
            else
            {
                col += (pos - start);
            }
        }

        public void parseIdentifier(string css)
        {
            var start = pos;

            // Skip all opening slashes:
            while (css.charAt(pos) == '/') pos++;

            // Read the string until we meet a punctuation mark:
            for (; pos < cssLength; pos++)
            {
                // Skip all '\':
                if (css.charAt(pos) == '\\') pos++;
                else if (Punctuation.ContainsKey(css.charAt(pos))) break;
            }

            var ident = css.substring(start, pos--);

            // Enter url mode if parsed substring is `url`:
            urlMode = urlMode || ident == "url";

            // Add identifier to tokens:
            pushToken(TokenType.Identifier, ident, col);
            col += (pos - start);
        }

        public void parseSLComment(string css)
        {
            var start = pos;

            // Read the string until we meet line break.
            // Since we already know first 2 characters (`//`), start reading
            // from `pos + 2`:
            for (pos += 2; pos < cssLength; pos++)
            {
                if (css.charAt(pos) == '\n' || css.charAt(pos) == '\r')
                {
                    break;
                }
            }

            // Add comment (including `//` and line break) to the list of tokens:
            pushToken(TokenType.CommentSL, css.substring(start, pos--), col);
            col += pos - start;
        }

        public void parseString(string css, char q)
        {
            var start = pos;

            // Read the string until we meet a matching quote:
            for (pos++; pos < cssLength; pos++)
            {
                // Skip escaped quotes:
                if (css.charAt(pos) == '\\') pos++;
                else if (css.charAt(pos) == q) break;
            }

            // Add the string (including quotes) to tokens:
            pushToken(q == '"' ? TokenType.StringDQ : TokenType.StringSQ,
                css.substring(start, pos + 1), col);
            col += (pos - start);
        }

        public void parseSpaces(string css)
        {
            var start = pos;

            // Read the string until we meet a non-space character:
            for (; pos < cssLength; pos++)
            {
                if (css.charAt(pos) != ' ') break;
            }

            // Add a substring containing only spaces to tokens:
            pushToken(TokenType.Space, css.substring(start, pos--), col);
            col += (pos - start);
        }

        public void parseDecimalNumber(string css)
        {
            var start = pos;

            // Read the string until we meet a character that's not a digit:
            for (; pos < cssLength; pos++)
            {
                if (!isDecimalDigit(css.charAt(pos))) break;
            }

            // Add the number to tokens:
            pushToken(TokenType.DecimalNumber, css.substring(start, pos--), col);
            col += (pos - start);
        }

        private void pushToken(string type, char value, int column)
        {
            pushToken(type, value.ToString(), column);
        }

        private void pushToken(string type, string value, int column)
        {
            tokens.push(new Token{
                tn = tn++,
                ln = ln,
                col = column,
                type = type,
                value = value
            });
        }

        private bool isDecimalDigit(char c)
        {
            return "0123456789".IndexOf(c) >= 0;
        }
    }
}
