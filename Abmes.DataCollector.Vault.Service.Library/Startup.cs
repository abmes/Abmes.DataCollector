using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abmes.DataCollector.Collector.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Vault.WebAPI.Authorization;
using Abmes.DataCollector.Utils.AspNetCore;
using Abmes.DataCollector.Vault.Service.Configuration;
using Microsoft.Extensions.Hosting;

namespace Abmes.DataCollector.Vault.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IContainer ApplicationContainer { get; set; }
        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddOptions();
            services.AddLogging(loggingBuilder =>
            {
                LoggingConfigurator.Configure(loggingBuilder);
            });

            var identityServerAuthenticationSettings = 
                    Configuration
                    .GetSection("IdentityServerAuthenticationSettings")
                    .Get<IdentityServerAuthenticationSettings>();

            services
                .AddAuthentication(IdentityModel.AspNetCore.OAuth2Introspection.OAuth2IntrospectionDefaults.AuthenticationScheme)
                .AddOAuth2Introspection(
                    IdentityModel.AspNetCore.OAuth2Introspection.OAuth2IntrospectionDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Authority = identityServerAuthenticationSettings.Authority;

                        // this maps to the API resource name and secret
                        options.ClientId = identityServerAuthenticationSettings.ApiName;
                        options.ClientSecret = identityServerAuthenticationSettings.ApiSecret;

                        options.DiscoveryPolicy.AuthorityValidationStrategy = new IdentityModel.Client.StringComparisonAuthorityValidationStrategy(StringComparison.OrdinalIgnoreCase);
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserAllowedDataCollection", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new UserAllowedDataCollectionRequirement());
                }
                );
            });

            services.AddSingleton<IAuthorizationHandler, UserAllowedDataCollectionHandler>();

            ServicesConfiguration.Configure(services, Configuration);
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you. If you
        // need a reference to the container, you need to use the
        // "Without ConfigureContainer" mechanism shown later.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac
            ContainerRegistrations.RegisterFor(builder, Configuration);
            builder.RegisterInstance(Configuration).As<IConfiguration>();
            //...
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
