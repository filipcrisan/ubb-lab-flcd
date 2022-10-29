using lab;

var program = File.ReadAllText("../../../res/p1.in");
var tokens = File.ReadAllLines("../../../res/token.in").ToList();

var scanner  = new Scanner(program, tokens);

try
{
    var result = scanner.Scan();
    
    var pifEntries = result
        .Item1
        .Select(pifEntry => "Token: " + pifEntry.Token!.Value + ", Position: (" + pifEntry.StPosition.Item1 + ", " + pifEntry.StPosition.Item2 + ")")
        .ToList();
    File.WriteAllLines("../../../res/PIF.out", pifEntries);

    var identifiers = result
        .Item2
        .GetAllIdentifiers()
        .Select(symbolTableEntry => "Symbol: " + symbolTableEntry.Item1 + ", Position: (" +
                                    symbolTableEntry.Item2.Item1 + ", " + symbolTableEntry.Item2.Item2 + ")")
        .ToList();
    
    var integerConstants = result
        .Item2
        .GetAllIntegerConstants()
        .Select(symbolTableEntry => "Symbol: " + symbolTableEntry.Item1 + ", Position: (" +
                                    symbolTableEntry.Item2.Item1 + ", " + symbolTableEntry.Item2.Item2 + ")")
        .ToList();
    
    var stringConstants = result
        .Item2
        .GetAllStringConstants()
        .Select(symbolTableEntry => "Symbol: " + symbolTableEntry.Item1 + ", Position: (" +
                                    symbolTableEntry.Item2.Item1 + ", " + symbolTableEntry.Item2.Item2 + ")")
        .ToList();
    
    File.WriteAllLines("../../../res/ST.out", new[] { "IDS "});
    File.AppendAllLines("../../../res/ST.out", identifiers);
    File.AppendAllLines("../../../res/ST.out", new [] { "\nINTEGER CONSTANTS" });
    File.AppendAllLines("../../../res/ST.out", integerConstants);
    File.AppendAllLines("../../../res/ST.out", new [] { "\nSTRING CONSTANTS" });
    File.AppendAllLines("../../../res/ST.out", stringConstants);
}
catch (LexicalException le)
{
    Console.WriteLine(le.Message + ". Line: " + le.Line);
}