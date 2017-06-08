using OculiService.Common;
using OculiService.Common.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace System.Threading.Tasks
{
    public static class CommonTaskExtensions
    {
        private static readonly Tracer LocalTracer = Tracer.GetTracer("DoubleTake.Common.Tasks.CommonTaskExtensions");
        private static readonly Dictionary<int, CommonTaskExtensions.TaskName> TaskNames = new Dictionary<int, CommonTaskExtensions.TaskName>();
        private static readonly Task CanceledTask = Task.Factory.CreateCanceled();
        private static readonly Task CompletedTask = Task.Factory.DoNothing();
        private const TaskContinuationOptions TaskNotOnFlags = TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion;

        public static Task CreateCanceled(this TaskFactory self)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("CreateCanceled: creating Task.");
            TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
            completionSource.SetCanceled();
            return (Task)completionSource.Task;
        }

        public static Task<T> CreateCanceled<T>(this TaskFactory<T> self)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("CreateCanceled: creating Task.");
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            completionSource.SetCanceled();
            return completionSource.Task;
        }

        public static Task<TResult> FromResult<TResult>(this TaskFactory self, TResult result)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("DoNothing: creating Task.");
            TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
            TResult result1 = result;
            completionSource.TrySetResult(result1);
            return completionSource.Task;
        }

        public static Task DoNothing(this TaskFactory self)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("DoNothing: creating Task.");
            TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
            object local = null;
            completionSource.TrySetResult((object)local);
            return (Task)completionSource.Task;
        }

        public static string GetName(this Task self)
        {
            lock (CommonTaskExtensions.TaskNames)
            {
                CommonTaskExtensions.TaskName local_2;
                if (CommonTaskExtensions.TaskNames.TryGetValue(self.Id, out local_2))
                    return local_2.Name;
                return string.Empty;
            }
        }

        public static Task OnError(this Task self, Action<AggregateException> continuationAction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationFunction");
            return self.OnError(continuationAction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task OnError(this Task self, Action<AggregateException> continuationAction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationFunction");
            return self.OnError(continuationAction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task OnError(this Task self, Action<AggregateException> continuationAction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.OnError(continuationAction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task OnError(this Task self, Action<AggregateException> continuationAction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.OnError(continuationAction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task OnError(this Task self, Action<AggregateException> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.OnError<object>(tcs, (Action<AggregateException>)(ae =>
            {
                continuationAction(ae);
                tcs.TrySetResult((object)null);
            }), cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task OnError(this Task self, Func<AggregateException, Task> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.OnError(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task OnError(this Task self, Func<AggregateException, Task> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.OnError(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task OnError(this Task self, Func<AggregateException, Task> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.OnError(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task OnError(this Task self, Func<AggregateException, Task> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.OnError(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task OnError(this Task self, Func<AggregateException, Task> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.OnError<object>(tcs, (Action<AggregateException>)(ae =>
            {
                Task task = continuationFunction(ae);
                int num = 0;
                task.ContinueWith<bool>((Func<Task, bool>)(t => tcs.TrySetFromTask<object>(t)), (TaskContinuationOptions)num);
            }), cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, T> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.OnError<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, Task<T>> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.OnError<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, Task<T>> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.OnError<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, Task<T>> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.OnError<T>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, Task<T>> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.OnError<T>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, Task<T>> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(self.AsyncState);
            self.OnError<T>(tcs, (Action<AggregateException>)(ae =>
            {
                Task<T> task = continuationFunction(ae);
                int num = 0;
                task.ContinueWith<bool>((Func<Task<T>, bool>)(t => tcs.TrySetFromTask<T>((Task)t)), (TaskContinuationOptions)num);
            }), cancellationToken, continuationOptions, scheduler);
            return tcs.Task;
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, T> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.OnError<T>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, T> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.OnError<T>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, T> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.OnError<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<T> OnError<T>(this Task<T> self, Func<AggregateException, T> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(self.AsyncState);
            self.OnError<T>(tcs, (Action<AggregateException>)(ae => tcs.TrySetResult(continuationFunction(ae))), cancellationToken, continuationOptions, scheduler);
            return tcs.Task;
        }

        public static Task Recover(this Task self, Func<AggregateException, Task> errorContinuation)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)errorContinuation, "errorContinuation");
            return self.Recover(errorContinuation, CancellationToken.None);
        }

        public static Task Recover(this Task self, Func<AggregateException, Task> errorContinuation, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)errorContinuation, "errorContinuation");
            Invariant.ArgumentNotNull((object)cancellationToken, "cancellationToken");
            CommonTaskExtensions.LocalTracer.TraceInformation("Recover: configuring continuation: {0}", (object)(self.GetName() ?? "(unnamed)"));
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.ContinueWith((Action<Task>)(t =>
            {
                if (t.IsFaulted)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("Recover: antecedent is faulted: {0}", (object)(self.GetName() ?? "(unnamed)"));
                    try
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: chaining errorContinuation.");
                        Task task = errorContinuation(t.Exception).Recover(errorContinuation, cancellationToken);
                        int num = 524288;
                        task.ContinueWith<bool>((Func<Task, bool>)(t2 => tcs.TrySetFromTask<object>(t2)), (TaskContinuationOptions)num);
                    }
                    catch (OperationCanceledException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: errorContinuation threw OperationCanceledException.");
                        tcs.TrySetCanceled();
                    }
                    catch (AggregateException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: errorContinuation threw AggregateException.");
                        tcs.TrySetException((IEnumerable<Exception>)ex.InnerExceptions);
                    }
                    catch (Exception ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: errorContinuation threw Exception.");
                        tcs.TrySetException(ex);
                    }
                }
                else
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("Recover: reporting result from antecedent.");
                    tcs.TrySetFromTask<object>(t);
                }
            }), cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
            cancellationToken.Register((Action)(() => tcs.TrySetCanceled()));
            return (Task)tcs.Task;
        }

        public static Task<T> Recover<T>(this Task<T> self, Func<AggregateException, Task<T>> errorContinuation)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)errorContinuation, "errorContinuation");
            return self.Recover<T>(errorContinuation, CancellationToken.None);
        }

        public static Task<T> Recover<T>(this Task<T> self, Func<AggregateException, Task<T>> errorContinuation, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)errorContinuation, "errorContinuation");
            Invariant.ArgumentNotNull((object)cancellationToken, "cancellationToken");
            CommonTaskExtensions.LocalTracer.TraceInformation("Recover: configuring continuation: {0}", (object)(self.GetName() ?? "(unnamed)"));
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(self.AsyncState);
            self.ContinueWith((Action<Task<T>>)(t =>
            {
                if (t.IsFaulted)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("Recover: antecedent is faulted: {0}", (object)(t.GetName() ?? "(unnamed)"));
                    try
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: chaining errorContinuation.");
                        Task<T> task = errorContinuation(t.Exception).Recover<T>(errorContinuation, cancellationToken);
                        int num = 524288;
                        task.ContinueWith<bool>((Func<Task<T>, bool>)(t2 => tcs.TrySetFromTask<T>((Task)t2)), (TaskContinuationOptions)num);
                    }
                    catch (OperationCanceledException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: errorContinuation threw OperationCanceledException.");
                        tcs.TrySetCanceled();
                    }
                    catch (AggregateException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: errorContinuation threw AggregateException.");
                        tcs.TrySetException((IEnumerable<Exception>)ex.InnerExceptions);
                    }
                    catch (Exception ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Recover: errorContinuation threw Exception.");
                        tcs.TrySetException(ex);
                    }
                }
                else
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("Recover: reporting result from antecedent.");
                    tcs.TrySetFromTask<T>((Task)t);
                }
            }), CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
            cancellationToken.Register((Action)(() => tcs.TrySetCanceled()));
            return tcs.Task;
        }

        public static Task<T> Return<T>(this TaskFactory<T> self, T result)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("Return: creating Task.");
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            T result1 = result;
            completionSource.TrySetResult(result1);
            return completionSource.Task;
        }

        public static TaskScheduler SchedulerForCurrentThread()
        {
            if (SynchronizationContext.Current == null)
                return TaskScheduler.Current;
            try
            {
                return TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch (InvalidOperationException ex)
            {
                return TaskScheduler.Current;
            }
        }

        public static Task SetName(this Task self, string name)
        {
            lock (CommonTaskExtensions.TaskNames)
            {
                CommonTaskExtensions.TaskNames[self.Id] = new CommonTaskExtensions.TaskName(self, name);
                return self;
            }
        }

        public static Task<T> SetName<T>(this Task<T> self, string name)
        {
            lock (CommonTaskExtensions.TaskNames)
            {
                CommonTaskExtensions.TaskNames[self.Id] = new CommonTaskExtensions.TaskName((Task)self, name);
                return self;
            }
        }

        public static Task Then(this Task self, Action continuationAction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.Then(continuationAction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then(this Task self, Action continuationAction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.Then(continuationAction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then(this Task self, Action continuationAction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then(continuationAction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then(this Task self, Action continuationAction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then(continuationAction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task Then(this Task self, Action continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.Then<object>(tcs, (Action)(() =>
            {
                continuationAction();
                tcs.TrySetResult((object)null);
            }), cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<TResult> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<TResult> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<TResult>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then<TResult>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<TResult> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then<TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(self.AsyncState);
            self.Then<TResult>(tcs, (Action)(() => tcs.TrySetResult(continuationFunction())), CancellationToken.None, TaskContinuationOptions.None, scheduler);
            return tcs.Task;
        }

        public static Task Then<T>(this Task<T> self, Action<T> continuationAction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.Then<T>(continuationAction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then<T>(this Task<T> self, Action<T> continuationAction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.Then<T>(continuationAction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then<T>(this Task<T> self, Action<T> continuationAction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then<T>(continuationAction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then<T>(this Task<T> self, Action<T> continuationAction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then<T>(continuationAction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task Then<T>(this Task<T> self, Action<T> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.Then<object>(tcs, (Action)(() =>
            {
                continuationAction(self.Result);
                tcs.TrySetResult((object)null);
            }), cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, TResult> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<T, TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, TResult> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<T, TResult>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then<T, TResult>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, TResult> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then<T, TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(self.AsyncState);
            self.Then<TResult>(tcs, (Action)(() => tcs.TrySetResult(continuationFunction(self.Result))), cancellationToken, continuationOptions, scheduler);
            return tcs.Task;
        }

        public static Task Then(this Task self, Func<Task> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then(this Task self, Func<Task> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then(this Task self, Func<Task> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then(this Task self, Func<Task> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task Then(this Task self, Func<Task> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.Then<object>(tcs, (Action)(() => continuationFunction().ContinueWith<bool>((Func<Task, bool>)(t => tcs.TrySetFromTask<object>(t)), TaskContinuationOptions.None)), cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<Task<TResult>> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<Task<TResult>> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<TResult>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<Task<TResult>> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then<TResult>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<Task<TResult>> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then<TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<TResult> Then<TResult>(this Task self, Func<Task<TResult>> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(self.AsyncState);
            self.Then<TResult>(tcs, (Action)(() => continuationFunction().ContinueWith<bool>((Func<Task<TResult>, bool>)(t => tcs.TrySetFromTask<TResult>((Task)t)), TaskContinuationOptions.None)), cancellationToken, continuationOptions, scheduler);
            return tcs.Task;
        }

        public static Task Then<T>(this Task<T> self, Func<T, Task> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then<T>(this Task<T> self, Func<T, Task> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<T>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then<T>(this Task<T> self, Func<T, Task> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then<T>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task Then<T>(this Task<T> self, Func<T, Task> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task Then<T>(this Task<T> self, Func<T, Task> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.Then<object>(tcs, (Action)(() => continuationFunction(self.Result).ContinueWith<bool>((Func<Task, bool>)(t => tcs.TrySetFromTask<object>(t)), TaskContinuationOptions.None)), cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, Task<TResult>> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<T, TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, Task<TResult>> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.Then<T, TResult>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, Task<TResult>> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.Then<T, TResult>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, Task<TResult>> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.Then<T, TResult>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<TResult> Then<T, TResult>(this Task<T> self, Func<T, Task<TResult>> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(self.AsyncState);
            self.Then<TResult>(tcs, (Action)(() => continuationFunction(self.Result).ContinueWith<bool>((Func<Task<TResult>, bool>)(t => tcs.TrySetFromTask<TResult>((Task)t)), TaskContinuationOptions.None)), cancellationToken, continuationOptions, scheduler);
            return tcs.Task;
        }

        public static Task ThenAlways(this Task self, Action continuationAction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.ThenAlways(continuationAction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task ThenAlways(this Task self, Action continuationAction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.ThenAlways(continuationAction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task ThenAlways(this Task self, Action continuationAction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.ThenAlways(continuationAction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task ThenAlways(this Task self, Action continuationAction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.ThenAlways(continuationAction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task ThenAlways(this Task self, Action continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.ThenAlways<object>(tcs, continuationAction, cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Action continuationAction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.ThenAlways<T>(continuationAction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Action continuationAction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            return self.ThenAlways<T>(continuationAction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Action continuationAction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.ThenAlways<T>(continuationAction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Action continuationAction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.ThenAlways<T>(continuationAction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Action continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationAction, "continuationAction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(self.AsyncState);
            self.ThenAlways<T>(tcs, continuationAction, cancellationToken, continuationOptions, scheduler);
            return tcs.Task;
        }

        public static Task ThenAlways(this Task self, Func<Task> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.ThenAlways(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task ThenAlways(this Task self, Func<Task> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.ThenAlways(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task ThenAlways(this Task self, Func<Task> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.ThenAlways(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task ThenAlways(this Task self, Func<Task> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.ThenAlways(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task ThenAlways(this Task self, Func<Task> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(self.AsyncState);
            self.ThenAlways<object>(tcs, continuationFunction, cancellationToken, continuationOptions, scheduler);
            return (Task)tcs.Task;
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Func<Task<T>> continuationFunction)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.ThenAlways<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Func<Task<T>> continuationFunction, CancellationToken cancellationToken)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            return self.ThenAlways<T>(continuationFunction, cancellationToken, TaskContinuationOptions.None, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Func<Task<T>> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            return self.ThenAlways<T>(continuationFunction, CancellationToken.None, continuationOptions, CommonTaskExtensions.SchedulerForCurrentThread());
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Func<Task<T>> continuationFunction, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            return self.ThenAlways<T>(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        public static Task<T> ThenAlways<T>(this Task<T> self, Func<Task<T>> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            Invariant.ArgumentNotNull((object)continuationFunction, "continuationFunction");
            Invariant.ArgumentEnumIsValidValue((Enum)continuationOptions, "continuationOptions");
            Invariant.ArgumentEnumDoesNotIncludeFlags((Enum)continuationOptions, (Enum)(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion), "continuationOptions");
            Invariant.ArgumentNotNull((object)scheduler, "scheduler");
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(self.AsyncState);
            self.ThenAlways<T>(tcs, (Func<Task>)(() => (Task)continuationFunction()), cancellationToken, continuationOptions, scheduler);
            return tcs.Task;
        }

        public static Task Throw(this TaskFactory self, Exception exception)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("Throw: creating Task.");
            TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
            try
            {
                throw exception;
            }
            catch (Exception ex)
            {
                completionSource.SetException(ex);
            }
            return (Task)completionSource.Task;
        }

        public static Task<T> Throw<T>(this TaskFactory self, Exception exception)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("Throw: creating Task.");
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            try
            {
                throw exception;
            }
            catch (Exception ex)
            {
                completionSource.SetException(ex);
            }
            return completionSource.Task;
        }

        public static Task<T> Throw<T>(this TaskFactory<T> self, Exception exception)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("Throw: creating Task.");
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            try
            {
                throw exception;
            }
            catch (Exception ex)
            {
                completionSource.SetException(ex);
            }
            return completionSource.Task;
        }

        public static TaskStatus WaitForCompletionStatus(this Task self)
        {
            Invariant.ArgumentNotNull((object)self, "self");
            CommonTaskExtensions.LocalTracer.TraceInformation("WaitForCompletionStatus: wating.");
            try
            {
                self.Wait();
            }
            catch
            {
            }
            return self.Status;
        }

        public static void WaitWithPumping(this Task task)
        {
            Invariant.ArgumentNotNull((object)task, "task");
            DispatcherFrame nestedFrame = new DispatcherFrame();
            task.ContinueWith<bool>((Func<Task, bool>)(_ => nestedFrame.Continue = false));
            Dispatcher.PushFrame(nestedFrame);
            task.Wait();
        }

        private static void OnError<TResult>(this Task self, TaskCompletionSource<TResult> tcs, Action<AggregateException> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("OnError: configuring continuation: {0}", (object)(self.GetName() ?? "(unnamed)"));
            self.ContinueWith((Action<Task>)(t =>
            {
                if (t.IsFaulted)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("OnError: antecedent was faulted: {0}", (object)(t.GetName() ?? "(unnamed)"));
                    try
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("OnError: calling continuationAction.");
                        continuationAction(t.Exception);
                    }
                    catch (OperationCanceledException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("OnError: continuationAction threw OperationCanceledException.");
                        tcs.TrySetCanceled();
                    }
                    catch (AggregateException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("OnError: continuationAction threw AggregateException.");
                        tcs.TrySetException((IEnumerable<Exception>)ex.InnerExceptions);
                    }
                    catch (Exception ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("OnError: continuationAction threw Exception.");
                        tcs.TrySetException(ex);
                    }
                }
                else
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("OnError: reporting result from antecedent: {0}", (object)(t.GetName() ?? "(unnamed)"));
                    tcs.TrySetFromTask<TResult>(t);
                }
            }), cancellationToken, continuationOptions, scheduler);
        }

        private static void Then<TResult>(this Task self, TaskCompletionSource<TResult> tcs, Action continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("Then: configuring continuation: {0}", (object)(self.GetName() ?? "(unnamed)"));
            self.ContinueWith((Action<Task>)(t =>
            {
                if (t.IsFaulted)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("Then: antecedent is faulted: {0}", (object)(t.GetName() ?? "(unnamed)"));
                    tcs.TrySetException((IEnumerable<Exception>)t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("Then: antecedent is canceled: {0}", (object)(t.GetName() ?? "(unnamed)"));
                    tcs.TrySetCanceled();
                }
                else
                {
                    try
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Then: calling continuationAction: {0}", (object)(t.GetName() ?? "(unnamed)"));
                        continuationAction();
                    }
                    catch (OperationCanceledException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Then: continuationAction threw OperationCanceledException.");
                        tcs.TrySetCanceled();
                    }
                    catch (AggregateException ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Then: continuationAction threw AggregateException.");
                        tcs.TrySetException((IEnumerable<Exception>)ex.InnerExceptions);
                    }
                    catch (Exception ex)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("Then: continuationAction threw Exception.");
                        tcs.TrySetException(ex);
                    }
                }
            }), cancellationToken, continuationOptions, scheduler);
        }

        private static void ThenAlways<TResult>(this Task self, TaskCompletionSource<TResult> tcs, Action continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: configuring continuation: {0}", (object)(self.GetName() ?? "(unnamed)"));
            self.ContinueWith((Action<Task>)(t =>
            {
                try
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: calling continuationAction and chaining result: {0}", (object)(t.GetName() ?? "(unnamed)"));
                    continuationAction();
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: setting result from antecedent.");
                    tcs.TrySetFromTask<TResult>(t);
                }
                catch (OperationCanceledException ex)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: continuationAction threw OperationCanceledException.");
                    if (t.IsFaulted)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: reporting fault from antecedent.");
                        tcs.TrySetException((IEnumerable<Exception>)t.Exception.InnerExceptions);
                    }
                    else
                        tcs.TrySetCanceled();
                }
                catch (AggregateException ex)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: continuationAction threw AggregateException.");
                    if (t.IsFaulted)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: combining result with antecedent.");
                        tcs.TrySetException(t.Exception.InnerExceptions.Concat<Exception>((IEnumerable<Exception>)ex.InnerExceptions));
                    }
                    else
                        tcs.TrySetException((IEnumerable<Exception>)ex.InnerExceptions);
                }
                catch (Exception ex)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: continuationAction threw Exception.");
                    if (t.IsFaulted)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: combining result with antecedent.");
                        tcs.TrySetException(t.Exception.InnerExceptions.Concat<Exception>(ex));
                    }
                    else
                        tcs.TrySetException(ex);
                }
            }), cancellationToken, continuationOptions, scheduler);
        }

        private static void ThenAlways<TResult>(this Task self, TaskCompletionSource<TResult> tcs, Func<Task> continuationFunc, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: configuring continuation: {0}", (object)(self.GetName() ?? "(unnamed)"));
            self.ContinueWith((Action<Task>)(t =>
            {
                try
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: calling continuationFunc and chaining result: {0}", (object)(t.GetName() ?? "(unnamed)"));
                    continuationFunc().ContinueWith((Action<Task>)(t2 =>
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: reporting result of chained continuation: {0}", (object)(t2.GetName() ?? "(unnamed)"));
                        if (t2.IsFaulted)
                        {
                            CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: chained continuation was faulted.");
                            if (t.IsFaulted)
                            {
                                CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: combining result with antecedent.");
                                tcs.TrySetException(t.Exception.InnerExceptions.Concat<Exception>((IEnumerable<Exception>)t2.Exception.InnerExceptions));
                            }
                            else
                                tcs.TrySetException((IEnumerable<Exception>)t2.Exception.InnerExceptions);
                        }
                        else if (t2.IsCanceled)
                        {
                            CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: chained continuation was canceled.");
                            if (t.IsFaulted)
                            {
                                CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: reporting fault from antecedent.");
                                tcs.TrySetException((IEnumerable<Exception>)t.Exception.InnerExceptions);
                            }
                            else
                                tcs.TrySetCanceled();
                        }
                        else
                        {
                            CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: reporting result from antecedent.");
                            tcs.TrySetFromTask<TResult>(t);
                        }
                    }));
                }
                catch (OperationCanceledException ex)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: continuationFunc threw OperationCanceledException.");
                    if (t.IsFaulted)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: reporting fault from antecedent.");
                        tcs.TrySetException((IEnumerable<Exception>)t.Exception.InnerExceptions);
                    }
                    else
                        tcs.TrySetCanceled();
                }
                catch (AggregateException ex)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: continuationFunc threw AggregateException.");
                    if (t.IsFaulted)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: combining result with antecedent.");
                        tcs.TrySetException(t.Exception.InnerExceptions.Concat<Exception>((IEnumerable<Exception>)ex.InnerExceptions));
                    }
                    else
                        tcs.TrySetException((IEnumerable<Exception>)ex.InnerExceptions);
                }
                catch (Exception ex)
                {
                    CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: continuationFunc threw Exception.");
                    if (t.IsFaulted)
                    {
                        CommonTaskExtensions.LocalTracer.TraceInformation("ThenAlways: combining result with antecedent.");
                        tcs.TrySetException(t.Exception.InnerExceptions.Concat<Exception>(ex));
                    }
                    else
                        tcs.TrySetException(ex);
                }
            }), cancellationToken, continuationOptions, scheduler);
        }

        private class DelayPromise : TaskCompletionSource<object>
        {
            private readonly CancellationToken cancellationToken;
            private CancellationTokenRegistration registration;
            private Timer timer;

            internal DelayPromise(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
                this.timer = new Timer((TimerCallback)(self => this.Complete()));
                this.registration = cancellationToken.CanBeCanceled ? cancellationToken.Register((Action)(() => this.Complete())) : new CancellationTokenRegistration();
            }

            internal void Complete()
            {
                if (this.cancellationToken.IsCancellationRequested)
                    this.TrySetCanceled();
                else
                    this.TrySetResult((object)null);
                this.timer.Dispose();
                this.registration.Dispose();
            }

            internal void StartTimer(int millisecondsDelay)
            {
                this.timer.Change(millisecondsDelay, -1);
            }
        }

        private class TaskName
        {
            private readonly string name;
            private readonly WeakReference task;

            public bool IsAlive
            {
                get
                {
                    return this.task.IsAlive;
                }
            }

            public string Name
            {
                get
                {
                    if (!this.task.IsAlive)
                        return (string)null;
                    return this.name;
                }
            }

            public TaskName(Task task, string name)
            {
                this.task = new WeakReference((object)task);
                this.name = name;
            }
        }
    }
}
