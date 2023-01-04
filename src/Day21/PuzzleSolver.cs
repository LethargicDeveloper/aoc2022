using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

public partial class PuzzleSolver
{
    readonly List<string> input;

    public PuzzleSolver()
    {
        this.input = File.ReadAllLines("Input001.txt").ToList();
    }

    public long SolvePart1()
    {
        var functions = this.input
            .Select(_ => $"public long {_.Replace(":", "() {")} }}")
            .Select(_ => Regex.Replace(_, @"(....) ([\+\-\*\/]) (....)", m =>
                $"return {m.Groups[1].Value}() {m.Groups[2].Value} {m.Groups[3].Value}();"))
            .Select(_ => Regex.Replace(_, @"(\d+)", m =>
                $"return {m.Groups[1].Value};"));

        var @class = """
            public class Solver {
            %%func%%
            }
            """.Replace("%%func%%", string.Join("\r\n", functions));

        var syntaxTree = CSharpSyntaxTree.ParseText(@class);
        var references = new[] {
              MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
              MetadataReference.CreateFromFile(typeof(ValueTuple<>).GetTypeInfo().Assembly.Location)
        };
        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            optimizationLevel: OptimizationLevel.Debug);
        var compliation = CSharpCompilation.Create(
            "InMemoryAssembly",
            references: references,
            options: options).AddSyntaxTrees(syntaxTree);
        
        using var stream = new MemoryStream();
        var emitResult = compliation.Emit(stream);

        long result = 0;
        if (emitResult.Success)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(stream.ToArray());
            var type = assembly.GetType("Solver")!;
            var solver = Activator.CreateInstance(type);
            result = (long)type.InvokeMember("root", BindingFlags.Default | BindingFlags.InvokeMethod, null, solver, null)!;
        }

        return result;
    }

    public long SolvePart2()
    {
        var root = this.input.First(_ => _.StartsWith("root:")).Substring(6).Replace("+", "=");

        while(true)
        {
            var match = MyRegex1().Matches(root).FirstOrDefault(_ => _.Value != "humn");
            if (match == null || !match.Success) break;
            
            var replace = this.input.First(_ => _.StartsWith($"{match.Value}:")).Substring(6);
            replace = $"({replace})";
            root = root.Replace(match.Value, replace);
        }

        //https://www.mathpapa.com/simplify-calculator/
        return 0;
    }

    [GeneratedRegex("(....)")]
    private static partial Regex MyRegex();
    [GeneratedRegex("([a-z]{4})")]
    private static partial Regex MyRegex1();
}