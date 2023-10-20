
namespace day12;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        (int,int) startCoor = FindCoordinateByValue(input, 'S');
        (int,int) endCoor = FindCoordinateByValue(input, 'E');

        input[startCoor.Item1] = input[startCoor.Item1].Replace('S', 'a');
        input[endCoor.Item1] = input[endCoor.Item1].Replace('E', 'z');

        TreeNode startNode = new TreeNode{
            Cost = 0,
            Column = startCoor.Item1,
            Row = startCoor.Item2
        };

        CheckMoves(startNode, input);

        
    }

    // 1. make collection of all nodes, and call it unvisited collection, give them all a cost of infinity
    // 2. remove startnode from unvisited collection, give it cost 0
    // 3. visit the 4 nodes around the visited nodes, i.e. the nodes up, down, left and right of this node. Only visit the reachable neighbouring nodes!
    //      Remove the nodes from the visited list after visiting each one as to avoid visiting a node twice in one iteration of step 3.
    //      Each visited node will get the value of the smallest costing neighbour + 1, as you have to take a step to get there (1 step = 1 cost)
    // 4. when endnode is visited, take the lowest cost path until startnode, this is the shortest path
    // maybe wait until all nodes are visited, since the shortest path is not neccessarily a straight line like the example on wiki...

    private static void PrintMoves(TreeNode node)
    {
        Console.WriteLine("Up: " + node.CostUp);
        Console.WriteLine("Down: " + node.CostDown);
        Console.WriteLine("Left: " + node.CostLeft);
        Console.WriteLine("Right: " + node.CostRight);
    }

    private static void CheckMoves(TreeNode node, string[] input)
    {
        node.CostUp = null;
        node.CostDown = null;
        node.CostLeft = null;
        node.CostRight = null;

        if(node.Row <= 0)
            node.CostUp = null;
        else
        {
            if(((byte)input[node.Row - 1][node.Column]) - 1 <= ((byte)input[node.Row][node.Column]))
                node.CostUp = int.MaxValue;
        }

        if(node.Row >= input.Count())
            node.CostDown = null;
        else
        {
            if(((byte)input[node.Row + 1][node.Column]) - 1 <= ((byte)input[node.Row][node.Column]))
                node.CostDown = int.MaxValue;
        }

        if(node.Column <= 0)
            node.CostLeft = null;
        else
        {
            if(((byte)input[node.Row][node.Column - 1]) - 1 <= ((byte)input[node.Row][node.Column]))
                node.CostLeft = int.MaxValue;
        }

        if(node.Column >= input[0].Count())
            node.CostRight = null;
        else
        {
            if(((byte)input[node.Row][node.Column + 1]) - 1 <= ((byte)input[node.Row][node.Column]))
                node.CostRight = int.MaxValue;
        }
    }

    private static (int, int) FindCoordinateByValue(string[] input, char value)
    {
        for(int i = 0; i < input.Count(); i++)
        {
            for(int j = 0; j < input[0].Count(); j++)
            {
                if(input[i][j] == value)
                    return (i,j);
            }
        }

        throw new Exception("AAAAAAAA");
    }
}

class TreeNode
{
    public int Cost;
    public int Column;
    public int Row;
    public int? CostUp;
    public int? CostDown;
    public int? CostLeft;
    public int? CostRight;
}
