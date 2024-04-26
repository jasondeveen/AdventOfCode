
using System.Text.RegularExpressions;

namespace day16;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        // 1. Make tree, with value=flow rate, edge=1
        // 2. Find optimal path

        /*
            volgende valve kiezen:
            (Tr * Fr) - d
            Tr = Time Remaining
            Fr = Flow Rate of selected node
            d = distance to selected node
        */

        List<Node> nodes = GetTree(input);
        

    }

    private static List<Node> GetTree(string[] input)
    {
        List<Node> nodes = new List<Node>();

        foreach (string s in input)
        {
            string name = s[6..8];
            int flowRate = int.Parse(s.Split(';')[0].Split('=')[1]);

            nodes.Add(new Node(name, flowRate));
        }

        foreach (string s in input)
        {
            string nodeName = s[6..8];
            Node node = nodes.First(n => n.Name == nodeName);


            string temp = s.Split(';')[1];
            IEnumerable<string> neighborNames = Regex.Matches(temp, "[A-Z]{2}").Select(m => m.Value);

            foreach (string neighborName in neighborNames)
            {
                node.Neighbors.Add((1, nodes.First(n => n.Name == neighborName)));
            }
        }

        return nodes;
    }

    public class Node
    {
        public string Name;
        public int FlowRate;
        public List<(int, Node)> Neighbors;

        public Node(string aName, int aFlowRate)
        {
            Name = aName;
            FlowRate = aFlowRate;
            Neighbors = new List<(int, Node)>();
        }
    }
}
