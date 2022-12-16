using System.Diagnostics;

namespace AocHelper;

[DebuggerDisplay("({X}, {Y})")]
public struct Point : IEquatable<Point>
{
    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public int X;
    public int Y;

    public static Point operator +(Point p1, Point p2)
        => new Point(p1.X + p2.X, p1.Y + p2.Y);

    public static Point operator -(Point p1, Point p2)
        => new Point(p1.X - p2.X, p1.Y - p2.Y);

    public static Point operator *(Point p1, int val)
        => new Point(p1.X * val, p1.Y * val);

    public static implicit operator Point((int, int) p)
        => new Point(p.Item1, p.Item2);

    public static implicit operator (int, int)(Point p)
        => (p.X, p.Y);

    public static bool operator ==(Point left, Point right)
        => left.Equals(right);

    public static bool operator !=(Point left, Point right)
        => !(left == right);

    public void Deconstruct(out int x, out int y)
    {
        x = this.X;
        y = this.Y;
    }

    public static int ManhattanDistance(Point p1, Point p2)
        => Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);

    public int ManhattanDistance(Point p)
        => ManhattanDistance(this, p);

    public int ManhattanDistance()
        => ManhattanDistance(this, (0, 0));

    public Point Rotate(Point center, int angle)
        => Point.Rotate(this, center, angle);

    public static Point Rotate(Point point, Point center, int angle)
    {
        var rad = angle * Math.PI / 180;
        var sin = Math.Sin(rad);
        var cos = Math.Cos(rad);

        var x = point.X - center.X;
        var y = point.Y - center.Y;

        var xnew = x * cos - y * sin;
        var ynew = x * sin + y * cos;

        xnew = Math.Round(xnew + center.X, MidpointRounding.AwayFromZero);
        ynew = Math.Round(ynew + center.Y, MidpointRounding.AwayFromZero);

        return new Point((int)xnew, (int)ynew);
    }

    public override bool Equals(object obj)
        => obj is Point point && Equals(point);

    public bool Equals(Point other)
        => this.X == other.X && this.Y == other.Y;

    public override int GetHashCode()
        => HashCode.Combine(this.X, this.Y);
}