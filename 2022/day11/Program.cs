namespace day11;

class Program
{
    static int superModulo = 1;

    static void Main(string[] args)
    {
        string[] input = File.ReadAllLines(args[0]);

        Monkey[] monkeys = new Monkey[(input.Count() + 1) / 7];

        // initiate monkeys
        for(int i = 0; i < monkeys.Count(); i++)
        {
            InitializeMonkey(input, monkeys, i);
            superModulo *= monkeys[i].TestOperand;
        }

        // play 20 rounds
        for (int i = 0; i < 10000; i++)
        {
            PlayRound(monkeys);
        }

        System.Console.WriteLine();
        for(int i = 0; i < monkeys.Count(); i++)
            System.Console.WriteLine($"Monkey {i} activity = {monkeys[i].levelOfMonkeyBusiness}");

        // get 2 most active monkeys
        List<Monkey> twoMostActiveMonkeys = monkeys
            .OrderByDescending(m => m.levelOfMonkeyBusiness)
            .Take(2)
            .ToList();

        System.Console.WriteLine($"Two most active monkeys: " + 
            $"{monkeys.ToList().IndexOf(twoMostActiveMonkeys.First())} : activity = {twoMostActiveMonkeys.First().levelOfMonkeyBusiness} & "  +
            $"{monkeys.ToList().IndexOf(twoMostActiveMonkeys.Last())}  : activity = {twoMostActiveMonkeys.Last().levelOfMonkeyBusiness}");

        long monkeyBusiness = (long)twoMostActiveMonkeys.First().levelOfMonkeyBusiness * (long)twoMostActiveMonkeys.Last().levelOfMonkeyBusiness;

        System.Console.WriteLine($"Total monkey business = {monkeyBusiness}");
    }

    private static void InitializeMonkey(string[] input, Monkey[] monkeys, int i)
    {
        List<long> startingItems = new List<long>();
        Func<long, long> operation = null;
        Func<long, bool> test = null;
        int testOperand = 1;
        int trueMonkeyNumber = -1;
        int falseMonkeyNumber = -1;

        foreach (string line in input[(i * 7)..((i * 7) + 6)])
        {
            if (line.Contains("Starting items"))
            {
                string[] nums = line.Split(':')[1].Split(',');
                foreach (string num in nums)
                    startingItems.Add(int.Parse(num));
            }

            if (line.Contains("Operation"))
            {
                string arg = line.Split(' ').Last();
                if (int.TryParse(arg, out int num))
                {
                    if (line.Contains('+'))
                        operation = (x => x + num);

                    if (line.Contains('*'))
                        operation = (x => x * num);
                }
                else if (arg.Trim() == "old")
                {
                    if (line.Contains('+'))
                        operation = (x => x + x);

                    if (line.Contains('*'))
                        operation = (x => x * x);
                }
            }

            if (line.Contains("Test"))
            {
                testOperand = int.Parse(line.Split(' ').Last());
                test = (x => x % testOperand == 0);
            }

            if (line.Contains("If true"))
                trueMonkeyNumber = int.Parse(line.Split(' ').Last());

            if (line.Contains("If false"))
                falseMonkeyNumber = int.Parse(line.Split(' ').Last());
        }

        monkeys[i] = new Monkey(startingItems, operation, test, testOperand, trueMonkeyNumber, falseMonkeyNumber);
    }

    private static void PlayRound(Monkey[] monkeys)
    {
        for(int i = 0; i < monkeys.Count(); i++)
        {
            // System.Console.WriteLine($"Monkey {i} plays:");
            PlayTurn(monkeys[i], monkeys);
        }
    }

    private static void PlayTurn(Monkey monkey, Monkey[] monkeys)
    {
        int numberOfItems = monkey.Items.Count();
        
        for(int i = 0; i < numberOfItems; i++)
        {
            // System.Console.WriteLine($"\t- Inspects item with value {monkey.Items.Peek()}");
            long newValue = monkey.PerformOperation(monkey.Items.Dequeue());
            // System.Console.WriteLine($"\tNew value = {newValue}");
            Monkey targetMonkey = null;
            if (monkey.PerformTest(newValue))
            {
                // System.Console.WriteLine($"\tTest succes: passing to monkey {monkey.trueMonkeyNumber}");
                targetMonkey = monkeys[monkey.trueMonkeyNumber];
            }
            else
            {
                // System.Console.WriteLine($"\tTest fail: passing to monkey {monkey.falseMonkeyNumber}");
                targetMonkey = monkeys[monkey.falseMonkeyNumber];
            }

            newValue = newValue % superModulo;
            targetMonkey.Items.Enqueue(newValue);
        }
    }
}


class Monkey
{
    public Queue<long> Items { get; private set; } = new Queue<long>();
    private Func<long, long> Operation { get; set; }
    private Func<long, bool> Test { get; set; }
    public int TestOperand { get; private set; }
    public int trueMonkeyNumber { get; private set; }
    public int falseMonkeyNumber { get; private set; }
    public int levelOfMonkeyBusiness;

    public Monkey(List<long> aItems, Func<long,long> aOperation, Func<long, bool> aTest, int aTestOperand, int aTrueMonkeyNumber, int aFalseMonkeyNumber)
    {
        Items = new Queue<long>(aItems);
        Operation = aOperation;
        Test = aTest;
        TestOperand = aTestOperand;
        trueMonkeyNumber = aTrueMonkeyNumber;
        falseMonkeyNumber = aFalseMonkeyNumber;
    }

    public long PerformOperation(long item)
    {
        levelOfMonkeyBusiness++;
        return Operation(item);
    }

    public bool PerformTest(long item)
    {
        return Test(item);
    }
}
