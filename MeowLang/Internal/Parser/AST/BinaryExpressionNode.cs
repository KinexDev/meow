namespace MeowLang.Internal.Parser.AST;

public class BinaryExpressionNode : AstNode
{
    public string Expression;
    public bool InBracket;
    public AstNode Left;
    public AstNode Right;

    public BinaryExpressionNode(string expression, AstNode left, AstNode right)
    {
        Expression = expression;
        Left = left;
        Right = right;
    }

    public override object Visit()
    {
        return Evaluate(this);
    }

    private object Evaluate(AstNode node)
    { 
        if (node is BinaryExpressionNode binaryNode)
        {
            object leftValue = Evaluate(binaryNode.Left);
            
            object rightValue = Evaluate(binaryNode.Right);
            if (leftValue is float litLVar && rightValue is float litRVar)
            {
                return EvaluateNumber(binaryNode.Expression, litLVar, litRVar);
            } else if (leftValue is string || rightValue is string)
            {
                return EvaluateStrings(binaryNode.Expression, leftValue, rightValue);
            } else if (leftValue is bool boolLVar && rightValue is bool boolRVar)
            {
                return EvaluateBools(binaryNode.Expression, boolLVar, boolRVar);
            }
        }
        else
        {
            return Parser.Evaluate(node);
        }
        throw new Exception($"Unsupported node type: {node.GetType()}");
    }

    public override string ToString()
    {
        return $"({Left.ToString()} {Expression} {Right.ToString()})";
    }

    private float EvaluateNumber(string expression, float lvar, float rvar)
    {
        switch (expression)
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
                throw new InvalidOperationException($"Unsupported operator: {expression}");
        }   
    }
    // only allow for concatenation 
    private string EvaluateStrings(string expression, object lvar, object rvar)
    {
        switch (expression)
        {
            case "+" or "..":
                return lvar.ToString() + rvar.ToString();
            default:
                throw new InvalidOperationException($"Unsupported operator: {expression}");
        }   
    }

    private bool EvaluateBools(string expression, bool lvar, bool rvar)
    {
        switch (expression)
        {
            case "and":
                return lvar && rvar;
            case "or":
                return lvar || rvar;
            default:
                throw new InvalidOperationException($"Unsupported operator: {expression}");
        }   
    }
}