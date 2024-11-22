namespace MeowLang.Internal.Lexer;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Lexer
{
    public static void FindTokens(string code, out Token[] tokens)
    {
        tokens = [];

        string pattern = @"(?<Number>\d+(\.\d+)?)" +
                         @"|(?<Comment>//.*?(?:\r?\n|$))" +
                         @"|(?<Operator>[+\-*|])" +
                         @"|(?<Keyword>if|function|while|null)" +
                         "|(?<Terminator>[;])" +
                         "|(?<Punctuation>[(){}.,;:])" +
                         @"|(?<Identifier>\b\w+\b)" +
                         @"|(?<String>""[^""]*"")";

        
        foreach (Match match in Regex.Matches(code, pattern, RegexOptions.Singleline))
        {
            foreach (var tokenName in Enum.GetNames(typeof(TokenType)))
            {
                if (tokenName == "Comment") continue;

                if (match.Groups[tokenName].Success)
                {
                    Array.Resize(ref tokens, tokens.Length + 1);
                    
                    if (Enum.TryParse(tokenName, out TokenType tokenType))
                    {
                        tokens[^1] = new Token(
                            tokenType, 
                            match.Value);
                    }
                    break;
                }
            }
        }
    }
}
