namespace Core.Utils;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public string Message { get; set; }
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string message) => new() { IsSuccess = false, Message = message };
}

public class Result<T>
{
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    public string? Message { get; private init; }
    public T? Data { get; private init; }

    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static Result<T> Failure(string message) => new() { IsSuccess = false, Message = message };
}