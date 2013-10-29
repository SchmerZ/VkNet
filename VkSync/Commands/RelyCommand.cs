using System;
using System.Windows.Input;

namespace VkSync.Commands
{
    public class RelyCommand : ICommand
    {
        readonly Action _action;
        readonly Func<bool> _canExecute;

        #region Ctors
        
        public RelyCommand(Action action)
            : this(action, null)
        { }

        public RelyCommand(Action action, Func<bool> canExecute)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _action = action;
            _canExecute = canExecute;
        } 

        #endregion

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();

            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}