using System.Text;
using System.Text.RegularExpressions;

public partial class PuzzleSolver2
{
    readonly string input;

    public PuzzleSolver2()
    {
        this.input = File.ReadAllText("InputExample.txt");
    }

    public long SolvePart2()
    {
        var (cube, cmds) = ParseCube();
        var monkeyCube = new MonkeyMapCube(cube, cmds);
        var password = monkeyCube.Solve();

        return password;
    }

    (Cube, (int move, char rot)[]) ParseCube()
    {
        //var faces = new[]
        //{
        //    new[] { 0, 1, 4},
        //    new[] { 0, 5, 0},
        //    new[] { 2, 6, 0},
        //    new[] { 3, 0, 0},
        //};

        var faces = new[]
        {
            new[] { 0, 0, 1, 0 },
            new[] { 3, 2, 5, 0 },
            new[] { 0, 0, 6, 4 }
        };

        var inputs = this.input.Split("\r\n\r\n");
        var mapInputs = inputs[0].Split("\r\n");
        var cmdInputs = inputs[1];

        var size = mapInputs.Min(_ => _.Where(c => c != ' ').Count());
        var cube = new Cube(size, faces.Select(_ => _.ToList()).ToList());

        for (int y = 0; y < faces.Length; ++y)
        {
            for (int x = 0; x < faces[0].Length; ++x)
            {
                var index = faces[y][x];
                if (index == 0) continue;

                var face = new List<char[]>();
                for (int y1 = y * size; y1 < (y * size) + size; ++y1)
                {
                    var line = mapInputs[y1].Substring(x * size, size).ToArray();
                    face.Add(line);
                }

                cube[index] = face.ToArray();
            }
        }

        var cmds = MyRegex()
            .Matches(cmdInputs)
            .Select(_ => (int.Parse(_.Groups[1].Value), _.Groups[2].Value.ElementAtOrDefault(0)))
            .ToArray();

        return (cube, cmds);
    }

    [GeneratedRegex("(\\d+)(.)?")]
    private static partial Regex MyRegex();
}

class Cube : Dictionary<int, char[][]>
{
    public Cube(int size, List<List<int>> faces)
    {
        this.Size = size;
        this.Faces = faces;
    }

    public int Size { get; init; }
    public List<List<int>> Faces { get; init; } = new();
}

class MonkeyMapCube
{
    const int Right = 0;
    const int Down = 1;
    const int Left = 2;
    const int Up = 3;

    static readonly Point[] dirMap = new[]
    {
        new Point(1, 0),
        new Point(0, 1),
        new Point(-1, 0),
        new Point(0, -1)
    };

    readonly Cube cube;
    readonly (int move, char rot)[] cmds;

    public MonkeyMapCube(Cube cube, (int, char)[] cmds)
    {
        this.cube = cube;
        this.cmds = cmds;
    }

    public long Solve()
    {
        var (p, facing) = GetEndingPos();
        var (pos, face) = p;

        var offset = new Point(0, 0);
        for (int y = 0; y < cube.Faces.Count; ++y)
            for (int x = 0; x < cube.Faces[0].Count; ++x)
                if (cube.Faces[y][x] == face)
                {
                    offset = (x, y);
                    break;
                }

        pos = (pos.X + (cube.Size * offset.X), pos.Y + (cube.Size * offset.Y));

        return (1000 * (pos.Y + 1)) + (4 * (pos.X + 1)) + facing;
    }

    void PrintCube()
    {
        Console.Clear();
        var sb = new StringBuilder();

        for (int y = 0; y < cube.Faces.Count; ++y)
        {
            for (int y1 = 0; y1 < cube.Size; ++y1)
            {
                for (int x = 0; x < cube.Faces[0].Count; ++x)
                {
                    var faceIndex = cube.Faces[y][x];
                    if (faceIndex == 0)
                    {
                        sb.Append(new string(' ', cube.Size));
                    }
                    else
                    {
                        var face = cube[faceIndex];
                        sb.Append(new string(face[y1]));
                    }
                }

                sb.AppendLine();
            }
        }

        Console.WriteLine(sb.ToString());
    }

    ((Point pos, int face), int facing) GetEndingPos()
    {
        var pos = GetStartingPos();
        var facing = Right;

        foreach (var cmd in cmds)
        {
            (pos, facing) = GetNextPos(pos, facing, cmd);
        }

        return (pos, facing);
    }

    ((Point, int), int) GetNextPos((Point, int) pos, int facing, (int move, char rot) cmd)
    {
        for (int i = 0; i < cmd.move; ++i)
        {
            (pos, facing, var hit) = FindNextPos(pos, facing, cmd.rot);
            if (hit) return (pos, facing);
        }

        return (pos, GetNextFacing(facing, cmd.rot));
    }

    ((Point, int), int, bool) FindNextPos((Point, int) p, int facing, char rot)
    {
        var max = cube.Size - 1;
        var (pos, face) = p;

        var dir = dirMap[facing];
        var newPos = pos + dir;
        var newFace = face;
        var newFacing = facing;

        cube[face][pos.Y][pos.X] = facing switch
        {
            Right => '>',
            Down => 'V',
            Left => '<',
            Up => '^',
            _ => throw new Exception("Invalid facing")
        };

        PrintCube();

        if (facing == Right && newPos.X > max)
        {
            (newPos, newFace, newFacing) = face switch
            {
                1 => ((max, max - pos.Y), 4, Left),
                2 => ((0, pos.Y), 5, Right),
                3 => ((0, pos.Y), 2, Right),
                4 => ((max, max - pos.Y), 1, Left),
                5 => ((max - pos.Y, 0), 4, Down),
                6 => ((0, pos.Y), 4, Right),
                _ => throw new Exception("Invalid face")
            };
        }

        if (facing == Down && newPos.Y > max)
        {
            (newPos, newFace, newFacing) = face switch
            {
                1 => ((pos.X, 0), 5, Down),
                2 => ((0, max - pos.X), 6, Right),
                3 => ((max - pos.X, max), 6, Up),
                4 => ((0, max - pos.X), 3, Right),
                5 => ((pos.X, 0), 6, Down),
                6 => ((max - pos.X, max), 3, Up),
                _ => throw new Exception("Invalid face")
            };
        }

        if (facing == Left && newPos.X < 0)
        {
            (newPos, newFace, newFacing) = face switch
            {
                1 => ((pos.Y, 0), 2, Down),
                2 => ((max, pos.Y), 3, Left),
                3 => ((max - pos.Y, max), 4, Up),
                4 => ((max, pos.Y), 6, Left),
                5 => ((max, pos.Y), 2, Left),
                6 => ((max - pos.Y, max), 2, Up),
                _ => throw new Exception("Invalid face")
            };
        }

        if (facing == Up && newPos.Y < 0)
        {
            (newPos, newFace, newFacing) = face switch
            {
                1 => ((max - pos.X, 0), 3, Down),
                2 => ((0, pos.X), 1, Right),
                3 => ((max - pos.X, 0), 1, Down),
                4 => ((max, max - pos.X), 5, Left),
                5 => ((pos.X, max), 1, Up),
                6 => ((pos.X, max), 5, Up),
                _ => throw new Exception("Invalid face")
            };
        }

        return cube[newFace][newPos.Y][newPos.X] == '#'
            ? (p, GetNextFacing(facing, rot), true)
            : ((newPos, newFace), newFacing, false);
    }

    int GetNextFacing(int facing, char rot)
    {
        if (rot == '\0') return facing;

        facing += rot == 'R' ? 1 : -1;
        return Mod(facing, 4);
    }

    int Mod(int pos, int mod)
    {
        var r = pos % mod;
        return r < 0 ? r + mod : r;
    }

    (Point, int) GetStartingPos()
    {
        var face = cube.Faces[0].First(_ => _ > 0);
        return ((cube[face][0].ToList().FindIndex(_ => _ != ' '), 0), face);
    }
}