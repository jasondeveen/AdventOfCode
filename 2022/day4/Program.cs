namespace day4;

class Program
{
    static void Main(string[] args)
    {
        int numberOfFullyContainedRanges = 0;
        int numberOfOverlappingRanges = 0;
        string[] lines = File.ReadAllLines(args[0]);

        foreach (string line in lines){
            string[] elfRanges = line.Split(',');
            int[] elf1IDs = GetIDsFromRange(elfRanges[0]);
            int[] elf2IDs = GetIDsFromRange(elfRanges[1]);

            IEnumerable<int> overlap = elf1IDs.Intersect(elf2IDs);

            if(overlap.Count() == elf1IDs.Count() || 
                overlap.Count() == elf2IDs.Count()){
                    numberOfFullyContainedRanges++;
                }

            if (overlap.Count() > 0)
                numberOfOverlappingRanges++;

        }

        System.Console.WriteLine("Number of fully contained ranges in the file:" + Environment.NewLine + numberOfFullyContainedRanges
            + Environment.NewLine + "Number of overlapping ranges in pairs: " + Environment.NewLine + numberOfOverlappingRanges);
    }

    static int[] GetIDsFromRange(string range){
        string[] limits = range.Split('-');
        int low = Convert.ToInt32(limits[0]);
        int high = Convert.ToInt32(limits[1]);

        List<int> results = new List<int>();

        for(int i = low; i <= high; i++){
            results.Add(i);
        }

        return results.ToArray();
    }
}
