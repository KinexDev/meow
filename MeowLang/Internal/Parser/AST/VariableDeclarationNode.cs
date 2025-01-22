namespace MeowLang.Internal.Parser.AST;

public class VariableDeclarationNode
{
    public string Identifier { get; set; }
    public AstNode Value { get; set; }
}