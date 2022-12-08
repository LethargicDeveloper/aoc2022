var input = File.ReadAllText("Input001.txt");

input
    .Split("\r\n\r\n")
    .Select(_ => _.Split("\r\n")
        .Select(int.Parse)
        .Sum())
    .Max()
    .Log("Part 1");

input
    .Split("\r\n\r\n")
    .Select(_ => _.Split("\r\n")
        .Select(int.Parse)
        .Sum())
    .OrderByDescending(_ => _)
    .Take(3)
    .Sum()
    .Log("Part 2");