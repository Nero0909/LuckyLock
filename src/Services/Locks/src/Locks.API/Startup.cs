using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Client;
using Locks.API.Infrastructure.Filters;
using Locks.API.Infrastructure.Services;
using Locks.EventPublishing;
using Locks.Repository;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locks.API
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

                    options.ApiName = "locks";
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

            builder.RegisterType<LocksTagsService>()
                .As<ILocksTagsService>()
                .SingleInstance();

            builder.RegisterType<LocksService>()
                .As<ILocksService>()
                .SingleInstance();

            builder.RegisterType<TokensClient>()
                .As<ITokensClient>()
                .SingleInstance();

            builder.RegisterType<LocksRepository>()
                .As<ILocksRepository>()
                .SingleInstance();

            builder.RegisterType<LocksTagsRepository>()
                .As<ILocksTagsRepository>()
                .SingleInstance();

            builder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            builder.RegisterType<EventPublisher>()
                .As<IEventPublisher>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        cfg.Host(new Uri(Configuration["RabbitMq.Url"]), h =>
                        {
                            h.Username(Configuration["RabbitMq.Username"]);
                            h.Password(Configuration["RabbitMq.Password"]);
                        });
                    });

                    return bus;
                })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();

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
