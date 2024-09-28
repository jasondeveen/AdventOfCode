
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace day16;

class Program
{
    static int ENDTIME = 26;
    static Room dummyRoom = new Room("dummy", 0);

    static int seenHits = 0;
    static object seenHitsLock = new object();
    static int SeenHits
    {
        get { return seenHits; }
        set { 
            lock(seenHitsLock)
            {
                seenHits = value;
            }
        }
    }

    static int evaluatedCounter = 0;
    static object evaluatedCounterLock = new object();
    static int EvaluatedCounter
    {
        get { return evaluatedCounter; }
        set
        {
            lock (evaluatedCounterLock)
            {
                evaluatedCounter = value;
            }
        }
    }

    static int highestpressurereleased = 0;
    static object highestpressurereleasedLock = new object();
    static int Highestpressurereleased
    {
        get { return highestpressurereleased; }
        set
        {
            lock (highestpressurereleasedLock)
            {
                highestpressurereleased = value;
            }
        }
    }

    static void Main(string[] args)
    {
        // https://nickymeuleman.netlify.app/garden/aoc2022-day16#helpers

        string[] input = File.ReadAllLines(args[0]);

        IReadOnlyList<Room> nodes = GetTree(input);
        IReadOnlyList<Room> relevantNodes = nodes.Where(n => n.FlowRate > 0).ToList();
        Room startNode = nodes.First(n => n.Name == "AA");

        Dictionary<Room, Dictionary<Room, Room>> paths = GetPaths(nodes);
        Dictionary<Room, Dictionary<Room, int>> distanceMap = GetDistanceMap(nodes, paths);

        int highestPressureReleased = 0;
        int evaluatedCounter = 0;

        ConcurrentQueue<State> q = new ConcurrentQueue<State>();
        q.Enqueue(new State
        {
            currentRoom = startNode,
            elephantCurrentRoom = startNode,
            elapsedTime = 0,
            openedRooms = new List<Room>(),
            currentPressureReleased = 0,
        });


        ConcurrentSeenStateHashSet seen = ConcurrentSeenStateHashSet.Instance;



        ThreadPool.SetMaxThreads(20, 20);
        while (q.Count > 0)
        {
        https://medium.com/devtechblogs/overview-of-c-async-programming-with-thread-pools-and-task-parallel-library-7b18c9fc192d
            ThreadPool.QueueUserWorkItem(Task.Run(() => EvaluateState(q, relevantNodes, distanceMap, paths))
        }

        System.Console.WriteLine($"Total pressure released: {highestPressureReleased}");
        System.Console.WriteLine(seenHits);

        Console.ReadLine();
    }

    /// <summary>
    /// async functie, moet q, seen, seenhits, evaluatedcounter, highestpressurereleased delen
    /// relevantnodes, distancemap, paths w gebruikt en moeten const zijn
    /// </summary>
    private static void EvaluateState(ConcurrentQueue<State> q, IReadOnlyList<Room> relevantNodes, Dictionary<Room, Dictionary<Room, int>> distanceMap, Dictionary<Room, Dictionary<Room, Room>> paths)
    {
        q.TryDequeue(out State currentState);

        var unopenedRelevantRooms = relevantNodes.Except(currentState.openedRooms);

        // if all flowing valves are opened, wait until the end
        if (unopenedRelevantRooms.Count() == 0 || currentState.elapsedTime >= ENDTIME)
        {
            int relievedAtEnd = GetTotalPressure(currentState);
            HighestPressureReleased = Math.Max(HighestPressureReleased, relievedAtEnd);
            return;
        }

        if (unopenedRelevantRooms.Count() == 1)
            unopenedRelevantRooms = unopenedRelevantRooms.Append(dummyRoom);

        foreach (var unopenedRoom in unopenedRelevantRooms)
        {
            foreach (var elephantUnopenedRoom in unopenedRelevantRooms.Except(new List<Room> { unopenedRoom }))
            {
                int cost = distanceMap[currentState.currentRoom][unopenedRoom] + 1;
                int elephantCost = distanceMap[currentState.elephantCurrentRoom][elephantUnopenedRoom] + 1;
                int newElapsed = currentState.elapsedTime + Math.Min(cost, elephantCost);

                // if opening the dest valve would exceed the time limit, wait until the end
                if (newElapsed >= ENDTIME)
                {
                    int relievedAtEnd = GetTotalPressure(currentState);
                    highestPressureReleased = Math.Max(highestPressureReleased, relievedAtEnd);
                    continue;
                }

                // relieve pressure of opened valves while we move to dest and open it
                int newRelieved = currentState.currentPressureReleased + (currentState.relievedPerMin * Math.Min(cost, elephantCost));

                // add opened valve to opened valves and determine new current position of me and elephant
                var newOpened = currentState.openedRooms.Select(r => r).ToList();
                Room newCurrentRoom;
                Room newElephantCurrentRoom;
                if (cost < elephantCost)
                {
                    // i win -> my curr pos = newly opened room, elephant pos = (cost) steps towards elephantUnopenedRoom
                    newOpened.Add(unopenedRoom);
                    newCurrentRoom = unopenedRoom;
                    newElephantCurrentRoom = TakeStepsTowards(paths, currentState.elephantCurrentRoom, elephantUnopenedRoom, cost);
                }
                else if (cost > elephantCost)
                {
                    // elephant wins -> e's curr pos = e newly opened room, my pos = (cost) steps towards unopenedRoom
                    newOpened.Add(elephantUnopenedRoom);
                    newElephantCurrentRoom = elephantUnopenedRoom;
                    newCurrentRoom = TakeStepsTowards(paths, currentState.currentRoom, unopenedRoom, elephantCost);
                }
                else
                {
                    // draw -> both positions are the newly opened rooms (the good ending)
                    newOpened.Add(unopenedRoom);
                    newOpened.Add(elephantUnopenedRoom);
                    newCurrentRoom = unopenedRoom;
                    newElephantCurrentRoom = elephantUnopenedRoom;
                }

                SeenState seenState = new SeenState
                {
                    OpenedRooms = newOpened,
                    Relieved = newRelieved,
                    RoomsOccupied = MergeRooms(newCurrentRoom, newElephantCurrentRoom),
                    TimeElapsed = newElapsed
                };
                if (ConcurrentSeenStateHashSet.Instance.Add(seenState))
                {
                    q.Enqueue(new State
                    {
                        currentRoom = newCurrentRoom,
                        elephantCurrentRoom = newElephantCurrentRoom,
                        elapsedTime = newElapsed,
                        openedRooms = newOpened,
                        currentPressureReleased = newRelieved,
                    });
                }
                else
                    seenHits++;
            }
        }

        System.Console.WriteLine("States evaluated: " + ++evaluatedCounter);
    }

    #region helpers
    private static int GetTotalPressure(State state)
    {
        int remainingTime = ENDTIME - state.elapsedTime;

        return state.currentPressureReleased + (remainingTime * state.relievedPerMin);
    }

    private static Dictionary<Room, Dictionary<Room, Room>> GetPaths(IReadOnlyList<Room> rooms)
    {
        Dictionary<Room, Dictionary<Room, Room>> paths = new();

        foreach (Room room in rooms)
        {
            Queue<Room> unseen = new Queue<Room>();
            unseen.Enqueue(room);

            List<Room> seen = new List<Room>();
            seen.Add(room);

            Dictionary<Room, Room> pathsForRoom = new Dictionary<Room, Room>();

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
                        pathsForRoom.Add(n, current);
                    }
                }
            }

            paths.Add(room, pathsForRoom);
        }
        return paths;
    }

    private static Dictionary<Room, Dictionary<Room, int>> GetDistanceMap(IReadOnlyList<Room> rooms, Dictionary<Room, Dictionary<Room, Room>> paths)
    {
        Dictionary<Room, Dictionary<Room, int>> pathCosts = new();

        foreach (Room room in rooms)
        {
            Dictionary<Room, int> pathCostsForRoom = new Dictionary<Room, int>();

            foreach (Room r in rooms.Except(new List<Room> { room }))
            {
                Room nextRoom = r;
                int distance = 0;
                while (nextRoom != room)
                {
                    distance++;
                    nextRoom = paths[room][nextRoom];
                }

                pathCostsForRoom.Add(r, distance);
            }

            pathCostsForRoom.Add(room, 0);

            pathCostsForRoom.Add(dummyRoom, 999);

            pathCosts.Add(room, pathCostsForRoom);
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

    private static Room TakeStepsTowards(Dictionary<Room, Dictionary<Room, Room>> paths, Room startRoom, Room destRoom, int steps)
    {
        Room currRoom = startRoom;

        if (destRoom.Name == "dummy")
            return dummyRoom;

        while (steps > 0)
        {
            currRoom = paths[destRoom][currRoom];
            steps--;
        }

        return currRoom;
    }

    private static string MergeRooms(Room a, Room b)
    {
        return string.Join("", (a.Name[0].ToString() + b.Name[1].ToString()).OrderBy(c => c));
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
        public Room elephantCurrentRoom;

        public int relievedPerMin => openedRooms.Select(r => r.FlowRate).Sum();
    }

    public class SeenState
    {
        public List<Room> OpenedRooms;
        public int TimeElapsed;
        public int Relieved;
        public string RoomsOccupied;
    }

    public class CustomSeenStateComparer : IEqualityComparer<SeenState>
    {
        bool IEqualityComparer<SeenState>.Equals(SeenState x, SeenState y)
        {
            if (x.TimeElapsed != y.TimeElapsed) return false;
            if (x.Relieved != y.Relieved) return false;
            if (x.RoomsOccupied != y.RoomsOccupied) return false;

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
            return $"{string.Join(",", obj.OpenedRooms.Select(i => i.ToString()))},{obj.TimeElapsed},{obj.Relieved},{obj.RoomsOccupied}".GetHashCode();
        }
    }

    public class ConcurrentSeenStateHashSet
    {
        private static ConcurrentSeenStateHashSet instance = null;
        public static ConcurrentSeenStateHashSet Instance
        {
            get
            {
                if(instance == null)
                    instance = new ConcurrentSeenStateHashSet();

                return instance;
            }
        }

        ConcurrentDictionary<SeenState, byte> _dict = new ConcurrentDictionary<SeenState, byte>();
        
        private ConcurrentSeenStateHashSet() {
            _dict = new ConcurrentDictionary<SeenState, byte>(new CustomSeenStateComparer());
        }

        public bool Add(SeenState inputS)
        {
            return _dict.TryAdd(inputS, byte.MinValue);
        }
    }
}

