
using System;
using System.IO;
using System.Text.Json;
using Backend.Modelos;
using Backend.Estructuras;

namespace Backend.Archivos
{
    public class Persistencia
    {
        private readonly string rutaArchivo;
        private readonly object bloqueo;

        public Persistencia(string carpetaBackend)
        {
            string carpetaDatos = Path.Combine(carpetaBackend, "Datos");

            Directory.CreateDirectory(carpetaDatos);

            rutaArchivo = Path.Combine(carpetaDatos, "estado.json");

            bloqueo = new object();
        }

        // Guardar el estado completo del sistema.
        public void Guardar(ListaContribuyentes contribuyentes, ListaFacturas documentos,
            ListaErrores errores, ListaCorrelativos correlativos, ListaSolicitudes solicitudes)
        {
            lock (bloqueo)
            {
                string temporal = rutaArchivo + ".tmp";

                try
                {
                    // Crear un archivo temporal.
                    using (FileStream archivo = new FileStream(temporal, FileMode.Create, FileAccess.Write))
                    {
                        using (Utf8JsonWriter escritor = new Utf8JsonWriter(archivo, new JsonWriterOptions
                                {
                                    Indented = true
                                }))
                        {
                            escritor.WriteStartObject();

                            // Guardar contribuyentes.
                            escritor.WritePropertyName("Contribuyentes");

                            escritor.WriteStartArray();

                            for (int i = 0; i < contribuyentes.ObtenerCantidad(); i++)
                            {
                                Contribuyente? dato = contribuyentes.ObtenerPorIndice(i);

                                if (dato != null)
                                {
                                    JsonSerializer.Serialize(escritor, dato);
                                }
                            }

                            escritor.WriteEndArray();

                            // Guardar documentos DTE.
                            escritor.WritePropertyName("Documentos");
                            escritor.WriteStartArray();

                            for (int i = 0; i < documentos.ObtenerCantidad();i++)
                            {
                                Factura? dato = documentos.ObtenerPorIndice(i);

                                if (dato != null)
                                {
                                    JsonSerializer.Serialize(escritor, dato);
                                }
                            }

                            escritor.WriteEndArray();

                            // Guardar errores.
                            escritor.WritePropertyName("Errores");
                            escritor.WriteStartArray();

                            for (int i = 0; i < errores.ObtenerCantidad(); i++)
                            {
                                ErrorFactura? dato = errores.ObtenerPorIndice(i);

                                if (dato != null)
                                {
                                    JsonSerializer.Serialize(escritor, dato);
                                }
                            }

                            escritor.WriteEndArray();

                            // Guardar correlativos.
                            escritor.WritePropertyName("Correlativos");
                            escritor.WriteStartArray();

                            for (int i = 0; i < correlativos.ObtenerCantidad(); i++)
                            {
                                Correlativo? dato = correlativos.ObtenerPorIndice(i);

                                if (dato != null)
                                {
                                    JsonSerializer.Serialize(escritor, dato);
                                }
                            }

                            escritor.WriteEndArray();

                            escritor.WritePropertyName("Solicitudes");
                            escritor.WriteStartArray();

                            for (int i = 0; i < solicitudes.ObtenerCantidad(); i++)
                            {
                                SolicitudProcesada? dato = solicitudes.ObtenerPorIndice(i);

                                if (dato != null)
                                {
                                    JsonSerializer.Serialize(
                                    escritor,
                                    dato
                                    );
                                }
                            }

                            escritor.WriteEndArray();

                            escritor.WriteEndObject();
                            escritor.Flush();

                            // Vaciar datos pendientes al disco.
                            archivo.Flush(true);
                        }
                    }

                    // Reemplazar el archivo de estado.
                    File.Move(temporal, rutaArchivo, true);
                }
                finally
                {
                    // Eliminar temporal si quedo pendiente.
                    if (File.Exists(temporal))
                    {
                        File.Delete(temporal);
                    }
                }
            }
        }

        // Recuperar el estado completo.
        public void Cargar(ListaContribuyentes contribuyentes, ListaFacturas documentos,
            ListaErrores errores, ListaCorrelativos correlativos, ListaSolicitudes solicitudes)
        {
            lock (bloqueo)
            {
                // Si nunca se guardo, iniciar vacio.
                if (!File.Exists(rutaArchivo))
                {
                    return;
                }

                // Cargar solamente sobre estructuras vacias.
                if (!contribuyentes.EstaVacia() || !documentos.EstaVacia() ||
                    !errores.EstaVacia() || !correlativos.EstaVacia() || !solicitudes.EstaVacia())
                {
                    throw new InvalidOperationException("La carga requiere estructuras vacias.");
                }

                string contenido = File.ReadAllText(rutaArchivo);

                using (JsonDocument archivo = JsonDocument.Parse(contenido))
                {
                    JsonElement raiz = archivo.RootElement;

                    // Recuperar contribuyentes.
                    foreach (JsonElement elemento in raiz.GetProperty("Contribuyentes").EnumerateArray())
                    {
                        Contribuyente? dato = elemento.Deserialize<Contribuyente>();

                        if (dato == null || !contribuyentes.Insertar(dato))
                        { 
                            throw new InvalidDataException("Contribuyente invalido o duplicado.");
                        }
                    }

                    // Recuperar documentos DTE.
                    foreach (JsonElement elemento in raiz.GetProperty("Documentos").EnumerateArray())
                    {
                        Factura? dato = elemento.Deserialize<Factura>();

                        if (dato == null || !documentos.Insertar(dato))
                        {
                            throw new InvalidDataException("DTE invalido o duplicado.");
                        }
                    }

                    // Recuperar errores.
                    foreach (JsonElement elemento in raiz.GetProperty("Errores").EnumerateArray())
                    {
                        ErrorFactura? dato = elemento.Deserialize<ErrorFactura>();

                        if (dato == null)
                        {
                            throw new InvalidDataException("Error DTE invalido.");
                        }

                        errores.Insertar(dato);
                    }

                    // Recuperar correlativos.
                    foreach (JsonElement elemento in raiz.GetProperty("Correlativos").EnumerateArray())
                    {
                        Correlativo? dato = elemento.Deserialize<Correlativo>();

                        if (dato == null || !correlativos.Insertar(dato))
                        {
                            throw new InvalidDataException("Correlativo invalido o duplicado.");
                        }
                    }
                    
                    // Recuperar las solicitudes procesadas.
                    if (raiz.TryGetProperty("Solicitudes", out JsonElement solicitudesJson))
                    {
                        if (solicitudesJson.ValueKind != JsonValueKind.Array)
                        {
                            throw new InvalidDataException("El registro de solicitudes debe ser un arreglo.");
                        }

                        foreach (JsonElement elemento in solicitudesJson.EnumerateArray())
                        {
                            SolicitudProcesada? dato = elemento.Deserialize<SolicitudProcesada>();

                            if (dato == null)
                            {
                                throw new InvalidDataException("Solicitud procesada invalida.");
                            }

        solicitudes.Insertar(dato);
    }
}

                }
            }
        }

        // Eliminar el archivo de estado.
        public void EliminarArchivo()
        {
            lock (bloqueo)
            {
                if (File.Exists(rutaArchivo))
                {
                    File.Delete(rutaArchivo);
                }
            }
        }
    }
}
