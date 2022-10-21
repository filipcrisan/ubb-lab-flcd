namespace lab;

public class SymbolTable
{
    private readonly HashTable<string> ids;
    private readonly HashTable<int> integerConstants;
    private readonly HashTable<string> stringConstants;
}