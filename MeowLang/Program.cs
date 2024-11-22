using System.Diagnostics;
using MeowLang.Internal.Lexer;

namespace MeowLang;

class Program
{
    static void Main(string[] args)
    {
        Lexer.FindTokens("print(\"Hello world!\")", out Token[] tokenList);
        
        /*
        this returns 
        Identifier : print
        Punctuation : (
        String : "Hello world!"
        Punctuation : )
        
        i'll set up an ast soon
        */
        foreach (var token in tokenList)
        {
            Console.WriteLine($"{token.tokenType} : {token.value}");
        }
    }
}