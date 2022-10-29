namespace lab;

public class SymbolTable
{
    private readonly HashTable<string> _identifiers = new(10);
    private readonly HashTable<int> _integerConstants = new(10);
    private readonly HashTable<string> _stringConstants = new(10);

    public (int, int) AddIdentifier(string identifier)
    {
        return _identifiers.Add(identifier);
    }
    
    public (int, int) AddIntegerConstant(int integerConstant)
    {
        return _integerConstants.Add(integerConstant);
    }
    
    public (int, int) AddStringConstant(string stringConstant)
    {
        return _stringConstants.Add(stringConstant);
    }
}