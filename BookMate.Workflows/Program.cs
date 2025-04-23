using Amazon;
using BookMate.Workflows.Activities;
using BookMate.Workflows.Workflows;
using BookMate.Workflows.Services;
using Temporalio.Client;
using Temporalio.Worker;
using Amazon.DynamoDBv2;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Temporal Client
builder.Services.AddSingleton<TemporalClient>(sp =>
{
    return TemporalClient.ConnectAsync(new TemporalClientConnectOptions
    {
        TargetHost = "localhost:7233"
    }).Result;
});

// DynamoDB
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddSingleton<IDynamoDBService, DynamoDBService>();
builder.Services.AddSingleton<BookInventoryActivities>();
builder.Services.AddSingleton<NotificationActivities>();

builder.Services.AddSingleton<IAmazonSimpleEmailService>(serviceProvider =>
{
    var region = RegionEndpoint.USEast1; 
    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    return new AmazonSimpleEmailServiceClient(region);
});
var host = builder.Build();

// Cancellation token to shutdown worker on ctrl+c
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

var client = host.Services.GetRequiredService<TemporalClient>();
var dynamoDBService = host.Services.GetRequiredService<IDynamoDBService>(); 
var sesClient = host.Services.GetRequiredService<IAmazonSimpleEmailService>();

//var activities = host.Services.GetRequiredService<BookInventoryActivities>();
var inventoryActivities = new BookInventoryActivities(dynamoDBService);
var notificationActivities = new NotificationActivities(sesClient);

// Create worker
using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions("bookmate-queue").
        AddAllActivities(inventoryActivities).
        AddAllActivities(notificationActivities).
        AddWorkflow<BookBorrowWorkflow>());

// Run worker until cancelled
Console.WriteLine("Running BookMate worker");
try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Worker cancelled");
}
