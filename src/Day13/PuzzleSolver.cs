using MoreLinq;
using System.Diagnostics;

public partial class PuzzleSolver
{
    readonly string input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("Input001.txt");
    }

    public long SolvePart1()
    {
        return this.input
            .Split("\r\n\r\n")
            .Select(_ => _.Split("\r\n").ToList())
            .Select(Packet.Parse)
            .Select((p, i) => (i: i + 1, v: Packet.Compare(p)))
            .Where(_ => _.v <= -1)
            .Sum(_ => _.i);
    }

    public long SolvePart2()
    {
        var packetInput = this.input
             .Split("\r\n\r\n")
             .Select(_ => _.Split("\r\n").ToList())
             .ToList();

        packetInput.Add(new List<string> { "[[2]]", "[[6]]" });

        var packets = packetInput
             .Select(Packet.Parse)
             .Select(_ => new[] { _.Item1, _.Item2 })
             .SelectMany(_ => _)
             .OrderBy(_ => _, new Packet())
             .Select((v, i) => (i: i + 1, v))
             .SkipWhile(_ => _.v.Input != "[[2]]")
             .TakeUntil(_ => _.v.Input == "[[6]]")
             .ToList();

        return packets[0].i * packets[^1].i;
    }

    class Packet : IComparer<Packet>
    {
        readonly string input;
        readonly List<object> packets;

        public Packet()
            : this("", new List<object>())
        { }

        [DebuggerStepThrough]
        private Packet(string input, List<object> packets)
        {
            this.input = input;
            this.packets = packets;
        }

        public string Input => this.input;

        public static (Packet, Packet) Parse(List<string> packet)
            => packet.Select(CreatePacket).ToArray() switch { var a => (a[0], a[1]) };

        static Packet CreatePacket(string input)
        {
            var packet = new List<object>();
            var stack = new Stack<List<object>>();
            stack.Push(packet);

            var current = packet;
            var doublePop = false; // couldn't be bothered.
            for (int i = 1; i < input.Length; ++i)
            {
                var token = input[i];
                if (token == ',') continue;
                
                if (token == '[')
                {
                    doublePop = true;
                    var list = new List<object>();
                    current.Add(list);
                    stack.Push(list);
                    current = list;
                }
                else if (token == ']')
                {
                    if (doublePop)
                    {
                        stack.TryPop(out _);
                        doublePop = false;
                    }

                    if (!stack.TryPop(out current))
                        break;
                }
                else
                {
                    var index = input.IndexOfAny(new[] { ',', ']' }, i);
                    var value = input[i..index];
                    current.Add(int.Parse(value));
                    i = index - 1;
                }
            }

            return new Packet(input, packet);
        }

        public int Compare(Packet? x, Packet? y)
            => Compare((x ?? new Packet(), y ?? new Packet()));

        public static int Compare((Packet, Packet) packets)
            => CompareInternal(packets).Item1;

        static (int, bool exitEarly) CompareInternal((Packet, Packet) packets)
        {
            var (p1, p2) = packets;

            for(int i = 0; i < p1.packets.Count; ++i)
            {
                if (i > p2.packets.Count - 1)
                    return (1, true);

                var p1item = p1.packets[i];
                var p2item = p2.packets[i];

                if (p1item is int int1 && p2item is int int2)
                {
                    if (int1 < int2) return (-1, true);
                    if (int1 > int2) return (1, true);
                }
                else if (p1item is List<object> list1 && p2item is List<object> list2)
                {
                    var result = CompareInternal((new Packet("", list1), new Packet("", list2)));
                    if (result.Item2) return result;
                }
                else
                {
                    var v1 = p1item is List<object> ? (List<object>)p1item : new List<object> { p1item }; 
                    var v2 = p2item is List<object> ? (List<object>)p2item : new List<object> { p2item };

                    p1.packets[i] = v1;
                    p2.packets[i] = v2;
                    i--;
                }
            }

            var exitEarly = p1.packets.Count < p2.packets.Count;
            var equal = p1.packets.Count == p2.packets.Count ? 0 : -1;
            return (equal, exitEarly);
        }
    }
}
