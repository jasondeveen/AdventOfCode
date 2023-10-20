namespace day7;

class Program
{
    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        MyDirectory root = BuildTree(input[1..]);
        
        // root.PrintSelf(0);

        int neededSize = -70000000 + (root.GetSize() ?? 0) + 30000000;

        var bigbois = FindInSubDirs(root, (aDir) => aDir.GetSize() >= neededSize);

        foreach(MyDirectory dir in bigbois.OrderBy(d => d.GetSize()))
            System.Console.WriteLine(dir.GetName() + "\t " + dir.GetSize());

        System.Console.WriteLine("Big bois:");
        foreach(MyDirectory dir in bigbois)
            System.Console.WriteLine(dir.GetName() + " " + dir.GetSize());

        MyDirectory smallestBigBoi = bigbois.OrderBy(d => d.GetSize()).First();

        System.Console.WriteLine(Environment.NewLine + "Smallest dir that would create enough space = " + smallestBigBoi.GetName() + ", size = " + smallestBigBoi.GetSize());
    }

    static MyDirectory BuildTree(string[] input)
    {
        MyDirectory currentDir = new MyDirectory("/", null);
        MyDirectory root = currentDir;

        foreach(string line in input)
        {
            if(line.StartsWith("$"))
            {
                if(line[2..4] == "cd")
                {
                    if(line[5..] == ".."){
                        currentDir = currentDir.Parent;
                    }
                    else
                    {
                        MyDirectory childDir = (MyDirectory)currentDir.Contents.First(c => c.GetName() == line.Substring(5));
                        currentDir = childDir;
                    }
                }
                else if(line[2..4] == "ls")
                {

                }
            }
            else if(line.StartsWith("dir"))
            {
                MyDirectory childDir = new MyDirectory(line.Substring(4), currentDir);
                currentDir.Contents.Add(childDir);
            }
            else
            {
                string[] lineParts = line.Split(' ');
                MyFile childFile = new MyFile(int.Parse(lineParts[0]), lineParts[1]);
                currentDir.Contents.Add(childFile);
            }
        }

        return root;
    }

    static List<MyDirectory> FindInSubDirs(MyDirectory parent, Func<MyDirectory, bool> predicate)
    {
        List<MyDirectory> result = new List<MyDirectory>();

        foreach (IListable c in parent.Contents)
        {
            if (c is MyDirectory MDc)
            {
                if(predicate(MDc))
                    result.Add(MDc);  

                result.AddRange(FindInSubDirs(MDc, predicate));
            }   
        }

        return result;
    }
}


interface IListable
{
    int? GetSize();
    string GetName();
    void PrintSelf(int indentation);
}

class MyDirectory : IListable
{
    string Name{get;set;}

    public MyDirectory? Parent{get;private set;}

    public List<IListable> Contents{get;set;} = new List<IListable>();

    public MyDirectory(string aName, MyDirectory? aParent)
    {
        this.Name = aName;
        this.Parent = aParent;
    }

    public string GetName()
    {
        return this.Name;
    }

    public int? GetSize()
    {
        return Contents.Sum(c => c?.GetSize());
    }

    public void PrintSelf(int indentation)
    {
        for(int i = 0; i < indentation; i++)
            System.Console.Write("-");
        
        Console.Write(Name + Environment.NewLine);

        foreach(IListable c in Contents)
            c.PrintSelf(indentation + 2);
    }
}

class MyFile : IListable
{
    int Size{get;set;}
    string Name{get;set;}

    public MyFile(int aSize, string aName)
    {
        this.Size = aSize;
        this.Name = aName;
    }
    public string GetName()
    {
        return this.Name;
    }

    public int? GetSize()
    {
        return this.Size;
    }

    public void PrintSelf(int indentation)
    {
        for(int i = 0; i < indentation; i++)
            System.Console.Write("-");

        System.Console.WriteLine(Name + $"({Size})");
    }
}
