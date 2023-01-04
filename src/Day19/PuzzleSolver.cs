using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    const int Ore = 0;
    const int Clay = 1;
    const int Obsidian = 2;
    const int Geode = 3;

    readonly string input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("Input001.txt");
    }

    [Benchmark]
    public long SolvePart1()
    {
        var factory = Factory.Create(input);
        return factory.CalculateTotalQualityLevel();
    }

    public long SolvePart2()
    {
        var factory = Factory.Create(input);
        return factory.CalculateTop3QualityLevel();
    }

    partial class Factory
    {
        readonly List<Blueprint> blueprints = new();

        private Factory(List<Blueprint> blueprints)
        {
            this.blueprints = blueprints;
        }

        public static Factory Create(string input)
        {
            var blueprints = ParseBlueprints(input);
            return new Factory(blueprints);
        }

        public long CalculateTotalQualityLevel()
        {
            return blueprints
                .Select((b, i) => (i + 1) * CalculateQualityLevel(b, 24))
                .Sum();
        }

        public long CalculateTop3QualityLevel()
        {
            return blueprints
                .Take(3)
                .Aggregate(1L, (acc, cur) => acc * CalculateQualityLevel(cur, 32));
        }

        float Score(State state) => ((state.TimeRemaining * state.Robots[Geode]) + state.Resources[Geode]);
        float BestPossibleScore(State state) => Score(state) +((state.TimeRemaining - 1) * state.TimeRemaining) / 2;

        long CalculateQualityLevel(Blueprint blueprint, int timeRemaining)
        {
            Console.WriteLine($"ore {blueprint[0][0]}, clay {blueprint[1][0]}, obsidian {blueprint[2][0]}/{blueprint[2][1]}, geode {blueprint[3][0]}/{blueprint[3][2]}");

            var initialState = new State(timeRemaining);
            var queue = new Queue<State>();
            queue.Enqueue(initialState);

            State maxState = new(timeRemaining);
            HashSet<State> seen = new();
            while (queue.TryDequeue(out var state))
            {
                if (state.Resources[Geode] > maxState.Resources[Geode])
                {
                    maxState = state;
                }

                if (state.TimeRemaining <= 0) continue;

                foreach (var robot in blueprint)
                {
                    var wait = (int)state.Resources
                        .Select((v, i) => robot.Value[i] switch
                        {
                            var c when c == 0 => 0,
                            var c when c <= state.Resources[i] => 1,
                            _ when state.Robots[i] == 0 => int.MaxValue,
                            var c => Math.Ceiling((c - state.Resources[i]) / (double)state.Robots[i]) + 1
                        }).Max();

                    if (wait == int.MaxValue) continue;
                    if (wait >= state.TimeRemaining)
                    {
                        var finalState = new State
                        {
                            TimeRemaining = state.TimeRemaining,
                            Resources = state.Resources.ToArray(),
                            Robots = state.Robots.ToArray()
                        };

                        var r = CollectResources(finalState, state.TimeRemaining);
                        finalState = new State
                        {
                            TimeRemaining = 0,
                            Resources = r,
                            Robots = state.Robots.ToArray()
                        };

                        if (BestPossibleScore(state) < Score(maxState)) continue;

                        if (seen.Add(finalState))
                            queue.Enqueue(finalState);
                        continue;
                    }

                    if (BestPossibleScore(state) < Score(maxState)) continue;

                    var resources = CollectResources(state, wait);
                    var newState = BuyRobot(robot.Key, state, resources, blueprint);
                    newState.TimeRemaining -= wait;

                    if (seen.Add(newState))
                        queue.Enqueue(newState);
                }
            }

            Console.WriteLine($"Geodes: {maxState.Resources[Geode]}");

            return maxState.Resources[Geode];
        }

        State BuyRobot(int robot, State state, int[] resources, Blueprint blueprint)
        {
            var newRobots = state.Robots.ToArray();
            var newResources = resources.ToArray();

            newRobots[robot]++;
            var bp = blueprint[robot];
            for (int i = 0; i < resources.Length; ++i)
            {
                newResources[i] -= bp[i];
            }

            return new State
            {
                Resources = newResources,
                Robots = newRobots,
                TimeRemaining = state.TimeRemaining
            };
        }

        int[] CollectResources(State state, int wait)
        {
            var resources = state.Resources.ToArray();

            for (int i = 0; i < state.Robots.Length; ++i)
            {
                resources[i] += (state.Robots[i] * wait);
            }

            return resources;
        }

        static List<Blueprint> ParseBlueprints(string input)
        {
            var blueprints = new List<Blueprint>();

            var lines = input.Split("\r\n");
            foreach (var line in lines)
            {
                var blueprint = new Blueprint();

                var instructions = line
                    .Split(':')[1]
                    .Split('.', StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => _.TrimEnd('.'));

                foreach (var instruction in instructions)
                {
                    var match = BlueprintRegex().Match(instruction);
                    var robot = match.Groups["robot"].Value;
                    var costsDict = match.Groups["cost"].Value
                        .Split(" and ")
                        .Select(_ => _.Split(' ') switch
                        {
                            var c => (int.Parse(c[0]), c[1])
                        }).ToDictionary(k => k.Item2, v => v.Item1);

                    var costs = new[]
                    {
                        costsDict.TryGetValue("ore", out int ore) ? ore : 0,
                        costsDict.TryGetValue("clay", out int clay) ? clay : 0,
                        costsDict.TryGetValue("obsidian", out int obsidian) ? obsidian : 0,
                        costsDict.TryGetValue("geode", out int geode) ? geode : 0,
                    };
                        
                    var index = robot switch
                    {
                        "ore" => 0,
                        "clay" => 1,
                        "obsidian" => 2,
                        "geode" => 3,
                        _ => throw new Exception("Invalid robot!")
                    };
                        
                    blueprint[index] = costs;
                }

                blueprints.Add(blueprint);
            }

            return blueprints;
        }

        class State : IEquatable<State?>
        {
            public State() { }

            public State(int timeRemaining)
            {
                this.TimeRemaining = timeRemaining;
            }

            public int TimeRemaining { get; set; }

            public int[] Resources = new[] { 0, 0, 0, 0 };

            public int[] Robots = new[] { 1, 0, 0, 0 };

            public override bool Equals(object? obj)
            {
                return Equals(obj as State);
            }

            public bool Equals(State? other)
            {
                return other is not null &&
                       TimeRemaining == other.TimeRemaining &&
                       Resources.SequenceEqual(other.Resources) &&
                       Robots.SequenceEqual(other.Robots);
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(TimeRemaining);
                for (int i = 0; i < Resources.Length; ++i)
                {
                    hash.Add(Robots[i]);
                    hash.Add(Resources[i]);
                }

                return hash.ToHashCode();
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

        [GeneratedRegex("(?<robot>ore|clay|obsidian|geode|) robot costs (?<cost>.*)")]
        private static partial Regex BlueprintRegex();
    }

    class Blueprint : Dictionary<int, int[]> { }
}
