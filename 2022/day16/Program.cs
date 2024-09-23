
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace day16;

class Program
{
    static int ENDTIME = 30;

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

        Queue<State> q = new Queue<State>();
        q.Enqueue(new State
        {
            currentRoom = startNode,
            elapsedTime = 0,
            openedRooms = new List<Room>(),
            currentPressureReleased = 0,
        });


        HashSet<(List<Room>, int, int)> seen = new()
        {
            (new List<Room>(), 0, 0)
        };

        while (q.Count > 0)
        {
            State currentState = q.Dequeue();

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

                if (seen.Add((newOpened.Select(r => r).ToList(), newElapsed, newRelieved)))
                {
                    q.Enqueue(new State
                    {
                        currentRoom = unopenedRoom,
                        elapsedTime = newElapsed,
                        openedRooms = newOpened.Select(r => r).ToList(),
                        currentPressureReleased = newRelieved
                    });
                }
            }



            System.Console.WriteLine("States evaluated: " + ++evaluatedCounter);
        }


        System.Console.WriteLine($"Total pressure released: {highestPressureReleased}");
    }

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
}
