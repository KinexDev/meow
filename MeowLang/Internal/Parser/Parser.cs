using MeowLang.Internal.Parser.AST;
using MeowLang.Internal.Tokenizer;
using System;
using System.Collections.Generic;

namespace MeowLang.Internal.Parser
{
    public static class Parser
    {
        public static float Evaluate(AstNode node)
        {
            // Base case: if it's a NumberNode, just return the value
            if (node is NumberNode numberNode)
            {
                return numberNode.Number;
            }
            
            // Recursive case: if it's a BinaryExpressionNode, evaluate the left and right operands
            if (node is BinaryExpressionNode binaryNode)
            {
                // Recursively evaluate the left and right operands
                float leftValue = Evaluate(binaryNode.Left);
                float rightValue = Evaluate(binaryNode.Right);

                // Perform the operation based on the operator
                switch (binaryNode.Expression)
                {
                    case "+":
                        return leftValue + rightValue;
                    case "-":
                        return leftValue - rightValue;
                    case "*":
                        return leftValue * rightValue;
                    case "/":
                        if (rightValue == 0)
                        {
                            throw new DivideByZeroException("Division by zero is not allowed.");
                        }
                        return leftValue / rightValue;
                    default:
                        throw new InvalidOperationException($"Unsupported operator: {binaryNode.Expression}");
                }
            }

            // If we get here, something went wrong (shouldn't happen if AST is correct)
            throw new InvalidOperationException("Invalid AST node type.");
        }
        
        public static AstNode Parse(Token[] tokens)
        {
            var operandStack = new Stack<AstNode>();
            var operatorStack = new Stack<string>();

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];

                if (token.tokenType == TokenType.Number)
                {
                    operandStack.Push(new NumberNode(float.Parse(token.value)));
                }
                else if (token.tokenType == TokenType.Operator)
                {
                    while (operatorStack.Count > 0 &&
                           OperatorPrecedence(operatorStack.Peek()) >= OperatorPrecedence(token.value) &&
                           operatorStack.Peek() != "(")
                    {
                        var right = operandStack.Pop();
                        var left = operandStack.Pop();
                        var op = operatorStack.Pop();

                        operandStack.Push(new BinaryExpressionNode
                        {
                            Left = left,
                            Right = right,
                            Expression = op
                        });
                    }

                    operatorStack.Push(token.value);
                }
                else if (token.tokenType == TokenType.Bracket)
                {
                    if (token.value == "(")
                    {
                        operatorStack.Push(token.value);
                    }
                    else if (token.value == ")")
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                        {
                            var right = operandStack.Pop();
                            var left = operandStack.Pop();
                            var op = operatorStack.Pop();

                            operandStack.Push(new BinaryExpressionNode
                            {
                                Left = left,
                                Right = right,
                                Expression = op
                            });
                        }

                        if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                        {
                            operatorStack.Pop();
                        }
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                var right = operandStack.Pop();
                var left = operandStack.Pop();
                var op = operatorStack.Pop();

                operandStack.Push(new BinaryExpressionNode
                {
                    Left = left,
                    Right = right,
                    Expression = op
                });
            }

            return operandStack.Pop();
        }

        private static int OperatorPrecedence(string operatorToken)
        {
            switch (operatorToken)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "(":
                    return 3;
                case ")":
                    return 4;
                default:
                    return 0;
            }
        }
    }
}
