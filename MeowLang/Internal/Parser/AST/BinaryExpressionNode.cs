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
    
    public object Evaluate(AstNode node)
    {
        if (node is LiteralNode numberNode)
        {
            return numberNode.Literal;
        }
            
        if (node is BinaryExpressionNode binaryNode)
        {
            object leftValue = Evaluate(binaryNode.Left);
            object rightValue = Evaluate(binaryNode.Right);
            
            if (leftValue is float lvar && rightValue is float rvar)
            {
                switch (binaryNode.Expression)
                {
                    case "+":
                        return lvar + rvar;
                    case "-":
                        return lvar - rvar;
                    case "*":
                        return lvar * rvar;
                    case "/":
                        return lvar / rvar;
                    default:
                        throw new InvalidOperationException($"Unsupported operator: {binaryNode.Expression}");
                }   
            }
        }
            
        throw new Exception($"Unsupported node type: {node.GetType()}");
    }
}