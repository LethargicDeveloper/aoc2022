using System.Text.RegularExpressions;

public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    (Point Sensor, Point Beacon) Parse(string input)
        => Regex.Matches(input, @"-?\d+").ToArray() switch
        {
            var a => (
                (X: int.Parse(a[0].Value), Y: int.Parse(a[1].Value)),
                (X: int.Parse(a[2].Value), Y: int.Parse(a[3].Value))
            )
        };

    HashSet<Point> DetectedArea(IEnumerable<(Point Sensor, Point Beacon)> points, int row)
    {
        var area = new HashSet<Point>();
        foreach (var point in points)
        {
            var (sensor, beacon) = point;
            var dist = sensor.ManhattanDistance(beacon);
            var offset = Math.Abs(row - sensor.Y);
            var startX = (sensor.X - dist) + offset;
            var endX = (sensor.X + dist) - offset;
            for (int x = startX; x < endX; ++x)
            {
                area.Add((x, row));
            }
        }

        return area;
    }

    Point FindBeacon(List<(Point Sensor, int Dist)> points, int max)
    {
        for (int y = 0; y < max; ++y)
        {
            var area = new List<(int sx, int ex)>();
            foreach (var point in points)
            {
                var (sensor, dist) = point;
                var offset = Math.Abs(y - sensor.Y);
                var startX = Math.Clamp((sensor.X - dist) + offset, 0, max);
                var endX = Math.Clamp((sensor.X + dist) - offset, 0, max);
                area.Add((startX, endX));
            }

            area.Sort();
            int cur = 0;
            for (int x = 0; x < area.Count; ++x)
            {
                var a = area[x];
                if (a.sx <= cur && a.ex > cur)
                {
                    cur = a.ex + 1;
                }
                else if (a.sx > cur && a.sx < max)
                {
                    return  (a.sx - 1, y);
                }
            }
        }

        return (0, 0);
    }

    public long TuningFrequency(Point p) => ((long)p.X * 4000000L) + (long)p.Y;

    public long SolvePart1()
    {
        var points = this.input.Select(Parse);
        var area = DetectedArea(points, row: 2000000).OrderBy(_ => _.X);

        return area.Count();
    }

    public long SolvePart2()
    {
        var points = this.input
            .Select(Parse)
            .Select(_ => (Sensor: _.Item1, Dist: _.Item1.ManhattanDistance(_.Item2)))
            .ToList();

        var beacon = FindBeacon(points, max: 4000000);
        var tuning = TuningFrequency(beacon);
        
        return tuning;
    }
}
