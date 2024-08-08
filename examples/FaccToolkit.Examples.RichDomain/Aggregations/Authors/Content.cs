using FaccToolkit.Domain.Rich;
using System;
using System.Collections.Generic;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public class Content : ValueObject
    {
        public string Value { get; }

        public const int MinLength = 255;
        public const int MaxLength = 4096;

        private Content(string value)
        {
            Value = value;
        }

        public static Content Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("The content can't be empty", nameof(value));

            if (value.Length < MinLength)
                throw new ArgumentException($"The content must contain at least {MinLength} characters", nameof(value));

            if (value.Length > MaxLength)
                throw new ArgumentException($"The content must contain a maximum of {MaxLength} characters", nameof(value));

            return new Content(value);
        }

        public static implicit operator Content(string value) => Create(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
