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
        
        public static object Evaluate(AstNode node)
        {
            return node.Visit();
        }
        
        public static ProgramAST Parse(Token[] tokens)
        {
            var dispatch = new List<DispatchData>();
            ProgramAST program = new ProgramAST();
            
            AstNode nodeCurrentlyIn = new AstNode();
            
            
            for (var i = 0; i < tokens.Length; i++)
            {
                switch (tokens[i].TokenType)
                {
                    case TokenType.Number:
                        if (dispatch.Count > 0)
                        {
                            var previousDispatchData = dispatch[^1];

                            if (previousDispatchData.Token.TokenType == TokenType.Operator &&
                                previousDispatchData.Token.Value == "-")
                            {
                                if (nodeCurrentlyIn is UnaryExpressionNode node)
                                {
                                    node.Operand =
                                        new NumberNode(float.Parse(tokens[i].Value));   

                                    ParseType(node, ref nodeCurrentlyIn);
                                    nodeCurrentlyIn = nod;
                                    dispatch.RemoveAt(dispatch.Count - 1);
                                    continue;
                                }
                            }
                        }
                        
                        NumberNode numberNode = new NumberNode(float.Parse(tokens[i].Value));
                        ParseType(numberNode, ref nodeCurrentlyIn);
                        break;
                    case TokenType.String:
                        StringNode stringNode = new StringNode(tokens[i].Value);
                        ParseType(stringNode, ref nodeCurrentlyIn);
                        break;
                    case TokenType.Operator:
                        if (i + 1 < tokens.Length)
                        {
                            //Unary parsing
                            if (tokens[i].Value == "not")
                            {
                                var nextToken = tokens[i + 1];
                                ParseUnaryOperator(nextToken, tokens[i],
                                    nextToken.TokenType != TokenType.Keyword &&
                                    nextToken.TokenType != TokenType.Operator &&
                                    nextToken.TokenType != TokenType.Bracket, ref nodeCurrentlyIn, ref dispatch);
                                continue;
                            } else if (tokens[i].Value == "-" && tokens[i - 1].TokenType != TokenType.Number) // - used as a unary operator e.g. '-10'
                            {
                                var nextToken = tokens[i + 1];
                                ParseUnaryOperator(nextToken, tokens[i],
                                    nextToken.TokenType != TokenType.Bracket && nextToken.TokenType != TokenType.Number, ref nodeCurrentlyIn, ref dispatch);
                                continue;
                            }
                            
                            //binary parsing
                            if (tokens[i + 1].TokenType == TokenType.Number)
                            {
                                NumberNode nextNum = new NumberNode(float.Parse(tokens[i + 1].Value));
                                ParseOperatorPrecedence(tokens[i], nextNum, ref nodeCurrentlyIn, ref i);
                            } else if (tokens[i + 1].TokenType == TokenType.Keyword)
                            {
                                if (tokens[i + 1].Value == "false" || tokens[i + 1].Value == "true")
                                {
                                    BooleanNode nextNum = new BooleanNode(bool.Parse(tokens[i + 1].Value));
                                    ParseOperatorPrecedence(tokens[i], nextNum, ref nodeCurrentlyIn, ref i);
                                }
                                else
                                    throw new InterpreterException(tokens[i + 1].Line, $"Incorrect keyword '{tokens[i + 1].Value}', expected 'true' or 'false'");
                            } else if (tokens[i + 1].TokenType == TokenType.String)
                            {
                                StringNode nextNum = new StringNode(tokens[i + 1].Value);
                                ParseOperatorPrecedence(tokens[i], nextNum, ref nodeCurrentlyIn, ref i);
                            }
                            else
                            {
                                if (tokens[i + 1].TokenType == TokenType.Bracket)
                                {
                                    if (tokens[i + 1].Value == ")")
                                        throw new InterpreterException(tokens[i + 1].Line, $"Expected a value after the operator '{tokens[i].Value}' near ')'");
                                    
                                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, nodeCurrentlyIn, new AstNode());
                                    nodeCurrentlyIn = newBinaryNode;
                                    continue;
                                }

                                if (tokens[i + 1].TokenType == TokenType.Operator && tokens[i + 1].Value == "not" ||
                                    tokens[i + 1].Value == "-")
                                {
                                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(tokens[i].Value, nodeCurrentlyIn, new AstNode());
                                    nodeCurrentlyIn = newBinaryNode;
                                    continue;
                                }
                                
                                if (!string.IsNullOrEmpty(tokens[i + 1].Value))
                                    throw new InterpreterException(tokens[i + 1].Line, $"Expected a value after the operator '{tokens[i].Value}' near token '{tokens[i + 1].Value}'");
                                else
                                    throw new InterpreterException(tokens[i + 1].Line, $"Expected a value after the operator '{tokens[i].Value}'");
                            }
                        }
                        else
                            throw new ArithmeticException($"Expected a value after the operator.");
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
                                    bebracketNode.InBracket = true;
                                
                                nodeCurrentlyIn = dispatch[^1].value;
                                
                                ParseBrackets(bracketNode, ref nodeCurrentlyIn, ref dispatch);
                                dispatch.RemoveAt(dispatch.Count - 1);

                            }
                            else
                                throw new InterpreterException(tokens[i + 1].Line, "Expected a ')' after the bracket in line.");
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
                                if ((tokens[i].Value == "false" || tokens[i].Value == "true") &&
                                    nodeCurrentlyIn is UnaryExpressionNode node)
                                {
                                    node.Operand =
                                        new BooleanNode(bool.Parse(tokens[i].Value));   
                                }
                                else
                                    throw new InterpreterException(tokens[i].Line, $"Incorrect keyword '{tokens[i].Value}'");
                                
                                nodeCurrentlyIn = previousDispatchData.value;

                                ParseType(node, ref nodeCurrentlyIn);
                                
                                dispatch.RemoveAt(dispatch.Count - 1);
                                continue;
                            }
                        }
                        
                        if (tokens[i].Value == "false" || tokens[i].Value == "true")
                        {
                            BooleanNode booleanNode = new BooleanNode(bool.Parse(tokens[i].Value));
                            ParseType(booleanNode, ref nodeCurrentlyIn);
                        }
                        break;
                    case TokenType.Terminator:
                        program.Statements.Add(nodeCurrentlyIn);
                        nodeCurrentlyIn = new AstNode();
                        break;
                }
            }

            if (dispatch.Count != 0)
            {
                throw new InterpreterException(dispatch[0].Token.Line, $"Code not ended at '{dispatch[0].Token.Value}'");
            }
            return program;
        }

        
        private static void ParseType(AstNode nodeType, ref AstNode nodeCurrentlyIn)
        {
            if (nodeCurrentlyIn is BinaryExpressionNode binaryNode)
            {
                binaryNode.Right = nodeType;
            }
            else
            {
                nodeCurrentlyIn = nodeType;
            }
        }

        private static void ParseUnaryOperator(Token nextToken, Token currentToken, bool check, ref AstNode nodeCurrentlyIn, ref List<DispatchData> dispatch)
        {
            if (check)
            {
                throw new InterpreterException(nextToken.Line, $"Invalid token for '{currentToken.Value}' operator");
            }
            
            UnaryExpressionNode node = new UnaryExpressionNode(currentToken.Value);
            dispatch.Add(new DispatchData(currentToken, nodeCurrentlyIn));
            nodeCurrentlyIn = node;
        }
        
        private static void ParseBrackets(AstNode bracketNode, ref AstNode nodeCurrentlyIn, ref List<DispatchData> dispatch)
        {
            if (nodeCurrentlyIn is BinaryExpressionNode brBinaryNode)
            {
                if (brBinaryNode.Left is BinaryExpressionNode leftBinaryNode)
                {
                    if (OperatorPrecedence(brBinaryNode.Expression) > OperatorPrecedence(leftBinaryNode.Expression) &&
                        !leftBinaryNode.InBracket)
                    {
                        BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(brBinaryNode.Expression,
                            leftBinaryNode.Right, bracketNode);

                        leftBinaryNode.Right = newBinaryNode;

                        nodeCurrentlyIn = leftBinaryNode;
                    }
                    else
                    {
                        BinaryExpressionNode newBinaryNode =
                            new BinaryExpressionNode(brBinaryNode.Expression, leftBinaryNode, bracketNode);
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
                Console.WriteLine("Oopsp");
                unaryNode.Operand = bracketNode;
                dispatch.RemoveAt(dispatch.Count - 1);
            }
            else if (nodeCurrentlyIn.GetType() == typeof(AstNode))
            {
                nodeCurrentlyIn = bracketNode;
            }
        }

        private static void ParseOperatorPrecedence(Token currentToken, AstNode NewNode, ref AstNode nodeCurrentlyIn, ref int i)
        {
            if (nodeCurrentlyIn is BinaryExpressionNode opBinaryNode)
            {
                if (OperatorPrecedence(opBinaryNode.Expression) < OperatorPrecedence(currentToken.Value) && !opBinaryNode.InBracket)
                {
                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(currentToken.Value, opBinaryNode.Right, NewNode);

                    opBinaryNode.Right = newBinaryNode;
                                    
                    nodeCurrentlyIn = opBinaryNode;
                                    
                }
                else
                {
                    BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(currentToken.Value, opBinaryNode, NewNode);
                    nodeCurrentlyIn = newBinaryNode;
                }
            }
            else
            {
                BinaryExpressionNode newBinaryNode = new BinaryExpressionNode(currentToken.Value, nodeCurrentlyIn, NewNode);

                nodeCurrentlyIn = newBinaryNode;
            }

            i++;
        }
        
        private static int OperatorPrecedence(string operatorToken)
        {
            switch (operatorToken)
            {
                case "+" or "-":
                    return 1;
                case "*" or "/":
                    return 2;
                case "or":
                    return -2;
                case "and":
                    return -1;
                default:
                    return 1;
            }
        }
    }
}
