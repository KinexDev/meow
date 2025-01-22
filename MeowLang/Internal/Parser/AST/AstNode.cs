namespace MeowLang.Internal.Parser.AST;

public class AstNode
{
    public virtual object Visit()
    {
        return new NullNode();
    }

    public override string ToString()
    {
        return $"({GetType().Name} {Visit()})";
    } 
}