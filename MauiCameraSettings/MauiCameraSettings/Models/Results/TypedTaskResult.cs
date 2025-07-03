namespace MauiCameraSettings.Models.Results;

public class TypedTaskResult<T>
{
    public bool Success { get; set; }        
    public string Message { get; set; }
    public T Content { get; set; }
    public bool IsCancelled { get; set; } = false;
    public TaskResult UntypedResult { get; set; }

    public TypedTaskResult()
    {
    }       

    public static TypedTaskResult<T> Failed(string message)
    {
        return new TypedTaskResult<T>() {
            Success = false,
            Message = message};
    }

    public static TypedTaskResult<T> Failed<T2>(TypedTaskResult<T2> failedtypedResult)
    {
        return new TypedTaskResult<T>()
        {
            Success = false,
            Message = failedtypedResult.Message,
            UntypedResult = failedtypedResult.UntypedResult
        };
    }

    public static TypedTaskResult<T> Succeeded()
    {
        return new TypedTaskResult<T>() { Success = true };
    }

    public static TypedTaskResult<T> Succeeded(T content, string msg = "")
    {
        return new TypedTaskResult<T>()
        {
            Success = true,
            Message = msg,
            Content = content
        };
    }

    public static TypedTaskResult<T> FromTaskResult(TaskResult result)
    {
        return new TypedTaskResult<T>()
        {
            UntypedResult = result,
            Success = result.Success,
            Message = result.Message,
            IsCancelled = result.IsCancelled
        };
    }
}