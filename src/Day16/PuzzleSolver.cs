using QuickGraph;
using System.Diagnostics;
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

        // get all possible paths that hit all important nodes
        var importantNodes = graph.Vertices.Where(ImportantNode).ToList();
        
        foreach (var pair in pairs.Where(_ => _[0].Name == "AA"))
        {
            var path = GetPaths(new[] { pair }.ToList(), pairs, importantNodes);
            
            long pressure = 0;
            foreach (var p in path)
            {
                if (p > pressure)
                {
                    pressure = p;
                    Console.WriteLine(pressure);
                }
            }
        }

        return 0;
    }

    public long SolvePart2()
    {
        return 0;
    }

    long GetPressure(List<List<Valve>> paths)
    {
        var pressures = new List<long>();
        foreach (var path in paths)
            pressures.Add(GetPressure(path));

        return pressures.Max();
    }

    long GetPressure(List<Valve> path)
    {
        //var answer = new List<Valve>
        //{
        //    new Valve {Name = "AA"},
        //    new Valve {Name = "DD"},
        //    new Valve {Name = "CC"},
        //    new Valve {Name = "BB"},
        //    new Valve {Name = "AA"},
        //    new Valve {Name = "II"},
        //    new Valve {Name = "JJ"},
        //    new Valve {Name = "II"},
        //    new Valve {Name = "AA"},
        //    new Valve {Name = "DD"},
        //    new Valve {Name = "EE"},
        //    new Valve {Name = "FF"},
        //    new Valve {Name = "GG"},
        //    new Valve {Name = "HH"},
        //    new Valve {Name = "GG"},
        //    new Valve {Name = "FF"},
        //    new Valve {Name = "EE"},
        //    new Valve {Name = "DD"},
        //    new Valve {Name = "CC"},
        //};

        var pressures = new List<(int ValveIndex, long Pressure, List<(Valve Valve, bool Open)> OpenValves)>();

        int time = 0;
        while (true)
        {
            // increase time
            time++;

            // we're done if all path lines time > 30
            if (time > 30) break;

            // check pressure
            for (int i = 0; i < pressures.Count; ++i)
            {
                var (_, pressure, openValves) = pressures[i];
                
                pressure += openValves.Where(_ => _.Open).Select(_ => _.Valve.Rate).Sum();
                pressures[i] = pressures[i] with
                {
                    Pressure = pressure,
                };
            }

            // get combinations for whether to open the valve or not
            if (pressures.Count == 0)
            {
                pressures.Add((0, 0, new List<(Valve, bool)> { (path[0], false) }));
            } 
            else
            {
                var updatedPressures = new List<(int ValveIndex, long Pressure, List<(Valve Valve, bool Open)> OpenValves)>();
                for (int i = 0; i < pressures.Count; ++i)
                {
                    var (valveIndex, pressure, openValves) = pressures[i];
                    var valve = path[valveIndex];

                    var openClosed = new List<(Valve valve, bool open)>
                    {
                        (valve, false)
                    };

                    if (valve.Rate > 0 && !openValves.Contains((valve, true)))
                    {
                        openClosed.Add((valve, true));
                    }

                    updatedPressures.AddRange((
                        from v in openClosed
                        from p in new[] { pressures[i] }
                        select p with
                        {
                            OpenValves = p.OpenValves.Concat(new[] { v }).ToList()
                        }
                    ).ToList());
                }
                pressures = updatedPressures;
            }

            // check whether to move or open a valve
            for (int i = 0; i < pressures.Count; ++i)
            {
                var (valveIndex, pressure, openValves) = pressures[i];

                if (!openValves[^1].Open)
                {
                    if (valveIndex < path.Count - 1)
                        valveIndex++;
                }

                pressures[i] = pressures[i] with
                {
                    ValveIndex = valveIndex
                };
            }
        }

        return pressures
            .Select(_ => _.Pressure)
            .Max();
    }

    IEnumerable<long> GetPaths(List<List<Valve>> segments, List<List<Valve>> shortestPaths, List<Valve> importantNodes)
    {
        var paths = (
            from segment in segments
            from shortestPath in shortestPaths
            where segment[^1] == shortestPath[0]
            select new[] { segment, shortestPath }.ToList()
        ).ToList();

        while (true)
        {
            var newPathSegments = new List<List<List<Valve>>>();
            foreach (var path in paths)
            {
                if (importantNodes.All(_ => path.SelectMany(v => v).Contains(_)))
                {
                    var p = RemoveDuplicateNodes(paths);
                    foreach (var p1 in p)
                    {
                        var pressure = GetPressure(p1);
                        yield return pressure;
                    }
                }

                newPathSegments.Add((
                    from segment in new[] { path[^1][^1] }
                    from shortestPath in shortestPaths
                    where segment == shortestPath[0]
                    where !path.Any(_ => _[0] == shortestPath[^1] || _[^1] == shortestPath[^1])
                    select shortestPath
                ).ToList());
            }

            var newPath = new List<List<List<Valve>>>();
            for (int i = 0; i < newPathSegments.Count; ++i)
            {
                var newPathSegment = newPathSegments[i];
                for (int j = 0; j < newPathSegment.Count; ++j)
                {
                    var newSegment = newPathSegment[j];
                    var p = new List<List<Valve>>();
                    p.AddRange(paths[i]);
                    p.Add(newSegment);
                    newPath.Add(p);
                }
            }

            if (newPath.Count == 0)
            {
                break;
            }

            paths = newPath;
        }
    }

    List<List<Valve>> RemoveDuplicateNodes(List<List<List<Valve>>> paths)
    {
        var p1 = paths.Select(_ => _.SelectMany(c => c).ToList()).ToList();
        var p2 = new List<List<Valve>>();

        foreach (var path in p1)
        {
            var valves = new List<Valve>();
            valves.Add(path[0]);

            for (int i = 1; i < path.Count; ++i)
            {
                if (valves[^1] != path[i])
                    valves.Add(path[i]);
            }

            p2.Add(valves);
        }

        return p2;
    }

    List<List<Valve>> RemoveDuplicatePaths(List<List<Valve>> paths)
    {
        var p = new List<List<Valve>>();
        
        for (int i = 0; i < paths.Count; ++i)
        {
            if (!p.Any(_ => Enumerable.SequenceEqual(_, paths[i])))
            {
                p.Add(paths[i]);
            }
        }

        return p;
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
