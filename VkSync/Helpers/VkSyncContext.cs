using System;
using VkSync.Models;
using VkSync.Serializers;

namespace VkSync.Helpers
{
    public static class VkSyncContext
    {
        private static readonly Lazy<Settings> _settings =
            new Lazy<Settings>(() => SettingsSerializer.Deserialize());

        private static readonly Lazy<SettingsSerializer> _settingsSerializer =
            new Lazy<SettingsSerializer>(() => new SettingsSerializer());

        public static Settings Settings
        {
            get
            {
                return _settings.Value;
            }
        }

        public static SettingsSerializer SettingsSerializer
        {
            get
            {
                return _settingsSerializer.Value;
            }
        }
    }
}