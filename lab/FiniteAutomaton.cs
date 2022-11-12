namespace lab;

public class FiniteAutomaton
{
    private readonly HashSet<string> _states;
    private readonly HashSet<string> _alphabet;
    private readonly string _initialState;
    private readonly HashSet<string> _finalStates;
    private readonly Dictionary<(string, string), HashSet<string>> _transitions;

    public FiniteAutomaton(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        _states = new HashSet<string>(lines[0].Split(" "));
        _alphabet = new HashSet<string>(lines[1].Split(" "));
        _initialState = lines[2];
        _finalStates = new HashSet<string>(lines[3].Split(" "));

        _transitions = new Dictionary<(string, string), HashSet<string>>();
        for (var i = 4; i < lines.Length; i++)
        {
            var source = lines[i].Split(" ")[0];
            var edge = lines[i].Split(" ")[1];
            var target = lines[i].Split(" ")[2];
            
            if (!_states.Contains(source) || !_states.Contains(target) || !_alphabet.Contains(edge)) continue;

            var key = (source, edge);
            if (!_transitions.ContainsKey(key))
            {
                var targetStates = new HashSet<string> { target };
                _transitions.Add(key, targetStates);
            }
            else
            {
                _transitions.GetValueOrDefault(key)!.Add(target);
            }
        }
    }

    public string StatesToString() => $"States: {string.Join(", ", _states)}";

    public string AlphabetToString() => $"Alphabet: {string.Join(", ", _alphabet)}";

    public string InitialStateToString() => $"Initial state: {_initialState}";

    public string FinalStatesToString() => $"Final states: {string.Join(", ", _finalStates)}";

    public string TransitionsToString()
    {
        var result = "Transitions:\n";

        foreach (var ((source, edge), targets) in _transitions)
        {
            result += $"Source: {source}, edge: {edge}, targets: {string.Join(", ", targets)}\n";
        }
        
        return result;
    }

    public bool IsDeterministic()
    {
        foreach (var (_, targets) in _transitions)
        {
            if (targets.Count > 1)
            {
                return false;
            } 
        }

        return true;
    }

    public bool IsSequenceValid(string sequence)
    {
        if (sequence.Length == 0)
        {
            return _finalStates.Contains(_initialState);
        }

        var currentState = _initialState;
        foreach (var key in sequence.Select(character => (currentState, character.ToString())))
        {
            if (!_transitions.ContainsKey(key)) return false;

            currentState = _transitions.GetValueOrDefault(key)!.First();
        }

        return _finalStates.Contains(currentState);
    }
    
    public string? GetNextToken(string program)
    {
        if (program.Length == 0)
        {
            return null;
        }

        var result = string.Empty;

        var currentState = _initialState;
        foreach (var key in program.Select(character => (currentState, character.ToString())))
        {
            if (!_transitions.ContainsKey(key)) break;

            currentState = _transitions.GetValueOrDefault(key)!.First();
            result += key.Item2;
        }

        return !_finalStates.Contains(currentState) ? null : result;
    }
}