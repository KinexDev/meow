using MeowLang.Internal.Parser.AST;
using MeowLang.Internal.Tokenizer;
using System;
using System.Collections.Generic;

namespace MeowLang.Internal.Parser
{
    public static class Parser
    {
        public class DispatchData
        {
            public string name;
            public AstNode value;
        }
        public static List<DispatchData> dispatch = new List<DispatchData>();
        
        public static float Evaluate(AstNode node)
        {
            if (node is LiteralNode numberNode)
            {
                return numberNode.Literal;
            }
            
            if (node is BinaryExpressionNode binaryNode)
            {
                float leftValue = Evaluate(binaryNode.Left);
                float rightValue = Evaluate(binaryNode.Right);

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
            
            throw new Exception($"Unsupported node type: {node.GetType()}");
        }
        
        public static AstNode Parse(Token[] tokens)
        {
            dispatch = new List<DispatchData>();
            AstNode nodeCurrentlyIn = new AstNode();
            
            for (var i = 0; i < tokens.Length; i++)
            {
                switch (tokens[i].TokenType)
                {
                    case TokenType.Number:
                        LiteralNode numberNode = new LiteralNode(float.Parse(tokens[i].Value));

                        if (nodeCurrentlyIn is BinaryExpressionNode binaryNode)
                        {
                            binaryNode.Right = numberNode;
                        }
                        else
                        {
                            nodeCurrentlyIn = numberNode;
                        }

                        break;

                    case TokenType.Operator:
                        if (i + 1 < tokens.Length)
                        {
                            if (tokens[i + 1].TokenType == TokenType.Number)
                            {
                                LiteralNode nextNum = new LiteralNode(float.Parse(tokens[i + 1].Value));

                                if (nodeCurrentlyIn is BinaryExpressionNode opBinaryNode)
                                {
                                    if (OperatorPrecedence(opBinaryNode.Expression) < OperatorPrecedence(tokens[i].Value))
                                    {
                                        BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, opBinaryNode.Right, nextNum);

                                        opBinaryNode.Right = newBinaryNode;
                                    
                                        nodeCurrentlyIn = opBinaryNode;
                                    
                                    }
                                    else
                                    {
                                        BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, opBinaryNode, nextNum);
                                        nodeCurrentlyIn = newBinaryNode;
                                    }
                                }
                                else
                                {
                                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, nodeCurrentlyIn, nextNum);

                                    nodeCurrentlyIn = newBinaryNode;
                                }

                                i++;
                            }
                            else
                            {
                                if (tokens[i + 1].TokenType == TokenType.Bracket)
                                {
                                    if (tokens[i + 1].Value == ")")
                                    {
                                        throw new InterpreterException(tokens[i + 1].Line, $"Expected a value after the operator near )");
                                    }
                                    
                                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, nodeCurrentlyIn, new AstNode());
                                    nodeCurrentlyIn = newBinaryNode;
                                    continue;
                                }
                                
                                throw new InterpreterException(tokens[i + 1].Line, $"Expected a value after the operator near token {tokens[i + 1].TokenType}");
                            }
                        }
                        else
                        {
                            throw new ArithmeticException($"Expected a value after the operator.");
                        }
                        break;
                    case TokenType.Bracket :
                        if (tokens[i].Value == "(")
                        {
                            dispatch.Add(new DispatchData() { name = "(", value = nodeCurrentlyIn });
                            nodeCurrentlyIn = new AstNode();
                        }

                        if (tokens[i].Value == ")")
                        {
                            if (dispatch[^1].name == "(")
                            {
                                var bracketNode = nodeCurrentlyIn;
                                nodeCurrentlyIn = dispatch[^1].value;
                                
                                if (nodeCurrentlyIn is BinaryExpressionNode brBinaryNode)
                                {
                                    if (brBinaryNode.Left is BinaryExpressionNode leftBinaryNode)
                                    {
                                        BinaryExpressionNode newBinaryNode =
                                            new BinaryExpressionNode(brBinaryNode.Expression, leftBinaryNode.Right,
                                                bracketNode);

                                        leftBinaryNode.Right = newBinaryNode;
                                        nodeCurrentlyIn = leftBinaryNode;
                                    }
                                    else
                                    {
                                        brBinaryNode.Right = bracketNode;
                                        nodeCurrentlyIn = brBinaryNode;
                                    }
                                }
                                
                                dispatch.RemoveAt(dispatch.Count - 1);

                            }
                            else
                            {
                                throw new InterpreterException(tokens[i + 1].Line, "Expected a ')' after the bracket in line.");
                            }
                        }
                        break;
                }
            }
            return nodeCurrentlyIn;
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
