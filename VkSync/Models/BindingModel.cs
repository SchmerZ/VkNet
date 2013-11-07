using System;
using System.ComponentModel;

using VkSync.Validation;

namespace VkSync.Models
{
    public abstract class BindingModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields

        private readonly ValidationHandler _validationHandler = new ValidationHandler();

        #endregion

        #region Properties

        protected ValidationHandler ValidationHandler
        {
            get
            {
                return _validationHandler;
            }
        }

        protected bool IsValid
        {
            get
            {
                return ValidationHandler.IsValid;
            }
        }

        public void AddBrokenRuleMessage(string property, string message)
        {
            ValidationHandler.ValidateRule(property, message, () => false);
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnCurrentPropertyChanged()
        //{
        //    var methodName = string.Empty;
        //    try
        //    {
        //        // get call stack
        //        var stackTrace = new StackTrace();

        //        // get method calls (frames)
        //        var stackFrames = stackTrace.GetFrames();

        //        //var tt = string.Join(", ", stackFrames.Select(frame => frame.GetMethod().Name));
        //        //MessageBox.Show(tt);

        //        if (stackFrames != null && stackFrames.Length > 1)
        //            methodName = stackFrames[1].GetMethod().Name;

        //        if (!methodName.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
        //            throw new NotSupportedException("OnCurrentPropertyChanged should be invoked only in property setter");

        //        var propertyName = methodName.Substring(4);

        //        OnPropertyChanged(propertyName);
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(string.Format("Error occured: {0}.", ex.StackTrace));
        //    }
        //}

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Implementation of IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
                return _validationHandler.BrokenRuleExists(columnName)
                           ? _validationHandler[columnName]
                           : null;
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}