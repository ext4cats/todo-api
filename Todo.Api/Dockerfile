# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Todo.Api/Todo.Api.csproj Todo.Api/
RUN dotnet restore Todo.Api
COPY . .
RUN dotnet build Todo.Api --no-restore -c Release

FROM build AS migrate
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
WORKDIR /src/Todo.Api
ENTRYPOINT ["dotnet-ef", "database", "update", "--no-build", "--configuration", "Release"]

FROM build AS publish
RUN dotnet publish Todo.Api --no-build -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
EXPOSE 8080
WORKDIR /app
COPY --from=publish /app/publish .
USER ${APP_UID}
ENTRYPOINT ["dotnet", "Todo.Api.dll"]
