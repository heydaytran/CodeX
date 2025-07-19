using Domain.Exceptions;

namespace Domain.Extensions;

public static class ErrorOrExtensions
{
    public static ErrorOr<List<T>> Join<T>(this List<ErrorOr<T>> errorOrs)
    {
        var errors = errorOrs.SelectMany(e => e.Errors).ToList();
        if (errors.Count != 0)
        {
            return ErrorOr<List<T>>.From(errors);
        }

        return ErrorOrFactory.From(errorOrs.Select(e => e.Value).ToList());
    }

    public static ErrorOr<T> ToErrorOr<T>(this Exception exp) => 
        ErrorOr<T>.From([exp.ToError()]);

    private static Error ToError(this Exception exp) =>
        Error.Failure(code: exp.GetType().ToString(), description: exp.Message);

    public static ErrorOr<IEnumerable<TOut>> SelectManyOrError<TIn, TOut>(this IEnumerable<TIn> values, Func<TIn, ErrorOr<IEnumerable<TOut>>> selector)
    {
        var selected = values.Select(selector).ToList();
        if (selected.Any(t => t.IsError))
        {
            return ErrorOr<IEnumerable<TOut>>.From(selected.SelectMany(t => t.Errors).ToList());
        }

        return ErrorOrFactory.From(selected.SelectMany(t => t.Value));
    }

    public static ErrorOr<IEnumerable<TOut>> SelectOrError<TIn, TOut>(this IEnumerable<TIn> values, Func<TIn, ErrorOr<TOut>> selector)
    {
        var selected = values.Select(selector).ToList();
        var errored = selected.Where(t => t.IsError);
        if (errored.Any())
        {
            return ErrorOr<IEnumerable<TOut>>.From(errored.SelectMany(t => t.Errors).ToList());
        }

        return ErrorOrFactory.From(selected.Select(t => t.Value));
    }

    public static string ToLogString(this IErrorOr errorOr) => 
        errorOr.Errors?.ToLogString() ?? string.Empty;

    public static string ToLogString(this List<Error> errors) => 
        string.Join(",", errors.Select(e => e.ToLogString()));

    public static string ToLogString(this Error error) =>
        $"{error.Code} - {error.Description}";

    public static void ThrowIfError(this IErrorOr errorOr)
    {
        if (!errorOr.IsError)
        {
            return;
        }

        if (errorOr.Errors is null || errorOr.Errors.Count == 0)
        {
            throw new Exception($"Unspecified erroror error.");
        }

        throw new ErrorOrException(errorOr.Errors);
    }

    public static ErrorOr<TOut> Map<TIn, TOut>(this ErrorOr<TIn> errorOr, TOut value) => 
        errorOr.IsError ?
            ErrorOr<TOut>.From(errorOr.Errors) :
            ErrorOrFactory.From(value);

    public static ErrorOr<IEnumerable<TOut>> UntilError<TIn, TOut>(
        this IEnumerable<TIn> items,
        Func<TIn, ErrorOr<TOut>> func)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(func);

        List<TOut> results = [];

        foreach (var item in items)
        {
            var result = func(item);
            if (result.IsError)
            {
                return ErrorOr<IEnumerable<TOut>>.From(result.Errors);
            }

            results.Add(result.Value);
        }

        return results.ToErrorOr<IEnumerable<TOut>>();
    }

    public static async Task<ErrorOr<IEnumerable<TOut>>> UntilErrorAsync<TIn, TOut>(
        this IEnumerable<TIn> items,
        Func<TIn, Task<ErrorOr<TOut>>> func)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(func);

        List<TOut> results = [];

        foreach (var item in items)
        {
            var result = await func(item);
            if (result.IsError)
            {
                return ErrorOr<IEnumerable<TOut>>.From(result.Errors);
            }

            results.Add(result.Value);
        }

        return results.ToErrorOr<IEnumerable<TOut>>();
    }

    public static async Task<ErrorOr<TOut>> UntilSuccessAsync<TIn, TOut>(
        this IEnumerable<TIn> items,
        Func<TIn, Task<ErrorOr<TOut>>> func,
        Error? noSuccessError = null,
        Action<TIn>? onError = null)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(func);

        ErrorOr<TOut> result = noSuccessError ?? Error.Unexpected();

        foreach (var item in items)
        {
            result = await func(item);
            if (!result.IsError)
            {
                return result.Value;
            }

            if (onError is not null && 
                item is not null)
            {
                onError(item);
            }
        }

        return result;
    }

    public static ErrorOr<TValue> ThenIf<TValue>(this ErrorOr<TValue> errorOr, bool predicate, Func<TValue, TValue> onValue) => 
        errorOr.ThenIf(() => predicate, onValue);

    public static ErrorOr<TValue> ThenIf<TValue>(this ErrorOr<TValue> errorOr, Func<bool> predicate, Func<TValue, TValue> onValue)
    {
        if (errorOr.IsError)
        {
            return errorOr.Errors;
        }

        return predicate() ? onValue(errorOr.Value) : errorOr.Value;
    }

    public static async Task<ErrorOr<TValue>> ThenDoIfAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, bool> predicate, Func<TValue, Task> action) => 
        await (await errorOr
            .ConfigureAwait(continueOnCapturedContext: false))
            .ThenDoIfAsync(predicate, action)
            .ConfigureAwait(continueOnCapturedContext: false);

    public static async Task<ErrorOr<TValue>> ThenDoIfAsync<TValue>(this ErrorOr<TValue> errorOr, Func<TValue, bool> predicate, Func<TValue, Task> action)
    {
        if (errorOr.IsError)
        {
            return errorOr.Errors;
        }

        if (predicate(errorOr.Value))
        {
            await action(errorOr.Value).ConfigureAwait(continueOnCapturedContext: false);
        }

        return errorOr;
    }
}