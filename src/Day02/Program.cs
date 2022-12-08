var input = File.ReadLines("Input001.txt");

input
    .Select(_ => (_[0] - '@', _[2] - 'W'))
    .Select(_ => _ switch
    {
        var (p1, p2) when p1 == p2 => 3 + p2,
        (1, 2) => 8,
        (2, 3) => 9,
        (3, 1) => 7,
        var (_, p2) => p2
    })
    .Sum()
    .Log("Part 1");

input    
    .Select(_ => (_[0] - '@', _[2] - 'W'))
    .Select(_ =>
    {
        var (p1, p2) = _;
        return p2 == 1 ? (--p1 < 1 ? 3 : p1) : p2 == 3 ? 6 + (++p1 > 3 ? 1 : p1) : 3 + p1;
    })
    .Sum()
    .Log("Part 2");