namespace FaccToolkit.Results.Abstractions
{
    public class FailureResultGetValueException : Exception
    {
        public const string ExceptionMessage = "The get value of failure result is not possible";

        public static FailureResultGetValueException Instance = new();

        public FailureResultGetValueException() : base(ExceptionMessage)
        {
            
        }
    }
}
