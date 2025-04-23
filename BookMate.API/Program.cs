using Amazon.DynamoDBv2;
using BookMate.Workflows.Activities;
using BookMate.Workflows.Contracts;
using BookMate.Workflows.Services;
using Temporalio.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register DynamoDB client for interaction
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddSingleton<IDynamoDBService, DynamoDBService>();


builder.Services.AddControllers();

builder.Services.AddSingleton(ctx =>
    TemporalClient.ConnectAsync(new()
    {
        TargetHost = "localhost:7233",
        LoggerFactory = ctx.GetRequiredService<ILoggerFactory>(),
    }).GetAwaiter().GetResult());

builder.Services.AddSingleton<BookInventoryActivities>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
