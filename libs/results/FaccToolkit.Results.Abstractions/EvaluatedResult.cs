using System;
using System.Threading.Tasks;

namespace FaccToolkit.Results.Abstractions
{
    public class EvaluatedResult<T> : BaseResult
    {
        private readonly T _value;

        public T Value => IsSuccess ? _value : throw FailureResultGetValueException.Instance;

        public EvaluatedResult(ResultType type, string? message, T value) : base(type, message)
        {
            _value = value;
        }

        public EvaluatedResult<TValue> Match<TValue>(Func<T, EvaluatedResult<TValue>> onSuccess, Func<string, EvaluatedResult<TValue>> onFail)
            => IsSuccess ? onSuccess(Value) : onFail(Message!);

        public Task<EvaluatedResult<TValue>> MatchAsync<TValue>(Func<T, Task<EvaluatedResult<TValue>>> onSuccess, Func<string, Task<EvaluatedResult<TValue>>> onFail)
            => IsSuccess ? onSuccess(Value) : onFail(Message!);

        public TValue Match<TValue>(Func<T, TValue> onSuccess, Func<string, TValue> onFail)
            => IsSuccess ? onSuccess(Value) : onFail(Message!);

        public Task<TValue> MatchAsync<TValue>(Func<T, Task<TValue>> onSuccess, Func<string, Task<TValue>> onFail)
            => IsSuccess ? onSuccess(Value) : onFail(Message!);
    }
}
