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
        object leftValue = Left.Visit();
        object rightValue = Right.Visit();

        if (leftValue is NullNode)
        {
            if (rightValue is float litLVar && Expression is "-" or "+")
            {
                return EvaluateNumber(Expression, 0, litLVar);
            }
        }
        else
        {
            if (leftValue is float litLVar && rightValue is float litRVar)
            {
                return EvaluateNumber(Expression, litLVar, litRVar);
            } else if (leftValue is string || rightValue is string)
            {
                return EvaluateStrings(Expression, leftValue, rightValue);
            } else if (leftValue is bool boolLVar && rightValue is bool boolRVar)
            {
                return EvaluateBools(Expression, boolLVar, boolRVar);
            }
        }

        throw new InvalidOperationException($"Unsupported expression: {leftValue} {Expression} {rightValue}");
    }
    
    public override string ToString()
    {
        return base.ToString()+ $" ({Left.ToString()} {Expression} {Right.ToString()})";
    }

    private object EvaluateNumber(string expression, float lvar, float rvar)
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
            case "==":
                return Math.Abs(lvar - rvar) < 0.00001f;
            case "!=":
                return Math.Abs(lvar - rvar) > 0.00001f;
            case "<":
                return lvar < rvar;
            case ">":
                return lvar > rvar;
            case "<=":
                return lvar <= rvar;
            case ">=":
                return lvar >= rvar;
            default:
                throw new InvalidOperationException($"Unsupported operator: {expression}");
        }   
    }
    // only allow for concatenation 
    private object EvaluateStrings(string expression, object lvar, object rvar)
    {
        switch (expression)
        {
            case "+" or "..":
                return lvar.ToString() + rvar.ToString();
            case "==":
                if (lvar.GetType() != rvar.GetType())
                    return false;
                return lvar.ToString() == rvar.ToString();
            case "!=":
                if (lvar.GetType() != rvar.GetType())
                    return false;
                return lvar.ToString() != rvar.ToString();
            default:
                throw new InvalidOperationException($"Unsupported operator: {expression}");
        }   
    }

    private object EvaluateBools(string expression, bool lvar, bool rvar)
    {
        switch (expression)
        {
            case "and":
                return lvar && rvar;
            case "or":
                return lvar || rvar;
            case "==":
                return lvar == rvar;
            case "!=":
                return lvar != rvar;
            default:
                throw new InvalidOperationException($"Unsupported operator: {expression}");
        }   
    }
}