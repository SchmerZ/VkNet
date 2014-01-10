using System;

namespace VkSync.Commands
{
    public class RelyCommand : BaseRelyCommand
    {
        private readonly Action _executeMethod;

        #region Ctors

        public RelyCommand(Action executeMethod)
            : this(executeMethod, null)
        { }

        public RelyCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base (canExecuteMethod)
        {
            if (executeMethod == null)
                throw new ArgumentNullException("executeMethod");

            _executeMethod = executeMethod;
        } 

        #endregion

        public override void Execute(object parameter)
        {
            Execute();
        }

        private void Execute()
        {
            if (_executeMethod != null)
                _executeMethod();
        }
    }
}