using BenchmarkDotNet.Running;
using Day04;

#if DEBUG
    new PuzzleSolver().SolveWithStringSplit().Log("Part 2 - String Split");
    new PuzzleSolver().SolveWithRegex().Log("Part 2 - Regex");
    new PuzzleSolver().SolveWithCompiledRegex().Log("Part 2 - Compiled Regex");
#else
    BenchmarkRunner.Run<PuzzleSolver>();
#endif