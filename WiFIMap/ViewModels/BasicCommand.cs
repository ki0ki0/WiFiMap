using System;

namespace WiFIMap.ViewModels
{
    class BasicCommand : Command<object>
    {
        public BasicCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
        {
        }

        public BasicCommand(Action<object> execute) : base(execute)
        {
        }
    }
}