using System.Linq;
using System.Management;

namespace VkSync.Helpers
{
	public static class WMIHelper
	{
		public static string GetProcessorId()
		{
			var processorId = string.Empty;

			using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
			{
				var queryCollection = searcher.Get();
				var managementObject = queryCollection.Cast<ManagementObject>().FirstOrDefault();

				if (managementObject != null)
					processorId = managementObject["ProcessorID"].ToString();
			}

			return processorId;
		}
	}
}