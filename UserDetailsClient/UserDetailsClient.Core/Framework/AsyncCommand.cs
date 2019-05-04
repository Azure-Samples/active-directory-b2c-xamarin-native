using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Framework
{
    public class AsyncCommand : Command
    {
        public AsyncCommand(Func<object, Task> execute)
            : base(param => execute(param).ConfigureAwait(false))
        {
        }

        public AsyncCommand(Func<Task> execute)
            : base(() => execute().ConfigureAwait(false))
        {
        }

        public AsyncCommand(Action<object> execute, Func<object, bool> canExecute)
            : base(execute, canExecute)
        {
        }

        public AsyncCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
        }
    }
}