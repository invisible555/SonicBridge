# 1. Budowanie aplikacji
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# 2. Uruchamianie aplikacji
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Kopiujemy wszystko z folderu publish
COPY --from=build /app/publish .

# Upewniamy siê, ¿e appsettings.json jest w œrodku
COPY appsettings.json .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Backend.dll"]
