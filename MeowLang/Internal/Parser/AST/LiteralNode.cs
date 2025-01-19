namespace MeowLang.Internal.Parser.AST;

public class LiteralNode : AstNode
{
    public float Literal { get; set; }

    public LiteralNode(float literal)
    {
        Literal = literal;
    }

    public override object Visit()
    {
        return Literal;
    }
}