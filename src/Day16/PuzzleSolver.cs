using QuickGraph;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("InputExample.txt").ToList();
    }

    Dictionary<Valve, List<Valve>> ParseInput()
    {
        var dict = new Dictionary<Valve, List<Valve>>();

        foreach (var line in this.input)
        {
            var valveMatch = ValveRegex().Match(line);
            var valve = valveMatch.Groups["valve"].Value;
            var rate = int.Parse(valveMatch.Groups["rate"].Value);
            var tunnels = valveMatch.Groups["tunnel"].Captures.Select(_ => _.Value).ToList();

            var key = new Valve
            {
                Name = valve,
                Rate = rate
            };

            dict[key] = new List<Valve>();
        }

        return dict;
    }

    public long SolvePart1()
    {
        //var graph = ParseInput()
        //    .ToDelegateVertexAndEdgeListGraph(
        //        kv => Array.ConvertAll(kv.Value.Tunnels.ToArray(), v => new Edge<Valve>(kv.Key, kv.Value)));

        return 0;
    }

    public long SolvePart2()
    {
        return 0;
    }

    class Valve : IEquatable<Valve?>
    {
        public string Name { get; init; } = "";
        public int Rate { get; init; }
        public List<Valve> Tunnels { get; init; } = new();

        public override bool Equals(object? obj)
        {
            return Equals(obj as Valve);
        }

        public bool Equals(Valve? other)
        {
            return other is not null &&
                   Name == other.Name;
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
