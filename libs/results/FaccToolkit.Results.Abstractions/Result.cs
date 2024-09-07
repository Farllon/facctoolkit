namespace FaccToolkit.Results.Abstractions
{
    public static class Result
    {
        public static BaseResult Success()
            => new BaseResult(ResultType.Success, null);

        public static BaseResult Failure(string message)
            => new BaseResult(ResultType.Failure, message);

        public static EvaluatedResult<T> Success<T>(T data)
            => new EvaluatedResult<T>(ResultType.Success, null, data);

        public static EvaluatedResult<T> Failure<T>(string message)
            => new EvaluatedResult<T>(ResultType.Failure, message, default!);
    }
}
