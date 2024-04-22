

using System.Text.RegularExpressions;

namespace day15;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);
        int lineToCheck = args.Length == 2 ? int.Parse(args[1]) : 10;

        IEnumerable<((int, int), (int, int))> SensorsAndBeacons = GetSensorsAndBeacons(input);

        IEnumerable<Circle> circles = GetCircles(SensorsAndBeacons);

        System.Console.WriteLine(GetMergedRanges(GetOccupiedRanges(lineToCheck, circles)).Select(r => r.End - r.Start).Sum());
    }

    private static List<Range> GetOccupiedRanges(int lineToCheck, IEnumerable<Circle> circles)
    {
        var occupiedRanges = new List<Range>();

        foreach (Circle circle in circles.Where(c => Math.Abs(lineToCheck - c.Center.Y) < c.Radius)) // circles that are in range
        {
            int leftover = circle.Radius - Math.Abs(lineToCheck - circle.Center.Y);

            Range range = new Range(circle.Center.X - leftover, circle.Center.X + leftover);

            occupiedRanges.Add(range);
        }

        return occupiedRanges;
    }

    private static int MergeRanges(List<Range> inputRanges, ref List<Range> mergedRanges)
    {
        mergedRanges = new List<Range>();
        int expansions = 0;

        foreach (Range r in inputRanges)
        {
            var containingRange = mergedRanges.FirstOrDefault(cr => (cr.Start <= r.Start && r.Start <= cr.End)
                                                                    || (cr.Start <= r.End && r.End <= cr.End));

            if (containingRange == null)
            {
                mergedRanges.Add(r);
            }
            else
            {
                expansions++;

                if (r.Start < containingRange.Start)
                    containingRange.Start = r.Start;

                if (r.End > containingRange.End)
                    containingRange.End = r.End;
            }
        }

        return expansions;
    }

    private static List<Range> GetMergedRanges(List<Range> occupiedRanges)
    {
        List<Range> mergedRanges = new List<Range>();
        MergeRanges(occupiedRanges, ref mergedRanges);

        int mergesFound = 0;
        do
        {
            List<Range> temp = mergedRanges.Select(r => new Range(r.Start, r.End)).ToList(); // Deep copy
            mergedRanges.Clear();
            mergesFound = MergeRanges(temp, ref mergedRanges);
        } while (mergesFound > 0);

        return mergedRanges;
    }

    private static IEnumerable<((int, int), (int, int))> GetSensorsAndBeacons(string[] input)
    {
        Regex regex = new Regex("=-?[0-9]*");
        foreach (string s in input)
        {
            MatchCollection matches = regex.Matches(s);
            if (matches.Count != 4)
                throw new Exception("Input can't be read!");

            List<int> nums = matches
                .Select(m => int.Parse(
                    m.Value.Substring(1))
                    )
                .ToList();

            yield return ((nums[0], nums[1]), (nums[2], nums[3]));
        }
    }

    private static IEnumerable<Circle> GetCircles(IEnumerable<((int, int), (int, int))> input)
    {
        foreach (((int, int), (int, int)) tuple in input)
        {
            yield return new Circle(new Coordinate(tuple.Item1.Item1, tuple.Item1.Item2), GetManhattanDistance(tuple.Item1, tuple.Item2));
        }
    }

    private static int GetManhattanDistance((int, int) item1, (int, int) item2)
    {
        return Math.Abs(item2.Item1 - item1.Item1) + Math.Abs(item2.Item2 - item1.Item2);
    }

    public class Range
    {
        public int Start;
        public int End;

        public Range(int aStart, int aEnd)
        {
            Start = aStart;
            End = aEnd;
        }
    }

    public class Coordinate
    {
        public int X;
        public int Y;

        public Coordinate(int aX, int aY)
        {
            X = aX;
            Y = aY;
        }
    }

    public class Circle
    {
        public Coordinate Center;
        public int Radius;
        public Circle(Coordinate aCenter, int aRadius)
        {
            Center = aCenter;
            Radius = aRadius;
        }
    }
}
