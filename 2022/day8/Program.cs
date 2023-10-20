

namespace day8;

class Program
{
    static void Main(string[] args)
    {
        // string[] arr = {"12345",
        //                 "abcde",
        //                 "vwxyz"};
        // string sub = arr[1..][0];

        // foreach(char i in sub)
        //     System.Console.WriteLine(i);

        // return;

        string[] input = File.ReadAllLines(args[0]);

        int numberOfEdgeTrees  = (input.Count() * input[0].Count()) - ((input.Count()-2) * (input[0].Count() - 2));

        int numberOfInnerTrees = (input.Count() * input[0].Count()) - numberOfEdgeTrees;

        int numberOfVisibleInnertrees = 0;

        int highestScenicScoreTreeX = 0;
        int highestScenicScoreTreeY = 0;
        int highestScenicScore = 0;
        int currentScenicScore = 0;

        for(int i = 1; i < input.Count() - 1; i++)
        {
            for(int j = 1; j < input[i].Count() - 1; j++)
            {
                if(IsVisible(i, j, input))
                    numberOfVisibleInnertrees++;

                currentScenicScore = GetScenicScore(i, j, input);
                if(currentScenicScore > highestScenicScore)
                {
                    highestScenicScoreTreeX = i;
                    highestScenicScoreTreeY = j;
                    highestScenicScore = currentScenicScore;
                }
            }
        }

        System.Console.WriteLine("Number of trees on the edge: " + numberOfEdgeTrees);
        System.Console.WriteLine("Number of inner trees visible: " + numberOfVisibleInnertrees);
        System.Console.WriteLine("Total number of visible trees: " + (numberOfEdgeTrees + numberOfVisibleInnertrees));

        System.Console.WriteLine($"Most scenic tree is found at [{highestScenicScoreTreeX},{highestScenicScoreTreeY}], score = {highestScenicScore}");
    }

    private static bool IsVisible(int i, int j, string[] input)
    {
        List<int> treesLeft = input[i][..j]
                            .Select(ch => int.Parse(ch.ToString()))
                            .OrderByDescending(i => i)
                            .ToList();

        List<int> treesRight = input[i][(j+1)..]
                            .Select(ch => int.Parse(ch.ToString()))
                            .OrderByDescending(i => i)
                            .ToList();

        List<int> treesUp = GetColumn(input[..i], j)
                            .Select(ch => int.Parse(ch.ToString()))
                            .OrderByDescending(i => i)
                            .ToList();

        List<int> treesDown = GetColumn(input[(i+1)..], j)
                            .Select(ch => int.Parse(ch.ToString()))
                            .OrderByDescending(i => i)
                            .ToList();

        int me = int.Parse(input[i][j].ToString());

        return treesLeft[0] < me || treesRight[0] < me || treesUp[0] < me || treesDown[0] < me;
    }

    private static string GetColumn(string[] rows, int column)
    {
        string result = "";

        foreach(string row in rows)
        {
            result += row[column];
        }

        return result;
    }

    private static int GetScenicScore(int value, string column)
    {
        int result = column.Select(ch => int.Parse(ch.ToString()))
                    .ToList()
                    .FindIndex(i => i >= value);

        if(result == -1) 
            return column.Count();

        return result + 1;
    }

    private static int GetScenicScore(int value, IEnumerable<char> column)
    {
        string stringcolumn = "";

        foreach(char c in column)
            stringcolumn += c;

        return GetScenicScore(value, stringcolumn);
    }

    private static int GetScenicScore(int i, int j, string[] input)
    {
        int me = int.Parse(input[i][j].ToString());

        int up = GetScenicScore(me, GetColumn(input[..i], j).Reverse());
        int down = GetScenicScore(me, GetColumn(input[(i+1)..], j));
        int left = GetScenicScore(me, input[i][..j].Reverse());
        int right = GetScenicScore(me, input[i][(j+1)..]);

        return up * down * left * right;
    }
}
