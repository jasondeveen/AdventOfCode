

using System.Text.RegularExpressions;

namespace day15;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);
        int lineToCheck = args[0] == "testinput.txt" ? 10 : 2000000;
        int maxSize = args[0] == "testinput.txt" ? 20 : 4000000;

        IEnumerable<((int, int), (int, int))> SensorsAndBeacons = GetSensorsAndBeacons(input);

        IEnumerable<Circle> circles = GetCircles(SensorsAndBeacons);

        List<Range> occupiedRanges = GetOccupiedRanges(lineToCheck, circles);
        CombineRanges(occupiedRanges);

        System.Console.WriteLine("Occupied positions in row " + lineToCheck + ": " + occupiedRanges.Select(r => r.End - r.Start).Sum());

        List<Range> takenPositions = new List<Range>();
        for (int i = 0; i < maxSize; i++)
        {
            CombineRanges(takenPositions, GetOccupiedRanges(i, circles));
        }
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

    private static void CombineRanges(List<Range> list1, List<Range> list2 = null)
    {
        int expansions = 0;
        do
        {
            // TODO 1 lijst op zichzelf merged lukt, maar hoe 2 lijsten naar 1? op deze manier moet list1 altijd gecleared worden.

            list2 = list1.Select(r => new Range(r.Start, r.End)).ToList(); // Deep copy
            list1.Clear();

            expansions = 0;

            foreach (Range r in list2)
            {
                var containingRange = list1.FirstOrDefault(cr => (cr.Start <= r.Start && r.Start <= cr.End)
                                                                    || (cr.Start <= r.End && r.End <= cr.End));

                if (containingRange == null)
                {
                    list1.Add(r);
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
        }
        while (expansions > 0);
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
