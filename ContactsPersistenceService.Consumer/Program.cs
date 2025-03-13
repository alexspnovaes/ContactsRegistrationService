using ContactsPersistenceService.Consumer.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ContactsPersistenceService.Consumer.Data.Repositories;
using ContactsPersistenceService.Consumer.Services;
using Microsoft.Extensions.Configuration;
using System;
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlServer(
            hostContext.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));


        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IContactService, ContactService>();
        services.AddHostedService<RabbitMqConsumerService>();
    });

await builder.Build().RunAsync();
