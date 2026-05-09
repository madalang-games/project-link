FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY server/src/ ./src/
COPY server/generated/ ./generated/
COPY shared/contracts/ ../shared/contracts/
RUN dotnet publish src/ProjectLink.API/ProjectLink.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ProjectLink.API.dll"]
