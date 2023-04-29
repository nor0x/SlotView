namespace SlotView.Maui.Demo;
public static class Helpers
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
    {
        var r = new Random();
        return enumerable.OrderBy(x => r.Next()).ToList();
    }
}
