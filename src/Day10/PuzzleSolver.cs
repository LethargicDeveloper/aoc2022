using BenchmarkDotNet.Attributes;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("InputExample.txt").ToList();
    }

    public long SolvePart1()
    {
        return 0;
    }

    public long SolvePart2()
    {
        return 0;
    }
}
