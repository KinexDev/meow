namespace MeowLang.Internal.Parser.AST;

public class UnaryExpressionNode : AstNode
{
    public AstNode Boolean { get; set; }

    public UnaryExpressionNode(AstNode boolean)
    {
        Boolean = boolean;
    }

    public UnaryExpressionNode()
    {
        
    }
    
    public override object Visit()
    {
        if (Boolean is BooleanNode booleanNode)
        {
            return !booleanNode.Boolean;
        } else if (Boolean is BinaryExpressionNode binaryExpressionNode)
        {
            var returnNode = binaryExpressionNode.Visit();
            if (returnNode is bool boolean)
            {
                return !boolean;
            }
        }
        else if (Boolean is UnaryExpressionNode unaryExpressionNode)
        {
            return unaryExpressionNode.Visit();
        }
        
        throw new InvalidOperationException($"Unsupported operand type for not operation: {Boolean.GetType()}");
    }
}