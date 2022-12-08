using BenchmarkDotNet.Running;

#if DEBUG
    var ps = new PuzzleSolver();
    ps.SolveWithStringSplit().Log("Part 2 - String Split");
    ps.SolveWithRegex().Log("Part 2 - Regex");
    ps.SolveWithCompiledRegex().Log("Part 2 - Compiled Regex");
#else
    BenchmarkRunner.Run<PuzzleSolver>();
#endif