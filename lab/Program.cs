using lab;

var program = File.ReadAllText("../../../res/p1.in");
var tokens = File.ReadAllLines("../../../res/token.in").ToList();

var scanner  = new Scanner(program, tokens);

try
{
    scanner.Scan();
    
}
catch (LexicalException le)
{
    Console.WriteLine(le.Message + ". Line: " + le.Line);
    return;
}

// write in files