using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using VkSync.Helpers;
using VkSync.Models;

namespace VkSync.Serializers
{
    public class SettingsSerializer : XmlSerializer
	{
		#region Fields

		private static string _processorId;
		private static readonly string _settingsFilePath = "Files\\settings.xml";

		#endregion

        #region Ctors

        public SettingsSerializer() : base (typeof(Settings))
        { }

        #endregion

        #region Properties

        private static string ProcessorId
		{
			get
			{
                if (string.IsNullOrEmpty(_processorId))
                    _processorId = WMIHelper.GetProcessorId();

                return _processorId;
			}
		}

		#endregion

        public void Serialize(Settings settings)
        {
            using(var xmlWriter = XmlWriter.Create(_settingsFilePath))
            {
                Serialize(xmlWriter, settings);
            }
        }

        protected override void Serialize(object o, XmlSerializationWriter writer)
        {
            var settings = o as Settings;
			
            if (settings == null)
				throw new ArgumentNullException("settings");

			var settingsToSave = (Settings)settings.Clone();
			settingsToSave.Password = CryptoHelper.Encrypt(settingsToSave.Password, ProcessorId, ProcessorId);

            base.Serialize(settingsToSave, writer);
		}

        public Settings Deserialize()
        {
            var result = new Settings();

            if (File.Exists(_settingsFilePath))
            {
                using (var xmlReader = XmlReader.Create(_settingsFilePath))
                {
                    if (CanDeserialize(xmlReader))
                        result = (Settings) Deserialize(xmlReader);
                }
            }

            DeserializationPostProcess(result);

            return result;
        }

        private void DeserializationPostProcess(Settings settings)
        {
            var decryptedPassword = CryptoHelper.Decrypt(settings.Password, ProcessorId, ProcessorId);

            settings.Password = string.IsNullOrEmpty(decryptedPassword)
                                  ? null
                                  : decryptedPassword;
        }
	}
}