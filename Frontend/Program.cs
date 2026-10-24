
using Frontend.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Registrar Razor Pages.
builder.Services.AddRazorPages();

// Registrar el cliente HTTP que se comunicara
// con el Backend.
builder.Services.AddHttpClient<ClienteApi>(cliente =>
{
    cliente.BaseAddress = new Uri("http://localhost:5080/");

    cliente.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

// Configuracion de errores.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Servir archivos estaticos.
app.UseStaticFiles();

app.UseRouting();

// Configurar autorizacion.
app.UseAuthorization();

// Registrar las paginas Razor.
app.MapRazorPages();

// Iniciar el Frontend.
app.Run();
