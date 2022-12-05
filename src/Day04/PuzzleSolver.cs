using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

namespace Day04;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
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
        => PuzzleInput.Input001
            .Split("\r\n")
            .Select(_ => _.Split(",").Select(ToRange).ToArray())
            .Where(_ => InRange(_[0], _[1]) || InRange(_[1], _[0]))
            .Count();

    [Benchmark]
    public int SolveWithStringSplit()
        => PuzzleInput.Input001
            .Split("\r\n")
            .Select(_ => _.Split(",")
                .Select(ToRange)
                .ToArray())
            .Where(Overlap)
            .Count();

    [Benchmark]
    public int SolveWithRegex()
        => PuzzleInput.Input001
            .Split("\r\n")
            .Select(_ => Regex.Matches(_, @"\d+")
                .Select(x => int.Parse(x.Value))
                .ToArray())
            .Where(Overlap)
            .Count();

    [Benchmark]
    public int SolveWithCompiledRegex()
        => PuzzleInput.Input001
            .Split("\r\n")
            .Select(_ => CompiledRegex.Matches(_)
                .Select(x => int.Parse(x.Value))
                .ToArray())
            .Where(Overlap)
            .Count();

    [Benchmark]
    public int SolveWithGeneratedRegex()
        => PuzzleInput.Input001
            .Split("\r\n")
            .Select(_ => GeneratedRegex().Matches(_)
                .Select(x => int.Parse(x.Value))
                .ToArray())
            .Where(Overlap)
            .Count();
}
