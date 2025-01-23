using MeowLang.Internal.Parser;
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
            Input += ";";
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
                foreach (var node in x.Statements)
                {
                    Console.WriteLine(Parser.Evaluate(node));
                }
            }
            catch (Exception e)
            {
                if (e is InterpreterException interpreterException)
                {
                    Console.WriteLine(interpreterException.FullMessage);
                    continue;
                }
                
                Console.WriteLine(e.Message);
            }
        }
    }
}