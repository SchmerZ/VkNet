using System;

namespace VkSync.Commands
{
    public class RelyParameterCommand : BaseRelyCommand
    {
        private readonly Action<object> _executeMethod;

        public RelyParameterCommand(Action<object> executeMethod)
            : this(executeMethod, null)
        { }

        public RelyParameterCommand(Action<object> executeMethod, Func<bool> canExecuteMethod) 
            : base(canExecuteMethod)
        {
            _executeMethod = executeMethod;
        }

        public override void Execute(object parameter)
        {
            if (_executeMethod != null)
                _executeMethod(parameter);
        }
    }
}