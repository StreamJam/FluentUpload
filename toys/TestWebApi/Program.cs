using FluentUploads;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast",
        () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast(
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();
            return TypedResults.Ok(forecast);
        })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapFileUpload("/files/{id}/upload", async (string id) =>
    {
        if (id != "123")
            return Results.BadRequest("Invalid ID");
        
        return Results.Ok();
    })
    .WithFileMetadata(async context => new { User = context.User.Identity.Name, RandomGarbage = "Whatever random shit you want" })
    .OnUploadComplete(async (context, metadata) =>
    {
        Console.WriteLine($"Upload complete! Completed by {metadata.User}. ${metadata.RandomGarbage}");
    });

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}