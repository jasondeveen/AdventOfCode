namespace day5;

class Program
{
    static void Main(string[] args)
    {
        IEnumerable<string> lines = File.ReadAllLines(args[0]);

        int indexOfNumberLine = lines.ToList().FindIndex(l => l[1] == '1');

        List<string> crateLines = lines.Take(indexOfNumberLine).ToList();
        List<string> actionLines = lines.Skip(indexOfNumberLine + 2).ToList();

        int numberOfStacks = (crateLines[0].Length + 1)/4;

        Stack<char>[] stacks = new Stack<char>[numberOfStacks];

        for(int i = 0; i < numberOfStacks; i++){
            stacks[i] = GetStackContentFromLines(crateLines, i);
        }

        PrintStacks(stacks);

        foreach(string action in actionLines){
            PerformAction9001(action, stacks);
        }

        PrintStacks(stacks);

        foreach(Stack<char> stack in stacks){
            System.Console.Write(stack.First());
        }
        System.Console.WriteLine();
    }

    private static void PrintStacks(Stack<char>[] stacks)
    {
        int stacknr = 0;
        foreach(Stack<char> stack in stacks){
            System.Console.Write($"Stack {++stacknr}");
            foreach(char crate in stack){
                System.Console.Write(" " + crate);
            }
            System.Console.WriteLine();
        }
        System.Console.WriteLine("--------------------------------------");
    }

    private static void PerformAction9000(string actionLine, Stack<char>[] stacks)
    {
        string[] wordsOfActionLine = actionLine.Split(' ');
        int numberOfRepetitions = Convert.ToInt32(wordsOfActionLine[1]);
        int fromStack = Convert.ToInt32(wordsOfActionLine[3]) - 1; // stacknumber in text is 1-based, array of stacks is 0-based
        int toStack = Convert.ToInt32(wordsOfActionLine[5]) - 1;

        for(int i = 0; i < numberOfRepetitions; i++){
            char movedCrate = stacks[fromStack].Pop();
            stacks[toStack].Push(movedCrate);
        }
    }

    private static void PerformAction9001(string actionLine, Stack<char>[] stacks)
    {
        string[] wordsOfActionLine = actionLine.Split(' ');
        int numberOfCratesPickedUp = Convert.ToInt32(wordsOfActionLine[1]);
        int fromStack = Convert.ToInt32(wordsOfActionLine[3]) - 1; // stacknumber in text is 1-based, array of stacks is 0-based
        int toStack = Convert.ToInt32(wordsOfActionLine[5]) - 1;

        char[] cratesPickedUp = new char[numberOfCratesPickedUp];
        for(int i = 0; i < numberOfCratesPickedUp; i++){
            cratesPickedUp[i] = stacks[fromStack].Pop();
        }

        foreach(char crate in cratesPickedUp.Reverse())
            stacks[toStack].Push(crate);
    }

    private static Stack<char> GetStackContentFromLines(List<string> lines, int stackNumber)
    {
        List<char> cratesTopToBottom = new List<char>();
        int xCoordinate = (stackNumber * 4) + 1;
        Stack<char> crateStack = new Stack<char>();

        string currentLine;
        for(int i = lines.Count() - 1; i >= 0; i--){
            currentLine = lines[i];
            char crate = currentLine[xCoordinate];
            if(crate != ' ')
                crateStack.Push(crate);
        }

        return crateStack;
    }
}
