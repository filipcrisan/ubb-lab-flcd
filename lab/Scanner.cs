using System.Text.RegularExpressions;

namespace lab;

public class Scanner
{
    private readonly string _program;
    private readonly List<string> _tokens;
    private readonly SymbolTable _symbolTable = new();
    private readonly List<PifEntry> _pif = new();
    private int _charIndex = 0;
    private int _lineIndex = 1;
    
    public Scanner(string program, List<string> tokens)
    {
        _program = program;
        _tokens = tokens;
    }

    public (List<PifEntry>, SymbolTable) Scan()
    {
        while (_charIndex < _program.Length)
        {
            ParseNextToken();
        }
        
        return (_pif, _symbolTable);
    }
    
    #region Private methods

    private void ParseNextToken()
    {
        SkipWhitespace();
        SkipComment();
        
        if (_charIndex == _program.Length) return;

        if (TryParseStringConstant()) return;

        if (TryParseIntegerConstant()) return;

        if (TryParseReservedToken()) return;

        if (TryParseIdentifier()) return;

        throw new LexicalException($"Invalid token: {FindInvalidToken()}", _lineIndex);
    }

    private void SkipWhitespace()
    {
        while (_charIndex < _program.Length && char.IsWhiteSpace(_program[_charIndex]))
        {
            if (_program[_charIndex] == '\n')
            {
                _lineIndex++;
            }

            _charIndex++;
        }
    }

    private void SkipComment()
    {
        if (!_program[_charIndex..].StartsWith("//")) return;
        
        while (_charIndex < _program.Length && _program[_charIndex] != '\n')
        {
            _charIndex++;
        }
    }

    private bool TryParseStringConstant()
    {
        const string pattern = "^\"[a-zA-Z0-9_?!#*./%+=<>;)(}{ ]+\"";
        var regex = new Regex(pattern);
        
        var match = regex.Match(_program[_charIndex..]);
        if (!match.Success)
        {
            return false;
        }

        _charIndex += match.Length;

        var stPosition = _symbolTable.AddStringConstant(match.Value);
        var pifEntry = new PifEntry
        {
            Token = Token.Constant,
            StPosition = stPosition
        };
        _pif.Add(pifEntry);
        return true;
    }
    
    private bool TryParseIntegerConstant()
    {
        const string pattern = "^0|[+|-][1-9]([0-9])*|[1-9]([0-9])*|[+|-][1-9]([0-9])*\\.([0-9])*|[1-9]([0-9])*\\.([0-9])*";
        var regex = new Regex(pattern);
        
        var match = regex.Match(_program[_charIndex..]);
        if (!match.Success)
        {
            return false;
        }

        _charIndex += match.Length;

        var stPosition = _symbolTable.AddIntegerConstant(int.Parse(match.Value));
        var pifEntry = new PifEntry
        {
            Token = Token.Constant,
            StPosition = stPosition
        };
        _pif.Add(pifEntry);
        return true;
    }
    
    private bool TryParseReservedToken()
    {
        foreach (var token in _tokens)
        {
            if (!_program[_charIndex..].StartsWith(token)) continue;
            
            var entry = new PifEntry
            {
                Token = new Token(token),
                StPosition = (-1, -1)
            };
            _pif.Add(entry);
            _charIndex += token.Length;
            return true;
        }

        return false;
    }
    
    private bool TryParseIdentifier()
    {
        const string pattern = "^[a-zA-Z]([a-z|A-Z|0-9|_])*";
        var regex = new Regex(pattern);

        var match = regex.Match(_program[_charIndex..]);
        if (!match.Success)
        {
            return false;
        }

        _charIndex += match.Length;

        var stPosition = _symbolTable.AddIdentifier(match.Value);
        var pifEntry = new PifEntry
        {
            Token = Token.Identifier,
            StPosition = stPosition
        };
        _pif.Add(pifEntry);
        return true;
    }

    private string FindInvalidToken()
    {
        var i = _charIndex;
        while (i < _program.Length && !char.IsWhiteSpace(_program[i]))
        {
            i++;
        }

        return _program.Substring(_charIndex, i - _charIndex);
    }
    
    #endregion
}