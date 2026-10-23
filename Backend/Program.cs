using Backend.Ayuda;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Registrar una sola instancia del estado del sistema.
builder.Services.AddSingleton<EstadoSistema>(servicios =>
{
    string carpetaBackend = builder.Environment.ContentRootPath;

    return new EstadoSistema(carpetaBackend);
});

// Configurar la aplicacion.
var app = builder.Build();

app.MapControllers();

app.Run();