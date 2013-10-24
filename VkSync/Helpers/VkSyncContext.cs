using System;
using VkSync.Models;
using VkSync.Serializers;

namespace VkSync.Helpers
{
    public static class VkSyncContext
    {
        private static readonly Lazy<Settings> _settings = new Lazy<Settings>(SettingsSerializer.Deserialize);

        public static Settings Settings
        {
            get
            {
                return _settings.Value;
            }
        }
    }
}