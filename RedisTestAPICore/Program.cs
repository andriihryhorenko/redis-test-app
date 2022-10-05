using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(new ConfigurationOptions()
{
    KeepAlive = 0,
    AllowAdmin = true,
    EndPoints = { { "master", 6379 }, { "slave", 6379 } },
    ConnectTimeout = 5000,
    ConnectRetry = 5,
    SyncTimeout = 5000,
    AbortOnConnectFail = false,

}));


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "myappname v1"));

app.UseAuthorization();

app.MapControllers();

app.Run();
