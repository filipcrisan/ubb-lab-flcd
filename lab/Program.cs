using lab;

try
{
    var scanner = new Scanner();
    scanner.Scan();
}
catch (LexicalException le)
{
    Console.WriteLine(le.Message + ". Line: " + le.Line);
}