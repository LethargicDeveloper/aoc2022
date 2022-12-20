using System.ComponentModel;
using System.Text;

public partial class PuzzleSolver
{
    readonly string input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("Input001.txt");
    }

    class Piece : IEquatable<Piece?>
    {
        public Point p { get; set; }
        public long i { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Piece);
        }

        public bool Equals(Piece? other)
        {
            return other is not null &&
                   p.Equals(other.p);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(p);
        }

        public static bool operator ==(Piece? left, Piece? right)
        {
            return EqualityComparer<Piece>.Default.Equals(left, right);
        }

        public static bool operator !=(Piece? left, Piece? right)
        {
            return !(left == right);
        }
    }

    public long SolvePart1(long stopAt = 10091)
    {
        var board = new HashSet<Piece>();
        int maxY = 0;
        int minX = 0;
        int maxX = 6;
        long count = 0;

        var lastPiece = new List<Point>();
        foreach (var pieceString in GetNextPiece())
        {
            count++;
            int minY = board.Count == 0 ? 0 : board.Select(_ => _.p.Y).Min() - 1;
            var piece = GetPointsFromPiece(pieceString);
            piece = AdjustToStartPos(piece, minY);
            lastPiece = piece;

            //if (minY == -53)
            //{
            //    Console.WriteLine(count);
            //    //console.writeline(miny);
            //    Console.ReadLine();
            //}

            char lastPattern = ' ';

            bool stop;
            foreach (var pattern in GetNextJetPattern())
            {
                lastPattern = pattern;
                //if (count == 29)
                //    PrintBoard(board, piece, pattern);

                if (pattern == '<')
                {
                    bool collision = piece.Any(_ =>
                        board.Select(_ => _.p).Contains((_.X - 1, _.Y)) || _.X - 1 < minX);
                    if (!collision)
                        piece = piece.Select(_ => new Point(_.X - 1, _.Y)).ToList();
                }
                else if (pattern == '>')
                {
                    bool collision = piece.Any(_ =>
                        board.Select(_ => _.p).Contains((_.X + 1, _.Y)) || _.X + 1 > maxX);
                    if (!collision)
                        piece = piece.Select(_ => new Point(_.X + 1, _.Y)).ToList();
                }

                //if (count == 29)
                //    PrintBoard(board, piece, pattern);

                stop = false;
                stop = piece.Any(_ =>
                    board.Select(_ => _.p).Contains((_.X, _.Y + 1)) || _.Y + 1 > maxY);
                if (!stop)
                {
                    piece = piece.Select(_ => new Point(_.X, _.Y + 1)).ToList();
                }

                if (stop)
                {
                    for (int i = 0; i < piece.Count; ++i)
                        board.Add(new Piece { p = piece[i], i = count });
                    
                    //board = CheckForTetris(board);
                    break;
                }
            }

            if (count == stopAt)
            {
                break;
            }

            //PrintBoard(board, lastPiece, ' ');
        }

        //PrintBoard(board, lastPiece, ' ');
        //PrintBoardToFile(board, lastPiece, ' ');
        //FindPattern(board);
        FindPatternBetter(board);
        return Math.Abs(board.Select(_ => _.p.Y).Min()) + 1;
    }

    HashSet<Piece> CheckForTetris(HashSet<Piece> board)
    {
        var line = board
            .GroupBy(_ => _.p.Y)
            .Where(_ => _.Count() == 7)
            .OrderByDescending(_ => _.Key)
            .FirstOrDefault()
            ?.Select(_ => _)
            .ToList();

        if (line != null)
        {
            board.RemoveWhere(line.Contains);
            var points = board.Where(_ => _.p.Y < line[0].p.Y).ToList();
            board.RemoveWhere(points.Contains);

            for (int i = 0; i < points.Count; ++i)
                points[i] = (new Piece { p = (points[i].p.X, points[i].p.Y + 1), i = points[i].i });

            for (int i = 0; i < points.Count; ++i)
                board.Add(points[i]);

            board = CheckForTetris(board);
        }

        return board;
    }

    void FindPatternBetter(HashSet<Piece> board)
    {
        Console.WriteLine("Searching for patterns...");

        var spread = 2000;
        var minY = board.Select(_ => _.p.Y).Min();

        bool running = true;
        var bestMatch = int.MinValue;
        var points = new List<Piece>();

        while (running)
        {
            running = false;
            points = board
                .Where(_ => _.p.Y >= minY && _.p.Y <= minY + spread)
                .Select(_ => new Piece { p = (_.p.X, _.p.Y - minY) })
                .ToList();

            for (int y = 0; y > minY + spread; --y)
            {
                var checkPoints = board
                    .Where(_ => _.p.Y <= y && _.p.Y >= y - spread)
                    .Select(_ => new Piece { p = (_.p.X, _.p.Y - y + spread) })
                    .ToList();

                if (checkPoints.Count == points.Count && checkPoints.All(points.Contains))
                {
                    if (y > bestMatch)
                    {
                        running = true;
                        bestMatch = y;
                    }

                    break;
                }
            }

            minY++;
        }

        if (bestMatch == int.MinValue)
        {
            Console.WriteLine("No match found :(");
            return;
        }    

        Console.WriteLine($"""

            Y: {Math.Abs(bestMatch)}
            Pre: {board.Where(_ => _.p.Y > bestMatch).Select(_ => _.i).Distinct().Count()}

            """);

        Console.WriteLine("Finding next match...");
        
        for (int y = bestMatch - spread; y > minY; --y)
        {
            var checkPoints = board
                .Where(_ => _.p.Y <= y && _.p.Y >= y - spread)
                .Select(_ => new Piece { p = (_.p.X, _.p.Y - y + spread) })
                .ToList();

            if (checkPoints.Count == points.Count && checkPoints.All(points.Contains))
            {
                var newSpread = Math.Abs(y) - Math.Abs(bestMatch);
                Console.WriteLine($"""

                    Post: {board.Where(_ => _.p.Y <= bestMatch && _.p.Y >= bestMatch - newSpread).Select(_ => _.i).Distinct().Count()}
                    Spread: {newSpread}

                    """);

                break;
            }
        }

        Console.WriteLine($"minY: {minY}");

        var maxPiece = board.Max(_ => _.i);
        var lastPiece = board.Where(_ => _.i == maxPiece).Select(_ => _.p).ToList();
        PrintBoardToFile(board, lastPiece, ' ');
    }

    void FindPattern(HashSet<Piece> board)
    {
        Console.WriteLine("Searching");
        var miny = board.Select(_ => _.p.Y).Min();
        // 53; -44
        for (int spread = 2500; spread * 3 < Math.Abs(miny); ++spread)
        {
            for (int y = 1800; y - (spread * 2) > miny; --y)
            {
                int maxy1 = y;
                int miny1 = y - spread;
                
                int maxy2 = y - spread;
                int miny2 = y - (spread * 2);

                int maxy3 = y - (spread * 2);
                int miny3 = y - (spread * 3);

                var p1 = board
                    .Where(_ => _.p.Y <= maxy1 && _.p.Y > miny1)
                    .OrderBy(_ => _.p.Y)
                    .ThenBy(_ => _.p.X)
                    .ToList();

                var p2 = board
                    .Where(_ => _.p.Y <= maxy2 && _.p.Y > miny2)
                    .Select(_ => new Point(_.p.X, _.p.Y + spread))
                    .OrderBy(_ => _.Y)
                    .ThenBy(_ => _.X)
                    .ToList();

                var p1p = p1.Select(_ => _.p).ToList();
                //PrintCompare(p1p, p2, miny);
                if (p1p.Count == p2.Count && p1p.All(p2.Contains))
                {
                    PrintCompare(p1p, p2, miny);
                    Console.WriteLine($"Spread: {spread}");
                    Console.WriteLine($"Y: {y}");

                    var post= p1.Select(_ => _.i).Distinct().Count();
                    var pre = board
                        .Where(_ => _.p.Y > maxy1)
                        .Select(_ => _.i)
                        .Distinct()
                        .Count();

                    Console.WriteLine($"Pre: {pre}");
                    Console.WriteLine($"Post: {post}");
                    return;
                }
            }
        }
    }

    public decimal SolvePart2()
    {
        decimal stoppedRocks = 1000000000000;
        decimal prePatternHeight = 25; // y;
        decimal startPattern = 0; // pre;
        decimal endPattern = 0; // post;
        decimal patternHeight = 52; // spread
        decimal answer = (((stoppedRocks - startPattern) / endPattern) * patternHeight) + prePatternHeight;
        return answer;
    }

    void PrintCompare(IEnumerable<Point> p1, IEnumerable<Point> p2, int miny)
    {
        Console.Clear();

        var minY = Math.Min(
            p1.Select(_ => _.Y).Min(),
            p2.Select(_ => _.Y).Min());

        var maxY = Math.Max(
            p1.Select(_ => _.Y).Max(),
            p2.Select(_ => _.Y).Max());

        var sb = new StringBuilder();
        for (int y = minY; y <= maxY; ++y)
        {
            var line = "|.......| |.......|".ToArray();

            for (int x = 0; x < 7; ++x)
            {
                if (p1.Contains((x, y)))
                    line[x + 1] = '#';

                if (p2.Contains((x, y)))
                    line[x + 11] = '#';
            }

            sb.AppendLine(new string(line));
        }

        sb.AppendLine($"+-------+ +-------+");
        Console.WriteLine(sb.ToString());

        Console.ReadLine();
    }

    void PrintBoard(HashSet<Point> board, List<Point> piece, char pattern)
    {
        Console.Clear();

        board.OrderBy(_ => _.Y).ThenBy(_ => _.X);
        int minY = piece.Select(_ => _.Y).Min();

        var sb = new StringBuilder();
        sb.AppendLine($"Pattern: {pattern}");
        sb.AppendLine();
        for (int y = minY; y <= 0; ++y)
        {
            sb.Append("|");
            for (int x = 0; x < 7; ++x)
            {
                if (piece.Contains((x, y)))
                    sb.Append("@");
                else if (board.Contains((x, y)))
                    sb.Append("#");
                else sb.Append(".");
            }
            sb.Append("|");
            sb.AppendLine();
        }

        sb.AppendLine("+-------+");
        Console.WriteLine(sb.ToString());

        Console.ReadLine();
    }

    void PrintBoardToFile(HashSet<Piece> board, List<Point> piece, char pattern)
    {
        board.OrderBy(_ => _.p.Y).ThenBy(_ => _.p.X);
        int minY = piece.Select(_ => _.Y).Min();

        var sb = new StringBuilder();
        for (int y = minY; y <= 0; ++y)
        {
            sb.Append("|");
            for (int x = 0; x < 7; ++x)
            {
                if (piece.Contains((x, y)))
                    sb.Append("@");
                else if (board.Select(_ => _.p).Contains((x, y)))
                    sb.Append("#");
                else sb.Append(".");
            }
            sb.Append("|");
            if (y % 10 == 0)
                sb.Append($" {y} || {board.Where(_ => _.p.Y == y).Select(_ => _.i).Max()}");
            sb.AppendLine();
        }

        sb.AppendLine("+-------+");
        File.WriteAllText("game.txt", sb.ToString());
    }


    List<Point> GetPointsFromPiece(string piece)
    {
        var points = new List<Point>();
        var lines = piece.Split("\r\n");
        
        for (int x = 0; x < lines[0].Length; ++x)
            for (int y = 0; y < lines.Length; ++y)
                if (lines[y][x] == '#')
                    points.Add((x, y));

        return points;
    }

    List<Point> AdjustToStartPos(List<Point> points, int minY)
    {
        var height = points.Select(_ => _.Y).Max();
        return points
                .Select(_ => new Point(_.X + 2, _.Y - 3 + minY - height))
                .ToList();
    }

    static int currentPiece = -1;
    IEnumerable<char> GetNextJetPattern()
    {
        while (true)
        {
            currentPiece = (currentPiece + 1) % this.input.Length;
            yield return this.input[currentPiece];
        }

    }

    IEnumerable<string> GetNextPiece()
    {
        while (true)
            for (int i = 0; i < Pieces.Count; ++i)
                yield return Pieces[i][0];
    }

    static List<string[]> Pieces = new()
    {
        new[]
        {
            "####"
        },
        new[]
        {
            """
            .#.-
            ###-
            .#.-
            """
        },
        new[]
        {
            """
            ..#-
            ..#-
            ###-
            """
        },
        new[]
        {
            """
            #---
            #---
            #---
            #---
            """
        },
        new[]
        {
            """
            ##--
            ##--
            """
        }
    };
}
