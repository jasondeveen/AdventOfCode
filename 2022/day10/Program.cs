
namespace day10;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        Queue<(int,int?)> queuedTasks = new();   // (timeSpent, value)

        char[,] CRT = new char[6,40];

        int xRegister = 1;
        int result = 0;
        int currentCycle = 0;
        int delayTimer = 0;

        int inputCount = input.Count();
        while (currentCycle < inputCount || queuedTasks.Count() != 0 || currentCycle < 220)
        {
            if(currentCycle < inputCount && input[currentCycle].StartsWith("addx"))
            {    
                int value = int.Parse(input[currentCycle][5..]);
                queuedTasks.Enqueue((2, value));
            }
            else if(currentCycle < inputCount && input[currentCycle].StartsWith("noop"))
            {
                queuedTasks.Enqueue((1, null));
            }

            currentCycle++;

            if((currentCycle - 20) % 40 == 0 && currentCycle != 0 && currentCycle <= 220)
                result += currentCycle * xRegister;

            System.Console.WriteLine($"Current Cycle = {currentCycle}, X = {xRegister}");

            CRT[(currentCycle - 1) / 40, (currentCycle - 1) % 40] = IsInRange(currentCycle - 1, xRegister) ? '#' : ' ';


            if (delayTimer == 0)
            {
                delayTimer = queuedTasks.Peek().Item1;
                if (delayTimer == 1) queuedTasks.Dequeue();
            }
            else if (queuedTasks.Count > 0)
                xRegister += (queuedTasks.Dequeue().Item2 ?? 0);

            delayTimer--;
        }

        System.Console.WriteLine($"\n\nSignal strength = {result}\n");

        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j< 40; j++)
            {
                System.Console.Write(CRT[i, j]);
            }
            System.Console.WriteLine();
        }
    }

    private static bool IsInRange(int numberToCheck, int xRegister)
    {
        if((numberToCheck % 40) >= xRegister - 1 && (numberToCheck % 40) <= xRegister + 1)
            return true;

        return false;
    }
}
