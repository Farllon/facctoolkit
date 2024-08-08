using FaccToolkit.Domain.Rich;
using System;
using System.Collections.Generic;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public class Name : ValueObject
    {
        public string Value { get; }

        public const int MinLength = 3;
        public const int MaxLength = 64;

        private Name(string value)
        {
            Value = value;
        }

        public static Name Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("The name can't be empty", nameof(value));

            if (value.Length < MinLength)
                throw new ArgumentException($"The name must contain at least {MinLength} characters", nameof(value));

            if (value.Length > MaxLength)
                throw new ArgumentException($"The name must contain a maximum of {MaxLength} characters", nameof(value));
            
            return new Name(value);
        }

        public static implicit operator Name(string value) => Create(value);  

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
