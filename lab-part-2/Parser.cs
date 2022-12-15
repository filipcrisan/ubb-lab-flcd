namespace lab_part_2;

public class Parser
{
    private readonly Grammar _grammar;
    private Dictionary<string, HashSet<string>> _firstSet;
    private Dictionary<string, HashSet<string>> _followSet;
    
    public Parser(Grammar grammar) {
        _grammar = grammar;
        _firstSet = new Dictionary<string, HashSet<string>>();
        _followSet = new Dictionary<string, HashSet<string>>();
        
        GenerateFirst();
        GenerateFollow();
    }

    public string FirstToString() => _firstSet.Aggregate("============= First =============\n",
        (current, keyValuePair) => current + $"{keyValuePair.Key}: [{string.Join(", ", keyValuePair.Value)}]\n");
    
    public string FollowToString() => _followSet.Aggregate("============= Follow =============\n",
        (current, keyValuePair) => current + $"{keyValuePair.Key}: [{string.Join(", ", keyValuePair.Value)}]\n");
    
    #region Private methods
    
    private void GenerateFirst() {
        // initialization
        foreach (var nonTerminal in _grammar.NonTerminals) {
            _firstSet.Add(nonTerminal, new HashSet<string>());
            var productionForNonTerminal = _grammar.ProductionSet.GetByNonTerminal(new List<string> { nonTerminal });
            foreach (var production in productionForNonTerminal) {
                if (_grammar.Terminals.Contains(production[0]) || production[0].Equals("epsilon"))
                    _firstSet[nonTerminal].Add(production[0]);
            }
        }

        // rest of iterations
        var isChanged = true;
        while (isChanged)
        {
            isChanged = false;
            var newColumn = new Dictionary<string, HashSet<string>>();

            foreach (var nonTerminal in _grammar.NonTerminals)
            {
                var productionForNonTerminal = _grammar.ProductionSet.GetByNonTerminal(new List<string> { nonTerminal });
                var toAdd = new HashSet<string>(_firstSet[nonTerminal]);
                foreach (var production in productionForNonTerminal)
                {
                    var rhsNonTerminals = new List<string>();
                    var rhsTerminal = string.Empty;
                    foreach (var symbol in production)
                    {
                        if (_grammar.NonTerminals.Contains(symbol))
                        {
                            rhsNonTerminals.Add(symbol);
                        }
                        else 
                        {
                            rhsTerminal = symbol;
                            break;
                        }
                    }
                    
                    toAdd.UnionWith(concatenationOfSizeOne(rhsNonTerminals, rhsTerminal));
                }
                
                if (!AreSetsEqual(toAdd, _firstSet[nonTerminal]))
                {
                    isChanged = true;
                }
                newColumn.Add(nonTerminal, toAdd);
            }
            _firstSet = newColumn;
        }
    }
    
    private void GenerateFollow() {
        // initialization
        foreach (var nonTerminal in _grammar.NonTerminals) {
            _followSet.Add(nonTerminal, new HashSet<string>());
        }
        _followSet[_grammar.StartingSymbol].Add("epsilon");

        // rest of iterations
        var isChanged = true;
        while (isChanged)
        {
            isChanged = false;
            var newColumn = new Dictionary<string, HashSet<string>>();

            foreach (var nonTerminal in _grammar.NonTerminals)
            {
                newColumn.Add(nonTerminal, new HashSet<string>());
                
                var productionsWithNonTerminalInRhs = _grammar.ProductionSet.ProductionsByNonTerminalInRhs(nonTerminal);
                
                var toAdd = new HashSet<string>();
                toAdd.UnionWith(_followSet[nonTerminal]);

                foreach (var (k, v) in productionsWithNonTerminalInRhs)
                {
                    foreach (var productionList in v)
                    {
                        for (var indexOfNonTerminal = 0; indexOfNonTerminal < productionList.Count; ++indexOfNonTerminal)
                        {
                            if (productionList[indexOfNonTerminal].Equals(nonTerminal))
                            {
                                // when there is a non-terminal X on the last position, follow set of X will be
                                // equivalent to follow set of LHS non-terminal
                                if (indexOfNonTerminal + 1 == productionList.Count)
                                {
                                    toAdd.UnionWith(_followSet[k]);
                                }
                                else
                                {
                                    var followSymbol = productionList[indexOfNonTerminal + 1];
                                    
                                    // non-terminal is not on the last position and is followed by a terminal
                                    if (_grammar.Terminals.Contains(followSymbol))
                                    {
                                        toAdd.Add(followSymbol);
                                    }
                                    // non-terminal is not on the last position and is followed by a non-terminal
                                    // case 1: epsilon belongs to first(non-terminal)
                                    // case 2: otherwise
                                    else
                                    {
                                        foreach (var symbol in _firstSet[followSymbol])
                                        {
                                            // firstSet(followSymbol) - epsilon U followSet(k)
                                            var followSetCopy = new HashSet<string>();
                                            followSetCopy.UnionWith(_followSet[k]);
                                            followSetCopy.UnionWith(_firstSet[followSymbol]);
                                            followSetCopy.Remove("epsilon");
                                            
                                            toAdd.UnionWith(symbol.Equals("epsilon")
                                                ? followSetCopy
                                                : _firstSet[followSymbol]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (!AreSetsEqual(toAdd, _followSet[nonTerminal]))
                {
                    isChanged = true;
                }
                newColumn[nonTerminal] = toAdd;
            }
            _followSet = newColumn;
        }
    }
    
    private HashSet<string> concatenationOfSizeOne(List<string> nonTerminals, string terminal) {
        if (nonTerminals.Count == 0)
        {
            return new HashSet<string>();
        }
        
        if (nonTerminals.Count == 1) {
            return _firstSet[nonTerminals.First()];
        }

        var concatenation = new HashSet<string>();
        var step = 0;
        var allEpsilon = true;

        nonTerminals.ForEach(x =>
        {
            if (!_firstSet[x].Contains("epsilon")) {
                allEpsilon = false;
            }
        });
        
        if (allEpsilon) {
            concatenation.Add(string.IsNullOrEmpty(terminal) ? "epsilon" : terminal);
        }

        while (step < nonTerminals.Count) {
            var thereIsOneEpsilon = false;
            foreach (var s in _firstSet[nonTerminals[step]])
            {
                if (s.Equals("epsilon"))
                    thereIsOneEpsilon = true;
                else
                    concatenation.Add(s);
            }

            if (thereIsOneEpsilon)
                step++;
            else
                break;
        }
        return concatenation;
    }

    private static bool AreSetsEqual<T>(IReadOnlyCollection<T> a, IReadOnlySet<T> b)
        => a.Count == b.Count && a.All(b.Contains);

    #endregion
}