using System.Security;

public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadLines("Input001.txt").ToList();
    }

    private Dictionary<string, Dir> ParseFileSystem()
    {
        var dict = new Dictionary<string, Dir>();

        Dir? currentDir = null;
        for (int i = 0; i < this.input.Count; ++i)
        {
            var line = this.input[i];
            if (line.StartsWith("$ cd "))
            {
                var dir = line[5..];
                if (dir == "..")
                {
                    currentDir = currentDir?.Parent;
                }
                else
                {
                    var fullName = (currentDir?.FullName ?? "") + dir + (currentDir != null ? "/" : "");
                    if (!dict.TryGetValue(fullName, out currentDir))
                    {
                        currentDir = new Dir
                        {
                            Name = dir
                        };
                        dict[fullName] = currentDir;
                    }

                    for (i += 2; i < input.Count && input[i][0] != '$'; ++i)
                    {
                        var item = input[i];
                        if (item.StartsWith("dir"))
                        {
                            if (!dict.TryGetValue(item[4..], out var childDir))
                            {
                                childDir = new Dir
                                {
                                    Name = item[4..],
                                    Parent = currentDir
                                };
                                dict[childDir.FullName] = childDir;
                            }

                            currentDir.Children.Add(childDir);
                        }
                        else
                        {
                            currentDir.Size += long.Parse(item.Split(' ')[0]);
                        }
                    }
                    --i;
                    continue;
                }
            }
        }

        return dict;
    }

    public long SolvePart1()
        => ParseFileSystem()
            .Select(_ => _.Value.TotalSize)
            .Where(_ => _ <= 100000)
            .Sum();

    public long SolvePart2()
    {
        var dict = ParseFileSystem();
        var unused = 70000000 - dict["/"].TotalSize;

        return dict
            .Where(_ => _.Key != "/")
            .Select(_ => _.Value.TotalSize)
            .Where(_ => unused + _ > 30000000)
            .Min();
    }

    class Dir
    {
        public string Name { get; init; } = string.Empty;
        public string FullName => Parent?.FullName == null ? Name : Parent.FullName + Name + "/";
        public long Size { get; set; }
        public List<Dir> Children { get; init; } = new List<Dir>();
        public Dir? Parent { get; set; }
        public long TotalSize => Size + Children.Sum(_ => _.TotalSize);
    }
}
