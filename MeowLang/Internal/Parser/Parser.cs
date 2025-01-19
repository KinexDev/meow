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
            public Token Token;
            public AstNode value;

            public DispatchData(Token token, AstNode value)
            {
                Token = token;
                this.value = value;
            }
        }
        public static List<DispatchData> dispatch = new List<DispatchData>();
        
        public static object Evaluate(AstNode node)
        {
            return node.Visit();
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
                    case TokenType.String:
                        StringNode stringNode = new StringNode(tokens[i].Value);

                        if (nodeCurrentlyIn is BinaryExpressionNode strbinaryNode)
                        {
                            strbinaryNode.Right = stringNode;
                        }
                        else
                        {
                            nodeCurrentlyIn = stringNode;
                        }
                        break;
                    case TokenType.Operator:
                        if (i + 1 < tokens.Length)
                        {
                            if (tokens[i].Value == "not")
                            {
                                var nextToken = tokens[i + 1];
                                if (nextToken.TokenType != TokenType.Keyword &&
                                    nextToken.TokenType != TokenType.Operator && 
                                    nextToken.TokenType != TokenType.Bracket)
                                {
                                    throw new InterpreterException(nextToken.Line, "Invalid token for 'not' operator");
                                }
                                
                                UnaryExpressionNode node = new UnaryExpressionNode();
                                dispatch.Add(new DispatchData(tokens[i], nodeCurrentlyIn));
                                nodeCurrentlyIn = node;
                                continue;
                            }

                            if (tokens[i + 1].TokenType == TokenType.Number)
                            {
                                LiteralNode nextNum = new LiteralNode(float.Parse(tokens[i + 1].Value));

                                if (nodeCurrentlyIn is BinaryExpressionNode opBinaryNode)
                                {
                                    if (OperatorPrecedence(opBinaryNode.Expression) < OperatorPrecedence(tokens[i].Value) && !opBinaryNode.InBracket)
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
                            } else if (tokens[i + 1].TokenType == TokenType.Keyword)
                            {
                                if (tokens[i + 1].Value == "false" || tokens[i + 1].Value == "true")
                                {
                                    BooleanNode nextNum = new BooleanNode(bool.Parse(tokens[i + 1].Value));

                                    if (nodeCurrentlyIn is BinaryExpressionNode opBinaryNode)
                                    {
                                        if (OperatorPrecedence(opBinaryNode.Expression) < OperatorPrecedence(tokens[i].Value) && !opBinaryNode.InBracket)
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
                                    throw new InterpreterException(tokens[i + 1].Line, $"Incorrect keyword '{tokens[i + 1].Value}', expected 'true' or 'false'");
                                }
                            } else if (tokens[i + 1].TokenType == TokenType.String)
                            {
                                StringNode nextNum = new StringNode(tokens[i + 1].Value);

                                if (nodeCurrentlyIn is BinaryExpressionNode opBinaryNode)
                                {
                                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, opBinaryNode, nextNum);
                                    nodeCurrentlyIn = newBinaryNode;
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
                                        throw new InterpreterException(tokens[i + 1].Line, $"Expected a value after the operator near ')'");
                                    }
                                    
                                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, nodeCurrentlyIn, new AstNode());
                                    nodeCurrentlyIn = newBinaryNode;
                                    continue;
                                }

                                if (tokens[i + 1].TokenType == TokenType.Operator && tokens[i + 1].Value == "not")
                                {
                                    continue;
                                }
                                
                                throw new InterpreterException(tokens[i + 1].Line, $"Expected a value after the operator near token '{tokens[i + 1].Value}'");
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
                            dispatch.Add(new DispatchData(tokens[i], nodeCurrentlyIn));
                            nodeCurrentlyIn = new AstNode();
                        }

                        if (tokens[i].Value == ")")
                        {
                            if (dispatch[^1].Token.Value == "(")
                            {
                                var bracketNode = nodeCurrentlyIn;

                                if (bracketNode is BinaryExpressionNode bebracketNode)
                                {
                                    bebracketNode.InBracket = true;
                                }
                                
                                nodeCurrentlyIn = dispatch[^1].value;

                                if (nodeCurrentlyIn is BinaryExpressionNode brBinaryNode)
                                {
                                    if (brBinaryNode.Left is BinaryExpressionNode leftBinaryNode)
                                    {
                                        if (OperatorPrecedence(brBinaryNode.Expression) > OperatorPrecedence(leftBinaryNode.Expression) && !leftBinaryNode.InBracket)
                                        {
                                            BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(brBinaryNode.Expression, leftBinaryNode.Right, bracketNode);

                                            leftBinaryNode.Right = newBinaryNode;
                                    
                                            nodeCurrentlyIn = leftBinaryNode;

                                        }
                                        else
                                        {
                                            BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(brBinaryNode.Expression, leftBinaryNode, bracketNode);
                                            nodeCurrentlyIn = newBinaryNode;
                                        }
                                    }
                                    else
                                    {
                                        brBinaryNode.Right = bracketNode;
                                        nodeCurrentlyIn = brBinaryNode;
                                    }
                                }
                                else if (nodeCurrentlyIn is UnaryExpressionNode unaryNode)
                                {
                                    unaryNode.Boolean = bracketNode;
                                    dispatch.RemoveAt(dispatch.Count - 1);
                                } else if (nodeCurrentlyIn.GetType() == typeof(AstNode))
                                {
                                    nodeCurrentlyIn = bracketNode;
                                }
                                
                                dispatch.RemoveAt(dispatch.Count - 1);

                            }
                            else
                            {
                                throw new InterpreterException(tokens[i + 1].Line, "Expected a ')' after the bracket in line.");
                            }
                        }
                        break;
                    case TokenType.Keyword:
                        //handeling not
                        if (dispatch.Count > 0)
                        {
                            var previousDispatchData = dispatch[^1];

                            if (previousDispatchData.Token.TokenType == TokenType.Operator &&
                                previousDispatchData.Token.Value == "not")
                            {
                                var node = nodeCurrentlyIn as UnaryExpressionNode;
                                if (tokens[i].Value == "false" || tokens[i].Value == "true")
                                {
                                    node.Boolean =
                                        new BooleanNode(bool.Parse(tokens[i].Value));
                                }
                                else
                                {
                                    throw new InterpreterException(tokens[i].Line, $"Incorrect keyword '{tokens[i].Value}'");
                                }
                                
                                nodeCurrentlyIn = previousDispatchData.value;
                                
                                if (nodeCurrentlyIn is BinaryExpressionNode boolBinaryNode)
                                {
                                    boolBinaryNode.Right = node;
                                }
                                else
                                {
                                    nodeCurrentlyIn = node;
                                }

                                
                                dispatch.RemoveAt(dispatch.Count - 1);
                                continue;
                            }
                        }
                        
                        if (tokens[i].Value == "false" || tokens[i].Value == "true")
                        {
                            BooleanNode booleanNode = new BooleanNode(bool.Parse(tokens[i].Value));

                            if (nodeCurrentlyIn is BinaryExpressionNode boolbinaryNode)
                            {
                                boolbinaryNode.Right = booleanNode;
                            }
                            else
                            {
                                nodeCurrentlyIn = booleanNode;
                            }
                        }
                        break;
                }
            }


            if (dispatch.Count != 0)
            {
                throw new InterpreterException(dispatch[0].Token.Line, $"Code not ended at '{dispatch[0].Token.Value}'");
            }
            
            return nodeCurrentlyIn;
        }


        private static int OperatorPrecedence(string operatorToken)
        {
            switch (operatorToken)
            {
                case "+":
                    return 1;
                case "-":
                    return 1;
                case "*":
                    return 2;
                case "/":
                    return 2;
                case "or":
                    return 3;
                case "and":
                    return 4;
                case "not":
                    return 5;
                case "(":
                    return 6;
                case ")":
                    return 7;
                default:
                    return 0;
            }
        }
    }
}
