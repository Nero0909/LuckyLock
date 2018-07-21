using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventAggregator.API.Infrastructure.Filters;
using EventAggregator.API.Infrastructure.Services;
using EventAggregator.EventConsuming.Consumers;
using EventAggregator.Repository;
using EventAggregator.Repository.Serializer;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventAggregator.API
{
    public class Startup
    {
        private IBusControl _bus;

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

                    options.ApiName = "event_aggregator";
                });

            DapperConfig.Init();

            services.AddSingleton(Configuration);

            var builder = new ContainerBuilder();

            builder.RegisterType<LocksActivityConsumer>()
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

                        cfg.ReceiveEndpoint("event_aggregator_subscriber", ec => { ec.Consumer<LocksActivityConsumer>(context); });
                    });

                    return bus;
                })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();

            builder.RegisterType<LocksActivityRepository>()
                .As<ILocksActivityRepository>()
                .SingleInstance();

            builder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            builder.RegisterType<LocksActivityService>()
                .As<ILocksActivityService>()
                .SingleInstance();

            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            builder.RegisterType<EventSerializer>()
                .As<IEventSerializer>()
                .SingleInstance();

            builder.Populate(services);
            ApplicationContainer = builder.Build();
            _bus = ApplicationContainer.Resolve<IBusControl>();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            app.UseAuthentication();
            app.UseMvc();

            appLifetime.ApplicationStarted.Register(() => _bus.Start());
            appLifetime.ApplicationStopping.Register(() => _bus.Stop());
        }
    }
}
