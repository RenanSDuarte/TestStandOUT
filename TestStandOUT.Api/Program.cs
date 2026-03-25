using Microsoft.EntityFrameworkCore;
using TestStandOUT.Api.Data;
using TestStandOUT.Api.Models;
using TestStandOUT.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IAlphaVantageService, AlphaVantageService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=fxrates.db"));

// 2. Add HttpClient for AlphaVantage
builder.Services.AddHttpClient<IAlphaVantageService, AlphaVantageService>(client => {
    client.BaseAddress = new Uri("https://www.alphavantage.co/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
    }
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
