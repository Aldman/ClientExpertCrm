FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["/src/CRMService/CRMService.csproj", "./CRMService/"]
COPY ["/src/Shared/Shared.csproj", "./Shared/"]
RUN dotnet restore CRMService/CRMService.csproj

COPY ["/src/CRMService/", "./CRMService/"]
COPY ["/src/Shared/", "./Shared/"]

WORKDIR /src/CRMService
RUN dotnet build CRMService.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish CRMService.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRMService.dll"]
