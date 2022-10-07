var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new Autofac.Extensions.DependencyInjection.AutofacServiceProviderFactory());

// Manually create an instance of the Startup class
var startup = new Abmes.DataCollector.Vault.Service.Startup(builder.Configuration);

// Manually call ConfigureServices()
startup.ConfigureServices(builder.Services);

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Host.ConfigureContainer<Autofac.ContainerBuilder>(startup.ConfigureContainer);


var app = builder.Build();


// Call Configure(), passing in the dependencies
startup.Configure(app, app.Environment);


app.Run();
