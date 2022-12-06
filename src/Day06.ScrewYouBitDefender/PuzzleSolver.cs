namespace Day06;

public partial class PuzzleSolver
{
    int FindIndex(string input, int size)
        => input.Select((c, i) => (i, v: input[i..(i + size)]))
          .First(_ => _.v.Distinct().Count() == size).i + size;

    public int SolvePart1() => FindIndex(
        File.ReadAllText("Input001.txt"),
        4);

    public int SolvePart2() => FindIndex(
        File.ReadAllText("Input001.txt"),
        4);
}
