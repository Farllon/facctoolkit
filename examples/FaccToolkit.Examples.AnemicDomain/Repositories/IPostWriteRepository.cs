﻿using FaccToolkit.Domain.Anemic;
using FaccToolkit.Examples.AnemicDomain.Entities;
using System;

namespace FaccToolkit.Examples.AnemicDomain.Repositories
{
    public interface IPostWriteRepository : IWriteRepository<Post, Guid>
    {
    }
}
