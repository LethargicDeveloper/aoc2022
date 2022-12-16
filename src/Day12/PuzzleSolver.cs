using BenchmarkDotNet.Attributes;
using Priority_Queue;
using System.Text;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    public char[][] CreateMap()
        => this.input
            .Select(_ => _.Select(chr => chr).ToArray())
            .ToArray();

    public long SolvePart1()
    {
        var map = CreateMap();

        var pathFinder = new PathFinder(map);
        var steps = pathFinder.MinDistance();
        
        return steps;
    }

    public long SolvePart2()
    {
        var map = CreateMap();

        var dist = new List<long>();
        foreach (var startPos in GetStartPositions(map))
        {
            var pathFinder = new PathFinder(map, startPos);
            dist.Add(pathFinder.MinDistance());
        }

        return dist.Where(_ => _ > 0).Min();
    }

    IEnumerable<(int, int)> GetStartPositions(char[][] map)
    {
        var list = new List<(int, int)>();
        for (int y = 0; y < map.Length; ++y)
            for (int x = 0; x < map[0].Length; ++x)
                if (map[y][x] == 'a') list.Add((y, x));

        return list;
    }

    class PathFinder
    {
        readonly char[][] map;
        (int, int)? startPos = null;

        public PathFinder(char[][] map, (int, int)? startPos = null)
        {
            this.startPos = startPos;
            this.map = map;
        }

        public long MinDistance()
        {
            var (dist, prev) = Dijkstra();
            var path = ShortestPath(prev);
            return dist[path[^1]];
        }

        bool InRange(char height1, char height2)
            => ("yz".Contains(height1) && height2 == 'E') || (height2 != 'E' && (height2 <= height1 + 1));

        public ((int, int), (int, int)) GetStartAndEndPos()
        {
            var startPos = this.startPos ?? (0, 0);
            var endPos = (0, 0);
            for (int y = 0; y < map.Length; ++y)
                for (int x = 0; x < map[0].Length; ++x)
                {
                    var c = map[y][x];

                    if (c == 'S')
                    {
                        map[y][x] = 'a';
                        if (this.startPos == null)
                        {
                            startPos = (y, x);
                        }
                    }

                    if (c == 'E')
                    {
                        endPos = (y, x);
                    }
                }

            return (startPos, endPos);
        }

        List<(int, int)> ShortestPath(Dictionary<(int, int), (int, int)> prev)
        {
            var stack = new Stack<(int, int)>();
            var (_, target) = GetStartAndEndPos();

            while (true)
            {
                stack.Push(target);
                if (!prev.TryGetValue(target, out target))
                    break;
            }

            return stack.ToList();
        }

        (Dictionary<(int, int), long> dist, Dictionary<(int, int), (int, int)> prev) Dijkstra()
        {
            var (start, end) = GetStartAndEndPos();

            var queue = new SimplePriorityQueue<(int Y, int X)>();
            var distance = new Dictionary<(int, int), long>();
            var previous = new Dictionary<(int, int), (int, int)>();

            for (int y = 0; y < map.Length; ++y)
                for (int x = 0; x < map[0].Length; ++x)
                {
                    var v = (y, x);
                    distance[v] = v == start ? 0 : long.MaxValue;
                    queue.Enqueue(v, distance[v]);
                }
            
            while (queue.TryDequeue(out var u))
            {
                //PrintMap(u, distance);
                if (u == end) break;

                foreach (var v in Neighbors(u).Where(queue.Contains))
                {
                    if (!InRange(map[u.Y][u.X], map[v.Y][v.X]))
                        continue;

                    var alt = distance[u] + 1;
                    if (alt < distance[v])
                    {
                        distance[v] = alt;
                        previous[v] = u;

                        queue.UpdatePriority(v, alt);
                    }
                }
            }


            return (distance, previous);
        }

        void PrintMap((int, int) pos, Dictionary<(int, int), long> dest)
        {
            var sb = new StringBuilder();
            for (int y = 0; y < map.Length; ++y)
            {
                for (int x = 0; x < map[0].Length; ++x)
                {
                    if ((y, x) == pos)
                        sb.Append("@");
                    else
                    {
                        sb.Append((Math.Abs(dest[(y, x)]) > 100
                            ? map[y][x]
                            : (Math.Abs(dest[(y, x)]) > 9
                                ? "+"
                                : dest[(y, x)])));
                    }
                }

                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }

        List<(int Y, int X)> Neighbors((int Y, int X) point)
        {
            var points = new List<(int, int)>();
            if (point.X > 0)
                points.Add((point.Y, point.X - 1));

            if (point.X < map[0].Length - 1)
                points.Add((point.Y, point.X + 1));

            if (point.Y > 0)
                points.Add((point.Y - 1, point.X));

            if (point.Y < map.Length - 1)
                points.Add((point.Y + 1, point.X));

            return points;
        }
    }
}