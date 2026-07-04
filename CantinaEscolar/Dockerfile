# ===== Etapa 1: Build + Restore + Publish =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# ===== Etapa 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

USER root
RUN mkdir -p /app/keys && chown -R app:app /app/keys
USER app

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=build /app ./

ENTRYPOINT ["dotnet", "CantinaEscolar.dll"]