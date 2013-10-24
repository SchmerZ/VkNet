using System;
using System.Xml.Serialization;

namespace VkSync.Models
{
    [XmlRoot("settings")]
    public class Settings : ICloneable
    {
        #region Properties

        [XmlAttribute("appId")]
        public int AppId
        {
            get;
            set;
        }

        [XmlElement("login")]
        public string Login
        {
            get;
            set;
        }

        [XmlElement("password")]
        public string Password
        {
            get;
            set;
        }

        [XmlElement("dataFolderPath")]
        public string DataFolderPath
        {
            get;
            set;
        }

        [XmlAttribute("concurrentDownloadThreadsCount")]
        public int ConcurrentDownloadThreadsCount
        {
            get; 
            set;
        }

        #endregion

        #region Implementation of ICloneable

        public object Clone()
        {
            return new Settings
            {
                Login = Login,
                Password = Password,
                AppId = AppId,
                DataFolderPath = DataFolderPath,
                ConcurrentDownloadThreadsCount = ConcurrentDownloadThreadsCount
            };
        }

        #endregion
    }
}