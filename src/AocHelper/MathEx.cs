namespace AocHelper;

public static class MathEx
{
    public static int Max(int[] nums)
    {
        if (nums == null || nums.Length == 0)
            return 0;

        return nums.Max();
    }

    public static int Max(int num1, int num2, params int[] nums)
    {
        return (nums == null || nums.Length == 0)
            ? Math.Max(num1, num2)
            : Math.Max(num1, num2) + Max(nums);
    }
}
