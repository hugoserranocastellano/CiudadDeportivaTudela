# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Qué es esto

App de gestión interna para la Peña Ciudad Deportiva Tudela: socios, mesas/reservas,
tickets de bar y su stock, gastos de la sociedad, inscripciones a eventos y
sugerencias. Blazor Server (.NET 10, `net10.0`) sobre PostgreSQL sirviendo (Supabase),
desplegada en Render.

## Comandos

```bash
dotnet build                    # compilar
dotnet ef migrations add <Nombre>   # nueva migración (tras tocar ApplicationDbContext u OnModelCreating)
dotnet ef database update       # aplicar migraciones pendientes
```

**Arrancar en local — NO uses `dotnet run`.** App Control de Windows bloquea el
`.exe` recién compilado en este equipo. Compila y lanza el ensamblado directamente:

```bash
dotnet build
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:5199 \
  dotnet bin/Debug/net10.0/CiudadDeportivaTudela.dll
```

Si `dotnet build` falla con MSB3021/MSB3027 (fichero bloqueado), hay una instancia
del proceso ya corriendo — párala antes de recompilar. La misma política bloquea
ensamblados en `%TEMP%`; cualquier proyecto de prueba de un solo uso debe crearse
dentro de `C:\Users\hugos\Desktop\`, no en carpetas temporales.

No hay proyecto de tests en el repo.

## Configuración y secretos

La cadena de conexión vive en la clave `ConnectionStrings:Supabase`:
- En local: user secrets (`UserSecretsId` en el `.csproj`), nunca en `appsettings.json`.
- En Render: variable de entorno `ConnectionStrings__Supabase`.

**Debe apuntar al pooler de Supabase, no a la conexión directa.** La conexión
directa (`db.<ref>.supabase.co`) sólo resuelve por IPv6 y Render no tiene salida
IPv6. El pooler (`*.pooler.supabase.com:5432`, session mode) sí tiene IPv4. Usa el
puerto 5432 (session mode, compatible con prepared statements de EF Core) y no el
6543 (transaction mode). El usuario **debe** llevar el sufijo del project-ref
(`postgres.<project-ref>`) — con `postgres` a secas el pooler no puede resolver el
tenant y Postgres responde `28P01` aunque la contraseña sea correcta.

`Data/PostgresConnectionString.cs` normaliza y valida la cadena antes de dársela a
Npgsql (acepta tanto `clave=valor` como la URI `postgresql://` que copia Supabase) y
falla rápido con mensajes explícitos ante los errores típicos de copiar/pegar desde
Supabase: placeholder `[YOUR-PASSWORD]` sin sustituir, usuario sin sufijo de
project-ref, contraseña vacía. Si tocas la lógica de conexión, extiende esa
validación en vez de dejar que Npgsql falle con su mensaje opaco.

## Arquitectura

**Blazor Server, no MVC/Razor Pages clásico.** Todas las páginas están bajo
`Components/Pages/*.razor`, con `@page` y render mode interactivo por servidor salvo
excepciones deliberadas (ver Login más abajo). `Program.cs` es el único punto de
composición: registro de EF Core, autenticación, forwarded headers y el pipeline
HTTP entero.

**Acceso a datos:** `IDbContextFactory<ApplicationDbContext>` inyectado, nunca
`ApplicationDbContext` directo — es el patrón correcto en Blazor Server porque un
componente vive más que una request HTTP y un `DbContext` compartido por circuito no
es thread-safe. Cada operación crea su propio contexto con
`await DbFactory.CreateDbContextAsync()` en un `using`/`await using` de vida corta.

**Mapeo EF Core:** todo el mapeo objeto-columna vive centralizado en
`Data/ApplicationDbContext.OnModelCreating` (nombres de tabla y columna en snake_case,
relaciones, tipos `numeric(10,2)` para importes, defaults `now()` para timestamps).
Los modelos en `Models/*.cs` son POCOs sin atributos de mapeo — al añadir un campo o
entidad nueva, el cableado va en `OnModelCreating`, no en el modelo.

**Autenticación propia, no ASP.NET Identity.** Los socios entran con su número de
socio como usuario y su teléfono como contraseña (`Services/SocioAuth.cs`). Cookie
auth con scheme `CookieAuthenticationDefaults.AuthenticationScheme`, expiración
deslizante de 12h. `SocioAuth.TelefonoCoincide` compara sólo dígitos (y los últimos 9
si hay prefijo internacional), porque el teléfono se teclea con formatos distintos.
El claim de rol (`ClaimTypes.Role`) es la columna `categoria` del socio; `cargo` es un
claim aparte cuando existe.

`Components/Pages/Login.razor` es la única página sin render mode interactivo
explícito: `HttpContext.SignInAsync` necesita el `HttpContext` real de la petición,
que sólo existe durante el render en servidor, no en un circuito interactivo ya
establecido. Si conviertes esta página a interactiva, el login deja de funcionar en
silencio.

**Despliegue en Render, no IIS/Kestrel expuesto directo.** Render termina TLS en su
proxy: `Program.cs` configura `ForwardedHeaders` sin restringir `KnownProxies` porque
la IP del proxy de Render no es fija. En producción no se llama a
`UseHttpsRedirection()` — Render ya redirige HTTP→HTTPS en el borde, y repetirlo
dentro rompería el health check interno (`/healthz`, que Render llama por HTTP y a
propósito no toca la base de datos, para no reiniciar el contenedor si Supabase va
lento).

**Logout es `POST /logout`, no un `<a>`/GET** — un enlace GET dispararía con el
prefetch del navegador y cerraría sesión sin que el socio lo pidiera.
