using System.Diagnostics;
using MeowLang.Internal.Parser;
using MeowLang.Internal.Parser.AST;
using MeowLang.Internal.Tokenizer;

namespace MeowLang;

class Program
{
    static void Main(string[] args)
    {
        //string? filePath = Directory.GetCurrentDirectory() + "/Test.meow";
        
        //Tokenizer.FindTokens(File.ReadAllText(filePath), out Token[] tokenList);

        try
        {
            AstNode node = new BinaryExpressionNode("*",
                new BinaryExpressionNode("+", new LiteralNode(3), new LiteralNode(5)),
                new LiteralNode(2)
            );
            
            Console.WriteLine(Parser.Evaluate(node));
            
            //var x = Parser.Parse(tokenList);
            //Console.WriteLine(Parser.Evaluate(x));
        }
        catch (InterpreterException e)
        {
            Console.WriteLine(e.DecoratedMessage);
            throw;
        }
        
        /*
        this returns 
        Identifier : print
        Punctuation : (
        String : "Hello world!"
        Punctuation : )
        
        i'll set up an ast soon
        */
        
        //foreach (var token in tokenList)
        //{
        //    Console.WriteLine($"{token.TokenType} : {token.Value}");
        //}
    }
}