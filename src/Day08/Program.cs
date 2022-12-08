using AocHelper;

var input = File.ReadLines("Input001.txt").ToList();

// 1779
// 172224
var ps = new PuzzleSolver(input);
ps.SolvePart1().Log("Part 1");
ps.SolvePart2().Log("Part 2");
