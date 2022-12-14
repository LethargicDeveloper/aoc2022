using BenchmarkDotNet.Attributes;
using System.Text;

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

    void NextCycle(StringBuilder crt, int pos, ref int cycles)
    {
        int x = (++cycles - 1) % 40;

        crt.Append(x >= pos - 1 && x <= pos + 1 ? "#" : ".");
        if (x == 39) crt.AppendLine();
    }

    public string SolvePart2()
    {
        var crt = new StringBuilder("\r\n");

        var pos = 1;
        var cycles = 0;

        foreach (var line in this.input)
        {
            var cmd = line.Split(' ');

            NextCycle(crt, pos, ref cycles);
            
            if (cmd[0] == "addx")
            {
                NextCycle(crt, pos, ref cycles);
                pos += int.Parse(cmd[1]);
            }
        }

        return crt.ToString();
    }
}
