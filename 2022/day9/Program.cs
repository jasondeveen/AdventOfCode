namespace day9;

partial class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        List<(int,int)> rope = new List<(int,int)>();
        for(int i = 0; i < int.Parse(args[1]); i++)
            rope.Add((0,0));
        
        List<(int,int)> visitedSpots = GetTailPath(input, rope);

        int numberOfVisitedSpots = visitedSpots.Distinct().Count();

        Console.WriteLine("Number of visited spots: " + numberOfVisitedSpots);
    }

    private static List<(int, int)> GetTailPath(string[] input, List<(int,int)> rope)
    {
        List<(int,int)> tailPath = new List<(int, int)>();

        foreach (string command in input)
        {
            for (int i = 0; i < int.Parse(command.Substring(2)); i++)
            {
                rope[0] = InterpretMove(command[0], rope[0]);

                for(int j = 1; j < rope.Count(); j++)
                {
                    if(ShouldMove(rope[j], rope[j-1]))
                        rope[j] = Move(rope[j], rope[j-1]);
                }

                tailPath.Add(rope[^1]);
                // System.Console.WriteLine(rope[^1]);
            }
        }

        return tailPath;
    }

    private static bool ShouldMove((int,int) follower, (int,int) leader)
    {
        if(Math.Abs(follower.Item1 - leader.Item1) > 1 || Math.Abs(follower.Item2 - leader.Item2) > 1)
            return true;

        return false;
    }

    private static (int, int) InterpretMove(char move, (int,int) pos)
    {
        switch(move)
        {
            case 'R':
                pos.Item2++;
                break;

            case 'L':
                pos.Item2--;
                break;
            
            case 'U':
                pos.Item1++;
                break;
            
            case 'D':
                pos.Item1--;
                break;
        }

        return pos;
    }

    private static (int,int) Move((int,int) follower, (int,int) leader)
    {
        // same row
        if(leader.Item1 == follower.Item1)
        {
            return (follower.Item1, follower.Item2 + (leader.Item2 - follower.Item2)/2);
        }
        // same column
        else if(leader.Item2 == follower.Item2)
        {
            return (follower.Item1 + (leader.Item1 - follower.Item1)/2, follower.Item2);
        }
        // diagonal movement
        else
        {
            (int,int) returnval = follower;

            if(leader.Item1 > follower.Item1)
                returnval = (returnval.Item1 + 1, returnval.Item2);
            else
                returnval = (returnval.Item1 - 1, returnval.Item2);

            if(leader.Item2 > follower.Item2)
                returnval = (returnval.Item1, returnval.Item2 + 1);
            else
                returnval = (returnval.Item1, returnval.Item2 - 1);

            return returnval;
        }
    }
}
