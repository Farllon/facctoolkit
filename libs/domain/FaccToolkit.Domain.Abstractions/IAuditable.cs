using System;

namespace FaccToolkit.Domain.Abstractions
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; }

        DateTime? UpdatedAt { get; }
    }
}
