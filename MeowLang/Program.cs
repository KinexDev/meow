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
        //foreach (var token in tokenList)
        //{
        //    Console.WriteLine($"{token.TokenType} : {token.Value}");
        //}

        while (true)
        {
            Console.WriteLine($"-------------INPUT-------------");
            string? Input = Console.ReadLine();
            Console.WriteLine($"-------------TOKENS-------------");
            Tokenizer.FindTokens(Input, out Token[] tokenList);  
            foreach (var token in tokenList)
            {
                Console.WriteLine($"{token.TokenType} : {token.Value}");
            }
            
            try
            {
                Console.WriteLine($"-------------OUTPUT-------------");
                var x = Parser.Parse(tokenList);
                Console.WriteLine(x.ToString());
                Console.WriteLine(Parser.Evaluate(x));
            }
            catch (Exception e)
            {
                if (e is InterpreterException interpreterException)
                {
                    Console.WriteLine(interpreterException.FullMessage);
                    continue;
                }
                
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}