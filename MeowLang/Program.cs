﻿using System.Diagnostics;
using MeowLang.Internal.Tokenizer;

namespace MeowLang;

class Program
{
    static void Main(string[] args)
    {
        string? filePath = Directory.GetCurrentDirectory() + "/Test.meow";
        
        Tokenizer.FindTokens(File.ReadAllText(filePath), out Token[] tokenList);
        
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