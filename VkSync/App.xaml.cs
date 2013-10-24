using System;
using System.Text;
using System.Windows;

namespace VkSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            var message = new StringBuilder();
            message.AppendLine("Unhandled exception was occured.");

            if (exception != null)
            {
                message.AppendLine(string.Format("StackTrace: {0}.", exception.StackTrace));
                message.AppendLine(string.Format("Exception message: {0}.", exception.Message));

                if (exception.InnerException != null)
                    message.AppendLine(string.Format("Inner exception message: {0}.", exception.InnerException.Message));
            }

            MessageBox.Show(message.ToString());
        }
    }
}