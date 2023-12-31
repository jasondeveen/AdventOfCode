﻿using System.Runtime.Intrinsics.Arm;

namespace day12;

class Program
{
    // 1. Calculate which steps are possible -> these steps are stored in the booleans Up, Down, Left, Right of the node
    // 2. Put every node in the Unvisited set, and assign value infinity to their cost
    // 3. Assign 0 to the cost of the starting node, and remove it from the Unvisited set, and put it in the Visited set
    // 4. For every node N1 in the Visited set: 
    //      For every node reachable from this node N2, which is in the Unvisited set, assign the cost value of the node N1 + 1 to the cost of node N2, 
    //          and remove N2 from the unvisited set and add it to visited
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

        // 1
        foreach(Node n in graph)
        {
            GenerateVertices(n, graph);
        }

        // PrintGraph(graph);
        // System.Console.WriteLine();

        // 5
        // ApplyAlgo(graph, startNode, GetReachableNeighbours, new List<Node>{endNode});
        // PrintCostGraph(graph);
        // System.Console.WriteLine("Cost to get to endnode from startnode: " + endNode.Cost);


        // Find shortest path between EndNode and any node with elevation a
        List<Node> aElevationNodes = graph.Cast<Node>()
                    .Where(node => node.Value == 'a')
                    .ToList();
                    
        ApplyAlgo(graph, endNode, GetNeighborsThatCanReachNode, aElevationNodes);
        // PrintCostGraph(graph);
        Node closestNodeToEndWithAElevation = aElevationNodes.OrderBy(n => n.Cost).First();
        System.Console.WriteLine("Cost to get from endNode to a node with elevation a: " + closestNodeToEndWithAElevation.Cost);

        Console.ReadLine();
    }

    private static void ApplyAlgo(Node[,] graph, Node startNode, Func<Node, Node[,], List<Node>> moveFinder, List<Node> nodesToFind)
    {
        // 2
        List<Node> unvisited = graph.Cast<Node>().ToList();

        // 3
        List<Node> visited = new();
        List<Node> toVisit = new();
        startNode.Cost = 0;
        unvisited.Remove(startNode);
        toVisit.Add(startNode);

        int cost = 1;

        while (unvisited.Count > 0 && toVisit.Count != 0 && !visited.Intersect(nodesToFind).Any())
        {
            List<Node> neighborsMet = new();
            // 4
            foreach (Node reachable in GetReachableNodesFromList(toVisit, graph, moveFinder).Distinct().Except(visited))
            {
                if (reachable.Cost > cost)
                    reachable.Cost = cost;

                unvisited.Remove(reachable);
                neighborsMet.Add(reachable);
            }
            cost++;
            visited.AddRange(toVisit);
            toVisit.Clear();
            toVisit.AddRange(neighborsMet);
        }
    }

    private static List<Node> GetReachableNodesFromList(List<Node> nodes, Node[,] graph, Func<Node, Node[,], List<Node>> moveFinder)
    {
        List<Node> returnValue = new();
        foreach(Node node in nodes)
        {
            returnValue.AddRange(moveFinder(node, graph));
        }

        return returnValue;
    }

    #region moveFinders

    private static List<Node> GetNeighborsThatCanReachNode(Node node, Node[,] graph)
    {
        List<Node> returnValue = new();

        // node above me
        if(node.Row > 0 && graph[node.Row - 1, node.Column].Down)
            returnValue.Add(graph[node.Row - 1, node.Column]);

        // node below me
        if(node.Row < graph.GetLength(0) - 1 && graph[node.Row + 1, node.Column].Up)
            returnValue.Add(graph[node.Row + 1, node.Column]);

        // node left of me
        if(node.Column > 0 && graph[node.Row, node.Column - 1].Right)
            returnValue.Add(graph[node.Row, node.Column - 1]);
        
        // node right of me
        if(node.Column < graph.GetLength(1) - 1 && graph[node.Row, node.Column + 1].Left)
            returnValue.Add(graph[node.Row, node.Column + 1]);

        return returnValue;
    }

    private static List<Node> GetReachableNeighbours(Node node, Node[,] graph)
    {
        List<Node> returnValue = new();

        if(node.Up)
            returnValue.Add(graph[node.Row - 1, node.Column]);

        if(node.Down)
            returnValue.Add(graph[node.Row + 1, node.Column]);

        if (node.Left)
            returnValue.Add(graph[node.Row, node.Column - 1]);

        if (node.Right)
            returnValue.Add(graph[node.Row, node.Column + 1]);

        return returnValue;
    }

    #endregion

    #region Printing
    private static void PrintGraph(Node[,] graph)
    {
        int graphHeight = graph.GetLength(0);
        int graphWidth = graph.GetLength(1);

        Console.SetWindowSize(graphWidth * 6, graphHeight * 6);

        for (int i = 0; i < graphHeight; i++)
        {
            for(int j = 0; j < graphWidth; j++)
            {
                // left
                Console.SetCursorPosition(j*4 + 1, i*4 + 4);
                System.Console.Write(graph[i,j].Left ? "<" : " ");
                System.Console.Write("-");
                
                // value
                Console.SetCursorPosition(j*4 + 4, i*4 + 4);
                Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Write(graph[i, j].Value); 
                Console.ForegroundColor = ConsoleColor.White;
                
                // right
                Console.SetCursorPosition(j*4 + 6, i*4 + 4);
                System.Console.Write("-");
                if(graph[i,j].Right) System.Console.Write(">");

                // up
                Console.SetCursorPosition(j*4 + 4, i*4 + 1);
                if(graph[i,j].Up) System.Console.Write("^");
                Console.SetCursorPosition(j*4 + 4, i*4 + 2);
                System.Console.Write("|");

                // down
                Console.SetCursorPosition(j*4 + 4, i*4 + 6);
                System.Console.Write("|");
                Console.SetCursorPosition(j*4 + 4, i*4 + 7);
                if(graph[i,j].Down) System.Console.Write("v");
            }
        }
    }

    private static void PrintCostGraph(Node[,] graph)
    {
        int graphHeight = graph.GetLength(0);
        int graphWidth = graph.GetLength(1);    

        for(int i = 0; i < graphHeight; i++)
        {
            for(int j = 0; j < graphWidth; j++)
            {
                System.Console.Write($"{(graph[i,j].Cost == 1000000 ? "inf" : graph[i,j].Cost.ToString()), 4}");
            }
            System.Console.WriteLine();
        }
    }

    #endregion

    #region setup

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

    #endregion
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
