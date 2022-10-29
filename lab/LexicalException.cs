namespace lab;

public class LexicalException : Exception
{
    public int Line { get; }
    
    public LexicalException(string message, int line) : base(message)
    {
        Line = line;
    }
}