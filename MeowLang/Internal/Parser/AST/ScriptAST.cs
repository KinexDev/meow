namespace MeowLang.Internal.Parser.AST;

public class ScriptAST : AstNode
{
    public List<AstNode> Statements = new List<AstNode>();
}