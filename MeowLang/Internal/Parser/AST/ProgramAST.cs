namespace MeowLang.Internal.Parser.AST;

public class ProgramAST : AstNode
{
    public List<AstNode> Statements = new List<AstNode>();
}