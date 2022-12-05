using BenchmarkDotNet.Running;
using Day05;

#if DEBUG
    new PuzzleSolver().SolvePart1().Log("Part 1");
    new PuzzleSolver().SolvePart2().Log("Part 2");
#else
    BenchmarkRunner.Run<PuzzleSolver>();
#endif