# ===== STAGE: base (runtime) =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Kestrel ouvindo na 8080 por padrão (pode ser sobrescrito por ASPNETCORE_URLS no deploy)
ENV ASPNETCORE_URLS=http://+:8080

# Expor apenas a porta HTTP padrão
EXPOSE 8080

# (Opcional) criar usuário não-root para rodar a app
RUN adduser --disabled-password --gecos "" appuser \
    && chown -R appuser /app
USER appuser

# ===== STAGE: build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia só o csproj e restaura (cache mais eficiente)
COPY CantinaEscolar/CantinaEscolar.csproj CantinaEscolar/
RUN dotnet restore "CantinaEscolar/CantinaEscolar.csproj"

# Copia o restante do código
COPY . .

# Compila
WORKDIR /src/CantinaEscolar
RUN dotnet build "CantinaEscolar.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===== STAGE: publish =====
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CantinaEscolar.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== STAGE: final =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "CantinaEscolar.dll"]
