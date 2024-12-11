namespace MeowLang.Internal.Parser.AST;

public class NumberNode : AstNode
{
    public float Number { get; set; }

    public NumberNode(float number)
    {
        Number = number;
    }
}