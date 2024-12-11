namespace MeowLang.Internal.Tokenizer;

public enum TokenType
{
    Number,
    Comment,
    Operator,
    Bracket,
    Terminator,
    Punctuation,
    Identifier,
    String,
    Keyword,
    EOL
}

public class Token
{
    public TokenType tokenType { get; }
    public string value { get; }

    public Token(TokenType tokenType, string value)
    {
        this.tokenType = tokenType;
        this.value = value;
    }
    
    public Token(TokenType tokenType)
    {
        this.tokenType = tokenType;
    }
}

