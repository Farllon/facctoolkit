using FaccToolkit.Domain.Abstractions;
using System;

namespace FaccToolkit.Examples.AnemicDomain.Entities
{
    public class Author : Entity<Guid>
    {
        public string Name { get; set; }

        public Author(Guid id, string name) : base(id)
        {
            Name = name;
        }

        public Author(string name) : this(Guid.NewGuid(), name)
        {
            
        }
    }
}
