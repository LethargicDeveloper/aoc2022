using BenchmarkDotNet.Running;
using Day05;

#if DEBUG
    new PuzzleSolver().SolvePart1();
    new PuzzleSolver().SolvePart2();
#else
    BenchmarkRunner.Run<PuzzleSolver>();
#endif