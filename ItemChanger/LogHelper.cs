namespace ItemChanger
{
    internal static class LogHelper
    {
        public static void Log(string message)
        {
            ItemChangerMod.instance.Log(message);
        }

        public static void Log<T>(T t) where T : struct
        {
            Log(t.ToString());
        }

        public static void Log(object o)
        {
            Log(o?.ToString());
        }

        public static void LogWarn(string message)
        {
            ItemChangerMod.instance.LogWarn(message);
        }

        public static void LogWarn<T>(T t) where T : struct
        {
            LogWarn(t.ToString());
        }

        public static void LogWarn(object o)
        {
            LogWarn(o?.ToString());
        }

        public static void LogError(string message)
        {
            ItemChangerMod.instance.LogError(message);
        }

        public static void LogError<T>(T t) where T : struct
        {
            LogError(t.ToString());
        }

        public static void LogError(object o)
        {
            LogError(o?.ToString());
        }
    }
}
