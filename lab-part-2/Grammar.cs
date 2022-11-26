namespace lab_part_2;

public class Grammar
{
    public List<string> NonTerminals { get; }

    public List<string> Terminals { get; }

    public string StartingSymbol { get; }
    
    public ProductionSet ProductionSet { get; }

    public Grammar(string filePath)
    {
        var lines = File.ReadAllLines(filePath).ToList();
        
        NonTerminals = lines[0].Split(" ").ToList();
        Terminals = lines[1].Split(" ").ToList();
        StartingSymbol = lines[2].Trim();
        
        ProductionSet = new ProductionSet(lines.Skip(3).ToList());
    }
    
    public bool IsCfg()
    {
        if (!NonTerminals.Contains(StartingSymbol))
        {
            return false;
        }
        
        var startingSymbolKey = ProductionSet.GetKey(new List<string> { StartingSymbol });
        if (startingSymbolKey is null)
        {
            return false;
        }

        if (ProductionSet.Productions().Keys.Any(x => x.Count != 1))
        {
            return false;
        }

        if (ProductionSet.Productions().Values.SelectMany(valuesOfKey => valuesOfKey).Any(rhs => rhs.Any(x => !NonTerminals.Contains(x) && !Terminals.Contains(x) && x != "epsilon")))
        {
            return false;
        }

        return true;
    }
}