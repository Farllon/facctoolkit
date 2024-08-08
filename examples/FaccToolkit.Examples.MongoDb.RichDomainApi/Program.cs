using FaccToolkit.Examples.MongoDb.RichDomainApi.Endpoints;
using FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfra();

var app = builder.Build();

app.MapAuthor();
app.MapPost();

app.Run();
