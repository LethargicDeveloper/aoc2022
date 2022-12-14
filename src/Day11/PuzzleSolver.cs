using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

[MinColumn, MaxColumn, MemoryDiagnoser]
public partial class PuzzleSolver
{
    readonly string input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllText("Input001.txt");
    }

    public decimal SolvePart1()
    {
        var monkeys = this.input
            .Split("\r\n\r\n")
            .Select(MonkeyPart1.Parse)
            .ToList();

        for (int i = 0; i < 20; ++i)
        {
            foreach (var monkey in monkeys)
            {
                monkey.ThrowItems(monkeys);
            }
        }

        return monkeys
            .Select(_ => _.InspectedItems)
            .OrderDescending()
            .Take(2)
            .Aggregate(1M, (acc, cur) => acc * cur);
    }

    public decimal SolvePart2()
    {
        var monkeys = this.input
            .Split("\r\n\r\n")
            .Select(MonkeyPart2.Parse)
            .ToList();

        var testValue = monkeys.Aggregate(1, (acc, cur) => acc * cur.TestValue);
        for (int i = 0; i < 10000; ++i)
        {
            foreach (var monkey in monkeys)
            {
                monkey.ThrowItems(monkeys, testValue);
            }
        }

        return monkeys
            .Select(_ => _.InspectedItems)
            .OrderDescending()
            .Take(2)
            .Aggregate(1.0M, (acc, cur) => acc * cur);
    }

    class MonkeyPart1
    {
        string operand1 = "";
        string operand2 = "";
        string @operator = "";
        int divisor = 1;
        int monkey1;
        int monkey2;

        private MonkeyPart1(int id)
        {
            this.Id = id;
        }

        public int Id { get; init; }
        public readonly Queue<decimal> Items = new Queue<decimal>();
        public decimal InspectedItems { get; private set; }

        public void ThrowItems(List<MonkeyPart1> monkeys)
        {
            while (Items.TryDequeue(out decimal item))
            {
                InspectedItems++;
                item = CalculateWorryLevel(item);
                item = (decimal)Math.Floor(item / 3.0M);

                var monkey = item % divisor == 0 ? monkey1 : monkey2;
                monkeys[monkey].Items.Enqueue(item);
            }
        }

        decimal CalculateWorryLevel(decimal item)
        {
            var op1 = operand1 == "old" ? item : decimal.Parse(operand1);
            var op2 = operand2 == "old" ? item : decimal.Parse(operand2);
            return @operator switch
            {
                "+" => op1 + op2,
                "*" => op1 * op2,
                _ => throw new InvalidOperationException()
            };
        }

        public static MonkeyPart1 Parse(string input)
        {
            var lines = input.Split("\r\n");
            var id = int.Parse(Regex.Match(lines[0], "\\d+").Value);
            var items = Regex.Matches(lines[1], "\\d+").Select(_ => decimal.Parse(_.Value));
            var func = Regex.Matches(lines[2], "new = (old|\\d+) ([*|+]) (old|\\d+)")[0].Groups;
            var divisor = int.Parse(Regex.Match(lines[3], "\\d+").Value);
            var test1 = Regex.Matches(lines[4], "(true|false):.*?(\\d+)")[0].Groups;
            var test2 = Regex.Matches(lines[5], "(true|false):.*?(\\d+)")[0].Groups;

            var monkey = new MonkeyPart1(id);
            foreach (var item in items)
                monkey.Items.Enqueue(item);
            monkey.operand1 = func[1].Value;
            monkey.@operator = func[2].Value;
            monkey.operand2 = func[3].Value;
            monkey.divisor = divisor;
            monkey.monkey1 = test1[1].Value == "true" ? int.Parse(test1[2].Value) : int.Parse(test2[2].Value);
            monkey.monkey2 = test2[1].Value == "false" ? int.Parse(test2[2].Value) : int.Parse(test1[2].Value);
            return monkey;
        }
    }

    class MonkeyPart2
    {
        string operand1 = "";
        string operand2 = "";
        string @operator = "";
        int divisor = 1;
        int monkey1;
        int monkey2;

        private MonkeyPart2(int id)
        {
            this.Id = id;
        }

        public int Id { get; init; }
        public readonly Queue<decimal> Items = new Queue<decimal>();
        public int TestValue => divisor;
        public decimal InspectedItems { get; private set; }

        public void ThrowItems(List<MonkeyPart2> monkeys, int testValue)
        {
            while (Items.TryDequeue(out decimal item))
            {
                InspectedItems++;
                item = CalculateWorryLevel(item);

                var monkey = item % divisor == 0 ? monkey1 : monkey2;
                item = item % testValue;

                monkeys[monkey].Items.Enqueue(item);
            }
        }

        decimal CalculateWorryLevel(decimal item)
        {
            checked
            {
                var op1 = operand1 == "old" ? item : decimal.Parse(operand1);
                var op2 = operand2 == "old" ? item : decimal.Parse(operand2);
                return @operator switch
                {
                    "+" => op1 + op2,
                    "*" => op1 * op2,
                    _ => throw new InvalidOperationException()
                };
            }
        }

        public static MonkeyPart2 Parse(string input)
        {
            var lines = input.Split("\r\n");
            var id = int.Parse(Regex.Match(lines[0], "\\d+").Value);
            var items = Regex.Matches(lines[1], "\\d+").Select(_ => decimal.Parse(_.Value));
            var func = Regex.Matches(lines[2], "new = (old|\\d+) ([*|+]) (old|\\d+)")[0].Groups;
            var divisor = int.Parse(Regex.Match(lines[3], "\\d+").Value);
            var test1 = Regex.Matches(lines[4], "(true|false):.*?(\\d+)")[0].Groups;
            var test2 = Regex.Matches(lines[5], "(true|false):.*?(\\d+)")[0].Groups;

            var monkey = new MonkeyPart2(id);
            foreach (var item in items)
                monkey.Items.Enqueue(item);
            monkey.operand1 = func[1].Value;
            monkey.@operator = func[2].Value;
            monkey.operand2 = func[3].Value;
            monkey.divisor = divisor;
            monkey.monkey1 = test1[1].Value == "true" ? int.Parse(test1[2].Value) : int.Parse(test2[2].Value);
            monkey.monkey2 = test2[1].Value == "false" ? int.Parse(test2[2].Value) : int.Parse(test1[2].Value);
            return monkey;
        }
    }
}
