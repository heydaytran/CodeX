using Domain.Extensions;

namespace Infrastructure.Extensions;

public static class FunctionalExtensions
{
    public static async Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> asyncFunc)
    {
        foreach (T element in collection)
        {
            await asyncFunc(element);
        }
    }

    /// <summary>
    /// Invokes the specified action for each element in the collection.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="action">The action to invoke for each element.</param>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T element in collection)
        {
            action(element);
        }
    }

    /// <summary>
    /// Invokes the specified action for each element in the collection.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="action">The action to invoke for each element along with the index.</param>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<int, T> action)
    {
        int index = 0;
        foreach (T element in collection)
        {
            action(index++, element);
        }
    }

    /// <summary>
    /// Performs the specified action and returns the same instance.
    /// </summary>
    /// <typeparam name="T">The instance type.</typeparam>
    /// <param name="instance">The instance.</param>
    /// <param name="action">The action to perform.</param>
    /// <returns>The same instance.</returns>
    public static T Tap<T>(this T instance, Action action)
    {
        action();

        return instance;
    }

    /// <summary>
    /// Performs the specified action with the current instance and returns the same instance.
    /// </summary>
    /// <typeparam name="T">The instance type.</typeparam>
    /// <param name="instance">The instance.</param>
    /// <param name="action">The action to perform.</param>
    /// <returns>The same instance.</returns>
    public static T Tap<T>(this T instance, Action<T> action)
    {
        action(instance);

        return instance;
    }

    /// <summary>
    /// Performs the specified async action with the current instance and returns the same instance.
    /// </summary>
    /// <typeparam name="T">The instance type.</typeparam>
    /// <param name="instance">The instance.</param>
    /// <param name="action">The async action to perform.</param>
    /// <returns>The same instance.</returns>
    public static async Task<T> TapAsync<T>(this T instance, Func<T, Task> action)
    { 
        await action(instance).ConfigureAwait(continueOnCapturedContext: false);

        return instance;
    }

    public static async Task<ErrorOr<T>> TimeoutAfter<T>(this Task<ErrorOr<T>> task, TimeSpan timeout, Func<Error> OnTimeout)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();

        var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == task)
        {
            timeoutCancellationTokenSource.Cancel();
            return await task;
        }
        else
        {
            return OnTimeout();
        }
    }

    public async static Task<ErrorOr<T>> TryCatchErrorAsync<T>(this Func<Task<ErrorOr<T>>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception exception)
        {
            return exception.ToErrorOr<T>();
        }
    }

    public static void TryCatchFinally(this Action action, Action<Exception> catchAction, Action finallyAction)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            catchAction(exception);
        }
        finally
        {
            finallyAction();
        }
    }
}