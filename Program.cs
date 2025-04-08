using Tcc_DbTracker_API.Models;
using Tcc_DbTracker_API.Repository;
using Tcc_DbTracker_API.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A variável de ambiente 'DefaultConnection' não está configurada.");
}

builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

builder.Services.Configure<EmailModel>(
    builder.Configuration.GetSection("EmailModel"));

// Add services to the container.
builder.Services.AddScoped<Conn_SqlServer>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
