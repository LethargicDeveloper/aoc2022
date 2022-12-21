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
            .Where(_ => _.v)
            .Sum(_ => _.i);
    }

    public long SolvePart2()
    {
        return 0;
    }

    class Packet
    {
        readonly List<object> packets;

        private Packet()
            : this(new List<object>())
        { }

        private Packet(List<object> packets)
        {
            this.packets = packets;
        }

        public static (Packet, Packet) Parse(List<string> packet)
            => packet.Select(CreatePacket).ToArray() switch { var a => (a[0], a[1]) };

        static Packet CreatePacket(string input)
        {
            var packet = new List<object>();
            var stack = new Stack<List<object>>();
            stack.Push(packet);

            var current = packet;
            for (int i = 0; i < input.Length; ++i)
            {
                var token = input[i];
                if (token == ',') continue;
                
                if (token == '[')
                {
                    var list = new List<object>();
                    current.Add(list);
                    stack.Push(current);
                    current = list;
                }
                else if (token == ']')
                {
                    if (!stack.TryPop(out current))
                        break;
                }
                else
                {
                    var index = input.IndexOfAny(new[] { ',', ']' }, i);
                    var value = input[i..index];
                    current.Add(int.Parse(value));
                    i = index;
                }
            }

            return new Packet((List<object>)packet[0]);
        }

        public static bool Compare((Packet, Packet) packets)
            => CompareInternal(packets).Item1;

        static (bool, bool exitEarly) CompareInternal((Packet, Packet) packets)
        {
            var (p1, p2) = packets;

            for(int i = 0; i < p1.packets.Count; ++i)
            {
                if (i > p2.packets.Count - 1)
                    return (false, true);

                var p1item = p1.packets[i];
                var p2item = p2.packets[i];

                if (p1item is int int1 && p2item is int int2)
                {
                    if (int1 < int2) return (true, true);
                    if (int1 > int2) return (false, true);
                }
                else if (p1item is List<object> list1 && p2item is List<object> list2)
                {
                    var result = CompareInternal((new Packet(list1), new Packet(list2)));
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

            return (true, p1.packets.Count < p2.packets.Count);
        }
    }
}
