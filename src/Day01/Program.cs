using Day01;

PuzzleInput.Input001
    .Split("\r\n\r\n")
    .Select(_ => _.Split("\r\n")
        .Select(int.Parse)
        .Sum())
    .Max()
    .Log("Part 1");

PuzzleInput.Input001
    .Split("\r\n\r\n")
    .Select(_ => _.Split("\r\n")
        .Select(int.Parse)
        .Sum())
    .OrderByDescending(_ => _)
    .Take(3)
    .Sum()
    .Log("Part 2");