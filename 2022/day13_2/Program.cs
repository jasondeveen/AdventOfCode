namespace day13_2;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);
        List<int> correctIndices = new();

        for (int i = 0; i < input.Count(); i += 3)
        {
            string left = input[i];
            string right = input[i + 1];

            bool? order = IsInOrder(left, right);
            if (order == null)
                throw new Exception("left and right are identical");

            if (order ?? false)
            {
                correctIndices.Add((i / 3) + 1); // 1-based
            }
        }

        System.Console.WriteLine($"Correct indices: {String.Join(',', correctIndices)}");
        System.Console.WriteLine($"Sum of correct indices: {correctIndices.Sum()}");
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
}
