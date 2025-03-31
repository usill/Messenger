namespace TestSignalR.Models.Helper
{
    public static class DateTimeHelper
    {
        public static long GetMoscowTimestampNow()
        {
            TimeZoneInfo moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"); // for linux "Europe/Moscow"
            DateTime moscowNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowZone);
            return ((DateTimeOffset)moscowNow).ToUnixTimeSeconds();
        }
    }
}
