public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    bool TreeIsVisible(List<List<int>> forest, int x, int y)
    {
        bool isVisibleTop = true;
        bool isVisibleBottom = true;
        bool isVisibleLeft = true;
        bool isVisibleRight = true;

        for (int y1 = 0; y1 < y; ++y1)
        {
            if (forest[y1][x] >= forest[y][x])
            {
                isVisibleTop = false;
                break;
            }    
        }

        for (int y1 = forest.Count - 1; y1 > y; --y1)
        {
            if (forest[y1][x] >= forest[y][x])
            {
                isVisibleBottom = false;
                break;
            }
        }

        for (int x1 = 0; x1 < x; ++x1)
        {
            if (forest[y][x1] >= forest[y][x])
            {
                isVisibleLeft= false;
                break;
            }
        }

        for (int x1 = forest[0].Count - 1; x1 > x; --x1)
        {
            if (forest[y][x1] >= forest[y][x])
            {
                isVisibleRight= false;
                break;
            }
        }

        return isVisibleTop || isVisibleBottom || isVisibleLeft || isVisibleRight;
    }

    (int, int, long) ScenicDistance(List<List<int>> forest, int x, int y)
    {
        long top = 0;
        long bottom = 0;
        long left = 0;
        long right = 0;

        for (int y1 = y + 1; y1 < forest.Count; ++y1)
        {
            if (y1 == forest.Count - 1 || forest[y1][x] >= forest[y][x])
            {
                bottom++;
                break;
            }

            bottom++;
        }

        for (int y1 = y - 1; y1 >= 0; --y1)
        {
            if (y1 == 0 || forest[y1][x] >= forest[y][x])
            {
                top++;
                break;
            }

            top++;
        }

        for (int x1 = x+1; x1 < forest[0].Count; ++x1)
        {
            if (x1 == forest[0].Count - 1 || forest[y][x1] >= forest[y][x])
            {
                right++;
                break;
            }

            right++;
        }

        for (int x1 = x-1; x1 >= 0; --x1)
        {
            if (x1 == 0 || forest[y][x1] >= forest[y][x])
            {
                left++;
                break;
            }

            left++;
        }

        return (x, y, top * bottom * left * right);
    }

    public long SolvePart1()
    {
        var forest = this.input
            .Select(_ => _.Select(_ => _ - '0').ToList())
            .ToList();

        long visible = 0;
        for (int x = 0; x < forest[0].Count; ++x)
        {
            for (int y = 0; y < forest.Count; ++y)
            {
                if(TreeIsVisible(forest, x, y))
                {
                    visible++;
                }
            }
        }

        return visible;
    }

    public long SolvePart2()
    {
        var forest = this.input
            .Select(_ => _.Select(_ => _ - '0').ToList())
            .ToList();

        var distances = new List<(int, int, long)>();
        for (int x = 0; x < forest[0].Count; ++x)
        {
            for (int y = 0; y < forest.Count; ++y)
            {
                var d = ScenicDistance(forest, x, y);
                distances.Add(d);
            }
        }

        return distances.Select(_ => _.Item3).Max();
    }
}
