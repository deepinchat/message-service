using Deepin.Messaging.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseUrls("http://*:5000");

builder.AddApplicationService();

var app = builder.Build();

app.UseApplicationService();

app.Run();