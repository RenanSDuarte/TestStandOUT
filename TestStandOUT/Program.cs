using TestStandOUT.Services;
using Microsoft.EntityFrameworkCore;
using TestStandOUT.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add Alpha
builder.Services.AddHttpClient<IAlphaVantageService, AlphaVantageService>(client =>
{
    client.BaseAddress = new Uri("https://www.alphavantage.co/");
});
// Add DBContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=fx.db"));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Creating the BD
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
