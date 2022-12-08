using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    readonly string input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("Input001");
    }

    Stack<char>[] InitializeStacks(int size)
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
        int stackSize = input[0][^2] - '0';
        var stacks = InitializeStacks(stackSize);
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
    public string SolvePart1()
    {
        var (stackInput, moveInput) = ParseInput(this.input);
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

        return new string(stacks.Select(_ => _.Pop()).ToArray());
    }

    [Benchmark]
    public string SolvePart2()
    {
        var (stackInput, moveInput) = ParseInput(this.input);
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

        return new string(stacks.Select(_ => _.Pop()).ToArray());
    }

    [Benchmark]
    public string SolvePart2Regex()
    {
        var (stackInput, moveInput) = ParseInputRegex(this.input);
        var stacks = stackInput
            .SplitLines()
            .Reverse()
            .Skip(1)
            .Aggregate(InitializeStacks(9), (acc, cur) =>
            {
                StackRegex().Matches(cur).ToList().ForEach(_ => acc[(_.Index - 1) / 4].Push(_.Value[0]));
                return acc;
            });

        return moveInput
            .SplitLines()
            .Select(_ => MoveRegex()
                .Matches(_)
                .Select(m => int.Parse(m.Value))
                .ToList())
            .Aggregate(stacks, (acc, cur) =>
            {
                Enumerable.Range(0, cur[0])
                    .Select(_ => acc[cur[1] - 1].Pop())
                    .Reverse()
                    .ToList()
                    .ForEach(acc[cur[2] - 1].Push);
                return acc;
            })
            .Select(_ => _.Pop())
            .AsString();
    }

    (string, string) ParseInputRegex(string input)
        => input.Split("\r\n\r\n") switch { var x => (x[0], x[1]) };

    [GeneratedRegex("[a-zA-Z]")]
    private static partial Regex StackRegex();

    [GeneratedRegex("\\d+")]
    private static partial Regex MoveRegex();
}
