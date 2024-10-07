
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace day16;

class Program
{
    static int ENDTIME = 26;

    static void Main(string[] args)
    {
        // https://nickymeuleman.netlify.app/garden/aoc2022-day16#helpers

        string[] input = File.ReadAllLines(args[0]);

        List<Room> nodes = GetTree(input);
        List<Room> relevantNodes = nodes.Where(n => n.FlowRate > 0).ToList();
        Room startNode = nodes.First(n => n.Name == "AA");

        Dictionary<Room, Dictionary<Room, int>> distanceMap = GetDistanceMap(nodes);

        int highestPressureReleased = 0;
        int evaluatedCounter = 0;
        int seenHits = 0;

        Queue<State> q = new Queue<State>();
        q.Enqueue(new State
        {
            currentRoom = startNode,
            elapsedTime = 0,
            openedRooms = new List<Room>(),
            currentPressureReleased = 0,
        });

        HashSet<SeenState> seen = new(new CustomSeenStateComparer());

        Dictionary<List<Room>, int> highestRewardPerSetOfValves = new(new CustomStateListComparer());

        while (q.Count > 0)
        {
            State currentState = q.Dequeue();

            int totalPressure = GetTotalPressure(currentState, ENDTIME);
            List<Room> key = currentState.openedRooms.OrderBy(r => r.Name).ToList();
            //if(key.Any())
            if (!highestRewardPerSetOfValves.TryAdd(key, totalPressure) && highestRewardPerSetOfValves[key] < totalPressure)
                highestRewardPerSetOfValves[key] = totalPressure;

            var unopenedRooms = relevantNodes.Except(currentState.openedRooms);

            // if all flowing valves are opened, wait until the end
            if (unopenedRooms.Count() == 0 || currentState.elapsedTime >= 30)
            {
                int relievedAtEnd = GetTotalPressure(currentState, ENDTIME);
                highestPressureReleased = Math.Max(highestPressureReleased, relievedAtEnd);
                continue;
            }

            // for every unopened valve, run simulation
            foreach (var unopenedRoom in unopenedRooms)
            {
                // how long would moving to dest take? +1 to open the valve
                int cost = distanceMap[currentState.currentRoom][unopenedRoom] + 1;
                int newElapsed = currentState.elapsedTime + cost;

                // if opening the dest valve would exceed the time limit, wait until the end
                if (newElapsed >= ENDTIME)
                {
                    int relievedAtEnd = GetTotalPressure(currentState, ENDTIME);
                    highestPressureReleased = Math.Max(highestPressureReleased, relievedAtEnd);
                    continue;
                }

                // relieve pressure of opened valves while we move to dest and open it
                int newRelieved = currentState.currentPressureReleased + (currentState.relievedPerMin * cost);

                // add opened valve to opened valves
                var newOpened = currentState.openedRooms.Select(r => r).ToList();
                newOpened.Add(unopenedRoom);

                var seenstate = new SeenState
                {
                    OpenedRooms = newOpened,
                    TimeElapsed = newElapsed,
                    Relieved = newRelieved
                };

                if (seen.Add(seenstate))
                {
                    q.Enqueue(new State
                    {
                        currentRoom = unopenedRoom,
                        elapsedTime = newElapsed,
                        openedRooms = newOpened.Select(r => r).ToList(),
                        currentPressureReleased = newRelieved
                    });
                }
                else
                    seenHits++;
            }



            System.Console.WriteLine("States evaluated: " + ++evaluatedCounter);
        }

        System.Console.WriteLine($"P1: Total pressure released: {highestPressureReleased}");
        Console.WriteLine($"seenhits: {seenHits}");


        var results = GetPermutations(highestRewardPerSetOfValves.Keys.ToList())
            .Select(tuple => new { roomsets = tuple, reward = highestRewardPerSetOfValves[tuple.Item1] + highestRewardPerSetOfValves[tuple.Item2] })
            //.Max(i => i.reward);
            .OrderBy(i => i.reward) 
            .ToList();

        //int DDEEHHReward = highestRewardPerSetOfValves
        //    .First(kv => kv.Key.Any(k => k.Name == "DD") && kv.Key.Any(k => k.Name == "EE") && kv.Key.Any(k => k.Name == "HH") && kv.Key.Count() == 3)
        //    .Value;

        //int BBCCJJReward = highestRewardPerSetOfValves
        //    .First(kv => kv.Key.Any(k => k.Name == "BB") && kv.Key.Any(k => k.Name == "CC") && kv.Key.Any(k => k.Name == "JJ") && kv.Key.Count() == 3)
        //    .Value;


        Console.WriteLine();
        Console.WriteLine($"Max possible reward with 2 players: {results.Last().reward}");
    }

    #region helpers
    private static int GetTotalPressure(State state, int maxTime)
    {
        int remainingTime = ENDTIME - state.elapsedTime;

        return state.currentPressureReleased + (remainingTime * state.relievedPerMin);
    }

    private static Dictionary<Room, Dictionary<Room, int>> GetDistanceMap(List<Room> nodes)
    {
        Dictionary<Room, Dictionary<Room, int>> dict = new Dictionary<Room, Dictionary<Room, int>>();

        foreach (Room room in nodes)
        {
            dict.Add(room, GetPathCosts(nodes, room));
        }

        return dict;
    }

    private static Dictionary<Room, int> GetPathCosts(List<Room> rooms, Room room)
    {
        Queue<Room> unseen = new Queue<Room>();
        unseen.Enqueue(room);

        List<Room> seen = new List<Room>();
        seen.Add(room);

        Dictionary<Room, Room> prevDict = new Dictionary<Room, Room>();

        while (unseen.Count > 0)
        {
            Room current = unseen.Dequeue();
            List<Room> neighbors = current.Neighbors.Select(n => n.Item2).ToList();

            foreach (Room n in neighbors)
            {
                if (!seen.Contains(n))
                {
                    unseen.Enqueue(n);
                    seen.Add(n);
                    prevDict.Add(n, current);
                }
            }
        }

        Dictionary<Room, int> pathCosts = new Dictionary<Room, int>();

        foreach (Room r in rooms.Except(new List<Room> { room }))
        {
            Room nextRoom = r;
            int distance = 0;
            while (nextRoom != room)
            {
                distance++;
                nextRoom = prevDict[nextRoom];
            }

            pathCosts.Add(r, distance);
        }

        return pathCosts;
    }

    private static List<Room> GetTree(string[] input)
    {
        List<Room> nodes = new List<Room>();

        foreach (string s in input)
        {
            string name = s[6..8];
            int flowRate = int.Parse(s.Split(';')[0].Split('=')[1]);

            nodes.Add(new Room(name, flowRate));
        }

        foreach (string s in input)
        {
            string nodeName = s[6..8];
            Room node = nodes.First(n => n.Name == nodeName);


            string temp = s.Split(';')[1];
            IEnumerable<string> neighborNames = Regex.Matches(temp, "[A-Z]{2}").Select(m => m.Value);

            foreach (string neighborName in neighborNames)
            {
                node.Neighbors.Add((1, nodes.First(n => n.Name == neighborName)));
            }
        }

        return nodes;
    }

    private static IEnumerable<(List<Room>, List<Room>)> GetPermutations(List<List<Room>> roomSets)
    {
        foreach (var roomSet1 in roomSets)
        {
            foreach (var roomSet2 in roomSets.Where(rs => rs.Intersect(roomSet1).Count() == 0))
            {
                yield return (roomSet1, roomSet2);
            }
        }
    }
    #endregion


    [DebuggerDisplay("Name = {Name}")]
    public class Room
    {
        public string Name;
        public int FlowRate;
        public bool ValveStatus;
        public List<(int, Room)> Neighbors;

        public Room(string aName, int aFlowRate)
        {
            Name = aName;
            FlowRate = aFlowRate;
            ValveStatus = false;
            Neighbors = new List<(int, Room)>();
        }
    }

    public class State
    {
        public int currentPressureReleased;
        public int elapsedTime;
        public List<Room> openedRooms = new List<Room>();
        public Room currentRoom;

        public int relievedPerMin => openedRooms.Select(r => r.FlowRate).Sum();
    }
    
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class SeenState
    {
        private List<Room> openedRooms = new List<Room>();
        public List<Room> OpenedRooms {
            get => openedRooms;
            set
            {
                openedRooms = value.OrderBy(r => r.Name).ToList();
            }
        }
        public int TimeElapsed;
        public int Relieved;

        private string DebuggerDisplay
        {
            get
            {
                return $"OpenedRooms: {string.Join(",", OpenedRooms.Select(r => r.Name))}, TimeElapsed: {TimeElapsed}, Relieved: {Relieved}";
            }
        }
    }

    public class CustomSeenStateComparer : IEqualityComparer<SeenState>
    {
        bool IEqualityComparer<SeenState>.Equals(SeenState x, SeenState y)
        {
            if (x.TimeElapsed != y.TimeElapsed) return false;
            if (x.Relieved != y.Relieved) return false;

            if (x.OpenedRooms.Count != y.OpenedRooms.Count) return false;
            for (int i = 0; i < x.OpenedRooms.Count; i++)
            {
                if (x.OpenedRooms[i].Name != y.OpenedRooms[i].Name)
                    return false;
            }

            return true;
        }

        int IEqualityComparer<SeenState>.GetHashCode(SeenState obj)
        {
            return $"{string.Join(",", obj.OpenedRooms.Select(i => i.Name))},{obj.TimeElapsed},{obj.Relieved}".GetHashCode();
        }
    }

    public class CustomStateListComparer : IEqualityComparer<List<Room>>
    {
        bool IEqualityComparer<List<Room>>.Equals(List<Room>? x, List<Room>? y)
        {
            if(x.Count != y.Count) return false;
            for(int i = 0; i < x.Count; i++)
            {
                if (x[i] != y[i]) return false;
            }

            return true;
        }

        int IEqualityComparer<List<Room>>.GetHashCode(List<Room> obj)
        {
            return $"{string.Join(",", obj.Select(i => i.Name))}".GetHashCode();
        }
    }
}
