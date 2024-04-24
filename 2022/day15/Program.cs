

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

        Map boundaries = new Map(maxSize, 0, 0, maxSize);
        List<Range> takenPositions = new List<Range>();
        List<(int, List<int>)> freePositionsPerRow = new List<(int, List<int>)>();
        List<int> freePositionsRow = new List<int>();
        for (int i = boundaries.Low; i < boundaries.High; i++)
        {
            takenPositions = GetOccupiedRanges(i, circles);
            CombineRanges(takenPositions);

            if (takenPositions.First().Start> 0 || takenPositions.First().End < maxSize)
            {
                freePositionsRow = GetFreePositions(takenPositions, boundaries);
                freePositionsPerRow.Add((i, freePositionsRow));
            }
            System.Console.WriteLine(i + " freePositionsPerRow.Length = " + freePositionsPerRow.Count);
        }

        System.Console.WriteLine("Left max: " + boundaries.Left + " Right max: " + boundaries.Right);
        foreach (var free in freePositionsPerRow)
        {
            System.Console.WriteLine("Free positions on row " + free.Item1);
            foreach (int pos in free.Item2)
            {
                System.Console.WriteLine("\t" + pos);
            }
        }

        if (freePositionsPerRow.Count == 1 && freePositionsPerRow.First().Item2.Count == 1)
        {
            double x = freePositionsPerRow.First().Item2.First();
            double y = freePositionsPerRow.First().Item1;
            System.Console.WriteLine($"Tuning frequency = {x} * 4.000.000 + {y} = {x * 4_000_000 + y}");
        }
    }

    private static List<int> GetFreePositions(List<Range> takenRanges, Map boundaries)
    {
        List<Range> freeRanges = new List<Range>();

        if (boundaries.Left < takenRanges.First().Start)
            freeRanges.Add(new Range(boundaries.Left, takenRanges.First().Start));

        if (takenRanges.Count > 1)
        {
            for (int i = 1; i < takenRanges.Count; i++)
            {
                if (takenRanges[i - 1].End < takenRanges[i].Start)
                    freeRanges.Add(new Range(takenRanges[i - 1].End, takenRanges[i].Start));
            }

            if (boundaries.Right > takenRanges.Last().End)
                freeRanges.Add(new Range(boundaries.Right, takenRanges.Last().End));
        }


        List<int> freePositions = new List<int>();
        foreach (Range freeRange in freeRanges)
        {
            for (int p = freeRange.Start + 1; p < freeRange.End; p++)
            {
                freePositions.Add(p);
            }
        }

        return freePositions;
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

    private static void CombineRanges(List<Range> list1)
    {
        int expansions = 0;
        do
        {
            var temp = list1.Select(r => new Range(r.Start, r.End)).ToList(); // Deep copy
            list1.Clear();

            expansions = 0;

            foreach (Range r in temp)
            {
                var containingRange = list1.FirstOrDefault(cr => (cr.Start <= r.Start && r.Start <= cr.End)
                                                              || (cr.Start <= r.End && r.End <= cr.End)
                                                              || (cr.Start >= r.Start && r.End >= cr.Start));

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

        list1 = list1.OrderBy(r => r.Start).ToList();
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

    public class Map
    {
        public int High;
        public int Low;
        public int Left;
        public int Right;

        public Map(int aHigh, int aLow, int aLeft, int aRight)
        {
            High = aHigh;
            Low = aLow;
            Left = aLeft;
            Right = aRight;
        }
    }
}
