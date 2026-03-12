using System.IO.Pipelines;

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
var tareas = new List<Tarea> {new Tarea(1, "aprender c#"),new Tarea(2,"aprender react") };

//creacion de rutas
app.MapGet("/api/tareas",() => tareas);

app.MapPost("/api/tareas", (Tarea nuevaTarea) =>
{
    var tarea = nuevaTarea with{Id = tareas.Count +1 };
    tareas.Add(tarea);
    return Results.Ok(tarea);
});

app.MapDelete("/api/tareas/{id}",(int id) =>
{
    var tareaEliminada = tareas.FirstOrDefault(t => t.Id ==id);
    if(tareaEliminada == null) return Results.NotFound();
    tareas.Remove(tareaEliminada);
    return Results.Ok(new {mensaje= "tarea eliminada", id});
}) ;

app.MapPut("/api/tareas/{id}", (int id, Tarea tareaEditada) =>
{
    var index = tareas.FindIndex(t => t.Id == id);
    if (index == -1) return Results.NotFound();

    // Actualizamos la tarea en la lista
    tareas[index] = tareaEditada with { Id = id };
    return Results.Ok(tareas[index]);
});
//app.MapGet("/api/saludo", () => new { mensaje = "¡Conexión exitosa!", usuario = "Carito" });

// esta linea hace que el servidor pueda arrancar
app.Run();

// Definición del objeto para el clima
record Tarea(int Id, string Nombre);