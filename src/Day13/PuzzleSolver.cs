using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

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

        var order = new List<int>();
        for (int i = 0; i < pairs.Count; ++i)
        {
            var pair = pairs[i];
            string p1 = pair[0];
            string p2 = pair[1];

            order.Add(ComparePackets(p1, p2) ? i + 1 : 0);
        }

        return order.Sum();
    }

    bool ComparePackets(string p1, string p2, bool comparingList = false)
    {
        while (!string.IsNullOrEmpty(p1) || !string.IsNullOrEmpty(p2))
        {
            if (p1.StartsWith("[") && !p2.StartsWith("["))
                p2 = "[" + p2;
            if (p2.StartsWith("[") && !p1.StartsWith("["))
                p1 = "[" + p1;

            var data1 = GetNextData(ref p1);
            var data2 = GetNextData(ref p2);

            if (string.IsNullOrEmpty(data1) && !string.IsNullOrEmpty(data2))
                return true;

            if (string.IsNullOrEmpty(data2) && !string.IsNullOrEmpty(data1))
                return comparingList;

            var isLeftInt = int.TryParse(data1, out int left);
            var isRightInt = int.TryParse(data2, out int right);

            if (isLeftInt && isRightInt)
            {
                if (right < left) return false;
            }
            else if (!isLeftInt && !isRightInt)
            {
                if (!ComparePackets(data1, data2, true))
                    return false;
            }
            else
            {
                data1 = isLeftInt ? $"[{data1}]" : data1;
                data2 = isRightInt ? $"[{data2}]" : data2;
                if (!ComparePackets(data1, data2, true))
                    return false;
            }
        }

        var noMismatch = string.IsNullOrEmpty(p1);
        return noMismatch;
    }

    string GetNextData(ref string packet)
    {
        var intMatch = Regex.Match(packet, @"^[\[,]?(\d+)[,\]]");
        if (intMatch.Success)
        {
            packet = packet.Substring(intMatch.Index + 2);
            if (string.IsNullOrEmpty(packet))
                return "";

            packet = packet[0] == ']' ? packet[1..] : packet;
            return intMatch.Groups[1].Value;
        }
        else
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
                    return list;
                }
            }

            return "";
        }
    }

    public long SolvePart2()
    {
        return 0;
    }
}
