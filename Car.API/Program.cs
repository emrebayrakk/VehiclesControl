using Car.API.Data;
using Car.API.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddScoped<IDbConnectionFactory, MongoDbConnectionFactory>();
builder.Services.AddScoped<ICarService, CarService>();

builder.Services.AddMassTransit(conf =>
{
    conf.SetKebabCaseEndpointNameFormatter();
    conf.SetInMemorySagaRepositoryProvider();

    var asb = typeof(Program).Assembly;

    conf.AddConsumers(asb);
    conf.AddSagaStateMachines(asb);
    conf.AddSagas(asb);
    conf.AddActivities(asb);

    conf.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("myuser");
            h.Password("mypass");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
