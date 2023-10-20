using System;
using System.Collections.Generic;
using System.Linq;

namespace day3;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        int resultSumPart1 = 0;
        int resultSumPart2 = 0;

        // part 2
        string[] group = new string[3];
        for (int i = 0; i < input.Length; i++)
        {
            int sLength = input[i].Length;
            string sub1 = input[i].Substring(0, sLength/2);
            string sub2 = input[i].Substring(sLength/2);
            resultSumPart1 += GetPriorityFromChar(sub1.Intersect(sub2).First());

            group[i%3] = input[i];

            if((i+1)%3 == 0 && i != 0)
                resultSumPart2 += GetPriorityFromChar(FindCommonChar(group));
        }

        System.Console.WriteLine("Result for part 1: " + resultSumPart1);
        System.Console.WriteLine("Result for part 2: " + resultSumPart2);
        System.Console.ReadLine();
    }

    private static char FindCommonChar(string[] group)
    {
        IEnumerable<char> matches1and2 = group[0].Intersect(group[1]);
        IEnumerable<char> matches = matches1and2.Intersect(group[2]);

        if(matches.Count() == 1)
            return matches.First();
        else
        { 
            string errorstring = "Not exactly 1 match found. Matches found:\n";
            foreach(char m in matches){
                errorstring += (m + "\n");
            }
            throw new Exception(errorstring);
        }
    }

    static int GetPriorityFromChar(char input)
    {
        int asciiValue = (byte)input;

        if (asciiValue >= 97 && asciiValue <= 122) // lower case
        {
            return asciiValue - 96;
        }
        else if (asciiValue >= 65)
        {
            return asciiValue - 38;
        }
        else
        {
            throw new Exception($"Invalid argurment. Input char = {input}");
        }
    }
}
