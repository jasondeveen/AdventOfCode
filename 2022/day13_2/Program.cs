﻿namespace day13_2;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        // // part 1
        // List<int> correctIndices = new();

        // for (int i = 0; i < input.Count(); i += 3)
        // {
        //     string left = input[i];
        //     string right = input[i + 1];

        //     bool? order = IsInOrder(left, right);
        //     if (order == null)
        //         throw new Exception("left and right are identical");

        //     if (order ?? false)
        //     {
        //         correctIndices.Add((i / 3) + 1); // 1-based
        //     }
        // }

        // System.Console.WriteLine($"Correct indices: {String.Join(',', correctIndices)}");
        // System.Console.WriteLine($"Sum of correct indices: {correctIndices.Sum()}");


        // part 2
        List<string> inputToSort = input
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        inputToSort.Add("[[2]]");
        inputToSort.Add("[[6]]");

        LinkedList<string> llInputToSort = new LinkedList<string>(inputToSort);

        QuickSort(llInputToSort);


    }

    private static bool? IsInOrder(string left, string right)
    {
        // handle integers
        if (int.TryParse(left, out int intLeft) && int.TryParse(right, out int intRight))
        {
            if (intLeft < intRight)
                return true;

            if (intLeft > intRight)
                return false;

            if (intLeft == intRight)
                return null;
        }

        if (int.TryParse(left, out int _))
        {
            return IsInOrder("[" + left + "]", right);
        }

        if (int.TryParse(right, out int _))
        {
            return IsInOrder(left, "[" + right + "]");
        }


        // handle empty lists
        if (left == "[]" && right == "[]")
            return null;

        if (left == "[]")
            return true;

        if (right == "[]")
            return false;


        // handle lists
        List<string> leftElements = GetElements(left);
        List<string> rightElements = GetElements(right);

        int longest = Math.Min(leftElements.Count, rightElements.Count);

        int idx = 0;
        while (idx < longest)
        {
            bool? res = IsInOrder(leftElements[idx], rightElements[idx]);
            if (res != null)
                return res;

            idx++;
        }

        if (leftElements.Count == rightElements.Count)
            return null;
        return leftElements.Count < rightElements.Count;
    }

    private static List<string> GetElements(string raw)
    {
        List<string> returnvalue = new List<string>();
        System.Console.WriteLine(raw);

        raw = raw[1..^1];

        int bracketLevel = 0;
        bool bracketOpen = false;
        int LastOpenBracketIndexLowestLevel = 0;
        for (int i = 0; i < raw.Length; i++)
        {
            if (raw[i] == '[')
            {
                bracketLevel++;

                if (bracketLevel == 1 && !bracketOpen)
                {
                    LastOpenBracketIndexLowestLevel = i;
                    bracketOpen = true;
                }
            }
            if (raw[i] == ']')
            {
                bracketLevel--;

                if (bracketLevel == 0)
                {
                    bracketOpen = false;
                    returnvalue.Add(raw[LastOpenBracketIndexLowestLevel..(i + 1)]);
                }
            }

            if (bracketLevel < 1)
            {
                int numberLength = 0;
                int j = i;
                while (j < raw.Length && int.TryParse(raw[j].ToString(), out int _))
                {
                    numberLength++;
                    j++;
                }
                if (numberLength > 0)
                {
                    returnvalue.Add(raw.Substring(i, numberLength));
                    i += j - i - 1; // -1 because the for-loop increments by 1 
                }
            }
        }

        // if (raw != "[]" && raw == returnvalue[0] && returnvalue[0][0] == '[' && returnvalue[0][^1] == ']')
        //     returnvalue[0] = returnvalue[0][1..^1];


        foreach (string o in returnvalue)
        {
            System.Console.WriteLine("\t" + o);
        }

        return returnvalue;
    }

    private static void QuickSort(LinkedList<string> input)
    {
        LinkedListNode<string> pivotNode = input.Last;

        LinkedListNode<string> lastMovedNode = input.First;
        for (LinkedListNode<string> currentNode = input.First; currentNode != pivotNode; currentNode = currentNode.Next)
        {
            if (IsInOrder(currentNode.Value, pivotNode.Value) ?? false)
            {
                if (lastMovedNode != currentNode)
                {
                    input.Remove(currentNode);
                    input.AddAfter(lastMovedNode, currentNode);
                    lastMovedNode = currentNode;
                }
            }
        }

        input.Remove(pivotNode);
        input.AddAfter(lastMovedNode, pivotNode);

        //todo selectie van begin tot pivotnode maken en dit recursive calle
        // selectie van 1 voorbij pivot tot einde maken en dit recursive callen
        // probleem: subselectie van linkedlist maken zonder een nieuwe linkelist te maken, want we hetzelfde object gebruiken (by ref)
        QuickSort(input.);
    }

    private static int FindIndex(LinkedList<string> list, LinkedListNode<string> node)
    {
        int c = 1;

        for (LinkedListNode<string> current = list.First; current != node; current = current.Next)
        {
            c++;
        }

        return c;
    }
}