using QuickGraph;
using QuickGraph.Algorithms;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public partial class PuzzleSolver
{
    readonly List<string> input;
    readonly DelegateVertexAndEdgeListGraph<Valve, EquatableEdge<Valve>> graph;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();

        graph = CreateGraph(ParseInput());
    }

    public long SolvePart1()
    {
        // calculate shortest path between each pair of important nodes
        var pairs = (
            from n1 in graph.Vertices.Where(_ => StartNode(_) || ImportantNode(_))
            from n2 in graph.Vertices.Where(_ => StartNode(_) || ImportantNode(_))
            where n1 != n2
            where n2.Name != "AA"
            select (start: n1, end: n2)
        ).Select(_ => ShortestPath(_.start)(_.end).ToList()).ToList();

        return GetBestPressure(pairs);
    }

    public long SolvePart2()
    {
        var pairs = (
            from n1 in graph.Vertices.Where(_ => StartNode(_) || ImportantNode(_))
            from n2 in graph.Vertices.Where(_ => StartNode(_) || ImportantNode(_))
            where n1 != n2
            where n2.Name != "AA"
            select (start: n1, end: n2)
        ).Select(_ => ShortestPath(_.start)(_.end).ToList()).ToList();

        return GetElephantPressure(pairs);
    }

    long GetBestPressure(List<List<Valve>> pairs)
    {
        var graph = new AdjacencyGraph<Valve, Edge<Valve>>();
        var costs = new Dictionary<Edge<Valve>, double>();

        foreach (var pair in pairs)
        {
            var source = pair[0];
            var target = pair[^1];
            double cost = pair.Count;

            var edge = new Edge<Valve>(source, target);
            graph.AddVerticesAndEdge(edge);
            costs.Add(edge, cost);
        }

        var func = AlgorithmExtensions.GetIndexer(costs);

        var startState = new State
        {
            Valve = graph.Vertices.First(_ => _.Name == "AA"),
            Pressure = 0,
            TimeRemaining = 26
        };

        var queue = new Queue<State>();
        queue.Enqueue(startState);

        long pressure = 0;
        long count = 0;
        while (queue.TryDequeue(out var state))
        {
            pressure = Math.Max(state.Pressure, pressure);

            foreach (var neighbor in graph.OutEdges(state.Valve))
            {
                if (state.Visited.Contains(neighbor.Target))
                    continue;

                var timeRemaning = state.TimeRemaining - (int)func(neighbor);
                if (timeRemaning < 0)
                    continue;

                queue.Enqueue(new State
                {
                    Valve = neighbor.Target,
                    Pressure = state.Pressure + (timeRemaning * neighbor.Target.Rate),
                    TimeRemaining = timeRemaning,
                    Visited = state.Visited.Concat(new[] { state.Valve } ).ToHashSet()
                });
                
                count = Math.Max(queue.Count, count);
            }
        }

        return pressure;
    }

    long GetElephantPressure(List<List<Valve>> pairs)
    {
        var graph = new AdjacencyGraph<Valve, Edge<Valve>>();
        var costs = new Dictionary<Edge<Valve>, double>();

        foreach (var pair in pairs)
        {
            var source = pair[0];
            var target = pair[^1];
            double cost = pair.Count;

            var edge = new Edge<Valve>(source, target);
            graph.AddVerticesAndEdge(edge);
            costs.Add(edge, cost);
        }

        var func = AlgorithmExtensions.GetIndexer(costs);

        var initialState = new State
        {
            Valve = graph.Vertices.First(_ => _.Name == "AA"),
            Pressure = 0,
            TimeRemaining = 26
        };

        var hash = new HashSet<(State, State)>();
        var queue = new Queue<(State, State)>();

        hash.Add((initialState, initialState));
        queue.Enqueue((initialState, initialState));

        long maxPressure = 0;
        (State, State) maxState;

        while (queue.TryDequeue(out var state))
        {
            var (state1, state2) = state;

            var pressure =  Math.Max(state1.Pressure + state2.Pressure, maxPressure);
            if (pressure > maxPressure)
            {
                maxPressure = pressure;
                maxState = state;
            }

            var newState1 = new List<State>();
            foreach (var state1Neighbor in graph.OutEdges(state1.Valve))
            {
                var state1TimeRemaining = state1.TimeRemaining - (int)func(state1Neighbor);
                if (state1TimeRemaining < 0) continue;

                var state1Visited = state1.Visited.Contains(state1Neighbor.Target);
                var state2Visited = state2.Visited.Contains(state1Neighbor.Target) || state2.Valve == state1Neighbor.Target;
                if (state1TimeRemaining > 0 && !state1Visited && !state2Visited)
                {
                    newState1.Add(new State
                    {
                        Valve = state1Neighbor.Target,
                        Pressure = state1.Pressure + (state1TimeRemaining * state1Neighbor.Target.Rate),
                        TimeRemaining = state1TimeRemaining,
                        Visited = state1.Visited.Concat(new[] { state1.Valve }).ToHashSet()
                    });
                }
            }

            if (newState1.Count == 0) newState1.Add(state1);


            var newState2 = new List<State>();
            foreach (var state2Neighbor in graph.OutEdges(state2.Valve))
            {
                var state2TimeRemaining = state2.TimeRemaining - (int)func(state2Neighbor);
                if (state2TimeRemaining < 0) continue;

                var state1Visited = state1.Visited.Contains(state2Neighbor.Target) || state1.Valve == state2Neighbor.Target;
                var state2Visited = state2.Visited.Contains(state2Neighbor.Target);
                if (state2TimeRemaining > 0 && !state1Visited && !state2Visited)
                {
                    newState2.Add(new State
                    {
                        Valve = state2Neighbor.Target,
                        Pressure = state2.Pressure + (state2TimeRemaining * state2Neighbor.Target.Rate),
                        TimeRemaining = state2TimeRemaining,
                        Visited = state2.Visited.Concat(new[] { state2.Valve }).ToHashSet()
                    });
                }
            }

            if (newState2.Count == 0) newState1.Add(state2);

            var newState = (
                from s1 in newState1
                from s2 in newState2
                where s1.Valve != s2.Valve
                select (s1, s2)
            ).ToList();

            foreach (var s in newState)
            {
                var (s1, s2) = s;
                if (hash.Contains(s)) continue;
                if (state1.Visited.Contains(s2.Valve) ||
                    state2.Visited.Contains(s1.Valve) ||
                    state1.Valve == s2.Valve ||
                    state2.Valve == s1.Valve ||
                    s1.Visited.Contains(s2.Valve) ||
                    s2.Visited.Contains(s1.Valve))
                    continue;

                hash.Add(s);
                queue.Enqueue(s);
            }
        }

        return maxPressure;
    }

    class State : IEquatable<State?>
    {
        public State() { }

        public State(Valve valve, int timeRemaining, long pressure, HashSet<Valve> visited)
        {
            this.Valve = valve;
            this.TimeRemaining = timeRemaining;
            this.Pressure = pressure;
            this.Visited = visited;
        }

        public Valve Valve { get; init; } = new();
        public int TimeRemaining { get; init; }
        public long Pressure { get; init; }
        public HashSet<Valve> Visited { get; init; } = new();

        public override bool Equals(object? obj)
        {
            return Equals(obj as State);
        }

        public bool Equals(State? other)
        {
            return other is not null &&
                   EqualityComparer<Valve>.Default.Equals(Valve, other.Valve) &&
                   TimeRemaining == other.TimeRemaining &&
                   Pressure == other.Pressure &&
                   Visited.SetEquals(other.Visited);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Valve, TimeRemaining, Pressure, Visited);
        }

        public static bool operator ==(State? left, State? right)
        {
            return EqualityComparer<State>.Default.Equals(left, right);
        }

        public static bool operator !=(State? left, State? right)
        {
            return !(left == right);
        }
    }

    bool StartNode(Valve valve) => valve.Name == "AA";
    bool ImportantNode(Valve valve) => valve.Rate > 0;

    Func<Valve, IEnumerable<Valve>> ShortestPath(Valve start)
    {
        var previous = new Dictionary<Valve, Valve>();

        var queue = new Queue<Valve>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var valve))
        {
            foreach (var edge in graph.OutEdges(valve))
            {
                var tunnel = edge.Target;
                if (previous.ContainsKey(tunnel))
                    continue;

                previous[tunnel] = valve;
                queue.Enqueue(tunnel);
            }
        }

        Func<Valve, IEnumerable<Valve>> shortestPath = end =>
        {
            var path = new List<Valve>();

            var current = end;
            while (!current.Equals(start))
            {
                path.Add(current);
                current = previous[current];
            }

            path.Add(start);
            path.Reverse();

            return path;
        };

        return shortestPath;
    }

    DelegateVertexAndEdgeListGraph<Valve, EquatableEdge<Valve>> CreateGraph(Dictionary<Valve, List<Valve>> dict)
    {
        return dict.ToVertexAndEdgeListGraph(
            kv => Array.ConvertAll(
                kv.Value.ToArray(),
                v => new EquatableEdge<Valve>(kv.Key, v)));
    }

    Dictionary<Valve, List<Valve>> ParseInput()
    {
        var dict = new Dictionary<Valve, List<Valve>>();
        var keys = new Dictionary<string, Valve>();

        foreach (var line in this.input)
        {
            var valveMatch = ValveRegex().Match(line);
            var name = valveMatch.Groups["valve"].Value;
            var rate = int.Parse(valveMatch.Groups["rate"].Value);
            var tunnelNames = valveMatch.Groups["tunnel"].Captures.Select(_ => _.Value).ToList();

            if (!keys.TryGetValue(name, out var key))
            {
                key = new Valve { Name = name };

                keys[name] = key;
                dict[key] = new List<Valve>();
            }

            key.Rate = rate;

            var tunnels = dict[key];
            foreach (var tunnelName in tunnelNames)
            {
                if (!keys.TryGetValue(tunnelName, out var tunnel))
                {
                    tunnel = new Valve { Name = tunnelName };
                    keys[tunnelName] = tunnel;
                    dict[tunnel] = new List<Valve>();
                }

                tunnels.Add(tunnel);
                key.Tunnels.Add(tunnel);
            }
        }

        return dict;
    }

    [DebuggerDisplay("{Name}")]
    class Valve : IEquatable<Valve?>
    {
        public string Name { get; set; } = "";
        public int Rate { get; set; }
        public List<Valve> Tunnels { get; set; } = new();

        public override bool Equals(object? obj)
        {
            return Equals(obj as Valve);
        }

        public bool Equals(Valve? other)
        {
            return other is not null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public static bool operator ==(Valve? left, Valve? right)
        {
            return EqualityComparer<Valve>.Default.Equals(left, right);
        }

        public static bool operator !=(Valve? left, Valve? right)
        {
            return !(left == right);
        }
    }

    [GeneratedRegex("Valve (?<valve>..) .*? rate=(?<rate>\\d+); .*? valves? ((?<tunnel>[A-Z]{2})(?:, )?)+")]
    private static partial Regex ValveRegex();
}
