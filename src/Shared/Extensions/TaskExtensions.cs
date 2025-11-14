namespace Shared.Extensions;

public static class TaskExtensions
{
    public static T WaitAndGetResult<T>(this Task<T> task)
    {
        return task
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public static void WaitProperly(this Task task)
    {
        task.ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}