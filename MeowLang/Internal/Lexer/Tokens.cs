namespace MeowLang.Internal.Lexer;

public enum TokenType
{
    Number,
    Comment,
    Operator,
    Terminator,
    Punctuation,
    Identifier,
    String,
    Keyword
}

public class Token(TokenType tokenType, string value)
{
    public TokenType tokenType { get; } = tokenType;
    public string value { get; } = value;
}

