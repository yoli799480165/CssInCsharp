﻿namespace CssInCSharp.Ast.Css
{
    public class Mark
    {
        public void markTokens(List<Token> tokens)
        {
            // Mark paired brackets:
            markBrackets(tokens);
            // Mark whitespaces and comments:
            markSpacesAndComments(tokens);
        }

        public void markBrackets(List<Token>  tokens)
        {
            var tokensLength = tokens.Count;
            List<int> ps = []; // Parentheses
            List<int> sbs = []; // Square brackets
            List<int> cbs = []; // Curly brackets
            Token t; // Current token

            // For every token in the token list, if we meet an opening (left)
            // bracket, push its index number to a corresponding array.
            // If we then meet a closing (right) bracket, look at the corresponding
            // array. If there are any elements (records about previously met
            // left brackets), take a token of the last left bracket (take
            // the last index number from the array and find a token with
            // this index number) and save right bracket's index as a reference:
            for (var i = 0; i < tokensLength; i++)
            {
                t = tokens[i];
                var type = t.type;

                if (type == TokenType.LeftParenthesis)
                {
                    ps.push(i);
                }
                else if (type == TokenType.RightParenthesis)
                {
                    if (ps.Count.AsBool())
                    {
                        t.left = ps.pop();
                        tokens[t.left].right = i;
                    }
                }
                else if (type == TokenType.LeftSquareBracket)
                {
                    sbs.push(i);
                }
                else if (type == TokenType.RightSquareBracket)
                {
                    if (sbs.Count.AsBool())
                    {
                        t.left = sbs.pop();
                        tokens[t.left].right = i;
                    }
                }
                else if (type == TokenType.LeftCurlyBracket)
                {
                    cbs.push(i);
                }
                else if (type == TokenType.RightCurlyBracket)
                {
                    if (cbs.Count.AsBool())
                    {
                        t.left = cbs.pop();
                        tokens[t.left].right = i;
                    }
                }
            }
        }

        public void markSpacesAndComments(List<Token> tokens)
        {
            var tokensLength = tokens.Count;
            int[] spaces = [-1, -1];
            string type; // Current token's type

            // For every token in the token list, mark spaces and line breaks
            // as spaces (set both `ws` and `sc` flags). Mark multiline comments
            // with `sc` flag.
            // If there are several spaces or tabs or line breaks or multiline
            // comments in a row, group them: take the last one's index number
            // and save it to the first token in the group as a reference:
            // e.g., `ws_last = 7` for a group of whitespaces or `sc_last = 9`
            // for a group of whitespaces and comments.
            var i = 0;
            for (; i < tokensLength; i++)
            {
                type = tokens[i].type;

                if (type == TokenType.Space ||
                        type == TokenType.Tab ||
                        type == TokenType.Newline)
                {
                    markSpace(tokens, i, spaces);
                }
                else if (type == TokenType.CommentML)
                {
                    markComment(tokens, i, spaces);
                }
                else
                {
                    markEndOfSpacesAndComments(tokens, i, spaces);
                }
            }

            markEndOfSpacesAndComments(tokens, i, spaces);
        }

        public void markSpace(List<Token> tokens, int i, int[] spaces)
        {
            var token = tokens[i];
            token.ws = true;
            token.sc = true;

            if (spaces[0] == -1) spaces[0] = i;
            if (spaces[1] == -1) spaces[1] = i;
        }

        private void markComment(List<Token> tokens, int i, int[] spaces)
        {
            var ws = spaces[0];
            tokens[i].sc = true;

            if (ws != -1)
            {
                tokens[ws].ws_last = i - 1;
                spaces[0] = -1;
            }
        }

        public void markEndOfSpacesAndComments(List<Token> tokens, int i, int[] spaces)
        {
            var ws = spaces[0];
            var sc = spaces[1];
            if (ws != -1)
            {
                tokens[ws].ws_last = i - 1;
                spaces[0] = -1;
            }
            if (sc != -1)
            {
                tokens[sc].sc_last = i - 1;
                spaces[1] = -1;
            }
        }
    }
}
