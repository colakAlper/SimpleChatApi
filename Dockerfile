FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/CB.Domain/CB.Domain.csproj src/CB.Domain/
COPY src/CB.Application/CB.Application.csproj src/CB.Application/
COPY src/CB.Infrastructure/CB.Infrastructure.csproj src/CB.Infrastructure/
COPY src/CB.Api/CB.Api.csproj src/CB.Api/

RUN dotnet restore src/CB.Api/CB.Api.csproj

COPY src/ src/
RUN dotnet publish src/CB.Api/CB.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CB.Api.dll"]
