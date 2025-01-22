namespace MeowLang.Internal.Parser.AST;

public class NullNode : AstNode
{
    public override object Visit()
    {
        return null;
    }
}