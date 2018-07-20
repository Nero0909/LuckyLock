using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tags.API.Infrastructure.Filters;
using Tags.API.Infrastructure.Services;
using Tags.Repositories;
using Client;
using Locks.API.Infrastructure.Services;

namespace Tags.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(opt => opt.Filters.Add<GlobalExceptionFilter>())
                .AddDataAnnotations()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["IdentityUrl"];
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "tags";
                });

            DapperConfig.Init();

            services.AddSingleton(Configuration);

            var builder = new ContainerBuilder();

            var gatewaySettings = new GatewaySettings
            {
                TagsUrl = Configuration["TagsUrl"],
                LocksUrl = Configuration["LocksUrl"]
            };
            builder.Register(x => new Gateway(gatewaySettings))
                .As<IGateway>()
                .SingleInstance();

            builder.RegisterType<TagsService>()
                .As<ITagsService>()
                .SingleInstance();

            builder.RegisterType<TagsRepository>()
                .As<ITagsRepository>()
                .SingleInstance();

            builder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            builder.RegisterType<TokensClient>()
                .As<ITokensClient>()
                .SingleInstance();

            builder.Populate(services);
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        { 
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
