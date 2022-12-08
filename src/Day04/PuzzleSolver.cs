using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    int[] ToRange(string list)
        => list.Split("-").Select(int.Parse).ToArray();

    bool InRange(int[] r1, int[] r2)
        => r1[0] <= r2[0] && r1[1] >= r2[1];

    bool Overlap(int[][] r)
        => r[0][0] <= r[1][1] && r[0][1] >= r[1][0];

    bool Overlap(int[] r)
        => r[0] <= r[3] && r[1] >= r[2];

    [GeneratedRegex(@"\d+", RegexOptions.Compiled, "en-US")]
    private static partial Regex GeneratedRegex();

    static Regex CompiledRegex = new Regex(@"\d+", RegexOptions.Compiled);

    public int SolvePart1()
        => this.input
            .Select(_ => _.Split(",").Select(ToRange).ToArray())
            .Where(_ => InRange(_[0], _[1]) || InRange(_[1], _[0]))
            .Count();

    [Benchmark]
    public int SolveWithStringSplit()
        => this.input
            .Select(_ => _.Split(",")
                .Select(ToRange)
                .ToArray())
            .Where(Overlap)
            .Count();

    [Benchmark]
    public int SolveWithRegex()
        => this.input
            .Select(_ => Regex.Matches(_, @"\d+")
                .Select(x => int.Parse(x.Value))
                .ToArray())
            .Where(Overlap)
            .Count();

    [Benchmark]
    public int SolveWithCompiledRegex()
        => this.input
            .Select(_ => CompiledRegex.Matches(_)
                .Select(x => int.Parse(x.Value))
                .ToArray())
            .Where(Overlap)
            .Count();

    [Benchmark]
    public int SolveWithGeneratedRegex()
        => this.input
            .Select(_ => GeneratedRegex().Matches(_)
                .Select(x => int.Parse(x.Value))
                .ToArray())
            .Where(Overlap)
            .Count();
}
