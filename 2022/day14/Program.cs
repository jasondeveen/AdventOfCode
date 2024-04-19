
namespace day14;

class Program
{
    static void Main(string[] args)
    {
        // 1. Get paths for every input
        // 2. Use paths to draw rocks in cave
        // 3. Simulate falling sand

        string[] input = File.ReadAllLines(args[0]);

        List<List<Coordinate>> paths = new List<List<Coordinate>>();
        foreach (string s in input)
        {
            paths.Add(GetPath(s));
        }
        int caveDeepest = paths.SelectMany(path => path.Select(p => p.Y)).Max();
        paths.Add(GetPath($"0,{caveDeepest+2} -> 1000,{caveDeepest+2}"));

        int caveMostLeft = paths.SelectMany(path => path.Select(p => p.X)).Min();
        int caveMostRight = paths.SelectMany(path => path.Select(p => p.X)).Max();


        int width = caveMostRight - caveMostLeft + 1;
        int height = caveDeepest + 2 + 1;

        Cave cave = new Cave(width, height);
        cave.LeftMostColumn = caveMostLeft;

        foreach (List<Coordinate> path in paths)
        {
            NormalizePath(path, caveMostLeft);
            cave.DrawRocks(path);
        }

        int numberOfSandGrains = 0;
        cave.Print();
        while (cave.DrawNextSandGrain())
        {
            // Console.Clear();
            // cave.Print();
            numberOfSandGrains++;
            // Thread.Sleep(1000);
        }


        for(int i = 0; i<10;i++){System.Console.WriteLine("=======================================================================================================");}
        cave.Print();
        System.Console.WriteLine("Sandgrains added: " + (numberOfSandGrains + 1));

        Console.ReadLine();
    }

    private static void NormalizePath(List<Coordinate> path, int caveMostLeft)
    {
        foreach (Coordinate p in path)
        {
            p.X -= caveMostLeft;
        }
    }

    static List<Coordinate> GetPath(string input)
    {
        List<Coordinate> path = new List<Coordinate>();

        Coordinate prev = null;
        foreach (string sPath in input.Split(" -> "))
        {
            string[] parts = sPath.Split(',');

            if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
            {
                Coordinate current = new Coordinate(x, y);

                if (prev != null)
                {
                    int tempX = prev.X;
                    int tempY = prev.Y;
                    while (tempX != current.X)
                    {
                        if (tempX < current.X)
                            tempX++;
                        else
                            tempX--;

                        path.Add(new Coordinate(tempX, tempY));
                    }

                    while (tempY != current.Y)
                    {
                        if (tempY < current.Y)
                            tempY++;
                        else
                            tempY--;

                        path.Add(new Coordinate(tempX, tempY));
                    }
                }

                prev = current;
                path.Add(current);
            }
        }

        return path;
    }
}


class Coordinate
{
    public int X;
    public int Y;
    public Coordinate(int ax, int ay)
    {
        X = ax;
        Y = ay;
    }
}


class Cave
{
    char[,] _cave;
    int leftMostColumn;
    int sandColumn;

    public int LeftMostColumn
    {
        get { return leftMostColumn; }
        set
        {
            leftMostColumn = value;
            sandColumn = 500 - leftMostColumn;
            _cave[0, sandColumn] = '+';
        }
    }
    public int Width;
    public int Height;

    public Cave(int aWidth, int aHeight)
    {
        Width = aWidth;
        Height = aHeight;
        _cave = new char[Height, Width];
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                _cave[i, j] = '.';
            }
        }
    }

    public void DrawRocks(List<Coordinate> path)
    {
        foreach (Coordinate p in path)
        {
            _cave[p.Y, p.X] = '#';
        }
    }

    /// <summary>
    /// Returns false if the sand falls forever
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool DrawNextSandGrain()
    {
        int Y = 0;
        int X = sandColumn;

        try
        {
            while (Y < Height - 1)
            {
                if (_cave[Y + 1, X] != '.')
                {
                    if (_cave[Y + 1, X - 1] == '.')
                        X = X - 1;
                    else if (_cave[Y + 1, X + 1] == '.')
                        X = X + 1;
                    else
                    {
                        if(_cave[Y, X] == '+')
                            return false;
                        
                        _cave[Y, X] = 'o';
                        return true;
                    }
                }

                Y++;

                // _cave[Y, X] = 'o';
                // Console.Clear();
                // this.Print();
                // _cave[Y, X] = '.';
                // Thread.Sleep(100);
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return false;
    }

    public void Print()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                System.Console.Write(_cave[i, j] + " ");  // half the margin on either side
            }
            System.Console.WriteLine();
        }
    }
}
