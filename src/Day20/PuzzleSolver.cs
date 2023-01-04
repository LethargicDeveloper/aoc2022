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
            .Select((v, i) => (Order: i, Value: int.Parse(v)))
            .ToList();

        var crypto = new CircularList(list.ToList());

        foreach (var num in list)
        {
            crypto.Move(num.Order);
        }

        return crypto.GroveCoordinate();
    }

    public long SolvePart2()
    {
        return 0;
    }

   class CircularList : List<(int Order, int Value)>
   {
        int index = 0;

        public CircularList(List<(int, int)> list)
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
            var index = this.FindIndex(_ => _.Order == order);
            var item = this[index];
            var value = this[index].Value;

            if (value == 0) return;

            if (value > 0)
            {
                int newIndex = (index + value) % (this.Count - 1);
                this.RemoveAt(index);
                this.Insert(newIndex, item);
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
                    this[newIndex] = item;
                    index = newIndex;
                }
            }
        }

        public IEnumerable<int> GetNumbers()
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