namespace lab;

public class SymbolTable
{
    private readonly HashTable<string> _identifiers = new(10);
    private readonly HashTable<int> _integerConstants = new(10);
    private readonly HashTable<string> _stringConstants = new(10);

    public (int, int) AddIdentifier(string identifier) => _identifiers.Add(identifier);
    
    public IEnumerable<(string, (int, int))> GetAllIdentifiers() => _identifiers.GetAll();
    
    public (int, int) AddIntegerConstant(int integerConstant) => _integerConstants.Add(integerConstant);

    public IEnumerable<(int, (int, int))> GetAllIntegerConstants() => _integerConstants.GetAll();
    
    public (int, int) AddStringConstant(string stringConstant) => _stringConstants.Add(stringConstant);
    
    public IEnumerable<(string, (int, int))> GetAllStringConstants() => _stringConstants.GetAll();
}