using lab;

void Menu()
{
    Console.WriteLine("1. Scan");
    Console.WriteLine("2. Show FA menu");
    Console.Write(">>>>>> ");

    var option = Convert.ToInt32(Console.ReadLine());

    switch (option)
    {
        case 1: Scan();
            break;
        case 2: FaMenu();
            break;
    }
}

int GetFaMenuOption()
{
    Console.WriteLine();
    Console.WriteLine("1. All states");
    Console.WriteLine("2. Alphabet");
    Console.WriteLine("3. Transitions");
    Console.WriteLine("4. Initial state");
    Console.WriteLine("5. Final states");
    Console.WriteLine("6. Check you sequence");
    Console.Write(">>>>>> ");
    return Convert.ToInt32(Console.ReadLine());
}

void FaMenu()
{
    var finiteAutomaton = new FiniteAutomaton();
    int option;
    do
    {
        option = GetFaMenuOption();

        switch (option)
        {
            case 1: Console.WriteLine(finiteAutomaton.StatesToString());
                break;
            case 2: Console.WriteLine(finiteAutomaton.AlphabetToString());
                break;
            case 3: Console.WriteLine(finiteAutomaton.TransitionsToString());
                break;
            case 4: Console.WriteLine(finiteAutomaton.InitialStateToString());
                break;
            case 5: Console.WriteLine(finiteAutomaton.FinalStatesToString());
                break;
            case 6:
            {
                if (!finiteAutomaton.IsDeterministic())
                {
                    Console.WriteLine("Non deterministic!");
                    continue;
                }
                Console.Write("Enter your sequence >>>>>> ");
                var sequence = Console.ReadLine() ?? string.Empty;
                Console.WriteLine(finiteAutomaton.IsSequenceValid(sequence) ? "Valid" : "Invalid");
            };
                break;
        }
    } while (option != 0);
}

void Scan()
{
    try
    {
        var scanner = new Scanner();
        scanner.Scan();
    }
    catch (LexicalException le)
    {
        Console.WriteLine(le.Message + ". Line: " + le.Line);
    }
}

Menu();