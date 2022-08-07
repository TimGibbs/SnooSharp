namespace SnooSharp;
public static class Constants
{
    internal static class Headers
    {
        public const string UserAgent = "User-Agent";
        public const string Auth = "Authorization";
    }

    internal static class HeaderValues
    {
        public const string UserAgent = "SNOO/351 CFNetwork/1121.2 Darwin/19.2.0";
    }
    internal static class UrlParts
    {
        public const string BaseUrl = "https://snoo-api.happiestbaby.com";
        public const string LoginEndpoint = "/us/login";
        public const string DataEndpoint = "/ss/v2/sessions/aggregated";
    }

    public static class Formats
    {
        public const string LevelDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    }

}