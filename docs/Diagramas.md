# Diagramas — Proyecto 3: API de autorización de DTE (SAT)
 
Diagramas generados a partir del código del repositorio `IPC2_Proy03_202602_202503481`
(commit `7657cb4`). Pensados como apéndice del ensayo (fuera de las 4–7 páginas de contenido).
 
## 1. Diagrama de clases
 
### 1.1 Controladores y servicios (Backend)
 
```mermaid
classDiagram
    class RtuController {
        +Actualizar(JsonElement) IActionResult
        +Consultar(string nit) IActionResult
        +Cantidad() IActionResult
    }
    class AutorizacionesController {
        +Procesar(JsonElement) IActionResult
    }
    class ConsultasController {
        +ConsultarContribuyente(nit, inicio, fin, tipo) IActionResult
        +ConsultarIva(fecha) IActionResult
        +ConsultarRango(inicio, fin, incluirIva) IActionResult
    }
 
    class EstadoSistema {
        +ListaContribuyentes Contribuyentes
        +ListaFacturas Documentos
        +ListaErrores Errores
        +ListaCorrelativos Correlativos
        +ListaSolicitudes Solicitudes
        -Persistencia persistencia
        -object bloqueo
        +Consultar~T~(Func~T~) T
        +EjecutarCambio(Action)
        +Inicializar()
    }
    class GestorRtu {
        +ProcesarContribuyente(nit, nombre, RespuestaRtu)
        +Buscar(nit) Contribuyente
        +ObtenerCantidad() int
    }
    class GestorAutorizaciones {
        -ValidadorFecha validadorFecha
        -ValidadorDTE validadorDTE
        -GestorCorrelativos gestorCorrelativos
        +ProcesarSolicitud(SolicitudDTE, ResultadoAutorizaciones, DateTime) bool
    }
    class GestorConsultas {
        +ConsultarContribuyente(nit, inicio, fin, emitidas) ListaResumenContribuyente
        +ConsultarIva(fecha) ListaResumenIva
        +ConsultarRango(inicio, fin) ListaResumenFecha
    }
    class GestorCorrelativos {
        -ListaCorrelativos correlativos
        -object bloqueo
        +GenerarCodigo(DateTime) string
        +ConsultarUltimoNumero(DateTime) int
    }
    class GeneradorRespuestaAutorizaciones {
        +Generar(ResultadoAutorizaciones) string
    }
    class Persistencia {
        -string rutaArchivo
        +Guardar(...)
        +Cargar(...)
    }
 
    RtuController --> EstadoSistema
    RtuController --> GestorRtu
    AutorizacionesController --> GestorAutorizaciones
    AutorizacionesController --> GeneradorRespuestaAutorizaciones
    ConsultasController --> GestorConsultas
    ConsultasController --> ValidadorRango
 
    GestorRtu --> ValidadorNit
    GestorAutorizaciones --> ValidadorFecha
    GestorAutorizaciones --> ValidadorDTE
    GestorAutorizaciones --> GestorCorrelativos
    GestorAutorizaciones --> EstadoSistema
    GestorConsultas --> EstadoSistema
    EstadoSistema --> Persistencia
    EstadoSistema *-- ListaContribuyentes
    EstadoSistema *-- ListaFacturas
    EstadoSistema *-- ListaErrores
    EstadoSistema *-- ListaCorrelativos
    EstadoSistema *-- ListaSolicitudes
```
 
### 1.2 Validaciones
 
```mermaid
classDiagram
    class ValidadorNit {
        -Regex formatoNit
        +ValidarFormato(string) bool
        +CalcularVerificador(string) char
        +Validar(string) bool
    }
    class ValidadorFecha {
        -Regex expresionFecha
        +IntentarObtenerFecha(string, out DateTime) bool
        +Validar(string) bool
        +ValidarRango(DateTime, DateTime) bool
    }
    class ValidadorDTE {
        -ValidadorNit validadorNit
        +CalcularIva(decimal) decimal
        +Validar(Factura, ListaContribuyentes, ListaFacturas, ListaErrores) bool
        -ValidarMonto(decimal) bool
        -RegistrarError(...)
    }
    class ValidadorRango {
        +ValidarFecha(string, out DateTime) bool
        +Validar(string, string, out DateTime, out DateTime) bool
    }
 
    ValidadorDTE --> ValidadorNit : usa
```
 
### 1.3 Modelos (DTOs)
 
```mermaid
classDiagram
    class Factura {
        +DateTime Fecha
        +string Referencia
        +string NitEmisor
        +string NitReceptor
        +decimal Valor
        +decimal Iva
        +decimal Total
        +string CodigoAutorizacion
        +bool Autorizada
    }
    class Contribuyente {
        +string Nit
        +string Nombre
    }
    class SolicitudDTE {
        +string Tiempo
        +string Referencia
        +string NitEmisor
        +string NitReceptor
        +decimal? Valor
        +decimal? Iva
        +decimal? Total
    }
    class ErrorFactura {
        +DateTime Fecha
        +string Referencia
        +string TipoError
        +string Descripcion
    }
    class ResultadoAutorizaciones {
        +int CantidadAutorizacionesAprobadas
        +int CantidadAutorizacionesRechazadasPorNitEmisorInexistente
        +int CantidadAutorizacionesRechazadasPorNitReceptorInexistente
        +ListaAprobaciones DetalleAprobaciones
        +AgregarAprobacion(referencia, codigo)
    }
    class DetalleAprobacion {
        +string Referencia
        +long CodigoAprobacion
    }
    class RespuestaRtu {
        +int ContribuyentesNuevos
        +int ContribuyentesActualizados
        +int NitsInvalidos
    }
 
    ResultadoAutorizaciones *-- DetalleAprobacion
```
 
### 1.5 Frontend
 
```mermaid
classDiagram
    class ClienteApi {
        -HttpClient cliente
        +ActualizarRtu(json) string
        +ProcesarDTE(json) string
        +ConsultarContribuyente(nit, inicio, fin, tipo) string
        +ConsultarIva(fecha) string
        +ConsultarRango(inicio, fin, incluirIva) string
        +BuscarContribuyente(nit) string
        +CantidadContribuyentes() string
    }
    class EnviarMensajeModel
    class ConsultarContribuyenteModel
    class ResumenIvaModel
    class ResumenRangoModel
 
    EnviarMensajeModel --> ClienteApi
    ConsultarContribuyenteModel --> ClienteApi
    ResumenIvaModel --> ClienteApi
    ResumenRangoModel --> ClienteApi
```
 
---
 
## 2. Diagramas de actividades
 
### 2.1 Validación del NIT (algoritmo módulo 11)
 
```mermaid
flowchart TD
    A([Inicio]) --> B[Recibir NIT como texto]
    B --> C{"¿Coincide con\n^[0-9]{1,20}[0-9K]$?"}
    C -- No --> Z1([NIT inválido]) --> F([Fin])
    C -- Sí --> D[Separar número base\ny dígito verificador]
    D --> E["factor = 2, suma = 0"]
    E --> G[Recorrer dígitos del número base\nde derecha a izquierda]
    G --> H["suma += dígito × factor\nfactor++"]
    H --> I{"¿Quedan dígitos?"}
    I -- Sí --> G
    I -- No --> J["residuo = suma mod 11\nresultado = (11 - residuo) mod 11"]
    J --> K{"¿resultado == 10?"}
    K -- Sí --> L["verificador calculado = 'K'"]
    K -- No --> M["verificador calculado = resultado"]
    L --> N{"¿verificador ingresado ==\nverificador calculado?"}
    M --> N
    N -- Sí --> Z2([NIT válido]) --> F
    N -- No --> Z1
```
 
### 2.2 Procesamiento de una solicitud de autorización DTE
 
```mermaid
flowchart TD
    A([Inicio]) --> B[Extraer fecha y hora del campo\nTIEMPO con ValidadorFecha]
    B --> C{"¿Fecha válida?"}
    C -- No --> E1[Registrar error FECHA_INVALIDA] --> R1([Solicitud rechazada]) --> F([Fin])
    C -- Sí --> D{"¿VALOR, IVA y TOTAL\nestán presentes?"}
    D -- No --> E2[Registrar error DATOS_INCOMPLETOS] --> R1
    D -- Sí --> G{"¿Referencia ya fue\nprocesada antes?"}
    G -- Sí --> E3[Registrar error REFERENCIA_DUPLICADA] --> R1
    G -- No --> H[Construir objeto Factura]
    H --> I["Validar con ValidadorDTE:\nreferencia, NIT emisor/receptor,\nvalor, IVA, total"]
    I --> J{"¿DTE válido?"}
    J -- No --> K[Contabilizar NIT inexistentes\ny registrar la factura rechazada] --> R1
    J -- Sí --> L[Generar código de autorización\ncon GestorCorrelativos]
    L --> M[Marcar factura como autorizada\ny guardarla]
    M --> N[Agregar aprobación al resultado]
    N --> O([Solicitud aprobada]) --> F
```
 
### 2.3 Generación del correlativo diario
 
```mermaid
flowchart TD
    A([Inicio: fecha de autorización]) --> B[Adquirir bloqueo compartido]
    B --> C{"¿Existe un correlativo\nregistrado para esa fecha?"}
    C -- No --> D["Crear correlativo para la fecha\ncon último número = 0"]
    C -- Sí --> E[Usar el correlativo existente]
    D --> F
    E --> F["número = últimoNúmero + 1"]
    F --> G["fechaTexto = fecha en formato yyyyMMdd"]
    G --> H["numeroTexto = número con 8 dígitos (ceros a la izquierda)"]
    H --> I["código = fechaTexto + numeroTexto"]
    I --> J[Actualizar el último número\nde esa fecha]
    J --> K[Liberar el bloqueo]
    K --> L([Fin: retorna código])
```
 
---
 
## 3. Diagrama de despliegue
 
```mermaid
flowchart LR
    subgraph Cliente["Nodo: equipo del usuario / evaluador"]
        Browser["Navegador web\n(Programa 1)"]
        Postman["Cliente externo\n(p. ej. Postman)"]
    end
 
    subgraph ServidorFrontend["Nodo: servidor de aplicación\n«ASP.NET Core Razor Pages»"]
        Frontend["Frontend\nPages + ClienteApi (HttpClient)"]
    end
 
    subgraph ServidorBackend["Nodo: servidor de aplicación\n«ASP.NET Core Web API»"]
        Backend["Backend\nControllers + Servicios + Validaciones + Estructuras"]
        Datos[("Datos/estado.json\n(persistencia en archivo)")]
        Backend --> Datos
    end
 
    Browser -- HTTPS --> Frontend
    Frontend -- "HTTP/JSON\n(ClienteApi)" --> Backend
    Postman -- "HTTP/JSON" --> Backend
```
 