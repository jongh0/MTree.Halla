namespace MTree.Configuration
{
    public class GeneralConfiguration
    {
        public string DateFormat { get; } = "yyyy-MM-dd";

        public string TimeFormat { get; } = "HH:mm:ss.fff";

        public string DateTimeFormat { get; } = "yyyy-MM-dd HH:mm:ss.fff";

        public bool OfflineMode { get; } = false;
    }
}
