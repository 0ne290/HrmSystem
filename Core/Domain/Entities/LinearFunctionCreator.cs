namespace Domain.Entities;

public static class LinearFunctionCreator
{
    public static Func<decimal, decimal> FromTwoPoint((decimal X, decimal Y) point1, (decimal X, decimal Y) point2)
    {
        var k = (point2.Y - point1.Y) / (point2.X - point1.X);
        var b = point1.Y - point1.X * k;

        return x => k * x + b;
    }
}