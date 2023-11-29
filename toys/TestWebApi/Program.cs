using FluentUploads;
using OneOf;

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

OneOf<IValueHttpResult<UploadMetadata>, IResult> result = TypedResults.BadRequest("yo");
GenericHack<UploadMetadata, IResult> hack = TypedResults.BadRequest("yo");
GenericHack<UploadMetadata, IResult> hack2 = TypedResults.Ok(new UploadMetadata("josh", "stuff"));

Console.WriteLine(hack2.Value);


app.MapFileUpload<UploadMetadata, string>("/files/{id}/upload",
        async (string id) =>
        {
            if (id != "josh")
            {
                return TypedResults.BadRequest("Invalid user");
            }

            return TypedResults.Ok(new UploadMetadata("josh", "stuff"));
        })
    .OnUploadComplete(async (context, metadata) =>
    {
        Console.WriteLine($"Upload complete! Completed by {metadata.User}. ${metadata.RandomGarbage}");
    });

app.Run();

record UploadMetadata(string User, string RandomGarbage);
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}