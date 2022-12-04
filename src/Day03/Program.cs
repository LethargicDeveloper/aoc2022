using Day03;

PuzzleInput.Input001
    .Split("\r\n")
    .Select(_ => new[]
    {
        _[..(_.Length / 2)],
        _[^(_.Length / 2)..]
    })
    .Select(_ => _[0].First(i => _[1].Contains(i)))
    .Select(_ => char.IsLower(_) ? _ - 96 : _ - 38)
    .Sum()
    .Log("Part 01");

PuzzleInput.Input001
    .Split("\r\n")
    .Select((v, i) => new { i, v })
    .GroupBy(_ => _.i / 3)
    .Select(_ => _.Select(v => v.v)
        .Aggregate((acc, cur) => new string(acc.Intersect(cur).ToArray()))
        .Select(_ => char.IsLower(_) ? _ - 96 : _ - 38))
    .SelectMany(_ => _)
    .Sum()
    .Log("Part 02");