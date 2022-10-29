namespace lab;

public class Token
{
    public string Value { get; }
    
    public static readonly Token Identifier = new("IDENTIFIER");
    public static readonly Token Constant = new("CONSTANT");
    
    public Token(string value)
    {
        Value = value;
    }
}