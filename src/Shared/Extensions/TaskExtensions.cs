namespace Shared.Extensions;

internal static class TaskExtensions
{
    internal static T WaitAndGetResult<T>(this Task<T> task)
    {
        return task
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}