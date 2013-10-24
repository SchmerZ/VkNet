using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using VkSync.Helpers;
using VkSync.Models;

namespace VkSync.Serializers
{
	public static class SettingsSerializer
	{
		#region Fields

		private static string _processorId;
		private static readonly string _settingsFilePath = "Files\\settings.xml";

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

		public static void Serialize(Settings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			var settingsToSave = (Settings)settings.Clone();

			settingsToSave.Password = CryptoHelper.Encrypt(settingsToSave.Password, ProcessorId, ProcessorId);

			var xmlSerizlizer = new XmlSerializer(typeof(Settings));
            var xmlWriter = XmlWriter.Create(_settingsFilePath);

			xmlSerizlizer.Serialize(xmlWriter, settingsToSave);
			xmlWriter.Close();
		}

		public static Settings Deserialize()
		{
			var settings = new Settings();

			if (File.Exists(_settingsFilePath))
			{
				var serializer = new XmlSerializer(typeof(Settings));

                using (var xmlReader = XmlReader.Create(_settingsFilePath))
				{
					if (serializer.CanDeserialize(xmlReader))
						settings = (Settings)serializer.Deserialize(xmlReader);
				}

				var decryptedPassword = CryptoHelper.Decrypt(settings.Password, ProcessorId, ProcessorId);

				settings.Password = string.IsNullOrEmpty(decryptedPassword)
										? null
										: decryptedPassword;
			}
			else
			{
				settings.Login = null;
			}

			return settings;
		}
	}
}