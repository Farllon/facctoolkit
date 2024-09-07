using System;
using System.Threading.Tasks;

namespace FaccToolkit.Results.Abstractions
{
    public class BaseResult
    {
        public bool IsSuccess => Type is ResultType.Success;

        public ResultType Type { get; }

        public string? Message { get; }

        public BaseResult(ResultType type, string? message) 
        {
            Type = type;
            Message = message;
        }

        public BaseResult Match(Func<BaseResult> onSuccess, Func<string, BaseResult> onFail)
            => IsSuccess ? onSuccess() : onFail(Message!);

        public Task<BaseResult> MatchAsync(Func<Task<BaseResult>> onSuccess, Func<string, Task<BaseResult>> onFail)
            => IsSuccess ? onSuccess() : onFail(Message!);

        public TValue Match<TValue>(Func<TValue> onSuccess, Func<string, TValue> onFail)
            => IsSuccess ? onSuccess() : onFail(Message!);

        public Task<TValue> MatchAsync<TValue>(Func<Task<TValue>> onSuccess, Func<string, Task<TValue>> onFail)
            => IsSuccess ? onSuccess() : onFail(Message!);
    }
}
