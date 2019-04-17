using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abmes.DataCollector.Collector.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.AccessTokenValidation;
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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddOptions();
            services.AddLogging();

            var identityServerAuthenticationSettings = 
                    Configuration
                    .GetSection("IdentityServerAuthenticationSettings")
                    .Get<IdentityServerAuthenticationSettings>();

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(x =>
                {
                    x.Authority = identityServerAuthenticationSettings.Authority;
                    x.ApiName = identityServerAuthenticationSettings.ApiName;
                    x.ApiSecret = identityServerAuthenticationSettings.ApiSecret;
                    x.RequireHttpsMetadata = false;
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

            var builder = new ContainerBuilder();

            builder.Populate(services);

            ContainerRegistrations.RegisterFor(builder, Configuration);
            builder.RegisterInstance(Configuration).As<IConfiguration>();

            this.ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            LoggingConfigurator.Configure(loggerFactory);
        }
    }
}
