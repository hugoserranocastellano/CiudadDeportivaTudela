FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos primero el csproj para que la capa de restore se cachee entre despliegues.
COPY CiudadDeportivaTudela.csproj ./
RUN dotnet restore CiudadDeportivaTudela.csproj

COPY . .
RUN dotnet publish CiudadDeportivaTudela.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Render inyecta $PORT. exec deja a dotnet como PID 1 para que reciba el SIGTERM del apagado.
ENTRYPOINT ["/bin/sh", "-c", "exec dotnet CiudadDeportivaTudela.dll --urls=http://+:${PORT:-8080}"]
