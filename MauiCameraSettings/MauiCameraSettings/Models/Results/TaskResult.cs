namespace MauiCameraSettings.Models.Results;

public class TaskResult
{
    public TaskResult()
    {
    }

    public bool Success { get; set; }
    public bool IsCancelled { get; set; } = false;
    public string Message { get; set; }
    public object Content { get; set; }


    public static TaskResult Failed(string msg)
    {
        return Failed(null, msg);
    }

    public static TaskResult Cancelled(string msg)
    {
        return Failed(null, msg, cancelled: true);
    }

    public static TaskResult Failed(object content, string msg, bool cancelled = false)
    {
        return new TaskResult()
        {
            Success = false,
            Message = msg,
            Content = content,
            IsCancelled = cancelled
        };
    }        

    public static TaskResult Succeeded()
    {
        return new TaskResult() { Success = true };
    }

    public static TaskResult Succeeded(object content, string msg = "")
    {
        return new TaskResult()
        {
            Success = true,
            Message = msg,
            Content = content
        };
    }

    public static TaskResult CreateFromTypedResult<T>(TypedTaskResult<T> typedTaskResult)
    {
        if (!typedTaskResult.Success && typedTaskResult.Content == null &&
            typedTaskResult.UntypedResult != null &&
            typedTaskResult.UntypedResult.Content != null)
        {
            return new TaskResult()
            {
                Success = typedTaskResult.Success,
                Message = typedTaskResult.Message,
                Content = typedTaskResult.UntypedResult.Content,
                IsCancelled = typedTaskResult.IsCancelled
            };
        }
        else
        {
            return new TaskResult()
            {
                Success = typedTaskResult.Success,
                Message = typedTaskResult.Message,
                Content = typedTaskResult.Content,
                IsCancelled = typedTaskResult.IsCancelled
            };
        }
        
    }
}