namespace lab_part_2;

public class Parser
{
    private readonly Grammar _grammar;
    private Dictionary<string, HashSet<string>> _firstSet;
    private Dictionary<string, HashSet<string>> _followSet;
    private readonly Dictionary<(string, string), (string, int)> _parseTable;
    private readonly List<List<string>> _productionsRhs;

    public Parser(Grammar grammar) {
        _grammar = grammar;
        _firstSet = new Dictionary<string, HashSet<string>>();
        _followSet = new Dictionary<string, HashSet<string>>();
        _parseTable = new Dictionary<(string, string), (string, int)>();
        _productionsRhs = new List<List<string>>();
        
        GenerateFirst();
        GenerateFollow();
        GenerateParseTable();
    }

    public string FirstToString() => _firstSet.Aggregate("============= First =============\n",
        (current, keyValuePair) => current + $"{keyValuePair.Key}: {keyValuePair.Value}\n");
    
    public string FollowToString() => _followSet.Aggregate("============= Follow =============\n",
        (current, keyValuePair) => current + $"{keyValuePair.Key}: {keyValuePair.Value}\n");

    public string ParseTableToString() => _parseTable.Aggregate("============= Parse Table =============\n",
        (current, keyValuePair) => current + $"{keyValuePair.Key} -> {keyValuePair.Value}\n");
    
    #region Private methods
    
    private void GenerateFirst() {
        //initialization
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
                
                var toAdd = new HashSet<string>(_followSet[nonTerminal]);

                foreach (var (k, v) in productionsWithNonTerminalInRhs)
                {
                    foreach (var productionList in v)
                    {
                        for (var indexOfNonTerminal = 0; indexOfNonTerminal < productionList.Count; ++indexOfNonTerminal)
                        {
                            if (productionList[indexOfNonTerminal].Equals(nonTerminal))
                            {
                                if (indexOfNonTerminal + 1 == productionList.Count)
                                {
                                    toAdd.UnionWith(_followSet[k]);
                                } 
                                else
                                {
                                    var followSymbol = productionList[indexOfNonTerminal + 1];
                                    if (_grammar.Terminals.Contains(followSymbol))
                                    {
                                        toAdd.Add(followSymbol);
                                    }
                                    else
                                    {
                                        foreach (var symbol in _firstSet[followSymbol])
                                        {
                                            toAdd.UnionWith(symbol.Equals("epsilon")
                                                ? _followSet[k]
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
    
    private void GenerateParseTable() {
        var rows = new List<string>();
        rows.AddRange(_grammar.NonTerminals);
        rows.AddRange(_grammar.Terminals);
        rows.Add("$");

        var columns = new List<string>();
        columns.AddRange(_grammar.Terminals);
        columns.Add("$");

        foreach (var row in rows)
            foreach (var col in columns)
                _parseTable[(row, col)] = ("err",-1);

        foreach (var col in columns)
            _parseTable[(col, col)] = ("pop",-1);

        _parseTable[("$", "$")] = ("acc",-1);
        
        foreach (var (k, v) in _grammar.ProductionSet.Productions())
        {
            var nonTerminal = k.First();
            foreach (var prod in v)
            {
                _productionsRhs.Add(!prod[0].Equals("epsilon") ? prod : new List<string> { "epsilon", nonTerminal });
            }
        }
        
        foreach (var (k, v) in _grammar.ProductionSet.Productions())
        {
            var key = k.First();
            
            foreach (var production in v)
            {
                var firstSymbol = production[0];

                if (_grammar.Terminals.Contains(firstSymbol))
                {
                    if (_parseTable[(key, firstSymbol)].Item1.Equals("err"))
                    {
                        _parseTable[(key, firstSymbol)] = (string.Join(" ", production), _productionsRhs.IndexOf(production)+1);
                    }
                    else
                    {
                        try
                        {
                            throw new Exception("CONFLICT: Pair "+key+","+firstSymbol);
                        } 
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                else if (_grammar.NonTerminals.Contains(firstSymbol))
                {
                    if (production.Count == 1)
                    {
                        foreach (var symbol in _firstSet[firstSymbol])
                        {
                            if (_parseTable[(key, symbol)].Item1.Equals("err"))
                            {
                                _parseTable[(key, symbol)] = (string.Join(" ", production),_productionsRhs.IndexOf(production)+1);
                            }
                            else
                            {
                                try
                                {
                                    throw new Exception("CONFLICT: Pair "+key+","+symbol);
                                } 
                                catch (Exception e) 
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                    else 
                    {
                        var i = 1;
                        var nextSymbol = production[1];
                        var firstSetForProduction = _firstSet[firstSymbol];

                        while (i < production.Count && _grammar.NonTerminals.Contains(nextSymbol)) {
                            var firstForNext = _firstSet[nextSymbol];
                            if (firstSetForProduction.Contains("epsilon")) {
                                firstSetForProduction.Remove("epsilon");
                                firstSetForProduction.UnionWith(firstForNext);
                            }

                            i++;
                            if (i < production.Count)
                                nextSymbol = production[i];
                        }

                        foreach (var symbol in firstSetForProduction)
                        {
                            var symbolToAdd = symbol.Equals("epsilon") ? "$" : symbol;
                            
                            if (_parseTable[(key, symbolToAdd)].Item1.Equals("err"))
                            {
                                _parseTable[(key, symbolToAdd)] = (string.Join(" ", production), _productionsRhs.IndexOf(production) + 1);
                            }
                            else
                            {
                                try
                                {
                                    throw new Exception("CONFLICT: Pair " + key + "," + symbolToAdd);
                                } 
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                } 
                else 
                {
                    var follow = _followSet[key];
                    foreach (var symbol in follow)
                    {
                        if (symbol.Equals("epsilon"))
                        {
                            if (_parseTable[(key, "$")].Item1.Equals("err"))
                            {
                                var prod = new List<string> { "epsilon", key };
                                _parseTable[(key, "$")] = ("epsilon", _productionsRhs.IndexOf(prod) + 1);
                            }
                            else
                            {
                                try
                                {
                                    throw new Exception("CONFLICT: Pair "+key+","+symbol);
                                } 
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                        else if (_parseTable[(key, symbol)].Item1.Equals("err"))
                        {
                            var prod = new List<string> { "epsilon", key };
                            _parseTable[(key, symbol)] = ("epsilon", _productionsRhs.IndexOf(prod) + 1);
                        }
                        else
                        {
                            try
                            {
                                throw new Exception("CONFLICT: Pair "+key+","+symbol);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
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