using System.Net.Security;
using System.Text.RegularExpressions;

// 5793 - too high
// 1483 - too low
// 1381 - too low

public partial class PuzzleSolver
{
    readonly string input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("InputExample.txt");
    }

    public long SolvePart1()
    {
        var pairs = this.input
            .Split("\r\n\r\n")
            .Select(_ => _.Split("\r\n").ToList())
            .ToList();

        var orderedPackets = new List<int>();
        for (int i = 0; i < pairs.Count; ++i)
        {
            var pair = pairs[i];
            string p1 = pair[0];
            string p2 = pair[1];

            var inOrder = ArePacketsInOrder(p1, p2);
            if (inOrder)
                orderedPackets.Add(i + 1);
        }

        return orderedPackets.Sum();
    }

    bool ArePacketsInOrder(string p1, string p2)
    {
        while (true)
        {
            var data1 = GetNextData(ref p1);
            var data2 = GetNextData(ref p2);

            var isLeftInt = int.TryParse(data1, out var left);
            var isRightInt = int.TryParse(data2, out var right);

            if (isLeftInt && isRightInt)
            {
                if (left < right) return true;
                if (left > right) return false;
            }
            else if (!isLeftInt && !isRightInt)
            {

            }
        }
    }

    string GetNextData(ref string packet)
    {
        var intMatch = Regex.Match(packet, @"^[\[](\d+)");
        if (intMatch.Success)
        {
            packet = intMatch.Index + 2 > packet.Length - 1
                ? ""
                : $"[{packet.Substring(intMatch.Index + 2).Trim(',')}";
            return intMatch.Groups[1].Value;
        }

        if (packet.StartsWith("["))
        {
            int count = 0;
            for (int i = 1; i < packet.Length; ++i)
            {
                var c = packet[i];
                if (c == '[') count++;
                else if (c == ']') count--;

                if (count == 0)
                {
                    var list = packet[1..(i + 1)];
                    packet = packet[(i + 2)..];
                    if (!string.IsNullOrEmpty(packet))
                        packet = $"[{packet.Trim(',')}";
                    return list;
                }
            }
        }

        return "";
    }

    public long SolvePart2()
    {
        return 0;
    }
}
