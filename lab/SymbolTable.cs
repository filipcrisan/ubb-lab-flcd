namespace lab;

public class SymbolTable
{
    private readonly HashTable<string> _identifiers = new HashTable<string>(10);
    private readonly HashTable<int> _integerConstants = new HashTable<int>(10);
    private readonly HashTable<string> _stringConstants = new HashTable<string>(10);
}