namespace MeowLang.Internal.Parser.AST;

public class AstNode
{
    public virtual object Visit()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Visit().ToString();
    }
}