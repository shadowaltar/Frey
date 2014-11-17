using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trading.Common.Utils
{
    public static class EventToTask
    {
        public static EAPTask<TEventArgs, EventHandler<TEventArgs>> FromEvent<TEventArgs>()
        {
            var tcs = new TaskCompletionSource<TEventArgs>();
            var handler = new EventHandler<TEventArgs>((s, e) => tcs.TrySetResult(e));
            return new EAPTask<TEventArgs, EventHandler<TEventArgs>>(tcs, handler);
        }
    }


    public sealed class EAPTask<TEventArgs, TEventHandler>
        where TEventHandler : class
    {
        private readonly TaskCompletionSource<TEventArgs> completionSource;
        private readonly TEventHandler eventHandler;

        public EAPTask(TaskCompletionSource<TEventArgs> completionSource, TEventHandler eventHandler)
        {
            this.completionSource = completionSource;
            this.eventHandler = eventHandler;
        }

        public EAPTask<TEventArgs, TOtherEventHandler> WithHandlerConversion<TOtherEventHandler>(
            Converter<TEventHandler, TOtherEventHandler> converter)
            where TOtherEventHandler : class
        {
            return new EAPTask<TEventArgs, TOtherEventHandler>(completionSource, converter(eventHandler));
        }

        public async Task<TEventArgs> Start(
            Action<TEventHandler> subscribe,
            Action action,
            Action<TEventHandler> unsubscribe,
            CancellationToken cancellationToken)
        {
            subscribe(eventHandler);
            try
            {
                using (cancellationToken.Register(() => completionSource.SetCanceled()))
                {
                    action();
                    return await completionSource.Task;
                }
            }
            finally
            {
                unsubscribe(eventHandler);
            }
        }
    }
}