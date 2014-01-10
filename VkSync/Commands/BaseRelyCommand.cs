using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace VkSync.Commands
{
    public abstract class BaseRelyCommand : ICommand
    {
        private readonly Func<bool> _canExecuteMethod;
        private List<WeakReference> _canExecuteChangedHandlers;

        #region Ctors

        public BaseRelyCommand()
            : this(null)
        { }

        public BaseRelyCommand(Func<bool> canExecuteMethod)
        {
            _canExecuteMethod = canExecuteMethod;
        } 

        #endregion

        #region Public Events

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        //public event EventHandler CanExecuteChanged
        //{
        //    add
        //    {
        //        CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value);
        //    }
        //    remove
        //    {
        //        CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
        //    }
        //}

        #endregion

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public abstract void Execute(object parameter);

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to determine if the command can be executed.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        private bool CanExecute()
        {
            if (_canExecuteMethod != null)
                return _canExecuteMethod();

            return true;
        }

        /// <summary>
        /// Raises the CanExecuteChaged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }

        #endregion
    }
}