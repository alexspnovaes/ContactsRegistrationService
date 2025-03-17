using ContactsRegistrationService.Api.Services;
using Prometheus;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Cadastro de Contatos",
        Version = "v1",
        Description = "API responsável pelo cadastro e publicação de contatos via RabbitMQ."
    });
});

builder.Services.AddOpenApi();

builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("ServiceBusConnection");
    return new ServiceBusClient(connectionString);
});

builder.Services.AddSingleton(provider =>
{
    var client = provider.GetRequiredService<ServiceBusClient>();
    return client.CreateSender("contacts-queue");
});


builder.Services.AddScoped<IServiceBusPublisher, ServiceBusPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Cadastro de Contatos v1");
    });
}

app.UseHttpMetrics();
app.UseMetricServer();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
