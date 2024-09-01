namespace FaccToolkit.Results.Abstractions
{
    public static class Results
    {
        public static Result Success()
            => new Result(ResultType.Success, null);

        public static Result Failure(string message)
            => new Result(ResultType.Failure, message);

        public static Result<T> Success<T>(T data)
            => new Result<T>(ResultType.Success, null, data);

        public static Result<T> Failure<T>(string message)
            => new Result<T>(ResultType.Failure, message, default);
    }
}
