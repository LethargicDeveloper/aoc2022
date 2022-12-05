using BenchmarkDotNet.Running;
using Day04;

#if DEBUG
    new ProblemSolver().SolveWithStringSplit().Log("Part 2 - String Split");
    new ProblemSolver().SolveWithRegex().Log("Part 2 - Regex");
    new ProblemSolver().SolveWithCompiledRegex().Log("Part 2 - Compiled Regex");
#else
    BenchmarkRunner.Run<ProblemSolver>();
#endif