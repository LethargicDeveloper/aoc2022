public partial class PuzzleSolver
{
    // -3629
    // -4832
    // -19918
    // -5226
    // -183
    // -5631
    // -16205

    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllLines("Input001.txt").ToList();
    }

    public long SolvePart1()
    {
        var list = this.input
            .Select((v, i) => (Order: (long)i, Value: long.Parse(v)))
            .ToList();

        var crypto = new CircularList(list.ToList());

        //Console.WriteLine(string.Join(", ", crypto));

        foreach (var num in list)
        {
            crypto.Move(num.Order);
        }

        //Console.WriteLine(string.Join(", ", crypto));

        return crypto.GroveCoordinate();
    }

    public long SolvePart2()
    {
        return 0;
    }

   class CircularList : List<(long Order, long Value)>, IEnumerable<(long Order, long Value)>
   {
        int index = 0;

        public CircularList(List<(long, long)> list)
            : base(list) { }

        public long GroveCoordinate()
        {
            var zero = this.FindIndex(0, _ => _.Value == 0);
            var n1000 = this[(zero + 1000) % (this.Count)].Value;
            var n2000 = this[(zero + 2000) % (this.Count)].Value;
            var n3000 = this[(zero + 3000) % (this.Count)].Value;

            return n1000 + n2000 + n3000;
        }

        public void Move(long order)
        {
            var item = this.Select((v, i) => (i, v)).First(_ => _.v.Order == order);
            var value = item.v.Value;
            var index = item.i;

            if (value == 0) return;

            if (value > 0)
            {
                int newIndex = (int)((index + value) % (this.Count));

                this.Remove(item.v);
                this.Insert(newIndex, item.v);
            }
            else
            {
                var newIndex = index;
                for (int i = 0; i < Math.Abs(value); ++i)
                {
                    newIndex--;
                    newIndex = newIndex < 0 ? this.Count - 1 : newIndex;

                    var prev = this[newIndex];
                    this[index] = prev;
                    this[newIndex] = item.v;
                    index = newIndex;
                }
            }
        }

        public IEnumerable<long> GetNumbers()
        {
            while (true)
            {
                yield return this[index].Value;
                index++;
                index = index > this.Count - 1 ? 0 : index;
            }
        }
    }
}