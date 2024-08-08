using FaccToolkit.Domain.Rich;
using System;
using System.Collections.Generic;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public class Title : ValueObject
    {
        public string Value { get; }

        public const int MinLength = 10;
        public const int MaxLength = 80;

        private Title(string value)
        {
            Value = value;
        }

        public static Title Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("The title can't be empty", nameof(value));

            if (value.Length < MinLength)
                throw new ArgumentException($"The title must contain at least {MinLength} characters", nameof(value));

            if (value.Length > MaxLength)
                throw new ArgumentException($"The title must contain a maximum of {MaxLength} characters", nameof(value));

            return new Title(value);
        }

        public static implicit operator Title(string value) => Create(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
