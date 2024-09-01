namespace FaccToolkit.Results.Abstractions
{
    public class Result
    {
        public bool IsSuccess => Type is ResultType.Success;

        public ResultType Type { get; }

        public string? Message { get; }

        public Result(ResultType type, string? message) 
        {
            Type = type;
            Message = message;
        }
    }

    public class Result<T> : Result
    {
        private T _value;

        public T Value => IsSuccess ? _value : throw FailureResultGetValueException.Instance;

        public Result(ResultType type, string? message, T value) : base(type, message)
        {
            _value = value;
        }
    }
}
