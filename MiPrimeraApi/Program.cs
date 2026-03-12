var builder = WebApplication.CreateBuilder(args);

// 1. SERVICIOS
// herramientas de uso para la api
builder.Services.AddOpenApi();
//configuracion para que react pida cosas
builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// 2. MIDDLEWARE
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//obliga la conexion segura https
app.UseHttpsRedirection();
//activa las configuraciones para react y debe ir antes que las rutas
app.UseCors("NuevaPolitica");

// 3. RUTAS
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

//creacion de rutas
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapGet("/api/saludo", () => new { mensaje = "¡Conexión exitosa!", usuario = "Carito" });

// esta linea hace que el servidor pueda arrancar
app.Run();

// Definición del objeto para el clima
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}