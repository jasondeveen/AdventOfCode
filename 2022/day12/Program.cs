
using Microsoft.VisualBasic;

namespace day12;

class Program
{
    // 1. Calculate which steps are possible -> these steps are stored in the booleans Up, Down, Left, Right of the node
    // 2. Put every node in the Unvisited set, and assign value infinity to their cost
    // 3. Assign 0 to the cost of the starting node, and remove it from the Unvisited set, and put it in the Visited set
    // 4. For every node N1 in the Visited set: 
    //      For every node reachable from this node N2, which is in the Unvisited set, assign the cost value of the node N1 + 1 to the cost of node N2, and remove N2 from the unvisited set and add it to visited
    // 5. Repeat 4. until all node are visited
    // 6. Backtrack from the ending node, taking only steps that allow you to go in the opposite directions, to the starting node. The path followed is the shortest path between S and E

    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        (int, int) startCoor = FindCoordinateByValue(input, 'S');
        (int, int) endCoor = FindCoordinateByValue(input, 'E');

        input[startCoor.Item1] = input[startCoor.Item1].Replace('S', 'a');
        input[endCoor.Item1] = input[endCoor.Item1].Replace('E', 'z');

        Node[,] graph = new Node[input.Count(), input[0].Count()];
        for (int i = 0; i < input.Count(); i++)
        {
            for (int j = 0; j < input[0].Count(); j++)
            {
                graph[i,j] = new Node
                {
                    Value = input[i][j],
                    Cost = 1000000, // 1m ~= infinity
                    Column = j,
                    Row = i
                };
            }
        }

        Node startNode = graph[startCoor.Item1, startCoor.Item2];
        Node endNode = graph[endCoor.Item1, endCoor.Item2];

        foreach(Node n in graph)
        {
            GenerateVertices(n, graph);
        }

        PrintGraph(graph);

        System.Console.ReadKey();
    }

    private static void PrintGraph(Node[,] graph)
    {
        // geprinte array is inverted...?
        for(int i = 0; i < graph.GetLength(0); i++)
        {
            for(int j = 0; j < graph.GetLength(1); j++)
            {
                // left
                Console.SetCursorPosition(i*4 + 1, j*4 + 4);
                System.Console.Write(graph[i,j].Left ? "<" : " ");
                System.Console.Write("- ");
                
                // value
                System.Console.Write(graph[i, j].Value); 
                
                // right
                System.Console.Write(" -");
                if(graph[i,j].Right) System.Console.Write(">");

                // up
                Console.SetCursorPosition(i*4 + 4, j*4 + 1);
                if(graph[i,j].Up) System.Console.Write("^");
                Console.SetCursorPosition(i*4 + 4, j*4 + 2);
                System.Console.Write("|");

                // down
                Console.SetCursorPosition(i*4 + 4, j*4 + 6);
                System.Console.Write("|");
                Console.SetCursorPosition(i*4 + 4, j*4 + 7);
                if(graph[i,j].Down) System.Console.Write("v");
            }
        }
    }


    private static void GenerateVertices(Node node, Node[,] graph)
    {
        node.Up = false;
        node.Down = false;
        node.Left = false;
        node.Right = false;

        if (node.Row <= 0)
            node.Up = false;
        else
        {
            if (((byte)graph[node.Row - 1, node.Column].Value) - 1 <= ((byte)graph[node.Row,node.Column].Value))
                node.Up = true;
        }

        if (node.Row >= graph.GetLength(0) - 1)
            node.Down = false;
        else
        {
            if (((byte)graph[node.Row + 1, node.Column].Value) - 1 <= ((byte)graph[node.Row, node.Column].Value))
                node.Down = true;
        }

        if (node.Column <= 0)
            node.Left = false;
        else
        {
            if (((byte)graph[node.Row, node.Column - 1].Value) - 1 <= ((byte)graph[node.Row, node.Column].Value))
                node.Left = true;
        }

        if (node.Column >= graph.GetLength(1) - 1)
            node.Right = false;
        else
        {
            if (((byte)graph[node.Row, node.Column + 1].Value) - 1 <= ((byte)graph[node.Row, node.Column].Value))
                node.Right = true;
        }
    }

    private static (int, int) FindCoordinateByValue(string[] input, char value)
    {
        for (int i = 0; i < input.Count(); i++)
        {
            for (int j = 0; j < input[0].Count(); j++)
            {
                if (input[i][j] == value)
                    return (i, j);
            }
        }

        throw new Exception("AAAAAAAA");
    }
}

class Node
{
    public char Value;
    public int Cost;
    public int Column;
    public int Row;
    public bool Up;
    public bool Down;
    public bool Left;
    public bool Right;
}
