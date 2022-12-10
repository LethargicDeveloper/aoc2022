using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using Microsoft.Diagnostics.Tracing.Parsers.JScript;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    long SignalStrength(long x, long cycles) => x * cycles;

    public long SolvePart1()
    {
        long x = 1;
        long cycles = 1;
        var strengths = new List<long>();

        foreach (var line in this.input)
        {
            var cmd = line.Split(' ');

            cycles++;
            if (cycles % 40 == 20)
                strengths.Add(SignalStrength(x, cycles));

            if (cmd[0] == "addx")
            {
                cycles++;
                x += long.Parse(cmd[1]);
                if (cycles % 40 == 20)
                    strengths.Add(SignalStrength(x, cycles));
            }
        }

        return strengths.Sum();
    }

    public string SolvePart2()
    {
        var crt = new char[6][];
        for (int y = 0; y < 6; ++y)
        {
            crt[y] = new char[40];
            for (int x = 0; x < 39; ++x)
                crt[y][x] = '.';
        }

        var h = 1;
        var v = 0;
        var cycles = 0;

        foreach (var line in this.input)
        {
            var cmd = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            cycles++;
            if (cycles > 40)
            {
                v++;
                cycles = 1;
            }
            if (cycles - 1 >= h - 1 && cycles - 1 <= h + 1)
            {
                crt[v][cycles - 1] = '#';
            }
            
            if (cmd[0] == "addx")
            {
                cycles++;
                if (cycles > 40)
                {
                    v++;
                    cycles = 1;
                }
                if (cycles - 1 >= h - 1 && cycles - 1 <= h + 1)
                {
                    crt[v][cycles - 1] = '#';
                }
                h += int.Parse(cmd[1]);
            }
        }

        return "\r\n" + string.Join("\r\n", crt.Select(_ => new string(_)));
    }
}
