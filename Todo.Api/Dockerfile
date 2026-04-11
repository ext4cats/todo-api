# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Todo.Api/Todo.Api.csproj Todo.Api/
RUN dotnet restore Todo.Api/Todo.Api.csproj
COPY . .
RUN dotnet publish Todo.Api/Todo.Api.csproj --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Todo.Api.dll"]
