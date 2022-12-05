using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

namespace Day05;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    Stack<char>[] CreateStacks(int size)
    {
        var stack = new Stack<char>[size];
        for (int i = 0; i < size; ++i)
        {
            stack[i] = new Stack<char>();
        }
        return stack;
    }

    (List<string> StackInput, List<string> MoveInput) ParseInput(string input)
        => input.Split("\r\n\r\n") switch 
        { 
            var x => (x[0].SplitLines().Reverse().ToList(), x[1].SplitLines().ToList()) 
        };

    Stack<char>[] ParseStacks(List<string> input)
    {
        //input
        //    .Skip(1)
        //    .Select(_ => _.Where((c, i) => (i == 1 || i % 4 == 0) && !char.IsWhiteSpace(c)))
        //    .Select(_ => _.sel)


        int stackSize = input[0][^2] - '0';
        var stacks = CreateStacks(stackSize);
        foreach (var line in input.Skip(1))
        {
            for (int i = 0, j = 1; j < input[0].Length; ++i, j += 4)
            {
                if (!char.IsWhiteSpace(line[j]))
                    stacks[i].Push(line[j]);
            }
        }

        return stacks;
    }

    IEnumerable<List<int>> ParseMoves(List<string> input)
        => input.Select(_ => Regex
            .Matches(_, "\\d+")
            .Select(m => int.Parse(m.Value))
            .ToList());

    [Benchmark]
    public void SolvePart1()
    {
        var (stackInput, moveInput) = ParseInput(PuzzleInput.Input001);


        var stacks = ParseStacks(stackInput);
        var moves = ParseMoves(moveInput);

        foreach (var move in moves)
        {
            for (int i = 0; i < move[0]; ++i)
            {
                var val = stacks[move[1] - 1].Pop();
                stacks[move[2] - 1].Push(val);
            }
        }

        new string(stacks.Select(_ => _.Pop()).ToArray()).Log("Part 1");
    }

    [Benchmark]
    public void SolvePart2()
    {
        var (stackInput, moveInput) = ParseInput(PuzzleInput.Input001);
        var stacks = ParseStacks(stackInput);
        var moves = ParseMoves(moveInput);

        foreach (var move in moves)
        {
            var list = new List<char>();
            for (int i = 0; i < move[0]; ++i)
            {
                list.Add(stacks[move[1] - 1].Pop());
            }

            foreach (var item in list.AsEnumerable().Reverse())
            {
                stacks[move[2] - 1].Push(item);
            }
        }

        new string(stacks.Select(_ => _.Pop()).ToArray()).Log("Part 2");
    }
}
