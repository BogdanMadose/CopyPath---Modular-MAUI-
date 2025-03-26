using NLog;

namespace CopyPath___Modular_MAUI_.Helpers
{
    public static class LoggingHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void LogError(string message)
        {
            Logger.Error(message);
        }

        public static void LogError(string message, Exception ex)
        {
            Logger.Error(message, ex);
        }

        public static void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public static void LogWarning(string message)
        {
            Logger.Warn(message);
        }
    }
}
