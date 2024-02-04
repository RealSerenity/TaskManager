namespace TaskManager.Data.Enums
{
    public enum JobType
    {
        Daily=1,
        Weekly = 7,
        Monthly = 30
    }

    public static class EnumExtensions
    {
        public static bool IsValidValue<T>(this T value) where T : struct, IConvertible
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}
