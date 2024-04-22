

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

        Map boundaries = GetBoundaries(circles);

        System.Console.WriteLine(CountOccupiedPositions(lineToCheck, circles, SensorsAndBeacons));
    }

    private static int CountOccupiedPositions(int lineToCheck, IEnumerable<Circle> circles, IEnumerable<((int, int), (int, int))> sensorsAndBeacons)
    {
        var occupiedRanges = new List<Range>();

        foreach (Circle circle in circles.Where(c => Math.Abs(lineToCheck - c.Center.Y) > c.Radius)) // circles that are in range
        {
            int leftover = Math.Abs(lineToCheck - circle.Center.Y);

            Range range = new Range(circle.Center.X - leftover, circle.Center.X + leftover);

            occupiedRanges.Add(range);
        }


        return CountIndices(occupiedRanges);
    }

    private static int CountIndices(List<Range> occupiedRanges)
    {
        List<Range> mergedRanges = new List<Range>();

        
    }

    private static Map GetBoundaries(IEnumerable<Circle> circles)
    {
        int left = 0;
        int right = 0;
        int high = 0;
        int low = 0;

        foreach (Circle circle in circles)
        {
            if (circle.Center.X - circle.Radius < left)
                left = circle.Center.X - circle.Radius;

            if (circle.Center.X + circle.Radius > right)
                right = circle.Center.X + circle.Radius;

            if (circle.Center.Y - circle.Radius < low)
                low = circle.Center.Y - circle.Radius;

            if (circle.Center.Y + circle.Radius > high)
                high = circle.Center.Y + circle.Radius;
        }

        return new Map(high, low, left, right);
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
