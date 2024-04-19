using System.Net.Mail;

namespace day13_2;

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

        foreach(string s in llInputToSort)
            System.Console.WriteLine(s);

        int idx2 = FindIndex(llInputToSort, llInputToSort.Find("[[6]]")) + 1;
        int idx6 = FindIndex(llInputToSort, llInputToSort.Find("[[2]]")) + 1;

        System.Console.WriteLine("[[2]]: " + idx2);
        System.Console.WriteLine("[[6]]: " + idx6);

        System.Console.WriteLine("Product = " + (idx2 * idx6));
        
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
        // System.Console.WriteLine(raw);

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

        // foreach (string o in returnvalue)
        // {
        //     System.Console.WriteLine("\t" + o);
        // }

        return returnvalue;
    }

    private static void QuickSort(LinkedList<string> input)
    {
        if (input.Count == 1)
            return;

        LinkedListNode<string> pivotNode = input.Last;

        LinkedListNode<string> lastMovedNode = null;
        for (LinkedListNode<string> currentNode = input.First; currentNode != pivotNode; currentNode = currentNode.Next)
        {
            if (IsInOrder(currentNode.Value, pivotNode.Value) ?? false)
            {
                if (lastMovedNode == null && lastMovedNode != currentNode)
                {
                    input.Remove(currentNode);
                    input.AddFirst(currentNode);
                    lastMovedNode = currentNode;
                }

                if (lastMovedNode != null && lastMovedNode != currentNode)
                {
                    input.Remove(currentNode);
                    input.AddAfter(lastMovedNode, currentNode);
                    lastMovedNode = currentNode;
                }
            }
        }

        input.Remove(pivotNode);
        if (lastMovedNode == null)
            input.AddFirst(pivotNode);
        else
        {
            input.AddAfter(lastMovedNode, pivotNode);
            LinkedList<string> left = TakeSubLL(input, input.First, lastMovedNode ?? input.First);
            QuickSort(left);
            Attach(input, left, true);
        }

        if (pivotNode.Next != null)
        {
            LinkedList<string> right = TakeSubLL(input, pivotNode.Next, input.Last);
            QuickSort(right);
            Attach(input, right, false);
        }
    }

    /// <summary>
    /// attach <paramref name="listToAttach"/> to <paramref name="mainList"/>
    /// </summary>
    /// <param name="mainList"></param>
    /// <param name="listToAttach"></param>
    /// <param name="attachToFront"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void Attach(LinkedList<string> mainList, LinkedList<string> listToAttach, bool attachToFront = false)
    {
        var first = listToAttach.First;
        var current = first.Next;
        listToAttach.RemoveFirst();

        if (attachToFront)
            mainList.AddFirst(first);
        else
            mainList.AddLast(first);

        LinkedListNode<string> next = null;
        LinkedListNode<string> lastAttached = first;
        while (listToAttach.First != null)
        {
            next = current.Next;
            listToAttach.Remove(current);
            mainList.AddAfter(lastAttached, current);
            lastAttached = current;
            current = next;
        }

        ;
    }

    /// <summary>
    /// 0-based. Returns -1 if not found
    /// </summary>
    /// <param name="list"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    private static int FindIndex(LinkedList<string> list, LinkedListNode<string> node)
    {
        int c = 0;
        bool found = false;

        for (LinkedListNode<string> current = list.First; current != null; current = current.Next)
        {
            if (node == current)
            {
                found = true;
                break;
            }
            c++;
        }

        return found ? c : -1;
    }

    /// <summary>
    /// includes start and last
    /// </summary>
    /// <param name="list"></param>
    /// <param name="start"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static LinkedList<string> TakeSubLL(LinkedList<string> list, LinkedListNode<string> start, LinkedListNode<string> last)
    {
        LinkedList<string> returnList = new LinkedList<string>();

        int startIdx = FindIndex(list, start);
        int lastIdx = FindIndex(list, last);

        if (startIdx == -1 || lastIdx == -1)
            throw new Exception("start or end are not in list");

        if (startIdx > lastIdx)
            throw new Exception("start comes after last");

        LinkedListNode<string> current = start;
        LinkedListNode<string> next = null;
        LinkedListNode<string> terminator = last.Next;

        while (current != terminator)
        {
            next = current.Next;
            list.Remove(current);
            returnList.AddLast(current);
            current = next;
        }

        return returnList;
    }
}
