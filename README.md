# FaccToolkit

FaccToolkit is a free, open-source framework for .NET. FaccToolkit speeds up the development process by providing a range of features used in many projects. 

Build Status
------------

| Branch | Status                                                                                                                                                                                                                                   |
|--------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/domain-abstractions.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/domain-abstractions.yml)                       |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/domain-anemic.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/domain-anemic.yml)                                   |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/domain-rich.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/domain-rich.yml)                                       |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/domain-rich-ext-mediatr.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/domain-rich-ext-mediatr.yml)               |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-abstractions.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-abstractions.yml)             |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ef-abstractions.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ef-abstractions.yml)       |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ef-anemic.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ef-anemic.yml)                   |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ef-rich.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ef-rich.yml)                       |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-mongo-abstractions.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-mongo-abstractions.yml) |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-mongo-anemic.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-mongo-anemic.yml)             |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-mongo-rich.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-mongo-rich.yml)                 |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/caching-abstractions.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/caching-abstractions.yml)                     |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/caching-redis.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/caching-redis.yml)                                   |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/caching-ser-systextjson.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/caching-ser-systextjson.yml)               |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ext-caching-anemic.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ext-caching-anemic.yml) |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ext-caching-rich.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/persistence-ext-caching-rich.yml)     |
| master | [![master](https://github.com/Farllon/facctoolkit/actions/workflows/results-abstractions.yml/badge.svg?branch=master&event=push)](https://github.com/Farllon/facctoolkit/actions/workflows/results-abstractions.yml)                     |

FaccToolkit NuGet Packages
---------------------------

| Package Name                                                                                                                                      | .NET     | .NET Standard | .NET Framework |
|---------------------------------------------------------------------------------------------------------------------------------------------------|----------|---------------|----------------|
| **Caching**                                                                                                                                       | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Caching.Abstractions](https://www.nuget.org/packages/FaccToolkit.Caching.Abstractions)                                               | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Caching.Redis](https://www.nuget.org/packages/FaccToolkit.Caching.Redis)                                                             | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Caching.Serializers.System.Text.Json](https://www.nuget.org/packages/FaccToolkit.Caching.Serializers.System.Text.Json)               | 6.0, 8.0 | 2.1           |                |
| **Domain**                                                                                                                                        | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Domain.Abstractions](https://www.nuget.org/packages/FaccToolkit.Domain.Abstractions)                                                 | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Domain.Anemic](https://www.nuget.org/packages/FaccToolkit.Domain.Anemic)                                                             | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Domain.Rich](https://www.nuget.org/packages/FaccToolkit.Domain.Rich)                                                                 | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Domain.Rich.Extensions.MediatR](https://www.nuget.org/packages/FaccToolkit.Domain.Rich.Extensions.MediatR)                           | 6.0, 8.0 | 2.1           |                |
| **Persistence**                                                                                                                                   | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Persistence.Abstractions](https://www.nuget.org/packages/FaccToolkit.Persistence.Abstractions)                                       | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Persistence.EntityFramework.Abstractions](https://www.nuget.org/packages/FaccToolkit.Persistence.EntityFramework.Abstractions)       | 6.0, 8.0 |               |                |
| [FaccToolkit.Persistence.EntityFramework.AnemicDomain](https://www.nuget.org/packages/FaccToolkit.Persistence.EntityFramework.AnemicDomain)       | 6.0, 8.0 |               |                |
| [FaccToolkit.Persistence.EntityFramework.RichDomain](https://www.nuget.org/packages/FaccToolkit.Persistence.EntityFramework.RichDomain)           | 6.0, 8.0 |               |                |
| [FaccToolkit.Persistence.Extensions.Caching.AnemicDomain](https://www.nuget.org/packages/FaccToolkit.Persistence.Extensions.Caching.AnemicDomain) | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Persistence.Extensions.Caching.RichDomain](https://www.nuget.org/packages/FaccToolkit.Persistence.Extensions.Caching.RichDomain)     | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Persistence.MongoDb.Abstractions](https://www.nuget.org/packages/FaccToolkit.Persistence.MongoDb.Abstractions)                       | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Persistence.MongoDb.AnemicDomain](https://www.nuget.org/packages/FaccToolkit.Persistence.MongoDb.AnemicDomain)                       | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Persistence.MongoDb.RichDomain](https://www.nuget.org/packages/FaccToolkit.Persistence.MongoDb.RichDomain)                           | 6.0, 8.0 | 2.1           |                |
| **Results**                                                                                                                                       | 6.0, 8.0 | 2.1           |                |
| [FaccToolkit.Results.Abstractions](https://www.nuget.org/packages/FaccToolkit.Results.Abstractions)                                               | 6.0, 8.0 | 2.1           |                |

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
