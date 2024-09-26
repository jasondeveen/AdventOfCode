﻿
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace day16;

/*
    Probleem part 2:
        oplossing path: 
            ik: A - J - B - C
            e : A - D - H - E
            1707
        
        mijn path:
            ik: A - B - J - C
            e : A - D - H - E
            1705
        
        suboptimaal resultaat...

        het moet zijn dat mijn algo het oplossing pad niet ziet, anders zou deze gekozen zijn als winnaar.
        wrm ziet mijn algo het oplossing pad niet?
*/

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

        Dictionary<Room, Dictionary<Room, Room>> paths = GetPaths(nodes);
        Dictionary<Room, Dictionary<Room, int>> distanceMap = GetDistanceMap(nodes, paths);

        int highestPressureReleased = 0;
        int evaluatedCounter = 0;
        State winningState = null;

        Queue<State> q = new Queue<State>();
        q.Enqueue(new State
        {
            currentRoom = startNode,
            elephantCurrentRoom = startNode,
            elapsedTime = 0,
            openedRooms = new List<Room>(),
            currentPressureReleased = 0,
        });

        int seenHits = 0;

        HashSet<(List<Room>, int, int)> seen = new()
        {
            (new List<Room>(), 0, 0)
        };

        while (q.Count > 0)
        {
            State currentState = q.Dequeue();

            var unopenedRelevantRooms = relevantNodes.Except(currentState.openedRooms);

            // if all flowing valves are opened, wait until the end
            if (unopenedRelevantRooms.Count() == 0 || currentState.elapsedTime >= ENDTIME)
            {
                int relievedAtEnd = GetTotalPressure(currentState, ENDTIME);
                if (relievedAtEnd > highestPressureReleased)
                {
                    highestPressureReleased = relievedAtEnd;
                    winningState = currentState;
                }
                // highestPressureReleased = Math.Max(highestPressureReleased, relievedAtEnd);
                continue;
            }


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
                        int relievedAtEnd = GetTotalPressure(currentState, ENDTIME);
                        if (relievedAtEnd > highestPressureReleased)
                        {
                            highestPressureReleased = relievedAtEnd;
                            winningState = currentState;
                        }
                        // highestPressureReleased = Math.Max(highestPressureReleased, relievedAtEnd);
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


                    // if (seen.Add((newOpened.Select(r => r).ToList(), newElapsed, newRelieved)))
                    // {
                    List<State> newHistory = new List<State> { currentState };
                    newHistory.AddRange(currentState.history);
                    q.Enqueue(new State
                    {
                        currentRoom = newCurrentRoom,
                        elephantCurrentRoom = newElephantCurrentRoom,
                        elapsedTime = newElapsed,
                        openedRooms = newOpened.Select(r => r).ToList(),
                        currentPressureReleased = newRelieved,
                        history = newHistory
                    });
                    // }
                    // else
                    //     seenHits++;
                }
            }

            System.Console.WriteLine("States evaluated: " + ++evaluatedCounter);
        }

        System.Console.WriteLine($"Total pressure released: {highestPressureReleased}");
        System.Console.WriteLine(seenHits);
    }

    private static int GetTotalPressure(State state, int maxTime)
    {
        int remainingTime = ENDTIME - state.elapsedTime;

        return state.currentPressureReleased + (remainingTime * state.relievedPerMin);
    }

    private static Dictionary<Room, Dictionary<Room, Room>> GetPaths(List<Room> rooms)
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

    private static Dictionary<Room, Dictionary<Room, int>> GetDistanceMap(List<Room> rooms, Dictionary<Room, Dictionary<Room, Room>> paths)
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

        while (steps > 0)
        {
            currRoom = paths[destRoom][currRoom];
            steps--;
        }

        return currRoom;
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
        public Room elephantCurrentRoom;

        public int relievedPerMin => openedRooms.Select(r => r.FlowRate).Sum();

        public List<State> history = new();
    }
}




/*
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
                            elephantCurrentRoom = elephantUnopenedRoom,
                            elapsedTime = newElapsed,
                            openedRooms = newOpened.Select(r => r).ToList(),
                            currentPressureReleased = newRelieved
                        });
                    }*/
