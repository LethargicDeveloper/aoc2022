using Day04;

int[] ToRange(string list)
    => list.Split("-").Select(int.Parse).ToArray();

bool InRange(int[] r1, int[] r2)
    => r1[0] <= r2[0] && r1[1] >= r2[1];

PuzzleInput.Input001
    .Split("\r\n")
    .Select(_ => _.Split(",").Select(ToRange).ToArray())
    .Where(_ => InRange(_[0], _[1]) || InRange(_[1], _[0]))
    .Count()
    .Log("Part 1");

bool Overlap(int[] r1, int[] r2)
    => r1[0] <= r2[1] && r1[1] >= r2[0];

PuzzleInput.Input001
    .Split("\r\n")
    .Select(_ => _.Split(",").Select(ToRange).ToArray())
    .Where(_ => Overlap(_[0], _[1]) || Overlap(_[1], _[0]))
    .Count()
    .Log("Part 2");