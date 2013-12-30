using System;
using System.Threading;
using System.Threading.Tasks;

namespace Automata.Core.Extensions
{
    public static class MultithreadingExtensions
    {
        /// <summary>
        /// Run the 2nd task <paramref name="continueWith"/>
        /// when <paramref name="task"/> is not finished because of any exception. Then
        /// run the (optional) 3rd task <paramref name="onFinished"/>
        /// by using <see cref="TaskScheduler.FromCurrentSynchronizationContext()"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="continueWith"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Task ContinueOnException<TResult>(this Task<TResult> task,
            Action<Task<TResult>> continueWith, Action<Task> onFinished = null)
        {
            return task.ContinueOn(continueWith, TaskContinuationOptions.OnlyOnFaulted, onFinished);
        }

        /// <summary>
        /// Run the 2nd task <paramref name="continueWith"/>
        /// when <paramref name="task"/> is canceled. Then
        /// run the (optional) 3rd task <paramref name="onFinished"/>
        /// by using <see cref="TaskScheduler.FromCurrentSynchronizationContext()"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="continueWith"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Task ContinueOnCancelled<TResult>(this Task<TResult> task,
            Action<Task<TResult>> continueWith, Action<Task> onFinished = null)
        {
            return task.ContinueOn(continueWith, TaskContinuationOptions.OnlyOnCanceled, onFinished);
        }

        /// <summary>
        /// Run the 2nd task <paramref name="continueWith"/>
        /// when <paramref name="task"/> is finished successfully. Then
        /// run the (optional) 3rd task <paramref name="onFinished"/>
        /// by using <see cref="TaskScheduler.FromCurrentSynchronizationContext()"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="continueWith"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Task ContinueOnCompleted<TResult>(this Task<TResult> task,
            Action<Task<TResult>> continueWith, Action<Task> onFinished = null)
        {
            return task.ContinueOn(continueWith, TaskContinuationOptions.OnlyOnRanToCompletion, onFinished);
        }

        private static Task ContinueOn<TResult>(this Task<TResult> task,
            Action<Task<TResult>> continueWith,
            TaskContinuationOptions option,
            Action<Task> onFinished)
        {
            if (onFinished == null)
                return task.ContinueWith(continueWith, option);

            return task.ContinueWith(continueWith, option)
                .ContinueWith(onFinished, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Task RunAsyncWithDelay(this Action action, int delayInMilliseconds)
        {
            if (action == null)
                return null;

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            return Task.Factory.StartNew(() =>
            {
                token.WaitHandle.WaitOne(delayInMilliseconds);
                action();
            }, token);
        }

        /// <summary>
        /// Run a parameterless function <paramref name="func"/>
        /// then calls <paramref name="callback"/> on complete.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="callback"></param>
        public static void RunAsync<T>(this Func<T> func, Action<T> callback)
        {
            func.BeginInvoke(r => callback(func.EndInvoke(r)), null);
        }
    }
}
