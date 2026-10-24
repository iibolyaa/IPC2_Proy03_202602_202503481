using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class GraficasModel : PageModel
    {
        private readonly ClienteApi clienteApi;

        [BindProperty]
        public string TipoGrafica { get; set; } = "iva";

        [BindProperty]
        public DateTime? Fecha { get; set; }

        [BindProperty]
        public DateTime? FechaInicio { get; set; }

        [BindProperty]
        public DateTime? FechaFin { get; set; }

        [BindProperty]
        public string TipoMonto { get; set; } = "total";

        public string? MensajeError { get; private set; }

        public bool MostrarGrafica { get; private set; }

        public string RespuestaJson { get; private set; } = "";

        public decimal ValorMaximo { get; private set; }

        public decimal TotalEmitido { get; private set; }

        public decimal TotalRecibido { get; private set; }

        public decimal MontoTotal { get; private set; }

        public int CantidadRegistros { get; private set; }

        public GraficasModel(ClienteApi clienteApi)
        {
            this.clienteApi = clienteApi;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (TipoGrafica != "iva" &&
                TipoGrafica != "rango")
            {
                MensajeError = "Seleccione una grafica valida.";
                return Page();
            }

            try
            {
                string respuesta;

                if (TipoGrafica == "iva")
                {
                    if (!Fecha.HasValue)
                    {
                        MensajeError =
                            "Seleccione la fecha del resumen de IVA.";
                        return Page();
                    }

                    string fechaTexto = Fecha.Value.ToString(
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture
                    );

                    respuesta = await clienteApi.ConsultarIva(
                        fechaTexto
                    );
                }
                else
                {
                    if (!FechaInicio.HasValue ||
                        !FechaFin.HasValue)
                    {
                        MensajeError =
                            "Seleccione ambas fechas.";
                        return Page();
                    }

                    if (FechaInicio.Value.Date >
                        FechaFin.Value.Date)
                    {
                        MensajeError =
                            "La fecha inicial no puede superar la final.";
                        return Page();
                    }

                    if (TipoMonto != "total" &&
                        TipoMonto != "siniva")
                    {
                        MensajeError =
                            "Seleccione un tipo de monto valido.";
                        return Page();
                    }

                    string inicio = FechaInicio.Value.ToString(
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture
                    );

                    string fin = FechaFin.Value.ToString(
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture
                    );

                    respuesta = await clienteApi.ConsultarRango(
                        inicio,
                        fin,
                        TipoMonto == "total"
                    );
                }

                using JsonDocument documento =
                    JsonDocument.Parse(respuesta);

                JsonElement raiz = documento.RootElement;

                JsonElement detalle =
                    raiz.GetProperty("detalle");

                if (detalle.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException(
                        "El detalle no es un arreglo JSON."
                    );
                }

                foreach (JsonElement registro in
                    detalle.EnumerateArray())
                {
                    CantidadRegistros++;

                    if (TipoGrafica == "iva")
                    {
                        decimal emitido = registro
                            .GetProperty("ivaEmitido")
                            .GetDecimal();

                        decimal recibido = registro
                            .GetProperty("ivaRecibido")
                            .GetDecimal();

                        TotalEmitido += emitido;
                        TotalRecibido += recibido;

                        if (emitido > ValorMaximo)
                        {
                            ValorMaximo = emitido;
                        }

                        if (recibido > ValorMaximo)
                        {
                            ValorMaximo = recibido;
                        }
                    }
                    else
                    {
                        decimal monto = registro
                            .GetProperty("monto")
                            .GetDecimal();

                        MontoTotal += monto;

                        if (monto > ValorMaximo)
                        {
                            ValorMaximo = monto;
                        }
                    }
                }

                RespuestaJson = respuesta;
                MostrarGrafica = true;
            }
            catch (HttpRequestException ex)
            {
                MensajeError =
                    "Error de comunicacion con el Backend: " +
                    ex.Message;
            }
            catch (TaskCanceledException)
            {
                MensajeError =
                    "La consulta excedio el tiempo de espera.";
            }
            catch (JsonException)
            {
                MensajeError =
                    "La respuesta del Backend tiene un formato incorrecto.";
            }
            catch (InvalidOperationException)
            {
                MensajeError =
                    "Los datos recibidos son incorrectos.";
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                MensajeError =
                    "La respuesta no contiene los campos esperados.";
            }

            return Page();
        }
    }
}
