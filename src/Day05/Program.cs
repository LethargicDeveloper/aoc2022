using BenchmarkDotNet.Running;

#if DEBUG
    var ps = new PuzzleSolver();
    ps.SolvePart1().Log("Part 1");
    ps.SolvePart2().Log("Part 2");
    ps.SolvePart2Regex().Log("Part 2");
#else
    BenchmarkRunner.Run<PuzzleSolver>();
#endif