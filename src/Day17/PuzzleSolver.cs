using System.Text;

public partial class PuzzleSolver
{
    readonly string input;
    readonly List<int> board = new List<int>() { Floor };
    readonly Dictionary<int, int> heightMap = new Dictionary<int, int>();

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("Input001.txt");
        PlayGame();
    }

    public long SolvePart1()
    {
        return board.Count - 1;
    }

    public long SolvePart2()
    {
        return FindPattern();
    }

    void PlayGame()
    {
        int top = 1;
        int currentJet = 0;

        for (int round = 0; round < 4000; ++round)
        {
            var piece = Pieces[round % 5];
            var y = top + 3;

            while (true)
            {
                if (this.input[currentJet++ % this.input.Length] == '<')
                {
                    var left = ShiftLeft(piece);
                    if (!IsCollision(left, y))
                        piece = left;
                }
                else
                {
                    var right = ShiftRight(piece);
                    if (!IsCollision(right, y))
                        piece = right;
                }

                if (IsCollision(piece, y - 1))
                {
                    var setPiece = SetPiece(piece);
                    for (int i = setPiece.Length - 1; i >= 0; --i)
                    {
                        var index = y + (setPiece.Length - i) - 1;
                        if (index >= board.Count)
                        {
                            board.Add(setPiece[i]);
                        }
                        else
                        {
                            board[index] |= setPiece[i];
                        }
                    }

                    break;
                }

                y--;
            }

            top = board.Count;

            heightMap[round + 1] = top - 1;
        }
    }

    long FindPattern()
    {
        int window = 10;
        int prePatternHeight = 0;
        bool found = false;

        while (window * 2 < board.Count)
        {
            for (int y = 0; y < board.Count - (window * 2); ++y)
            {
                var pattern1 = board.Skip(y).Take(window);
                var pattern2 = board.Skip(y + window).Take(window);
                if (Enumerable.SequenceEqual(pattern1, pattern2))
                {
                    prePatternHeight = y - 1;
                    found = true;
                    goto DONE; // yup; it's a goto
                };
            }

            window++;
        }

        DONE:

        if (!found)
        {
            Console.WriteLine("No pattern found!");
            return 0;
        }

        var startPattern = heightMap.SkipWhile(_ => _.Value < prePatternHeight).First();
        var startPatternRound = startPattern.Key;
        var endPatternRound = heightMap.Where(_ => _.Value == heightMap[startPatternRound] + window).Last().Key - startPatternRound;

        long p1 = 1000000000000 - startPatternRound;
        long p2q = p1 / endPatternRound;
        int p2r = (int)(p1 % endPatternRound);
        int p3 = heightMap[startPatternRound + p2r] - prePatternHeight;
        return prePatternHeight + p2q * window + p3;
    }

    bool IsCollision(int[] piece, int y)
    {
        for (int i = 0; i < piece.Length; ++i)
        {
            var p = piece[i];
            var index = y + piece.Length - i - 1;
            var line = board.Count > index ? board[index] : Wall;
            if ((line ^ p) != (line | p))
                return true;
        }

        return false;
    }

    int[] SetPiece(int[] piece)
    {
        var line = new int[piece.Length];
        for (int i = 0; i < piece.Length; ++i)
            line[i] = Wall | piece[i];

        return line;
    }

    int[] ShiftLeft(int[] piece)
    {
        var newPiece = new int[piece.Length];
        for (int i = 0; i < piece.Length; ++i)
        {
            newPiece[i] = piece[i] << 1;
        }

        return newPiece;
    }

    int[] ShiftRight(int[] piece)
    {
        var newPiece = new int[piece.Length];
        for (int i = 0; i < piece.Length; ++i)
        {
            newPiece[i] = piece[i] >> 1;
        }

        return newPiece;
    }

    void ExportGame()
    {
        var sb = new StringBuilder();
        for (int y = board.Count - 1; y > 0; --y)
        {
            string height = "";
            string round = "";

            var heights = heightMap.Where(_ => _.Value == y).ToList();
            if (heights.Count > 0)
            {
                var currentHeight = heights.MaxBy(_ => _.Key);
                round = currentHeight.Key.ToString();
                height = currentHeight.Value.ToString();
            }

            var line = Convert.ToString(board[y], 2).PadLeft(9, '0')
                .Replace("1", "#")
                .Replace("0", ".");
            sb.AppendLine($"|{line[1..^1]}| {round} - {height}");
        }
        sb.AppendLine("+-------+");
        File.WriteAllText("game.txt", sb.ToString());
    }

    static int Floor = 0b111111111;
    static int Wall = 0b100000001;
    static int[][] Pieces = new[]
    {
        new int[] { 0b000111100 },
        new int[] { 0b000010000, 0b000111000, 0b000010000 },
        new int[] { 0b000001000, 0b000001000, 0b000111000 },
        new int[] { 0b000100000, 0b000100000, 0b000100000, 0b000100000 },
        new int[] { 0b000110000, 0b000110000 }
    };
}
