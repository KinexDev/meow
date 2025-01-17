namespace MeowLang.Internal.Parser;

public class InterpreterException(int line, string message) : Exception
{
    public int Line { get; } = line;
    public string Message { get; } = message;
    public string DecoratedMessage => $"Line {Line}: {Message}";
}