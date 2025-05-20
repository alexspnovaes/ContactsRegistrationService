# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080  
EXPOSE 8081  

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar os projetos
COPY ["src/Contacts.Shared/Contacts.Shared.csproj", "src/Contacts.Shared/"]
COPY ["src/ContactsPersistenceService.Consumer/ContactsPersistenceService.Consumer.csproj", "src/ContactsPersistenceService.Consumer/"]
COPY ["src/ContactsRegistrationService.Api/ContactsRegistrationService.Api.csproj", "src/ContactsRegistrationService.Api/"]

# Restaurar dependências
RUN dotnet restore "src/ContactsRegistrationService.Api/ContactsRegistrationService.Api.csproj"
RUN dotnet restore "src/ContactsPersistenceService.Consumer/ContactsPersistenceService.Consumer.csproj"

# Copiar o código completo
COPY . .

# Build do Shared (referenciado pelos outros)
WORKDIR "/src/src/Contacts.Shared"
RUN dotnet build "Contacts.Shared.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Build do Consumer
WORKDIR "/src/src/ContactsPersistenceService.Consumer"
RUN dotnet build "ContactsPersistenceService.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/consumer

# Build da API
WORKDIR "/src/src/ContactsRegistrationService.Api"
RUN dotnet build "ContactsRegistrationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/api

# Etapa de publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publicação do Consumer
WORKDIR /src/src/ContactsPersistenceService.Consumer
RUN dotnet publish "ContactsPersistenceService.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish/consumer /p:UseAppHost=false

# Publicação da API
WORKDIR /src/src/ContactsRegistrationService.Api
RUN dotnet publish "ContactsRegistrationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish/api /p:UseAppHost=false

# Etapa final - API
FROM base AS api
WORKDIR /app
COPY --from=publish /app/publish/api .
ENTRYPOINT ["dotnet", "ContactsRegistrationService.Api.dll"]

# Etapa final - Consumer
FROM base AS consumer
WORKDIR /app
COPY --from=publish /app/publish/consumer .
ENTRYPOINT ["dotnet", "ContactsPersistenceService.Consumer.dll"]
