public partial class PuzzleSolver
{
    readonly string input;

    public PuzzleSolver(string input)
    {
        this.input = input;
    }

    int FindIndex(int size)
        => input.Select((c, i) => (i, v: input[i..(i + size)]))
          .First(_ => _.v.Distinct().Count() == size).i + size;

    public int SolvePart1() => FindIndex(4);
    public int SolvePart2() => FindIndex(14);
}
