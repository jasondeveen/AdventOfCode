using System.Security;

namespace day13;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);
        List<int> correctIndices = new();

        for (int i = 0; i < input.Count(); i += 3)
        {
            if (IsInOrder(SplitIntoElements(input[i], true), SplitIntoElements(input[i + 1], true)) ?? false)
            {
                correctIndices.Add((i / 3) + 1); // 1-based
            }
        }

        System.Console.WriteLine($"Correct indices: {String.Join(',', correctIndices)}");
        System.Console.WriteLine($"Sum of correct indices: {correctIndices.Sum()}");
    }

    private static bool? IsInOrder(List<string> left, List<string> right)
    {
        // if(left[0] == "[]" && right[0] == "10")
        //     ;

        int numLeft = 0;
        int numRight = 0;

        bool leftIsNumber = IsNumber(left, ref numLeft);
        bool rightIsNumber = IsNumber(right, ref numRight);

        if (leftIsNumber && rightIsNumber)
        {
            if (numLeft == numRight)
            {
                return null;
            }
            else
                return numLeft < numRight;
        }

        if((left.Count() == 1 && left[0] == "[]") ^ (right.Count() == 1 && right[0] == "[]"))
            return left.Count() == 1 && left[0] == "";

        if(left.Count() == 1 && left[0] == "[]" && right.Count() == 1 && right[0] == "[]")
            throw new Exception("infinite loop: both sides are []");

        for (int index = 0; index < left.Count(); index++)
        {
            if (index >= right.Count())
                return false;

            List<string> leftElements = SplitIntoElements(left[index], false);
            List<string> rightElements = SplitIntoElements(right[index], false);
            if(leftElements.Count() == 0 ^ rightElements.Count() == 0) 
                return leftElements.Count() == 0;
            bool? answer = IsInOrder(leftElements, rightElements);
            if (answer != null)
                return answer;
        }

        return true;
    }

    private static bool IsNumber(List<string> input, ref int output)
    {
        return input.Count() == 1 && int.TryParse(input[0], out output);
    }

    private static List<string> SplitIntoElements(string input, bool isTopLevel)
    {
        List<string> returnvalue = new();

        System.Console.WriteLine("---------------------------------------------------------------------------------");
        System.Console.WriteLine("input: " + input + ", isTopLevel: " + isTopLevel);

        

        // if(input == "5,6,7")
        //     ;

        if(isTopLevel)
            input = input[1..^1];

        if(input == "[]")
            return new List<string>();
        if (input == "")
            return null;

        int bracketLevel = 0;
        bool bracketOpen = false;
        int LastOpenBracketIndex = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '[')
            {
                bracketLevel++;    

                if(bracketLevel == 1 && !bracketOpen){
                    LastOpenBracketIndex = i;
                    bracketOpen = true;
                }
            }
            if (input[i] == ']')
            {
                bracketLevel--;
                if (bracketLevel == 0)
                {
                    bracketOpen = false;
                    returnvalue.Add(input[LastOpenBracketIndex..(i+1)]);
                }
            }

            if (bracketLevel < 1)
            {
                int numberLength = 0;
                int j = i;
                while (j < input.Length && int.TryParse(input[j].ToString(), out int _))
                {
                    numberLength++;
                    j++;
                }
                if (numberLength > 0){
                    returnvalue.Add(input.Substring(i, numberLength));
                    i += j - i - 1;
                    }
            }
        }

        if(input != "[]" && input == returnvalue[0] && returnvalue[0][0] == '[' && returnvalue[0][^1] == ']')
            returnvalue[0] = returnvalue[0][1..^1];

        
        foreach(string o in returnvalue){
            System.Console.WriteLine("\t" + o);
        }
        
        return returnvalue;
    }
}
