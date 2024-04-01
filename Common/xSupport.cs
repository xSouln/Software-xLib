using System;
using System.Windows.Threading;

namespace xLibV100.Common
{
    public class xSupport
    {
        //public static ActionAccessUI<object> PointEntryUI;
        public static DispatcherObject Context;
        public static void ActionThreadUI(xAction action, object arg)
        {
            RequestThreadUI(action, arg);
        }

        public static void ActionThreadUI<TRequest>(xAction action, TRequest arg)
        {
            RequestThreadUI(action, arg);
        }

        public static void ActionThreadUI<TRequest>(Action<TRequest> action)
        {
            if (Context != null)
            {
                try
                {
                    Context.Dispatcher.Invoke(action);
                }
                catch { }
            }
        }

        public static void ActionThreadUI(object arg)
        {
            if (arg != null)
            {
                RequestThreadUI((xAction)arg, null);
            }
        }

        public static void ActionThreadUI(Action action)
        {
            if (Context != null)
            {
                try
                {
                    Context.Dispatcher.Invoke(() =>
                    {
                        action();
                    });
                }
                catch { }
            }
        }

        private static void RequestThreadUI(xAction request, object arg)
        {
            if (Context != null)
            {
                try
                {
                    Context.Dispatcher.Invoke(() =>
                    {
                        request?.Invoke(arg);
                    });
                }
                catch { }
            }
        }
    }
}
