namespace MeowLang.Internal.Parser.AST;

public class BinaryExpressionNode : AstNode
{
    public string Expression;
    public AstNode Left;
    public AstNode Right;

    public BinaryExpressionNode(string expression, AstNode left, AstNode right)
    {
        Expression = expression;
        Left = left;
        Right = right;
    }

    public BinaryExpressionNode()
    {
        
    }
}