using AocHelper;

var input = File.ReadLines("Input001.txt").ToList();

// Part 1: 1444896
// Part 2: 404395
var ps = new PuzzleSolver(input);
ps.SolvePart1().Log("Part 1");
ps.SolvePart2().Log("Part 2");
