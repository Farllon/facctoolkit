using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Endpoints;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfra(builder.Configuration);

var app = builder.Build();

app.MapAuthor();
app.MapPost();

app.Run();
