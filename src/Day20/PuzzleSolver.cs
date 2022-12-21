using System;

public partial class PuzzleSolver
{
    // -3629
    // -19918
    // -5226
    // -183

    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllLines("Input001.txt").ToList();
    }


    public long SolvePart1()
    {
        var list = this.input.Select(long.Parse).ToList();
        var crypto = new CircularList(list.ToList());

        //Console.WriteLine(string.Join(", ", crypto));

        foreach (var num in list)
        {
            crypto.Move(num);
            //Console.WriteLine(string.Join(", ", crypto));
        }

        return crypto.GroveCoordinate();
    }

    public long SolvePart2()
    {
        return 0;
    }

    class CircularList : List<long>
    {
        private int index;

        public CircularList(IEnumerable<long> list)
            : base(list) { }

        public long Current() => this[index];

        public long Next()
        {
            this.index++;
            this.index %= Count;

            return this[index];
        }

        public long Previous()
        {
            this.index--;
            if (this.index < 0)
                this.index = Count - 1;
            return this[index];
        }

        public void Move(long item)
        {
            if (item == 0) return;

            while (this.Next() != item) { }

            if (item > 0)
            {
                for (int i = 0; i < item; ++i)
                {
                    var current = (i: this.index, v: this[this.index]);
                    var next = Next();
                    this[current.i] = next;
                    this[this.index] = current.v;
                }
            }
            else
            {
                for (int i = 0; i < Math.Abs(item); ++i)
                {
                    var current = (i: this.index, v: this[this.index]);
                    var prev = Previous();
                    this[current.i] = prev;
                    this[this.index] = current.v;
                }
            }
        }

        public long GroveCoordinate()
        {
            while (this.Next() != 0) { }

            var l = new List<long>() { 0 };

            for (int i = 0; i < 3000; ++i)
                l.Add(this.Next());

            return l[1000] + l[2000] + l[3000];
        }
    }
}