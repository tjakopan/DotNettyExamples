using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Common.Concurrency;

namespace Qotm.Client {
  internal static class TaskEx {
    internal static Task TimeoutAfter(this Task task, TimeSpan timeSpan) {
      // Short-circuit #1: infinite timeout or task already completed.
      if (task.IsCompleted || timeSpan == Timeout.InfiniteTimeSpan) {
        // Either the task has already completed or timeout will never occur. No proxy necessary.
        return task;
      }

      // tcs.Task will be return as a proxy to the caller.
      TaskCompletionSource tcs = new TaskCompletionSource();

      // Short-circuit #2: zero timeout.
      if (timeSpan == TimeSpan.Zero) {
        // We've already timed out.
        tcs.SetException(new TimeoutException());
        return tcs.Task;
      }

      // Set up a timer to complete after the specified timeout period.
      Timer timer = new Timer(state => {
        // Recover our state data.
        TaskCompletionSource myTcs = (TaskCompletionSource) state;

        // Fault out proxy Task with a TimeoutException.
        myTcs.TrySetException(new TimeoutException());
      }, tcs, timeSpan, Timeout.InfiniteTimeSpan);

      // Wire up the logic for what happens when source task completes.
      task.ContinueWith((antecedent, state) => {
          // Recover our state data.
          Tuple<Timer, TaskCompletionSource> tuple = (Tuple<Timer, TaskCompletionSource>) state;

          tuple.Item1.Dispose(); // Cancel the timer.
          MarshalTaskResults(antecedent, tuple.Item2); // Marshal results to proxy.
        }, Tuple.Create(timer, tcs), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously,
        TaskScheduler.Default);

      return tcs.Task;
    }

    private static void MarshalTaskResults(Task source, TaskCompletionSource proxy) {
      switch (source.Status) {
        case TaskStatus.Faulted:
          // ReSharper disable once AssignNullToNotNullAttribute
          proxy.TrySetException(source.Exception);
          break;
        case TaskStatus.Canceled:
          proxy.TrySetCanceled();
          break;
        case TaskStatus.RanToCompletion:
          proxy.TrySetResult(0);
          break;
      }
    }
  }
}
