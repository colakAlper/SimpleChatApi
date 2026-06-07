FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/SCA.Domain/SCA.Domain.csproj src/SCA.Domain/
COPY src/SCA.Application/SCA.Application.csproj src/SCA.Application/
COPY src/SCA.Infrastructure/SCA.Infrastructure.csproj src/SCA.Infrastructure/
COPY src/SCA.Api/SCA.Api.csproj src/SCA.Api/

RUN dotnet restore src/SCA.Api/SCA.Api.csproj

COPY src/ src/
RUN dotnet publish src/SCA.Api/SCA.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SCA.Api.dll"]
