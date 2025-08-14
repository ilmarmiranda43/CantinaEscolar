# ===== Etapa 1: Build + Restore + Publish =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia só os .csproj primeiro (melhora cache)
COPY CantinaEscolar/*.csproj CantinaEscolar/
# Se existir uma .sln na raiz, copie também (opcional)
# COPY *.sln ./

# Restore com caminho explícito do projeto
RUN dotnet restore CantinaEscolar/CantinaEscolar.csproj

# Agora copia todo o repo
COPY . .

# Publica direto (não use BUILD_CONFIGURATION; fixe Release)
RUN dotnet publish CantinaEscolar/CantinaEscolar.csproj -c Release -o /app --no-restore

# ===== Etapa 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Render injeta a porta em $PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# Copia artefatos publicados
COPY --from=build /app ./

# Se o nome do projeto é "CantinaEscolar", o DLL padrão é CantinaEscolar.dll
ENTRYPOINT ["dotnet", "CantinaEscolar.dll"]
