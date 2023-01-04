public partial class PuzzleSolver
{
    readonly string input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("InputExample.txt");
    }
        
    public long SolvePart1()
    {
        var dec = this.input
            .Split("\r\n")
            .Select(_ => new Snafu(_))
            .Select(_ => _.ToDecimal())
            .Sum();

        return 0;
    }

    public long SolvePart2()
    {
        return 0;
    }

    struct Snafu
    {
        readonly string value;

        public Snafu(string value)
        {
            this.value = value;
        }

        public int ToDecimal()
        {
            int dec = 0;
            for (int i = 0; i < value.Length; ++i)
            {
                char digit = value[i];
                int place = (int)Math.Pow(5, (value.Length - i - 1));
                dec += digit switch
                {
                    '-' => -1 * place,
                    '=' => -2 * place,
                    var n => int.Parse(n.ToString()) * place
                };
            }

            return dec;
        }

        public static Snafu FromDecimal(int dec)
        {
            int search = 0;

            var queue = new Queue<int>();
            queue.Enqueue(0);

            while (queue.TryDequeue(out int val))
            {
                if (val == search) break;
            }
        }
    }
}