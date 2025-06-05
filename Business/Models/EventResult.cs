namespace Business.Models;

public class EventResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }
}

public class EventResult<T> : EventResult
{
    public T? Result { get; set; }
}
