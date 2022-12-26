public partial class PuzzleSolver
{
    readonly string input;
    readonly HashSet<(int x, int y, int z)> points;
    readonly (int minX, int maxX, int minY, int maxY, int minZ, int maxZ) range;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("Input001.txt");
        this.points = GetPoints();
        this.range = GetMinMaxRanges(points);
    }
        
    public long SolvePart1()
    {
        var surfaceArea = points.Count * 6;

        var seen = new HashSet<(int x, int y, int z)>();
        foreach (var point in points)
        {
            var (x, y, z) = point;
            var adjacent = new[]
            {
                (x + 1, y, z),
                (x - 1, y, z),
                (x, y + 1, z),
                (x, y - 1, z),
                (x, y, z + 1),
                (x, y, z - 1)
            };

            var adjacentCount = seen.Where(adjacent.Contains).Count();
            surfaceArea -= (adjacentCount * 2);

            seen.Add(point);
        }

        return surfaceArea;
    }

    public long SolvePart2()
    {
        (int x, int y, int z)[] GetNeighbors(int x, int y, int z) =>
            new[] { (x + 1, y, z), (x - 1, y, z), (x, y + 1, z), (x, y - 1, z), (x, y, z + 1), (x, y, z - 1) };

        var points = GetPoints();

        var start = (range.minX, range.minY, range.minZ);
        var seen = new HashSet<(int x, int y, int z)>();
        var stack = new Stack<(int x, int y, int z)>();

        seen.Add(start);
        stack.Push(start);
        
        while (stack.TryPop(out var point))
        {
            var (x, y, z) = point;

            foreach (var neighbor in GetNeighbors(x, y, z))
            {
                if (!InRange(neighbor)) continue;
                if (seen.Contains(neighbor)) continue;
                if (points.Contains(neighbor))
                {
                    seen.Add(neighbor);
                    continue;
                };

                seen.Add(neighbor);
                stack.Push(neighbor);
            }
        }

        var surfaceArea = SolvePart1();
        int _x = 0, _y = 0, _z = 0;
        for (_x = range.minX; _x <= range.maxX; ++_x)
        {
            for (_y = range.minY; _y <= range.maxY; ++_y)
            {
                for (_z = range.minZ; _z <= range.maxZ; ++_z)
                {
                    if (seen.Contains((_x, _y, _z)) || points.Contains((_x, _y, _z)))
                        continue;

                    var n = GetNeighbors(_x, _y, _z);
                    surfaceArea -= n.Where(_ => seen.Contains(_) || points.Contains(_)).Count();
                }
            }
        }

        return surfaceArea;
    }

    bool InRange((int x, int y,int z) point)
    {
        var (x, y, z) = point;
        return (x >= range.minX && y >= range.minY && z >= range.minZ &&
                x <= range.maxX && y <= range.maxY && z <= range.maxZ);
    }

    (int minX, int maxX, int minY, int maxY, int minZ, int maxZ) GetMinMaxRanges(HashSet<(int x, int y, int z)> points)
    {
        var minX = points.Min(_ => _.x) - 1;
        var maxX = points.Max(_ => _.x) + 1;
        var minY = points.Min(_ => _.y) - 1;
        var maxY = points.Max(_ => _.y) + 1;
        var minZ = points.Min(_ => _.z) - 1;
        var maxZ = points.Max(_ => _.z) + 1;
        return (minX, maxX, minY, maxY, minZ, maxZ);
    }

    HashSet<(int x, int y, int z)> GetPoints() => this.input
        .Split("\r\n")
        .Select(_ => _.Split(',') switch
        {
            var p => (int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]))
        }).ToHashSet();

    enum Direction
    {
        Origin,
        Back,
        Forward,
        Left,
        Right,
        Up,
        Down
    }
}