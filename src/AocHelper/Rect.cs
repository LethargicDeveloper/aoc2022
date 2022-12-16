using System.Diagnostics;

namespace AocHelper;

[DebuggerDisplay("({Left}, {Top})-({Right}, {Bottom})")]
public struct Rect : IEquatable<Rect>
{
    public Rect() { }

    public Rect(int x1, int y1, int x2, int y2)
    {
        this.Left = x1;
        this.Top = y1;
        this.Right = x2;
        this.Bottom = y2;
    }

    public int Left { get; init; }
    public int Right { get; init; }
    public int Top { get; init; }
    public int Bottom { get; init; }

    public static bool operator == (Rect left, Rect right)
        => left.Equals(right);

    public static bool operator != (Rect left, Rect right)
        => !(left == right);

    public bool ContainsScreenPoint(Point point)
        => point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;

    public bool ContainsCartesianPoint(Point point)
        => point.X >= Left && point.X <= Right && point.Y <= Top && point.Y >= Bottom;

    public Rect Rotate(Point center, int angle)
        => Rect.Rotate(this, center, angle);

    public static Rect Rotate(Rect rect, Point center, int angle)
    {
        var points = new Point[]
        {
            (rect.Left, rect.Top),
            (rect.Right, rect.Top),
            (rect.Left, rect.Bottom),
            (rect.Right, rect.Bottom),
        };

        for (int i = 0; i < points.Length; ++i)
        {
            points[i] = Point.Rotate(points[i], center, angle);
        }

        return new Rect(
            points.Min(_ => _.X),
            points.Min(_ => _.Y),
            points.Max(_ => _.X),
            points.Max(_ => _.Y));
    }

    public override bool Equals(object obj)
      => obj is Rect point && Equals(point);

    public bool Equals(Rect other)
        =>  this.Top == other.Top &&
            this.Right == other.Right &&
            this.Bottom == other.Bottom &&
            this.Left == other.Left;

    public override int GetHashCode()
        => HashCode.Combine(this.Top, this.Right, this.Bottom, this.Left);
}