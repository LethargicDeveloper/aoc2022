using System.Linq;

public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    char[][] ParseInput()
    {
        var grid = new char[1000][];
        for (int y = 0; y < grid.Length; ++y)
        {
            grid[y] = new char[1000];
            for (int x = 0; x < grid[0].Length; ++x)
                grid[y][x] = '.';
        }
        grid[0][500] = '+';

        var points = this.input
            .Select(line => line
                .Split(" -> ")
                .Select(_ => _.Split(",") switch { var a => (Y: int.Parse(a[1]), X: int.Parse(a[0])) })
                .ToArray())
            .ToArray();

        foreach (var point in points)
        {
            for (int i = 0; i < point.Length - 1; ++i)
            {
                var yStart = Math.Min(point[i].Y, point[i + 1].Y);
                var yRange = Enumerable.Range(yStart, Math.Abs(point[i + 1].Y - point[i].Y) + 1).ToArray();

                var xStart = Math.Min(point[i].X, point[i + 1].X);
                var xRange = Enumerable.Range(xStart, Math.Abs(point[i + 1].X - point[i].X) + 1).ToArray();

                for (int y = yRange[0]; y <= yRange[^1]; ++y)
                {
                    for (int x = xRange[0]; x <= xRange[^1]; ++x)
                    {
                        grid[y][x] = '#';
                    }
                }
            }
        }

        return grid;
    }

    int CalcXMin(char[][] grid) => grid.Select(_ => Array.IndexOf(_, '#')).Where(_ => _ != -1).Min();
    int CalcXMax(char[][] grid) => grid.Select(_ => Array.LastIndexOf(_, '#')).Where(_ => _ != -1).Max() + 1;
    int CalcYMax(char[][] grid) => grid.Select((_, i) => (i, v: Array.IndexOf(_, '#'))).Last(_ => _.v != -1).i + 1;

    void PrintGrid(char[][] grid, (int Y, int X)? sandPos = null)
    {
        Console.Clear();
        var xMin = CalcXMin(grid);
        var xMax = CalcXMax(grid);
        var yMin = 0;
        var yMax = CalcYMax(grid);

        for (int y = yMin; y < yMax; ++y)
        {
            for (int x = xMin; x < xMax; ++x)
            {
                var c = sandPos != null && (y, x) == sandPos ? 'O' : grid[y][x];
                Console.Write(c);
            }

            Console.WriteLine();
        }
    }

    public long SolvePart1()
    {
        var grid = ParseInput();
        
        var spawnPos = (Y:0, X:500);
        var gridBottom = CalcYMax(grid);

        var sandPos = spawnPos;
        int sandCount = 0;
        while (sandPos.Y < gridBottom)
        {
            if ("#O".Contains(grid[sandPos.Y + 1][sandPos.X]))
            {
                if ("#O".Contains(grid[sandPos.Y + 1][sandPos.X - 1]))
                {
                    if ("#O".Contains(grid[sandPos.Y + 1][sandPos.X + 1]))
                    {
                        grid[sandPos.Y][sandPos.X] = 'O';
                        sandPos = spawnPos;
                        sandCount++;
                        continue;
                    }
                    else
                    {
                        sandPos = (sandPos.Y + 1, sandPos.X + 1);
                    }
                }
                else
                {
                    sandPos = (sandPos.Y + 1, sandPos.X - 1);
                }
            }
            else
            {
                sandPos = (sandPos.Y + 1, sandPos.X);
            }
        }

        return sandCount;
    }

    public long SolvePart2()
    {
        var grid = ParseInput();

        var spawnPos = (Y: 0, X: 500);
        var gridBottom = CalcYMax(grid) + 1;
        for (int x = 0; x < grid[0].Length; ++x)
            grid[gridBottom][x] = '#';

        var sandPos = spawnPos;
        int sandCount = 1;
        while (true)
        {
            if ("#O".Contains(grid[sandPos.Y + 1][sandPos.X]))
            {
                if ("#O".Contains(grid[sandPos.Y + 1][sandPos.X - 1]))
                {
                    if ("#O".Contains(grid[sandPos.Y + 1][sandPos.X + 1]))
                    {
                        grid[sandPos.Y][sandPos.X] = 'O';
                        if (sandPos == spawnPos) break;
                        sandPos = spawnPos;
                        sandCount++;
                        continue;
                    }
                    else
                    {
                        sandPos = (sandPos.Y + 1, sandPos.X + 1);
                    }
                }
                else
                {
                    sandPos = (sandPos.Y + 1, sandPos.X - 1);
                }
            }
            else
            {
                sandPos = (sandPos.Y + 1, sandPos.X);
            }
        }

        return sandCount;
    }
}
