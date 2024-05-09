var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new Autofac.Extensions.DependencyInjection.AutofacServiceProviderFactory());

// Manually create an instance of the Startup class
var startup = new Abmes.DataCollector.Vault.Web.Library.Startup(builder.Configuration);

// Manually call ConfigureServices()
startup.ConfigureServices(builder.Services);

builder.Host.ConfigureContainer<Autofac.ContainerBuilder>(startup.ConfigureContainer);


var app = builder.Build();


// Call Configure(), passing in the dependencies
startup.Configure(app, app.Environment);


app.Run();
