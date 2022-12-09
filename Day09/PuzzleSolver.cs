using BenchmarkDotNet.Attributes;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    public long SolvePart1()
    {
        var visited = new HashSet<(int, int)>();
        var headPos = (x: 0, y: 0);
        var tailPos = (x: 0, y: 0);
        visited.Add(tailPos);

        foreach (var line in this.input)
        {
            var (dir, dist) = line.Split(" ") switch { var x => (x[0], int.Parse(x[1])) };
            headPos = dir switch
            {
                "U" => headPos with { y = headPos.y - dist },
                "D" => headPos with { y = headPos.y + dist },
                "L" => headPos with { x = headPos.x - dist },
                "R" => headPos with { x = headPos.x + dist },
                _ => throw new NotImplementedException()
            };

            while (Math.Abs(headPos.x - tailPos.x) > 1 || Math.Abs(headPos.y - tailPos.y) > 1)
            {
                var x = headPos.x switch
                {
                    { } when tailPos.x < headPos.x => tailPos.x + 1,
                    { } when tailPos.x > headPos.x => tailPos.x - 1,
                    _ => tailPos.x
                };

                var y = headPos.y switch
                {
                    { } when tailPos.y < headPos.y => tailPos.y + 1,
                    { } when tailPos.y > headPos.y => tailPos.y - 1,
                    _ => tailPos.y
                };

                tailPos = (x, y);
                visited.Add(tailPos);
            }
        }

        return visited.Count();
    }

    public long SolvePart2()
    {
        var visited = new HashSet<(int, int)>();
        var knotPos = Enumerable.Range(0, 10).Select(_ => (x: 0, y: 0)).ToList();
        visited.Add(knotPos[9]);

        foreach (var line in this.input)
        {
            var (dir, dist) = line.Split(" ") switch { var x => (x[0], int.Parse(x[1])) };
            for (int dx = 0; dx < dist; ++dx)
            {
                var headPos = knotPos[0];
                knotPos[0] = dir switch
                {
                    "U" => (headPos.x, headPos.y - 1),
                    "D" => (headPos.x, headPos.y + 1),
                    "L" => (headPos.x - 1, headPos.y),
                    "R" => (headPos.x + 1, headPos.y),
                    _ => throw new NotImplementedException()
                };

                for (int i = 1; i < knotPos.Count; ++i)
                {
                    var knot = knotPos[i];
                    var prevKnot = knotPos[i - 1];
                    if (Math.Abs(prevKnot.x - knot.x) > 1 || Math.Abs(prevKnot.y - knot.y) > 1)
                    {
                        var x = knot.x + Math.Sign(prevKnot.x - knot.x);
                        var y = knot.y + Math.Sign(prevKnot.y - knot.y);

                        knotPos[i] = (x, y);
                    }
                    
                }
                
                visited.Add(knotPos[9]);
            }
        }

        return visited.Count();
    }
}
